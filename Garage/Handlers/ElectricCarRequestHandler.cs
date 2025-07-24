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
        var isAddElectricCarRequest = request is AddElectricCarRequest;
        
        return isAddElectricCarRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);

        return vehicle;
    }
}