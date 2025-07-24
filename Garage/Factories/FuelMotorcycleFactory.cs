using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class FuelMotorcycleFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var fuelMotorcycleRequest = request as AddFuelMotorcycleRequest
                             ?? throw new ArgumentException("Invalid request type");

        var motorcycle =  new Motorcycle
        {
            ManufacturerName = fuelMotorcycleRequest.ManufacturerName,
            ModelName = fuelMotorcycleRequest.ModelName,
            LicensePlate = fuelMotorcycleRequest.LicensePlate,
            RemainingEnergyPercentage = fuelMotorcycleRequest.RemainingEnergyPercentage,
            Engine = fuelMotorcycleRequest.Engine, 
            Owner = fuelMotorcycleRequest.Owner,
            Wheels = fuelMotorcycleRequest.Wheels,
            Status = Status.Pending,
            VehicleType = fuelMotorcycleRequest.VehicleType,
            TreatmentTypes = fuelMotorcycleRequest.TreatmentTypes,
            TreatmentsPrice = fuelMotorcycleRequest.TreatmentsPrice,
            EngineVolumeCC = fuelMotorcycleRequest.EngineVolumeCC,
            LicenseType = fuelMotorcycleRequest.LicenseType,
        };

        return motorcycle;
    }
}