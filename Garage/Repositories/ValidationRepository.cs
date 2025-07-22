using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class ValidationRepository : IValidationRepository
{
    private readonly GarageState _garageState;
    private readonly InMemoryDatabase _db;

    public ValidationRepository(GarageState garageState,  InMemoryDatabase db)
    {
        _garageState  = garageState;
        _db = db;
    }
    public void CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        var errors = new List<string>();

        ValidateCommonCarInput(request, request.DesiredWheelPressures, errors);

        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
            errors.Add("Cannot have a refuel treatment for an electric car.");

        if (request.Engine.MaxEnergy > 2.8)
            errors.Add("Maximum charge is above 2.8 which is the maximum.");

        if (errors.Any())
            throw new InvalidOperationException("Invalid input:\n- " + string.Join("\n- ", errors));
    }


    public void CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        var errors = new List<string>();

        ValidateCommonCarInput(request, request.DesiredWheelPressures, errors);

        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
            errors.Add("Cannot have a recharge treatment for a fuel car.");

        if (request.Engine.MaxEnergy > 50)
            errors.Add("Maximum fuel tank capacity is 50. Current value: is above 50.");

        if (errors.Any())
            throw new InvalidOperationException("Invalid input:\n- " + string.Join("\n- ", errors));
    }

    
    private void ValidateCommonCarInput(Vehicle request, List<float> desiredWheelPressures, List<string> errors)
    {
        if (!_garageState.IsInitialized)
            errors.Add("Garage must be initialized before adding vehicles.");

        if (_db.Vehicles.ContainsKey(request.LicensePlate))
            errors.Add("Car already in garage.");

        if (request.TreatmentTypes.Count is > 2 or < 1)
            errors.Add("Number of treatments must be either 1 or 2.");

        if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
            errors.Add("Engine has too much energy.");

        if (request.Wheels.Count != desiredWheelPressures.Count)
            errors.Add("Number of wheels does not match the number of desired wheel pressures.");

        if (request.Wheels.Count != 4 || desiredWheelPressures.Count != 4)
        {
            errors.Add("Number of wheels for cars must equal 4, and the desired pressures must contain 4 pressures.");
        }

        for (int i = 0; i < request.Wheels.Count; i++)
        {
            if (request.Wheels[i].CurrentPressure > desiredWheelPressures[i])
                errors.Add("One of the wheel's current pressure is above the desired pressure.");

            if (request.Wheels[i].CurrentPressure > 30)
                errors.Add("One of the wheel's current pressure is above the maximum (30).");
        }
    }
}