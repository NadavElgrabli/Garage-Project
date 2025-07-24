using Garage.Factories;
using Garage.Models;

namespace Garage.Handlers;

public class ElectricMotorcycleRequestHandler : IVehicleRequestHandler
{
    private readonly ElectricMotorcycleFactory _factory;

    public ElectricMotorcycleRequestHandler(ElectricMotorcycleFactory factory)
    {
        _factory = factory;
    }

    public bool IsMatch(Vehicle request)
    {
        var isAddElectricMotorcycleRequest = request is AddElectricMotorcycleRequest;
        
        return isAddElectricMotorcycleRequest;
    }

    public Vehicle Handle(Vehicle request)
    {
        var vehicle = _factory.CreateVehicle(request);
        
        return vehicle;
    }
}