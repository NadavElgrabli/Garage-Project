using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public interface IGarageRepository
{
    void AddVehicleToGarage(Vehicle vehicle);
    void RemoveVehicleFromGarage(Vehicle vehicle);
    
    Vehicle? GetVehicleByLicensePlate(string licensePlate);
    List <VehicleInfo> DisplayVehiclesByStatus(Status status);
}