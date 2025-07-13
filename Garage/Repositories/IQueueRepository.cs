using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IQueueRepository
{
    void EnqueVehicle(TreatmentRequest firstRequest, TreatmentRequest secondRequest);
    bool TryPeekRequest(ITreatmentService treatmentService, out TreatmentRequest? request);
    bool TryDequeueRequest(ITreatmentService treatmentService, out TreatmentRequest? request);
    Queue<TreatmentRequest> DrainQueue(ITreatmentService treatmentService);
    void RebuildQueue(ITreatmentService treatmentService, Queue<TreatmentRequest> reorderedQueue);
    void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request);
}