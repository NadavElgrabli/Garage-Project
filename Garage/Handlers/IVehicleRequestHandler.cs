using Garage.Models;

namespace Garage.Handlers;

public interface IVehicleRequestHandler
{
    bool IsMatch(Vehicle request);
    Vehicle Handle(Vehicle request);
}
