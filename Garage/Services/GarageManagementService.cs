using Garage.Data;
using Garage.Enums;
using Garage.Factories;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class GarageManagementService
{
    private readonly IGarageRepository _garageRepository;
    private readonly GarageState _garageState;
    private readonly Dictionary<Type, IVehicleFactory> _vehicleFactories;
    
    public GarageManagementService(
        IGarageRepository garageRepository,
        GarageState  garageState,
        Dictionary<Type, IVehicleFactory> vehicleFactories)
    {
        _garageRepository = garageRepository;
        _garageState = garageState;
        _vehicleFactories = vehicleFactories;
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
            throw new KeyNotFoundException("Vehicle's license plate is not in the garage.");

        if (vehicle.Status != Status.Ready)
            throw new InvalidOperationException("Vehicle's status isn't ready, so it's not ready for pickup.");

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
    
    private Vehicle CreateVehicle(Vehicle request)
    {
        var requestType = request.GetType();

        if (!_vehicleFactories.TryGetValue(requestType, out var factory))
            throw new InvalidOperationException("No factory registered for request type");

        return factory.CreateVehicle(request);
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
    
    public List<TreatmentRequest> GenerateElectricCarTreatmentRequests(Vehicle car, AddElectricCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Recharge))
            requests.Add(CreateChargeRequest(car, request.HoursToCharge));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures));

        return requests;
    }
    
    public Vehicle CreateAndAddVehicleToGarage(Vehicle vehicle)
    {
        var car = CreateVehicle(vehicle);
        AddVehicleToGarage(car);
        return car;
    }
    
    public List<TreatmentRequest> GenerateFuelCarTreatmentRequests(Vehicle car, AddFuelCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Refuel))
            requests.Add(CreateFuelRequest(car, request.LitersToFuel));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures));

        return requests;
    }
}