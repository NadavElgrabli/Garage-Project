using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RefuelService : ITreatmentService
{
    private readonly GarageState _garageState;

    public RefuelService(GarageState garageState)
    {
        _garageState = garageState;
    }
    
    public async Task TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not FuelRequest fuelRequest)
            throw new ArgumentException("Invalid data type. Expected FuelRequest.");

        float litersToFuel = fuelRequest.RequestedLiters;

        await _garageState.FuelStationsRequestsSemaphore.WaitAsync();
        await _garageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;
            float missingFuelAmount = vehicle.Engine.MaxEnergy - vehicle.Engine.CurrentEnergy;
            float totalPrice = litersToFuel * 5;
            
            // Overflow of fuel
            if (vehicle.Engine.CurrentEnergy + litersToFuel > vehicle.Engine.MaxEnergy)
            {
                totalPrice = missingFuelAmount * 5;
                totalPrice += 25; // Made a mess, spilled fuel, cost to clean is
                // We will wait the amount of time it takes to fill up the missingFuelAmount
                int timeToFullyRefuel = (int)(missingFuelAmount) * 250;
                await Task.Delay(timeToFullyRefuel);
                vehicle.Engine.CurrentEnergy = vehicle.Engine.MaxEnergy;
            }
            else
            {
                int milliseconds = (int)litersToFuel * 250;
                vehicle.Engine.CurrentEnergy += litersToFuel;
                await Task.Delay(milliseconds);
            }
            vehicle.TreatmentsPrice += totalPrice;
            vehicle.TreatmentTypes.Remove(TreatmentType.Refuel);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;

        }
        finally
        {
            _garageState.WorkersSemaphore.Release();
            _garageState.FuelStationsRequestsSemaphore.Release();
        }
    }

    public TreatmentType GetTreatmentType()
    {
        return TreatmentType.Refuel;
    }
}
