namespace Garage.Models;

public class AirRequest
{
    public Vehicle Vehicle { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
}