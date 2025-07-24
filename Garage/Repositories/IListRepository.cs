using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IListRepository
{
    TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService);
    bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request);
    void AddRequest(TreatmentType type, TreatmentRequest request);

}