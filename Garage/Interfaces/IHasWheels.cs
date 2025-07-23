using Garage.Models;

namespace Garage.Interfaces;

public interface IHasWheels
{
    List<Wheel> Wheels { get; }

}