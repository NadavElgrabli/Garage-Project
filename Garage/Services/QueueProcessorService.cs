using Garage.Enums;
using Garage.Models;
using Garage.Data;

namespace Garage.Services;

public class QueueProcessorService
{
    private readonly TreatmentService _treatmentService;

    public QueueProcessorService(TreatmentService treatmentService)
    {
        _treatmentService = treatmentService;
    }

    public async Task StartProcessingAsync()
    {
        _ = Task.Run(() => ProcessFuelQueueAsync());
        _ = Task.Run(() => ProcessChargeQueueAsync());
        _ = Task.Run(() => ProcessAirQueueAsync());
    }

    private async Task ProcessFuelQueueAsync()
    {
        while (true)
        {
            if (!InMemoryDatabase.FuelStationRequests.TryPeek(out var fuelRequest))
            {
                await Task.Delay(100);
                continue;
            }

            var vehicle = fuelRequest.Vehicle;

            if (vehicle.Status == Status.Ready)
            {
                InMemoryDatabase.FuelStationRequests.TryDequeue(out _); // remove it
                continue;
            }

            if (vehicle.Status != Status.InTreatment && vehicle.TreatmentTypes.Contains(TreatmentType.Refuel))
            {
                if (InMemoryDatabase.FuelStationRequests.TryDequeue(out var readyRequest))
                {
                    try
                    {
                        await _treatmentService.RefuelAsync(readyRequest.Vehicle, readyRequest.RequestedLiters);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Refueling error: {ex.Message}");
                    }
                }
            }
            else
            {
                // Vehicle is either in treatment or doesn't need refuel
                if (InMemoryDatabase.FuelStationRequests.TryDequeue(out var currentRequest))
                {
                    bool requeued = false;

                    if (currentRequest.Vehicle.Status == Status.InTreatment &&
                        currentRequest.Vehicle.TreatmentTypes.Contains(TreatmentType.Refuel))
                    {
                        if (InMemoryDatabase.FuelStationRequests.TryPeek(out var nextRequest) &&
                            nextRequest.Vehicle.Status != Status.InTreatment &&
                            nextRequest.Vehicle.TreatmentTypes.Contains(TreatmentType.Refuel))
                        {
                            if (InMemoryDatabase.FuelStationRequests.TryDequeue(out var cutterRequest))
                            {
                                try
                                {
                                    await _treatmentService.RefuelAsync(cutterRequest.Vehicle, cutterRequest.RequestedLiters);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Refueling error (cutter): {ex.Message}");
                                }
                            }

                            // Re-queue the original busy vehicle at the front
                            var tempQueue = new Queue<FuelRequest>();
                            tempQueue.Enqueue(currentRequest);
                            while (InMemoryDatabase.FuelStationRequests.TryDequeue(out var item))
                                tempQueue.Enqueue(item);

                            foreach (var item in tempQueue)
                                InMemoryDatabase.FuelStationRequests.Enqueue(item);

                            requeued = true;
                        }
                    }

                    if (!requeued)
                    {
                        InMemoryDatabase.FuelStationRequests.Enqueue(currentRequest);
                        await Task.Delay(100);
                    }
                }
            }
        }
    }

    private async Task ProcessChargeQueueAsync()
{
    while (true)
    {
        if (InMemoryDatabase.ChargeStationRequests.TryPeek(out var chargeRequest))
        {
            var vehicle = chargeRequest.Vehicle;

            if (vehicle.Status == Status.Ready)
            {
                InMemoryDatabase.ChargeStationRequests.TryDequeue(out _);
                continue;
            }

            if (vehicle.Status != Status.InTreatment && vehicle.TreatmentTypes.Contains(TreatmentType.Recharge))
            {
                if (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var readyRequest))
                {
                    try
                    {
                        await _treatmentService.RechargeAsync(readyRequest.Vehicle, readyRequest.RequestedHoursToCharge);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Charging error: {ex.Message}");
                    }
                }
            }
            else if (vehicle.Status == Status.InTreatment && vehicle.TreatmentTypes.Contains(TreatmentType.Recharge))
            {
                if (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var waitingRequest))
                {
                    var tempQueue = new Queue<ChargeRequest>();
                    bool treatmentStarted = false;

                    while (!treatmentStarted)
                    {
                        await Task.Delay(100);

                        if (waitingRequest.Vehicle.Status != Status.InTreatment)
                        {
                            try
                            {
                                await _treatmentService.RechargeAsync(waitingRequest.Vehicle, waitingRequest.RequestedHoursToCharge);
                                treatmentStarted = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Charging error (delayed): {ex.Message}");
                                break;
                            }
                        }
                        else if (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var nextRequest))
                        {
                            tempQueue.Enqueue(waitingRequest);
                            try
                            {
                                await _treatmentService.RechargeAsync(nextRequest.Vehicle, nextRequest.RequestedHoursToCharge);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Charging error (pass-through): {ex.Message}");
                            }

                            // Restore queue
                            while (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var remaining))
                                tempQueue.Enqueue(remaining);

                            foreach (var req in tempQueue)
                                InMemoryDatabase.ChargeStationRequests.Enqueue(req);

                            break;
                        }
                    }
                }
            }
            else
            {
                InMemoryDatabase.ChargeStationRequests.TryDequeue(out _);
            }
        }
        else
        {
            await Task.Delay(100);
        }
    }
}


    private async Task ProcessAirQueueAsync()
{
    while (true)
    {
        if (InMemoryDatabase.AirStationRequests.TryPeek(out var airRequest))
        {
            var vehicle = airRequest.Vehicle;

            if (vehicle.Status == Status.Ready)
            {
                InMemoryDatabase.AirStationRequests.TryDequeue(out _);
                continue;
            }

            if (vehicle.Status != Status.InTreatment && vehicle.TreatmentTypes.Contains(TreatmentType.Inflate))
            {
                if (InMemoryDatabase.AirStationRequests.TryDequeue(out var readyRequest))
                {
                    try
                    {
                        await _treatmentService.InflateTiresAsync(readyRequest.Vehicle, readyRequest.DesiredWheelPressures);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Air inflation error: {ex.Message}");
                    }
                }
            }
            else if (vehicle.Status == Status.InTreatment && vehicle.TreatmentTypes.Contains(TreatmentType.Inflate))
            {
                if (InMemoryDatabase.AirStationRequests.TryDequeue(out var waitingRequest))
                {
                    var tempQueue = new Queue<AirRequest>();
                    bool treatmentStarted = false;

                    while (!treatmentStarted)
                    {
                        await Task.Delay(100);

                        if (waitingRequest.Vehicle.Status != Status.InTreatment)
                        {
                            try
                            {
                                await _treatmentService.InflateTiresAsync(waitingRequest.Vehicle, waitingRequest.DesiredWheelPressures);
                                treatmentStarted = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Air inflation error (delayed): {ex.Message}");
                                break;
                            }
                        }
                        else if (InMemoryDatabase.AirStationRequests.TryDequeue(out var nextRequest))
                        {
                            tempQueue.Enqueue(waitingRequest);
                            try
                            {
                                await _treatmentService.InflateTiresAsync(nextRequest.Vehicle, nextRequest.DesiredWheelPressures);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Air inflation error (pass-through): {ex.Message}");
                            }

                            while (InMemoryDatabase.AirStationRequests.TryDequeue(out var remaining))
                                tempQueue.Enqueue(remaining);

                            foreach (var req in tempQueue)
                                InMemoryDatabase.AirStationRequests.Enqueue(req);

                            break;
                        }
                    }
                }
            }
            else
            {
                InMemoryDatabase.AirStationRequests.TryDequeue(out _);
            }
        }
        else
        {
            await Task.Delay(100);
        }
    }
}

}
