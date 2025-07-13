using Garage.Models;
using Garage.Services;

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
    
    // For all types
    bool TryPeekRequest(ITreatmentService treatmentService, out TreatmentRequest request);
    bool TryDequeueRequest(ITreatmentService treatmentService, out TreatmentRequest request);
    Queue<TreatmentRequest> DrainQueue(ITreatmentService treatmentService);
    void RebuildQueue(ITreatmentService treatmentService, Queue<TreatmentRequest> reorderedQueue);
    void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request);
}