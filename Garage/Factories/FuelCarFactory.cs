using Garage.Enums;
using Garage.Models;

namespace Garage.Factories;

public class FuelCarFactory : IVehicleFactory
{
    public Vehicle CreateVehicle(Vehicle request)
    {
        var fuelCarRequest = request as AddFuelCarRequest
                              ?? throw new ArgumentException("Invalid request type");

        var car =  new Car
        {
            ManufacturerName = fuelCarRequest.ManufacturerName,
            ModelName = fuelCarRequest.ModelName,
            LicensePlate = fuelCarRequest.LicensePlate,
            RemainingEnergyPercentage = fuelCarRequest.RemainingEnergyPercentage,
            // Engine = new FuelEngine // Error, current energy sint updated after RefuelService
            // {
            //     CurrentEnergy = fuelCarRequest.Engine.CurrentEnergy,
            //     MaxEnergy = fuelCarRequest.Engine.MaxEnergy,
            //     FuelType = FuelType.Octane95
            // },
            Engine = fuelCarRequest.Engine, //no errors if we have this in current energy
            Owner = fuelCarRequest.Owner,
            Wheels = fuelCarRequest.Wheels,
            Status = Status.Pending,
            VehicleType = fuelCarRequest.VehicleType,
            TreatmentTypes = fuelCarRequest.TreatmentTypes,
            TreatmentsPrice = fuelCarRequest.TreatmentsPrice,
            NumberOfDoors = fuelCarRequest.NumberOfDoors,
            Color = fuelCarRequest.Color
        };

        return car;
    }
}