using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class QueueProcessorService
{
    private readonly IEnumerable<ITreatmentService> _treatmentServices;
    private readonly IQueueRepository _queueRepository;


    public QueueProcessorService(
        IEnumerable<ITreatmentService> treatmentServices,
        IQueueRepository queueRepository)
    {
        _treatmentServices = treatmentServices;
        _queueRepository = queueRepository;
    }

    
    public async Task StartProcessingAsync()
    {
        var processingTasks = _treatmentServices
            .Select(service => Task.Run(() => ProcessQueueAsync(service)))
            .ToList();

        //await Task.WhenAll(processingTasks);
    }

    
    public async Task ProcessQueueAsync(ITreatmentService treatmentService)
    {
        while (true)
        {
            if (!_queueRepository.TryPeekRequest(treatmentService, out var request))
            {
                //If the queue is empty: wait 100ms and try again.
                await Task.Delay(100);
                continue;
            }

            var vehicle = request.Vehicle;
            
            // if it doesn't need the treatment TODO: Delete "if"
            if (!treatmentService.IsMatch(vehicle))
            {
                _queueRepository.TryDequeueRequest(treatmentService, out _); // remove it
                continue;
            }
            
            // if its available and needs the treatment TODO: Check how to avoid two tasks to enter this "if" at the same time
            if (vehicle.Status != Status.InTreatment)
            {
                // remove from the queue and begin treatment TODO: maybe different name for readyRequest
                if (_queueRepository.TryDequeueRequest(treatmentService, out var readyRequest))
                {
                    try
                    {
                        float price = await treatmentService.TreatAsync(readyRequest.Vehicle, 
                            readyRequest);
                        readyRequest.Vehicle.TreatmentsPrice += price;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in providing service: {treatmentService.GetTreatmentType()} error: {ex.Message}");
                    }
                }
            }
            else
            {
                // can't treat now - vehicle is currently in treatment
                if (_queueRepository.TryDequeueRequest(treatmentService, out var currentRequest))
                {
                    bool requeued = false;

                    // Currently in treatment but also needs another treatment TODO: this if is redundant
                    if (currentRequest.Vehicle.Status == Status.InTreatment &&
                        treatmentService.IsMatch(currentRequest.Vehicle))
                    {
                        // if there is a next vehicle in the queue that can cut the current vehicle TODO: delete the isMAtch from the if statement
                        if (_queueRepository.TryPeekRequest(treatmentService, out var nextRequest) &&
                            nextRequest.Vehicle.Status != Status.InTreatment &&
                            treatmentService.IsMatch(nextRequest.Vehicle))
                        {
                            // cut in line TODO: 2 tasks can enter the if at the same time, and both will go through treatment
                            if (_queueRepository.TryDequeueRequest(treatmentService, out var cutterRequest))
                            {
                                try
                                {
                                    float price = await treatmentService.TreatAsync(cutterRequest.Vehicle, cutterRequest);
                                    cutterRequest.Vehicle.TreatmentsPrice += price;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Treatment error (cutter): {ex.Message}");
                                }
                            }
                            //TODO: the whole block use in a seperate function called: Put back the request in the first place in the que
                            // Re-queue the original busy vehicle at the front
                            var tempQueue = new Queue<TreatmentRequest>();
                            tempQueue.Enqueue(currentRequest);

                            // Drain the rest of the Requests queue into the temp queue, in order
                            var drainedQueue = _queueRepository.DrainQueue(treatmentService);
                            foreach (var item in drainedQueue)
                                tempQueue.Enqueue(item);

                            // Rebuild the Requests queue with the updated order.
                            _queueRepository.RebuildQueue(treatmentService, tempQueue);

                            requeued = true;
                        }
                    }

                    // If there was no cutter, just put the current vehicle back at the end, wait a bit, and retry.
                    if (!requeued)
                    {
                        _queueRepository.EnqueueRequest(treatmentService, currentRequest);
                        await Task.Delay(100);
                    }
                }
            }
        }
    }

}

// public Task StartProcessingAsync()
// {
//     foreach (var service in _treatmentServices)
//     {
//         _ = Task.Run(() => ProcessQueueAsync(service));
//     }
//
//     return Task.CompletedTask;
// }


    // private async Task ProcessFuelQueueAsync()
    // {
    //     while (true)
    //     {
    //         // Checks the first car in the fuel queue (without removing it) and assign to fuelRequest.
    //         if (!_queueRepository.TryPeekFuelRequest(out var fuelRequest))
    //         {
    //             //If the queue is empty: wait 100ms and try again.
    //             await Task.Delay(100);
    //             continue;
    //         }
    //
    //         var vehicle = fuelRequest.Vehicle;
    //
    //         if (!_fuelService.IsMatch(vehicle))
    //         {
    //             _queueRepository.TryDequeueFuelRequest(out _); // remove it
    //             continue;
    //         }
    //         
    //         // if its available and needs refueling
    //         if (vehicle.Status != Status.InTreatment)
    //         {
    //             // remove from the queue and begin refueling
    //             if (_queueRepository.TryDequeueFuelRequest(out var readyRequest))
    //             {
    //                 try
    //                 {
    //                     float price = await _fuelService.TreatAsync(readyRequest.Vehicle, 
    //                         readyRequest.RequestedLiters);
    //                     readyRequest.Vehicle.TreatmentsPrice += price;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Console.WriteLine($"Refueling error: {ex.Message}");
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // can't refuel now - vehicle is currently in treatment
    //             if (_queueRepository.TryDequeueFuelRequest(out var currentRequest))
    //             {
    //                 bool requeued = false;
    //
    //                 // Currently pumping air but needs also to refuel
    //                 if (currentRequest.Vehicle.Status == Status.InTreatment &&
    //                     _fuelService.IsMatch(currentRequest.Vehicle))
    //                 {
    //                     // if there is a next vehicle in the queue that can cut the current vehicle
    //                     if (_queueRepository.TryPeekFuelRequest(out var nextRequest) &&
    //                         nextRequest.Vehicle.Status != Status.InTreatment &&
    //                         _fuelService.IsMatch(nextRequest.Vehicle))
    //                     {
    //                         // cut in line
    //                         if (_queueRepository.TryDequeueFuelRequest(out var cutterRequest))
    //                         {
    //                             try
    //                             {
    //                                 float price = await _fuelService.TreatAsync(cutterRequest.Vehicle, cutterRequest.RequestedLiters);
    //                                 cutterRequest.Vehicle.TreatmentsPrice += price;
    //                             }
    //                             catch (Exception ex)
    //                             {
    //                                 Console.WriteLine($"Refueling error (cutter): {ex.Message}");
    //                             }
    //                         }
    //
    //                         // Re-queue the original busy vehicle at the front
    //                         var tempQueue = new Queue<FuelRequest>();
    //                         tempQueue.Enqueue(currentRequest);
    //
    //                         // Drain the rest of the FuelStationRequests queue into the temp queue, in order
    //                         var drainedQueue = _queueRepository.DrainFuelQueue();
    //                         foreach (var item in drainedQueue)
    //                             tempQueue.Enqueue(item);
    //
    //                         // Rebuild the FuelStationRequests queue with the updated order.
    //                         _queueRepository.RebuildFuelQueue(tempQueue);
    //
    //                         requeued = true;
    //                     }
    //                 }
    //
    //                 // If there was no cutter, just put the current vehicle back at the end, wait a bit, and retry.
    //                 if (!requeued)
    //                 {
    //                     _queueRepository.EnqueueFuelRequest(currentRequest);
    //                     await Task.Delay(100);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // private async Task ProcessChargeQueueAsync()
    // {
    //     while (true)
    //     {
    //         // Checks the first car in the charge queue (without removing it) and assign to chargeRequest.
    //         if (!_queueRepository.TryPeekChargeRequest(out var chargeRequest))
    //         {
    //             // If the queue is empty: wait 100ms and try again.
    //             await Task.Delay(100);
    //             continue;
    //         }
    //
    //         var vehicle = chargeRequest.Vehicle;
    //
    //         if (!_chargeService.IsMatch(vehicle))
    //         {
    //             _queueRepository.TryDequeueChargeRequest(out _); // remove it
    //             continue;
    //         }
    //         
    //         // if vehicle is available 
    //         if (vehicle.Status != Status.InTreatment)
    //         {
    //             // remove from the queue and begin charging
    //             if (_queueRepository.TryDequeueChargeRequest(out var readyRequest))
    //             {
    //                 try
    //                 {
    //                     float price = await _chargeService.TreatAsync(readyRequest.Vehicle, 
    //                         readyRequest.RequestedHoursToCharge);
    //                     readyRequest.Vehicle.TreatmentsPrice += price;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Console.WriteLine($"Charging error: {ex.Message}");
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // can't charge now - vehicle is either already being treated or not ready
    //             if (_queueRepository.TryDequeueChargeRequest(out var currentRequest))
    //             {
    //                 bool requeued = false;
    //
    //                 // Currently in another treatment but needs also to recharge
    //                 if (currentRequest.Vehicle.Status == Status.InTreatment &&
    //                     _chargeService.IsMatch(currentRequest.Vehicle))
    //                 {
    //                     // if there is a next vehicle in the queue that can cut the current vehicle
    //                     if (_queueRepository.TryPeekChargeRequest(out var nextRequest) &&
    //                         nextRequest.Vehicle.Status != Status.InTreatment &&
    //                         _chargeService.IsMatch(nextRequest.Vehicle))
    //                     {
    //                         // cut in line
    //                         if (_queueRepository.TryDequeueChargeRequest(out var cutterRequest))
    //                         {
    //                             try
    //                             {
    //                                 float price = await _chargeService.TreatAsync(
    //                                     cutterRequest.Vehicle, cutterRequest.RequestedHoursToCharge);
    //                                 cutterRequest.Vehicle.TreatmentsPrice += price;
    //                             }
    //                             catch (Exception ex)
    //                             {
    //                                 Console.WriteLine($"Charging error (cutter): {ex.Message}");
    //                             }
    //                         }
    //
    //                         // Re-queue the original busy vehicle at the front
    //                         var tempQueue = new Queue<ChargeRequest>();
    //                         tempQueue.Enqueue(currentRequest);
    //
    //                         // Drain the rest of the ChargeStationRequests queue into the temp queue, in order
    //                         var drainedQueue = _queueRepository.DrainChargeQueue();
    //                         foreach (var item in drainedQueue)
    //                             tempQueue.Enqueue(item);
    //
    //                         // Rebuild the ChargeStationRequests queue with the updated order.
    //                         _queueRepository.RebuildChargeQueue(tempQueue);
    //
    //                         requeued = true;
    //                     }
    //                 }
    //
    //                 // If there was no cutter, just put the current vehicle back at the end, wait a bit, and retry.
    //                 if (!requeued)
    //                 {
    //                     _queueRepository.EnqueueChargeRequest(currentRequest);
    //                     await Task.Delay(100);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    //
    // private async Task ProcessAirQueueAsync()
    // {
    //     while (true)
    //     {
    //         // Checks the first car in the air queue (without removing it) and assign to airRequest.
    //         if (!_queueRepository.TryPeekAirRequest(out var airRequest))
    //         {
    //             // If the queue is empty: wait 100ms and try again.
    //             await Task.Delay(100);
    //             continue;
    //         }
    //
    //         var vehicle = airRequest.Vehicle;
    //
    //         if (!_inflateService.IsMatch(vehicle))
    //         {
    //             _queueRepository.TryDequeueAirRequest(out _); // remove it
    //             continue;
    //         }
    //
    //         if (vehicle.Status != Status.InTreatment && _inflateService.IsMatch(vehicle))
    //         {
    //             // remove from the queue and begin inflating
    //             if (_queueRepository.TryDequeueAirRequest(out var readyRequest))
    //             {
    //                 try
    //                 {
    //                     float price = await _inflateService.TreatAsync(
    //                         readyRequest.Vehicle, readyRequest.DesiredWheelPressures);
    //                     readyRequest.Vehicle.TreatmentsPrice += price;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Console.WriteLine($"Air inflation error: {ex.Message}");
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // can't inflate now - vehicle is already in another treatment
    //             if (_queueRepository.TryDequeueAirRequest(out var currentRequest))
    //             {
    //                 bool requeued = false;
    //
    //                 // Currently in treatment but also needs air
    //                 if (currentRequest.Vehicle.Status == Status.InTreatment &&
    //                     _inflateService.IsMatch(currentRequest.Vehicle))
    //                 {
    //                     // if a next vehicle can cut in
    //                     if (_queueRepository.TryPeekAirRequest(out var nextRequest) &&
    //                         nextRequest.Vehicle.Status != Status.InTreatment &&
    //                         _inflateService.IsMatch(nextRequest.Vehicle))
    //                     {
    //                         // cut in line
    //                         if (_queueRepository.TryDequeueAirRequest(out var cutterRequest))
    //                         {
    //                             try
    //                             {
    //                                 float price = await _inflateService.TreatAsync(
    //                                     cutterRequest.Vehicle, cutterRequest.DesiredWheelPressures);
    //                                 cutterRequest.Vehicle.TreatmentsPrice += price;
    //                             }
    //                             catch (Exception ex)
    //                             {
    //                                 Console.WriteLine($"Air inflation error (cutter): {ex.Message}");
    //                             }
    //                         }
    //
    //                         // Re-queue the original busy vehicle at the front
    //                         var tempQueue = new Queue<AirRequest>();
    //                         tempQueue.Enqueue(currentRequest);
    //
    //                         // Drain and rebuild queue
    //                         var drainedQueue = _queueRepository.DrainAirQueue();
    //                         foreach (var item in drainedQueue)
    //                             tempQueue.Enqueue(item);
    //
    //                         _queueRepository.RebuildAirQueue(tempQueue);
    //
    //                         requeued = true;
    //                     }
    //                 }
    //
    //                 // If there was no cutter, just put the current vehicle back at the end and wait
    //                 if (!requeued)
    //                 {
    //                     _queueRepository.EnqueueAirRequest(currentRequest);
    //                     await Task.Delay(100);
    //                 }
    //             }
    //         }
    //     }
    // }
