using Garage.Enums;

namespace Garage.Models;

public class AddDroneRequest : Vehicle
{
    public Engine Engine { get; set; }
    public float DesiredHoursToCharge { get; set; }
    public DroneControl ControlOptions { get; set; }
}