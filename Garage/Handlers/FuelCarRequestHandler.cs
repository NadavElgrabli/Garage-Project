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
        if (request is not AddFuelCarRequest)
            throw new ArgumentException("Invalid request type for FuelCarRequestHandler");

        return _factory.CreateVehicle(request);
    }
}