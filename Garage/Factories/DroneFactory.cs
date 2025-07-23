using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class DroneFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var droneRequest = request as AddDroneRequest
                           ?? throw new ArgumentException("Invalid request type");

        var drone = new Drone
        {
            ManufacturerName = droneRequest.ManufacturerName,
            ModelName = droneRequest.ModelName,
            LicensePlate = droneRequest.LicensePlate,
            RemainingEnergyPercentage = droneRequest.RemainingEnergyPercentage,
            Engines = droneRequest.Engines,
            Owner = droneRequest.Owner,
            Status = Status.Pending,
            VehicleType = droneRequest.VehicleType,
            TreatmentTypes = droneRequest.TreatmentTypes,
            TreatmentsPrice = droneRequest.TreatmentsPrice,
            ControlOptions = droneRequest.ControlOptions
        };

        return drone;
    }
}