using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IListRepository
{
    void AddVehicleRequestToMatchingList(TreatmentRequest firstRequest, TreatmentRequest secondRequest);
    TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService);
    bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request);

}