using Garage.Enums;

namespace Garage.Models;

public class AddElectricCarRequest : Vehicle
{
    public List<float> DesiredWheelPressures { get; set; }
    public int NumberOfDoors { get; set; }
    public Color Color { get; set; }
    public float HoursToCharge { get; set; }
}