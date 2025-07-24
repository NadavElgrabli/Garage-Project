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
        if (request is not AddElectricMotorcycleRequest)
            throw new ArgumentException("Invalid request type for ElectricMotorcycleRequestHandler");

        return _factory.CreateVehicle(request);
    }
}