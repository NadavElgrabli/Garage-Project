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
        return request is AddDroneRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        if (request is not AddDroneRequest)
            throw new ArgumentException("Invalid request type for DroneRequestHandler");

        return _factory.CreateVehicle(request);
    }
}