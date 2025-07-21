using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public interface IListRepository
{
    TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService);
    bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request);
}