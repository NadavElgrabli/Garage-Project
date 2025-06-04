using System.Collections.Concurrent;
using Garage.Models;

namespace Garage.Data;

public class InMemoryDatabase
{
    public static Dictionary<string, Vehicle> Vehicles { get; set; } = new();
    
    public static ConcurrentQueue<FuelRequest> FuelStationRequests = new();
    public static ConcurrentQueue<ChargeRequest> ChargeStationRequests { get; set; } = new();
    public static ConcurrentQueue<AirRequest> AirStationRequests { get; set; } = new();

}