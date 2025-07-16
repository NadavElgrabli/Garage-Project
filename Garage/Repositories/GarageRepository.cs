using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class GarageRepository : IGarageRepository
{
    public void AddVehicleToGarage(Vehicle vehicle)
    {
        InMemoryDatabase.Vehicles.Add(vehicle.LicensePlate, vehicle);
    }

    public void RemoveVehicleFromGarage(Vehicle vehicle)
    {
        InMemoryDatabase.Vehicles.Remove(vehicle.LicensePlate);
    }
    
    public Vehicle? GetVehicleByLicensePlate(string licensePlate)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(licensePlate))
        {
            return InMemoryDatabase.Vehicles[licensePlate];
        }
        return null;
    }

    public List<VehicleInfo> DisplayVehiclesByStatus(Status status)
    {
     return InMemoryDatabase.Vehicles.Where(pair => pair.Value.Status == status).Select(pair => new VehicleInfo
     {
         LicensePlate = pair.Key,
         Owner = pair.Value.Owner.Name,
         VehicleType = pair.Value.VehicleType,
     }).ToList();   
    }
}