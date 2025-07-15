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

    public void AddVehicleToGarage(Vehicle vehicle)
    {
        _garageRepository.AddVehicleToGarage(vehicle);
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
    
    public (TreatmentRequest FirstRequest, TreatmentRequest SecondRequest) PrepareElectricCar(AddElectricCarRequest request)
    {
        var car = CreateElectricCar(request);
        var chargeRequest = CreateChargeRequest(car, request.HoursToCharge);
        var airRequest = CreateAirRequest(car, request.DesiredWheelPressures);
        AddVehicleToGarage(car);

        return (chargeRequest, airRequest);
    }

    public (TreatmentRequest FirstRequest, TreatmentRequest SecondRequest) PrepareFuelCar(AddFuelCarRequest request)
    {
        var car = CreateFuelCar(request);
        var fuelRequest = CreateFuelRequest(car, request.LitersToFuel);
        var airRequest = CreateAirRequest(car, request.DesiredWheelPressures);
        AddVehicleToGarage(car);

        return (fuelRequest, airRequest);
    }

    
    
}