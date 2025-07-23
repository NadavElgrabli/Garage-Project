namespace Garage.Models;

public class AirRequest : TreatmentRequest
{
    public List<Wheel> Wheels { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
}