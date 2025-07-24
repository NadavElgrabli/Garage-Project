using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class FuelCarRequestHandler : IVehicleRequestHandler
{
    private readonly FuelCarFactory _factory;

    public FuelCarRequestHandler(FuelCarFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        var isAddFuelCarRequest = request is AddFuelCarRequest;
        
        return isAddFuelCarRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);
        
        return vehicle;
    }
}