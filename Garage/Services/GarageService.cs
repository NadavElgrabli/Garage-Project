using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class GarageService
{
    private readonly IGarageRepository _garageRepository;

    public GarageService(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public Task CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        return _garageRepository.CheckValidElectricCarInput(request);
    }
    
    public Task CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        return _garageRepository.CheckValidFuelCarInput(request);
    }
    
    public void AddAndEnqueFuelVehicle(FuelRequest fuelRequest, AirRequest airRequest)
    {
        _garageRepository.AddAndEnqueFuelVehicle(fuelRequest, airRequest);
    }

    public void AddAndEnqueElectricVehicle(ChargeRequest chargeRequest, AirRequest airRequest)
    {
        _garageRepository.AddAndEnqueElectricVehicle(chargeRequest, airRequest);
    }
    
    public Vehicle? GetVehicleByLicensePlate(string licensePlate)
    {
        return _garageRepository.GetVehicleByLicensePlate(licensePlate);
    }
    
    public Car CreateElectricCar(AddElectricCarRequest request)
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
    
    public Car CreateFuelCar(AddFuelCarRequest request)
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

    public FuelRequest CreateFuelRequest(Vehicle vehicle, float requestedLiters)
    {
        var fuelRequest = new FuelRequest
        {
            Vehicle = vehicle,
            RequestedLiters = requestedLiters,
        };
        return fuelRequest;
    }

    public ChargeRequest CreateChargeRequest(Vehicle vehicle, float requestedHoursToCharge)
    {
        var chargeRequest = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = requestedHoursToCharge
        };
        return chargeRequest;
    }
    
    public AirRequest CreateAirRequest(Vehicle vehicle, List<float> desiredWheelPressures)
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
}