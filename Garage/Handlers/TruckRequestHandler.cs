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
        var isTruckRequest = request is AddTruckRequest;
        
        return isTruckRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);
        
        return vehicle;
    }
}