using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RefuelService : ITreatmentService
{
    private readonly GarageState _garageState;
    private readonly float _fuelPricePerLiter;
    private readonly float _spillCleanupCost;
    private readonly int _millisecondsPerLiter;

    public RefuelService(GarageState garageState, IConfiguration config)
    {
        _garageState = garageState;
        
        _fuelPricePerLiter = config.GetValue<float>(ConfigurationKeys.Refuel.FuelPricePerLiter);
        _spillCleanupCost = config.GetValue<float>(ConfigurationKeys.Refuel.SpillCleanupCost);
        _millisecondsPerLiter = config.GetValue<int>(ConfigurationKeys.Refuel.MillisecondsPerLiter);
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
            float missingFuelAmount = fuelRequest.Engine.MaxEnergy - fuelRequest.Engine.CurrentEnergy;
            float totalPrice = litersToFuel * _fuelPricePerLiter;
            
            if (fuelRequest.Engine.CurrentEnergy + litersToFuel > fuelRequest.Engine.MaxEnergy)
            {
                totalPrice = missingFuelAmount * _fuelPricePerLiter;
                totalPrice += _spillCleanupCost; 
                int timeToFullyRefuel = (int)(missingFuelAmount) * _millisecondsPerLiter;
                
                await Task.Delay(timeToFullyRefuel);
                
                fuelRequest.Engine.CurrentEnergy = fuelRequest.Engine.MaxEnergy;
            }
            else
            {
                int milliseconds = (int)litersToFuel * _millisecondsPerLiter;
                fuelRequest.Engine.CurrentEnergy += litersToFuel;
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
        var treatmentType = TreatmentType.Refuel;
        
        return treatmentType;
    }
}
