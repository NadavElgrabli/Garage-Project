using Garage.Enums;

namespace Garage.Models;

public class AddFuelMotorcycleRequest : Vehicle
{
    public FuelEngine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
    public int EngineVolumeCC {get; set;}
    public LicenseType LicenseType {get; set;}
    public float LitersToFuel { get; set; }
}