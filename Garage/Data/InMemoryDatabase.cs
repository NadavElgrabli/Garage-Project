using Garage.Enums;
using Garage.Models;
namespace Garage.Data;

public class InMemoryDatabase
{
    public Dictionary<string, Vehicle> Vehicles { get; set; } = new();
    
    //TODO: move to list repository
    public Dictionary<TreatmentType, LinkedList<TreatmentRequest>> TreatmentLists { get; } =
        Enum.GetValues(typeof(TreatmentType))
            .Cast<TreatmentType>()
            .ToDictionary(
                type => type,
                type => new LinkedList<TreatmentRequest>()
            );

    public Dictionary<TreatmentType, object> TreatmentLocks { get; } =
        Enum.GetValues(typeof(TreatmentType))
            .Cast<TreatmentType>()
            .ToDictionary(t => t, t => new object());
    
    //TODO: dont use classes as keys in dicts,  only use value types as keys.
    public readonly Dictionary<Type, TreatmentType> RequestTypeToTreatmentType = new()
    {
        { typeof(FuelRequest), TreatmentType.Refuel },
        { typeof(ChargeRequest), TreatmentType.Recharge },
        { typeof(AirRequest), TreatmentType.Inflate }
    };
}

