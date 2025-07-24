using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class DroneRequestHandler : IVehicleRequestHandler
{
    private readonly DroneFactory _factory;

    public DroneRequestHandler(DroneFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        var isAddDroneRequest = request is AddDroneRequest;
        
        return isAddDroneRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);
        
        return vehicle;
    }
}