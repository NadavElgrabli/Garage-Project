using Garage.Models;

namespace Garage.Factories;

public interface IVehicleFactory
{
    Vehicle CreateVehicle(Vehicle vehicle);
}