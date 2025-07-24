using Garage.Enums;

namespace Garage.Models;

public class Motorcycle : Vehicle
{
    public Engine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public int EngineVolumeCC {get; set;}
    public LicenseType LicenseType {get; set;}
}