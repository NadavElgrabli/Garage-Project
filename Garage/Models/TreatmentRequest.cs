using Garage.Enums;

namespace Garage.Models;

public class TreatmentRequest
{
    public string LicensePlate { get; set; }
    public TreatmentType Type { get; set; }

    public TreatmentRequest(string licensePlate, TreatmentType type)
    {
        LicensePlate = licensePlate;
        Type = type;
    }
}