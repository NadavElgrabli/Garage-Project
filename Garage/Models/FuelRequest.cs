namespace Garage.Models;

public class FuelRequest : TreatmentRequest
{
    public Engine Engine { get; set; }
    public float RequestedLiters { get; set; }
}