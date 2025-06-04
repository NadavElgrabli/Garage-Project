using Garage.Enums;

namespace Garage.Models;

public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public Color Color { get; set; }
}