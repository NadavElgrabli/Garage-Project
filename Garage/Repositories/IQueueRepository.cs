using Garage.Models;

namespace Garage.Repositories;

public interface IQueueRepository
{
    // Fuel queue
    bool TryPeekFuelRequest(out FuelRequest request);
    bool TryDequeueFuelRequest(out FuelRequest request);
    void EnqueueFuelRequest(FuelRequest request);

    // Charge queue
    bool TryPeekChargeRequest(out ChargeRequest request);
    bool TryDequeueChargeRequest(out ChargeRequest request);
    void EnqueueChargeRequest(ChargeRequest request);

    // Air queue
    bool TryPeekAirRequest(out AirRequest request);
    bool TryDequeueAirRequest(out AirRequest request);
    void EnqueueAirRequest(AirRequest request);

    // Advanced operations
    Queue<FuelRequest> DrainFuelQueue();
    void RebuildFuelQueue(Queue<FuelRequest> reorderedQueue);

    Queue<ChargeRequest> DrainChargeQueue();
    void RebuildChargeQueue(Queue<ChargeRequest> reorderedQueue);

    Queue<AirRequest> DrainAirQueue();
    void RebuildAirQueue(Queue<AirRequest> reorderedQueue);
}