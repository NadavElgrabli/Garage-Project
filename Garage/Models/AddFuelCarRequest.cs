using Garage.Enums;
using Garage.Interfaces;

namespace Garage.Models;

public class AddFuelCarRequest : Vehicle
{
    public FuelEngine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
    public int NumberOfDoors { get; set; }
    public Color Color { get; set; }
    public float LitersToFuel { get; set; }
}