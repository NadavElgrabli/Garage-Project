using Garage.Data;
using Garage.Enums;
using Garage.Handlers;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class GarageManagementService
{
    private readonly IGarageRepository _garageRepository;
    private readonly GarageState _garageState;
    private readonly IEnumerable<IVehicleRequestHandler> _handlers;
    
    public GarageManagementService(
        IGarageRepository garageRepository,
        GarageState  garageState,
        IEnumerable<IVehicleRequestHandler> handlers)
    {
        _garageRepository = garageRepository;
        _garageState = garageState;
        _handlers = handlers;
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
        var handler = _handlers.FirstOrDefault(h => h.IsMatch(request))
                      ?? throw new InvalidOperationException("No handler found for this vehicle type");

        return handler.Handle(request);
    }

    public Vehicle CreateAndAddVehicleToGarage(Vehicle vehicle)
    {
        var v = CreateVehicle(vehicle);
        AddVehicleToGarage(v);
        return v;
    }
    
    public List<VehicleInfo> DisplayVehiclesByStatus(Status status)
    {
        return _garageRepository.DisplayVehiclesByStatus(status);
    }

    private FuelRequest CreateFuelRequest(Vehicle vehicle, float requestedLiters, Engine engine)
    {
        var fuelRequest = new FuelRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedLiters = requestedLiters,
        };
        
        return fuelRequest;
    }

    private ChargeRequest CreateChargeRequest(Vehicle vehicle, float requestedHoursToCharge, Engine engine)
    {
        var chargeRequest = new ChargeRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedHoursToCharge = requestedHoursToCharge
        };
        
        return chargeRequest;
    }

    private AirRequest CreateAirRequest(Vehicle vehicle, List<float> desiredWheelPressures, List<Wheel> wheels)
    {
        var airRequest = new AirRequest
        {
            Vehicle = vehicle,
            Wheels = wheels,
            DesiredWheelPressures = desiredWheelPressures
        };
        
        return airRequest;
    }
    
    public List<TreatmentRequest> GenerateElectricCarTreatmentRequests(Vehicle car, AddElectricCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Recharge))
            requests.Add(CreateChargeRequest(car, request.HoursToCharge, request.Engine));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures, request.Wheels));

        return requests;
    }
    
    public List<TreatmentRequest> GenerateFuelCarTreatmentRequests(Vehicle car, AddFuelCarRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (car.TreatmentTypes.Contains(TreatmentType.Refuel))
            requests.Add(CreateFuelRequest(car, request.LitersToFuel, request.Engine));

        if (car.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(car, request.DesiredWheelPressures, request.Wheels));

        return requests;
    }
    
    public List<TreatmentRequest> GenerateFuelMotorcycleTreatmentRequests(Vehicle motorcycle, AddFuelMotorcycleRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (motorcycle.TreatmentTypes.Contains(TreatmentType.Refuel))
            requests.Add(CreateFuelRequest(motorcycle, request.LitersToFuel, request.Engine));

        if (motorcycle.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(motorcycle, request.DesiredWheelPressures, request.Wheels));

        return requests;
    }
    
    public List<TreatmentRequest> GenerateTruckTreatmentRequests(Vehicle truck, AddTruckRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (truck.TreatmentTypes.Contains(TreatmentType.Refuel))
            requests.Add(CreateFuelRequest(truck, request.LitersToFuel, request.Engine));

        if (truck.TreatmentTypes.Contains(TreatmentType.Inflate))
            requests.Add(CreateAirRequest(truck, request.DesiredWheelPressures, request.Wheels));

        return requests;
    }
    
    public List<TreatmentRequest> GenerateDroneTreatmentRequest(Vehicle drone, AddDroneRequest request)
    {
        var requests = new List<TreatmentRequest>();

        if (drone.TreatmentTypes.Contains(TreatmentType.Recharge))
            requests.Add(CreateChargeRequest(drone, request.DesiredHoursToCharge, request.Engine));

        return requests;
    }
}