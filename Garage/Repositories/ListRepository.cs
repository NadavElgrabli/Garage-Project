using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class ListRepository : IListRepository
{
    private readonly InMemoryDatabase _db;

    public ListRepository(InMemoryDatabase db)
    {
        _db = db;
    }
    
    public TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService)
    {
        var type = treatmentService.GetTreatmentType();

        if (!_db.TreatmentLists.TryGetValue(type, out var list))
            return null;

        return list.FirstOrDefault(request => request.Vehicle.Status != Status.InTreatment);
    }
    
    public bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        var type = treatmentService.GetTreatmentType();

        if (!_db.TreatmentLists.TryGetValue(type, out var list))
            return false;

        return list.Remove(request);
    }
    
    public void AddRequest(TreatmentType type, TreatmentRequest request)
    {
        _db.TreatmentLists[type].AddLast(request);
    }

}
