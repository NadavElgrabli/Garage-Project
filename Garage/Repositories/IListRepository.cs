using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IListRepository
{
    void AddVehicleRequestToMatchingList(TreatmentRequest firstRequest, TreatmentRequest secondRequest);
    bool TryGetFirstRequest(ITreatmentService treatmentService, out TreatmentRequest? request);
    bool TryRemoveFirstRequest(ITreatmentService treatmentService, out TreatmentRequest? request);
    Queue<TreatmentRequest> DrainTreatmentList(ITreatmentService treatmentService);
    void RebuildTreatmentList(ITreatmentService treatmentService, IEnumerable<TreatmentRequest> reorderedRequests);
    void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request);
    
    TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService);
    bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request);

}