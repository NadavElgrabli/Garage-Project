using Garage.Enums;
using Garage.Models;
namespace Garage.Data;

public class InMemoryDatabase
{
    public Dictionary<string, Vehicle> Vehicles { get; set; } = new();
    
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
}


// public Dictionary<Type, ITreatmentService> TreatmentHandlers { get; } = new()
// {
//     { typeof(FuelRequest), new RefuelService() },
//     { typeof(ChargeRequest), new RechargeService() },
//     { typeof(AirRequest), new InflateService() }
// };