namespace Garage.Models;

public class ChargeRequest
{
    public Vehicle Vehicle { get; set; }
    public float RequestedHoursToCharge { get; set; }
}