using Garage.Enums;

namespace Garage.Models;

public class AddElectricMotorcycleRequest : Vehicle
{
    public Engine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
    public int EngineVolumeCC {get; set;}
    public LicenseType LicenseType {get; set;}
    public float HoursToCharge { get; set; }
}