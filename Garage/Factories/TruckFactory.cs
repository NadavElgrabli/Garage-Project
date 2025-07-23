using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class TruckFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var truckRequest = request as AddTruckRequest
                                 ?? throw new ArgumentException("Invalid request type");

        var truck = new Truck
        {
            ManufacturerName = truckRequest.ManufacturerName,
            ModelName = truckRequest.ModelName,
            LicensePlate = truckRequest.LicensePlate,
            RemainingEnergyPercentage = truckRequest.RemainingEnergyPercentage,
            // Engine = new FuelEngine
            // {
            //     CurrentEnergy = truckRequest.Engine.CurrentEnergy,
            //     MaxEnergy = truckRequest.Engine.MaxEnergy,
            //     FuelType = FuelType.Solar
            // },
            Engine = truckRequest.Engine,
            Owner = truckRequest.Owner,
            Wheels = truckRequest.Wheels,
            Status = Status.Pending,
            VehicleType = truckRequest.VehicleType,
            TreatmentTypes = truckRequest.TreatmentTypes,
            TreatmentsPrice = truckRequest.TreatmentsPrice,
            CarryDangerousSubstances = truckRequest.CarryDangerousSubstances,
            CargoVolume = truckRequest.CargoVolume
        };

        return truck;
    }
}