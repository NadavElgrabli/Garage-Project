using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class FuelMotorcycleRequestHandler : IVehicleRequestHandler
{
    private readonly FuelMotorcycleFactory _factory;

    public FuelMotorcycleRequestHandler(FuelMotorcycleFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        var isAddFuelMotorcycleRequest = request is AddFuelMotorcycleRequest;
        
        return isAddFuelMotorcycleRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);
        
        return vehicle;
    }
}