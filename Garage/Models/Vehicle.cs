using Garage.Enums;

namespace Garage.Models;

public abstract class Vehicle
{
    public string ManufacturerName { get; set; }
    public string ModelName { get; set; }
    public string LicensePlate { get; set; }
    public float RemainingEnergyPercentage { get; set; }
    public Owner Owner { get; set; }
    public Status Status { get; set; }
    public VehicleType VehicleType { get; set; }
    public List<TreatmentType> TreatmentTypes { get; set; }
    public float TreatmentsPrice { get; set; }
}

//public Engine Engine { get; set; }

//public List<Wheel> Wheels { get; set; }