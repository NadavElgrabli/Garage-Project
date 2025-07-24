using Garage.Enums;

namespace Garage.Models;

public class Drone : Vehicle
{
    public Engine Engine { get; set; }
    public DroneControl ControlOptions { get; set; }
}