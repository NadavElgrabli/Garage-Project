using Garage.Enums;

namespace Garage.Models;

public class Drone : Vehicle
{
    public List<Engine> Engines { get; set; }
    public float FlightTimeMinutes {get; set;}
    public DroneControl ControlOptions { get; set; }
}