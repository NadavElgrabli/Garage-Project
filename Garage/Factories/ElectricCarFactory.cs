using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class ElectricCarFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var electricCarRequest = request as AddElectricCarRequest
                              ?? throw new ArgumentException("Invalid request type");

        var car = new Car
        {
            ManufacturerName = electricCarRequest.ManufacturerName,
            ModelName = electricCarRequest.ModelName,
            LicensePlate = electricCarRequest.LicensePlate,
            RemainingEnergyPercentage = electricCarRequest.RemainingEnergyPercentage,
            Engine = electricCarRequest.Engine,
            Owner = electricCarRequest.Owner,
            Wheels = electricCarRequest.Wheels,
            Status = Status.Pending,
            VehicleType = electricCarRequest.VehicleType,
            TreatmentTypes = electricCarRequest.TreatmentTypes,
            TreatmentsPrice = electricCarRequest.TreatmentsPrice,
            NumberOfDoors = electricCarRequest.NumberOfDoors,
            Color = electricCarRequest.Color
        };

        return car;
    }
}
