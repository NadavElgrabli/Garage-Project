using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RefuelService : ITreatmentService
{
    public async Task<float> TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not FuelRequest fuelRequest)
            throw new ArgumentException("Invalid data type. Expected FuelRequest.");

        float litersToFuel = fuelRequest.RequestedLiters;

        await GarageState.FuelStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;

            if (vehicle.Engine.CurrentEnergy == vehicle.Engine.MaxEnergy)
                throw new Exception("Engine fully fueled already");

            float missingFuelAmount = vehicle.Engine.MaxEnergy - vehicle.Engine.CurrentEnergy;
            float totalPrice = missingFuelAmount * 5;
            int milliseconds = (int)missingFuelAmount * 250;

            await Task.Delay(milliseconds);

            if (vehicle.Engine.CurrentEnergy + litersToFuel > vehicle.Engine.MaxEnergy)
                totalPrice += 25; // Made a mess, spilled fuel, cost to clean is

            vehicle.Engine.CurrentEnergy = vehicle.Engine.MaxEnergy;
            vehicle.TreatmentTypes.Remove(TreatmentType.Refuel);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;

            return totalPrice;
        }
        finally
        {
            GarageState.WorkersSemaphore.Release();
            GarageState.FuelStationsRequestsSemaphore.Release();
        }
    }

    public bool IsMatch(Vehicle vehicle) =>
        vehicle.TreatmentTypes.Contains(TreatmentType.Refuel);

    public TreatmentType GetTreatmentType()
    {
        return TreatmentType.Refuel;
    }
}
