using Garage.Enums;

namespace Garage.Models;

public class Car : Vehicle
{
    public Engine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public int NumberOfDoors { get; set; }
    public Color Color { get; set; }
}