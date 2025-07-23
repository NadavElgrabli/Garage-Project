namespace Garage.Models;

public class Truck : Vehicle
{
    public Engine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public bool CarryDangerousSubstances {get; set;}
    public float CargoVolume {get; set;}
}