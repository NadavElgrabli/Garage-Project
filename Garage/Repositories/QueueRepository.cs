using Garage.Data;
using Garage.Models;

namespace Garage.Repositories;

public class QueueRepository : IQueueRepository
{
    //Fuel
    // Tries to get the first vehicle in the fuel queue without removing it.
    // Returns true if successful, false if the queue is empty.
    public bool TryPeekFuelRequest(out FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.TryPeek(out request);

    // Tries to remove and return the first vehicle in the fuel queue.
    // Returns true if successful, false if the queue is empty.
    public bool TryDequeueFuelRequest(out FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.TryDequeue(out request);

    // Adds a new fuel request to the end of the fuel queue.
    public void EnqueueFuelRequest(FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.Enqueue(request);

    // Removes all fuel requests from the fuel queue and returns them in order.
    // Used when temporarily rearranging the queue (e.g., to insert a vehicle at the front).
    public Queue<FuelRequest> DrainFuelQueue()
    {
        var temp = new Queue<FuelRequest>();
        while (InMemoryDatabase.FuelStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    // Rebuilds the fuel queue with a new order of requests.
    // Useful after rearranging queue logic (like cutting in line).
    public void RebuildFuelQueue(Queue<FuelRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.FuelStationRequests.Enqueue(r);
    }


    // Charge
    public bool TryPeekChargeRequest(out ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.TryPeek(out request);

    public bool TryDequeueChargeRequest(out ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.TryDequeue(out request);

    public void EnqueueChargeRequest(ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.Enqueue(request);

    public Queue<ChargeRequest> DrainChargeQueue()
    {
        var temp = new Queue<ChargeRequest>();
        while (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    public void RebuildChargeQueue(Queue<ChargeRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.ChargeStationRequests.Enqueue(r);
    }

    // Air 
    public bool TryPeekAirRequest(out AirRequest request) =>
        InMemoryDatabase.AirStationRequests.TryPeek(out request);

    public bool TryDequeueAirRequest(out AirRequest request) =>
        InMemoryDatabase.AirStationRequests.TryDequeue(out request);

    public void EnqueueAirRequest(AirRequest request) =>
        InMemoryDatabase.AirStationRequests.Enqueue(request);

    public Queue<AirRequest> DrainAirQueue()
    {
        var temp = new Queue<AirRequest>();
        while (InMemoryDatabase.AirStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    public void RebuildAirQueue(Queue<AirRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.AirStationRequests.Enqueue(r);
    }
}
