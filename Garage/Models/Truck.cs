namespace Garage.Models;

public class Truck : Vehicle
{
    public bool CarryDangerousSubstances {get; set;}
    public float CargoVolume {get; set;}
}