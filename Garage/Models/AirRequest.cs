namespace Garage.Models;

public class AirRequest : TreatmentRequest
{
    public List<float> DesiredWheelPressures { get; set; }
}