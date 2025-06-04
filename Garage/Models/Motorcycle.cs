using Garage.Enums;

namespace Garage.Models;

public class Motorcycle : Vehicle
{
    public int EngineVolumeCC {get; set;}
    public LicenseType LicenseType {get; set;}
}