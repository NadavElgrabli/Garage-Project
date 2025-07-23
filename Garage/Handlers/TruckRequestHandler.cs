using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class TruckRequestHandler : IVehicleRequestHandler
{
    private readonly TruckFactory _factory;

    public TruckRequestHandler(TruckFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        return request is AddTruckRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        if (request is not AddTruckRequest)
            throw new ArgumentException("Invalid request type for TruckRequestHandler");

        return _factory.CreateVehicle(request);
    }
}