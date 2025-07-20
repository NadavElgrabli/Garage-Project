using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class GarageRepository : IGarageRepository
{
    private readonly InMemoryDatabase _db;

    public GarageRepository(InMemoryDatabase db)
    {
        _db = db;
    }
    
    public void AddVehicleToGarage(Vehicle vehicle)
    {
        _db.Vehicles.Add(vehicle.LicensePlate, vehicle);
    }

    public void RemoveVehicleFromGarage(Vehicle vehicle)
    {
        _db.Vehicles.Remove(vehicle.LicensePlate);
    }
    
    public Vehicle? GetVehicleByLicensePlate(string licensePlate)
    {
        if (_db.Vehicles.ContainsKey(licensePlate))
        {
            return _db.Vehicles[licensePlate];
        }
        return null;
    }

    public List<VehicleInfo> DisplayVehiclesByStatus(Status status)
    {
     return _db.Vehicles.Where(pair => pair.Value.Status == status).Select(pair => new VehicleInfo
     {
         LicensePlate = pair.Key,
         Owner = pair.Value.Owner.Name,
         VehicleType = pair.Value.VehicleType,
     }).ToList();   
    }
}