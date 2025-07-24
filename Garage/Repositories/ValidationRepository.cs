using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class ValidationRepository : IValidationRepository
{
    private readonly GarageState _garageState;
    private readonly InMemoryDatabase _db;

    private readonly float _maxElectricCarEnergy;
    private readonly float _maxFuelCarEnergy;
    private readonly float _maxFuelMotorcycleEnergy;
    private readonly float _maxElectricMotorcycleEnergy;
    private readonly float _maxTruckEnergy;
    private readonly float _maxDroneEnergy;
    
    private readonly int _minTreatments;
    private readonly int _maxTreatments;
    
    private readonly int _numberOfCarWheels;
    private readonly int _numberOfTruckWheels;
    private readonly int _numberOfMotorcycleWheels;
    
    private readonly float _carWheelMaxPressure;
    private readonly float _truckWheelMaxPressure;
    private readonly float _motorcycleWheelMaxPressure;

    public ValidationRepository(GarageState garageState,  InMemoryDatabase db, IConfiguration  config)
    {
        _garageState  = garageState;
        _db = db;

        _maxElectricCarEnergy = config.GetValue<float>("Validation:MaxElectricCarEnergy");
        _maxFuelCarEnergy = config.GetValue<float>("Validation:MaxFuelCarEnergy");
        _maxTruckEnergy = config.GetValue<float>("Validation:MaxTruckEnergy");
        _maxFuelMotorcycleEnergy =  config.GetValue<float>("Validation:MaxFuelMotorcycleEnergy");
        _maxElectricMotorcycleEnergy =  config.GetValue<float>("Validation:MaxElectricMotorcycleEnergy");
        _maxDroneEnergy = config.GetValue<float>("Validation:MaxDroneEnergy");
        _minTreatments = config.GetValue<int>("Validation:MinimumNumberOfTreatments");
        _maxTreatments = config.GetValue<int>("Validation:MaximumNumberOfTreatments");
        _numberOfCarWheels = config.GetValue<int>("Validation:NumberOfCarWheels");
        _numberOfMotorcycleWheels =  config.GetValue<int>("Validation:NumberOfMotorcycleWheels");
        _numberOfTruckWheels = config.GetValue<int>("Validation:NumberOfTruckWheels");
        _carWheelMaxPressure = config.GetValue<float>("Validation:CarWheelMaxPressure");
        _motorcycleWheelMaxPressure =  config.GetValue<float>("Validation:MotorcycleWheelMaxPressure");
        _truckWheelMaxPressure = config.GetValue<float>("Validation:TruckWheelMaxPressure");
    }
    
    public void CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxElectricCarEnergy, errors, errorData);
        ValidateWheels(
            request.Wheels,
            request.DesiredWheelPressures,
            _numberOfCarWheels,
            _carWheelMaxPressure,
            errors,
            errorData
        );
        
        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
            errors.Add("Cannot have a refuel treatment for an electric car.");

        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
                ex.Data[pair.Key] = pair.Value;

            throw ex;
        }
    }
    
    public void CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxFuelCarEnergy, errors, errorData);
        ValidateWheels(
            request.Wheels,
            request.DesiredWheelPressures,
            _numberOfCarWheels,
            _carWheelMaxPressure,
            errors,
            errorData
        );
        
        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
            errors.Add("Cannot have a recharge treatment for a fuel car.");

        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
            {
                ex.Data[pair.Key] = pair.Value;
            }

            throw ex;
        }
    }
    
    public void CheckValidFuelMotorcycleInput(AddFuelMotorcycleRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxFuelMotorcycleEnergy, errors, errorData);
        ValidateWheels(
            request.Wheels,
            request.DesiredWheelPressures,
            _numberOfMotorcycleWheels,
            _motorcycleWheelMaxPressure,
            errors,
            errorData
        );
        
        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
            errors.Add("Cannot have a recharge treatment for a fuel motorcycle.");

        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
            {
                ex.Data[pair.Key] = pair.Value;
            }

            throw ex;
        }
    }
    
    public void CheckValidElectricMotorcycleInput(AddElectricMotorcycleRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxElectricMotorcycleEnergy, errors, errorData);
        ValidateWheels(
            request.Wheels,
            request.DesiredWheelPressures,
            _numberOfMotorcycleWheels,
            _motorcycleWheelMaxPressure,
            errors,
            errorData
        );
        
        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
            errors.Add("Cannot have a refuel treatment for an electric motorcycle.");

        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
            {
                ex.Data[pair.Key] = pair.Value;
            }

            throw ex;
        }
    }

    private void ValidateCommonVehicleInput(Vehicle request, List<string> errors)
    {
        if (!_garageState.IsInitialized)
            errors.Add("Garage must be initialized before adding vehicles.");

        if (_db.Vehicles.ContainsKey(request.LicensePlate))
            errors.Add("Car already in garage.");

        if (request.TreatmentTypes.Count > _maxTreatments ||
            request.TreatmentTypes.Count < _minTreatments)
            errors.Add("Number of treatments must be either 1 or 2.");
    }
    
    public void CheckValidTruckInput(AddTruckRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxTruckEnergy, errors, errorData);
        ValidateWheels(
            request.Wheels,
            request.DesiredWheelPressures,
            _numberOfTruckWheels,
            _truckWheelMaxPressure,
            errors,
            errorData
        );

        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
            errors.Add("Cannot have a recharge treatment for a truck.");
        
        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
                ex.Data[pair.Key] = pair.Value;

            throw ex;
        }
    }

    public void CheckValidDroneInput(AddDroneRequest request)
    {
        var errors = new List<string>();
        var errorData = new Dictionary<string, object>();

        ValidateCommonVehicleInput(request, errors);
        ValidateEngine(request.Engine, _maxDroneEnergy, errors, errorData);

        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
            errors.Add("Cannot have a refuel treatment for a drone.");

        if (errors.Any())
        {
            var message = "Invalid input:\n- " + string.Join("\n- ", errors);
            var ex = new InvalidOperationException(message);

            foreach (var pair in errorData)
                ex.Data[pair.Key] = pair.Value;

            throw ex;
        }
    }

    
    private void ValidateWheels(List<Wheel> wheels, List<float> desiredPressures, int expectedCount, float maxPressure, List<string> errors, Dictionary<string, object> errorData)
    {
        if (wheels.Count != desiredPressures.Count)
            errors.Add("Number of wheels does not match the number of desired wheel pressures.");

        if (wheels.Count != expectedCount || desiredPressures.Count != expectedCount)
            errors.Add($"Number of wheels must equal {expectedCount}, and the desired pressures must contain {expectedCount} values.");

        for (int i = 0; i < wheels.Count; i++)
        {
            var current = wheels[i].CurrentPressure;
            var desired = desiredPressures[i];

            if (current > desired)
            {
                errors.Add($"Wheel {i}: current pressure above desired.");
                errorData[$"Wheel_{i}_CurrentVsDesired"] = new { Current = current, Desired = desired };
            }

            if (current > maxPressure)
            {
                errors.Add($"Wheel {i}: pressure above max ({maxPressure}).");
                errorData[$"Wheel_{i}_Pressure_Too_High"] = current;
            }
        }
    }
    
    private void ValidateEngine(Engine engine, float maxEnergy, List<string> errors, Dictionary<string, object> errorData)
    {
        if (engine.CurrentEnergy > engine.MaxEnergy)
        {
            errors.Add("Engine's current energy is higher than its maximum capacity.");
            errorData["Engine_CurrentEnergy"] = engine.CurrentEnergy;
        }

        if (engine.MaxEnergy > maxEnergy)
        {
            errors.Add("Engine's max energy is above the allowed maximum of energy.");
            errorData["Engine_MaxEnergy"] = engine.MaxEnergy;
        }
    }
}


// private void ValidateCommonCarInput(Vehicle request, List<float> desiredWheelPressures, List<string> errors, Dictionary<string, object> errorData)
// {
//     if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
//     {
//         errors.Add("Engine's current energy is higher than the maximum capacity.");
//         errorData["CurrentEngineEnergy"] = request.Engine.CurrentEnergy;
//     }
//     
//     if (request.Wheels.Count != desiredWheelPressures.Count)
//         errors.Add("Number of wheels does not match the number of desired wheel pressures.");
//
//     if (request.Wheels.Count != _numberOfCarWheels ||
//         desiredWheelPressures.Count != _numberOfCarWheels)
//         errors.Add("Number of wheels for cars must equal 4, and the desired pressures must contain 4 pressures.");
//
//     for (int i = 0; i < request.Wheels.Count; i++)
//     {
//         var current = request.Wheels[i].CurrentPressure;
//         var desired = desiredWheelPressures[i];
//
//         if (current > desired)
//         {
//             errors.Add($"Wheel at index {i} has current pressure above desired pressure.");
//             errorData[$"Wheel[{i}].CurrentPressureVsDesired"] = new { Current = current, Desired = desired };
//         }
//         
//         if (current > _carWheelMaxPressure)
//         {
//             errors.Add($"Wheel at index {i} has current pressure above the maximum (30).");
//             errorData[$"Wheel[{i}].CurrentPressure"] = current;
//         }
//     }
// }




// public void CheckValidElectricCarInput(AddElectricCarRequest request)
// {
//     var errors = new List<string>();
//
//     ValidateCommonVehicleInput(request, errors);
//     ValidateCommonCarInput(request, request.DesiredWheelPressures, errors);
//
//     if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
//         errors.Add("Cannot have a refuel treatment for an electric car.");
//
//     //TODO: use exception params.
//     if (request.Engine.MaxEnergy > 2.8)
//         errors.Add("Maximum charge is above 2.8 which is the maximum.");
//
//     if (errors.Any())
//         throw new InvalidOperationException("Invalid input:\n- " + string.Join("\n- ", errors));
// }


// public void CheckValidFuelCarInput(AddFuelCarRequest request)
// {
//     var errors = new List<string>();
//
//     ValidateCommonVehicleInput(request, errors);
//     ValidateCommonCarInput(request, request.DesiredWheelPressures, errors);
//
//     if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
//         errors.Add("Cannot have a recharge treatment for a fuel car.");
//
//     //TODO: use exception params here to save the current maximum value.
//     if (request.Engine.MaxEnergy > 50)
//         errors.Add("Maximum fuel tank capacity is 50. Current maximum value is above 50.");
//
//     if (errors.Any())
//         throw new InvalidOperationException("Invalid input:\n- " + string.Join("\n- ", errors));
// }



// private void ValidateCommonVehicleInput(Vehicle request, List<string> errors)
// {
//     if (!_garageState.IsInitialized)
//         errors.Add("Garage must be initialized before adding vehicles.");
//
//     if (_db.Vehicles.ContainsKey(request.LicensePlate))
//         errors.Add("Car already in garage.");
//
//     if (request.TreatmentTypes.Count is > 2 or < 1)
//         errors.Add("Number of treatments must be either 1 or 2.");
//
//     //TODO: use exception params, save current energy as .Data
//     if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
//         errors.Add("Engine's current energy is higher than the maximum capacity.");
// }



// private void ValidateCommonCarInput(Vehicle request, List<float> desiredWheelPressures, List<string> errors)
// {
//     if (request.Wheels.Count != desiredWheelPressures.Count)
//         errors.Add("Number of wheels does not match the number of desired wheel pressures.");
//
//     if (request.Wheels.Count != 4 || desiredWheelPressures.Count != 4)
//     {
//         errors.Add("Number of wheels for cars must equal 4, and the desired pressures must contain 4 pressures.");
//     }
//
//     for (int i = 0; i < request.Wheels.Count; i++)
//     {
//         if (request.Wheels[i].CurrentPressure > desiredWheelPressures[i])
//             errors.Add("One of the wheel's current pressure is above the desired pressure.");
//
//         if (request.Wheels[i].CurrentPressure > 30)
//             errors.Add("One of the wheel's current pressure is above the maximum (30).");
//     }
// }


// public void CheckValidTruckInput(AddTruckRequest request)
// {
//     var errors = new List<string>();
//     
//     ValidateCommonVehicleInput(request, errors);
//     
//     if (request.Wheels.Count != request.DesiredWheelPressures.Count)
//         errors.Add("Number of wheels does not match the number of desired wheel pressures.");
//
//     if (request.Wheels.Count != 16 || request.DesiredWheelPressures.Count != 16)
//     {
//         errors.Add("Number of wheels for trucks must equal 16, and the desired pressures must contain 16 pressures.");
//     }
//
//     for (int i = 0; i < request.Wheels.Count; i++)
//     {
//         if (request.Wheels[i].CurrentPressure > request.DesiredWheelPressures[i])
//             errors.Add("One of the wheel's current pressure is above the desired pressure.");
//
//         if (request.Wheels[i].CurrentPressure > 26)
//             errors.Add("One of the wheel's current pressure is above the maximum (26).");
//     }
//     
//     if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
//         errors.Add("Cannot have a recharge treatment for a truck.");
//
//     //TODO: use .Data
//     if (request.Engine.MaxEnergy > 110)
//         errors.Add("Maximum fuel tank capacity is 50. Current value is above 50.");
//     
//     if (errors.Any())
//         throw new InvalidOperationException("Invalid input:\n- " + string.Join("\n- ", errors));
// }