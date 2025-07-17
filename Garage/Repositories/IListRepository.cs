using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IListRepository
{
    void AddVehicleRequestToMatchingList(List<TreatmentRequest> treatmentRequests);

    TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService);
    bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request);

}