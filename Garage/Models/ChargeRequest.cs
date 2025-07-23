namespace Garage.Models;

public class ChargeRequest : TreatmentRequest
{
    public Engine Engine { get; set; }
    public float RequestedHoursToCharge { get; set; }
}