using Garage.Enums;

namespace Garage.Models;

public class AddDroneRequest : Vehicle
{
    public List<Engine> Engines { get; set; }
    public List<float> DesiredHoursToCharge { get; set; }
    public DroneControl ControlOptions { get; set; }
}