using Garage.Enums;

namespace Garage.Models;

public class TreatmentResult
{
    public string LicensePlate {get; set;}
    public TreatmentType Type {get; set;}
    public bool Success {get; set;}
    public float Cost {get; set;}

    public TreatmentResult(string licensePlate, TreatmentType type, bool success, float cost)
    {
        LicensePlate = licensePlate;
        Type = type;
        Success = success;
        Cost = cost;
    }
}