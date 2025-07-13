using System.Collections.Concurrent;
using Garage.Models;

namespace Garage.Data;

public class InMemoryDatabase
{
    public static Dictionary<string, Vehicle> Vehicles { get; set; } = new();
    
    public static ConcurrentQueue<TreatmentRequest> FuelStationRequests = new();
    public static ConcurrentQueue<TreatmentRequest> ChargeStationRequests { get; set; } = new();
    public static ConcurrentQueue<TreatmentRequest> AirStationRequests { get; set; } = new();

}