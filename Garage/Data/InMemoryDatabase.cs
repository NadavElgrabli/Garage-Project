using Garage.Enums;
using Garage.Models;

namespace Garage.Data;

public class InMemoryDatabase
{
    public static Dictionary<string, Vehicle> Vehicles { get; set; } = new();
    
    public static Dictionary<TreatmentType, LinkedList<TreatmentRequest>> TreatmentLists { get; } =
        Enum.GetValues(typeof(TreatmentType))
            .Cast<TreatmentType>()
            .ToDictionary(
                type => type,
                type => new LinkedList<TreatmentRequest>()
            );

    public static Dictionary<TreatmentType, object> TreatmentLocks { get; } =
        Enum.GetValues(typeof(TreatmentType))
            .Cast<TreatmentType>()
            .ToDictionary(t => t, t => new object());


}