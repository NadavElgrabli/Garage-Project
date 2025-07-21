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
    
    public void AddVehicleRequestToMatchingList(List<TreatmentRequest> treatmentRequests)
    {
        foreach (var request in treatmentRequests)
        {
            var requestType = request.GetType();
            if (!_db.RequestTypeToTreatmentType.TryGetValue(requestType, out var treatmentType))
            {
                throw new ArgumentException($"Unsupported treatment request type: {requestType.Name}");
            }

            _db.TreatmentLists[treatmentType].AddLast(request);
        }
    }
    
    // Returns the first request that's vehicle is not "IntTreatment" or null if vehicle / list doesn't exist.
    public TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService)
    {
        var type = treatmentService.GetTreatmentType();

        if (!_db.TreatmentLists.TryGetValue(type, out var list))
            return null;

        return list.FirstOrDefault(request => request.Vehicle.Status != Status.InTreatment);
    }
    
    // Returns true if the request was found and successfully removed; false if it was not found or the list does not exist.
    public bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        var type = treatmentService.GetTreatmentType();

        if (!_db.TreatmentLists.TryGetValue(type, out var list))
            return false;

        return list.Remove(request);
    }
}
