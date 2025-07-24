using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class ElectricMotorcycleFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var electricMotorcycleRequest = request as AddElectricMotorcycleRequest
                                    ?? throw new ArgumentException("Invalid request type");

        var motorcycle =  new Motorcycle
        {
            ManufacturerName = electricMotorcycleRequest.ManufacturerName,
            ModelName = electricMotorcycleRequest.ModelName,
            LicensePlate = electricMotorcycleRequest.LicensePlate,
            RemainingEnergyPercentage = electricMotorcycleRequest.RemainingEnergyPercentage,
            Engine = electricMotorcycleRequest.Engine, 
            Owner = electricMotorcycleRequest.Owner,
            Wheels = electricMotorcycleRequest.Wheels,
            Status = Status.Pending,
            VehicleType = VehicleType.Motorcycle,
            TreatmentTypes = electricMotorcycleRequest.TreatmentTypes,
            TreatmentsPrice = electricMotorcycleRequest.TreatmentsPrice,
            EngineVolumeCC = electricMotorcycleRequest.EngineVolumeCC,
            LicenseType = electricMotorcycleRequest.LicenseType,
        };

        return motorcycle;
    }
}