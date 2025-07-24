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
        if (request is not AddFuelMotorcycleRequest)
            throw new ArgumentException("Invalid request type for FuelMotorcycleRequestHandler");

        return _factory.CreateVehicle(request);
    }
}