using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class ElectricCarRequestHandler : IVehicleRequestHandler
{
    private readonly ElectricCarFactory _factory;

    public ElectricCarRequestHandler(ElectricCarFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        return request is AddElectricCarRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        if (request is not AddElectricCarRequest)
            throw new ArgumentException("Invalid request type for ElectricCarRequestHandler");

        return _factory.CreateVehicle(request);
    }
}