using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class GarageRepository : IGarageRepository
{
    public void EnqueVehicle(TreatmentRequest firstRequest, TreatmentRequest secondRequest)
    {
        if (firstRequest.Vehicle.TreatmentTypes.Contains(TreatmentType.Refuel))
        {
            InMemoryDatabase.FuelStationRequests.Enqueue(firstRequest);
        }
        
        else if (firstRequest.Vehicle.TreatmentTypes.Contains(TreatmentType.Recharge))
        {
            InMemoryDatabase.ChargeStationRequests.Enqueue(firstRequest);
        }

        if (secondRequest.Vehicle.TreatmentTypes.Contains(TreatmentType.Inflate))
        {
            InMemoryDatabase.AirStationRequests.Enqueue(secondRequest);
        }
    }
    
    public void AddVehicleToGarage(Vehicle vehicle)
    {
        InMemoryDatabase.Vehicles.Add(vehicle.LicensePlate, vehicle);
    }
    
    
    public Vehicle? GetVehicleByLicensePlate(string licensePlate)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(licensePlate))
        {
            return InMemoryDatabase.Vehicles[licensePlate];
        }
        return null;
    }

    public Task CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(request.LicensePlate))
        {
            throw new InvalidOperationException("Car already in garage");
        }

        if (request.Wheels.Count != request.DesiredWheelPressures.Count)
        {
            throw new InvalidOperationException("Number of wheels does not match");
        }

        if (request.TreatmentTypes.Count is > 2 or < 1)
        {
            throw new InvalidOperationException("Number of treatments must be between 1 or 2");
        }
        
        if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
        {
            throw new InvalidOperationException("Engine has too much electricity");
        }
        
        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
        {
            throw new InvalidOperationException("Cannot have a refuel treatment for an electric car");
        }
        
        return Task.CompletedTask;
    }
    
    
    public Task CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(request.LicensePlate))
        {
            throw new InvalidOperationException("Car already in garage");
        }

        if (request.Wheels.Count != request.DesiredWheelPressures.Count)
        {
            throw new InvalidOperationException("Number of wheels does not match");
        }

        if (request.TreatmentTypes.Count is > 2 or < 1)
        {
            throw new InvalidOperationException("Number of treatments must be between 1 and 2");
        }

        if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
        {
            throw new InvalidOperationException("Engine has too much fuel");
        }

        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
        {
            throw new InvalidOperationException("Cannot have a recharge treatment for a car that runs on fuel");
        }
        
        return Task.CompletedTask;
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