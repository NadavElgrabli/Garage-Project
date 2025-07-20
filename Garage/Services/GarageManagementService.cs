using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class GarageManagementService
{
    private readonly IGarageRepository _garageRepository;
    private readonly GarageState _garageState;


    public GarageManagementService(IGarageRepository garageRepository, GarageState  garageState)
    {
        _garageRepository = garageRepository;
        _garageState = garageState;
    }
    
    public void InitializeGarage(GarageInit init)
    {
        if (init.Workers <= 0 || init.FuelStations <= 0 || init.AirStations <= 0 || init.ChargingStations <= 0)
            throw new ArgumentException("All values must be positive.");
    
        _garageState.Initialize(
            init.Workers,
            init.FuelStations,
            init.AirStations,
            init.ChargingStations);
    }

    private void AddVehicleToGarage(Vehicle vehicle)
    {
        _garageRepository.AddVehicleToGarage(vehicle);
    }

    private void RemoveVehicleFromGarage(Vehicle vehicle)
    {
        _garageRepository.RemoveVehicleFromGarage(vehicle);
    }
    
    public Vehicle PickUpVehicle(string licensePlate)
    {
        var vehicle = _garageRepository.GetVehicleByLicensePlate(licensePlate);

        if (vehicle is null)
            throw new KeyNotFoundException($"Vehicle with license plate {licensePlate} is not in the garage.");

        if (vehicle.Status != Status.Ready)
            throw new InvalidOperationException($"Vehicle with license plate {licensePlate} is not ready for pickup. Current status: {vehicle.Status}");

        RemoveVehicleFromGarage(vehicle);

        return vehicle;
    }
    
    public Vehicle GetVehicleByLicensePlateOrThrow(string licensePlate)
    {
        var vehicle = _garageRepository.GetVehicleByLicensePlate(licensePlate);

        if (vehicle is null)
            throw new KeyNotFoundException("Vehicle not found");

        return vehicle;
    }

    private Car CreateElectricCar(AddElectricCarRequest request)
    {
        var car = new Car
        {
            ManufacturerName = request.ManufacturerName,
            ModelName = request.ModelName,
            LicensePlate = request.LicensePlate,
            RemainingEnergyPercentage = request.RemainingEnergyPercentage,
            Engine = request.Engine,
            Owner = request.Owner,
            Wheels = request.Wheels,
            Status = Status.Pending,
            VehicleType = request.VehicleType,
            TreatmentTypes = request.TreatmentTypes,
            TreatmentsPrice = request.TreatmentsPrice,
            NumberOfDoors = request.NumberOfDoors,
            Color = request.Color
        };
        return car;
    }
    
    //TODO: Factory design pattern to create cars
    private Car CreateFuelCar(AddFuelCarRequest request)
    {
        var car = new Car
        {
            ManufacturerName = request.ManufacturerName,
            ModelName = request.ModelName,
            LicensePlate = request.LicensePlate,
            RemainingEnergyPercentage = request.RemainingEnergyPercentage,
            Engine = new FuelEngine
            {
                CurrentEnergy = request.Engine.CurrentEnergy,
                MaxEnergy = request.Engine.MaxEnergy,
                FuelType = FuelType.Octane95
            },
            Owner = request.Owner,
            Wheels = request.Wheels,
            Status = Status.Pending,
            VehicleType = request.VehicleType,
            TreatmentTypes = request.TreatmentTypes,
            TreatmentsPrice = request.TreatmentsPrice,
            NumberOfDoors = request.NumberOfDoors,
            Color = request.Color
        };

        return car;
    }

    private FuelRequest CreateFuelRequest(Vehicle vehicle, float requestedLiters)
    {
        var fuelRequest = new FuelRequest
        {
            Vehicle = vehicle,
            RequestedLiters = requestedLiters,
        };
        
        return fuelRequest;
    }

    private ChargeRequest CreateChargeRequest(Vehicle vehicle, float requestedHoursToCharge)
    {
        var chargeRequest = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = requestedHoursToCharge
        };
        return chargeRequest;
    }

    private AirRequest CreateAirRequest(Vehicle vehicle, List<float> desiredWheelPressures)
    {
        var airRequest = new AirRequest
        {
            Vehicle = vehicle,
            DesiredWheelPressures = desiredWheelPressures
        };
        return airRequest;
    }
    
    public List<VehicleInfo> DisplayVehiclesByStatus(Status status)
    {
        return _garageRepository.DisplayVehiclesByStatus(status);
    }
    
    public List<TreatmentRequest> PrepareElectricCar(AddElectricCarRequest request)
    {
        var car = CreateElectricCar(request);
        AddVehicleToGarage(car);

        var treatmentRequests = new List<TreatmentRequest>();
        if (car.TreatmentTypes.Contains(TreatmentType.Recharge))
        {
            var chargeRequest = CreateChargeRequest(car, request.HoursToCharge);
            treatmentRequests.Add(chargeRequest);
        }
        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
        {
            var airRequest = CreateAirRequest(car, request.DesiredWheelPressures);
            treatmentRequests.Add(airRequest);
        }

        return treatmentRequests;
    }
    
    public Car CreateAndAddElectricCarToGarage(AddElectricCarRequest request)
    {
        var car = CreateElectricCar(request);
        AddVehicleToGarage(car);
        return car;
    }
    
    public List<TreatmentRequest> GenerateElectricCarTreatmentRequests(Car car, AddElectricCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Recharge))
            requests.Add(CreateChargeRequest(car, request.HoursToCharge));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures));

        return requests;
    }
    
    public List<TreatmentRequest> PrepareFuelCar(AddFuelCarRequest request)
    {
        var car = CreateFuelCar(request);
        AddVehicleToGarage(car);

        var treatmentRequests = new List<TreatmentRequest>();
        if (car.TreatmentTypes.Contains(TreatmentType.Refuel))
        {
            var fuelRequest = CreateFuelRequest(car, request.LitersToFuel);
            treatmentRequests.Add(fuelRequest);
        }
        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
        {
            var airRequest = CreateAirRequest(car, request.DesiredWheelPressures);
            treatmentRequests.Add(airRequest);
        }
        
        return treatmentRequests;
    }
    
    public Car CreateAndAddFuelCarToGarage(AddFuelCarRequest request)
    {
        var car = CreateFuelCar(request);
        AddVehicleToGarage(car);
        return car;
    }
    
    public List<TreatmentRequest> GenerateFuelCarTreatmentRequests(Car car, AddFuelCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Refuel))
            requests.Add(CreateFuelRequest(car, request.LitersToFuel));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures));

        return requests;
    }
    

}