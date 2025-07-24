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
        
        _maxElectricCarEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxElectricCarEnergy);
        _maxFuelCarEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxFuelCarEnergy);
        _maxTruckEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxTruckEnergy);
        _maxFuelMotorcycleEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxFuelMotorcycleEnergy);
        _maxElectricMotorcycleEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxElectricMotorcycleEnergy);
        _maxDroneEnergy = config.GetValue<float>(ConfigurationKeys.Validation.MaxDroneEnergy);

        _minTreatments = config.GetValue<int>(ConfigurationKeys.Validation.MinimumNumberOfTreatments);
        _maxTreatments = config.GetValue<int>(ConfigurationKeys.Validation.MaximumNumberOfTreatments);

        _numberOfCarWheels = config.GetValue<int>(ConfigurationKeys.Validation.NumberOfCarWheels);
        _numberOfMotorcycleWheels = config.GetValue<int>(ConfigurationKeys.Validation.NumberOfMotorcycleWheels);
        _numberOfTruckWheels = config.GetValue<int>(ConfigurationKeys.Validation.NumberOfTruckWheels);

        _carWheelMaxPressure = config.GetValue<float>(ConfigurationKeys.Validation.CarWheelMaxPressure);
        _motorcycleWheelMaxPressure = config.GetValue<float>(ConfigurationKeys.Validation.MotorcycleWheelMaxPressure);
        _truckWheelMaxPressure = config.GetValue<float>(ConfigurationKeys.Validation.TruckWheelMaxPressure);
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
                errors.Add("Wheel current pressure above desired.");
                errorData[$"Wheel_{i}_CurrentVsDesired"] = new { Current = current, Desired = desired };
            }

            if (current > maxPressure)
            {
                errors.Add("Wheel pressure above max).");
                errorData[$"Wheel_{i}_Pressure_Too_High"] = current;
            }
        }
    }
    
    private void ValidateEngine(Engine engine, float maxEnergy, List<string> errors, Dictionary<string, object> errorData)
    {
        if (engine.CurrentEnergy > engine.MaxEnergy)
        {
            errors.Add("Engine's current energy is higher than its maximum capacity.");
            errorData["engineCurrentEnergy"] = engine.CurrentEnergy;
        }

        if (engine.MaxEnergy > maxEnergy)
        {
            errors.Add("Engine's max energy is above the allowed maximum of energy.");
            errorData["engineMaxEnergy"] = engine.MaxEnergy;
        }
    }
}