using Garage.Enums;

namespace Garage.Models;

public class Drone : Vehicle
{
    public float FlightTimeMinutes {get; set;}
    public DroneControl ControlInterface { get; set; }
}