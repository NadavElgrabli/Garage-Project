using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class ListRepository : IListRepository
{
    public void AddVehicleRequestToMatchingList(List<TreatmentRequest> treatmentRequests)
    {
        foreach (var request in treatmentRequests)
        {
            if (request is FuelRequest)
            {
                InMemoryDatabase.TreatmentLists[TreatmentType.Refuel].AddLast(request);
            }
            else if (request is ChargeRequest)
            {
                InMemoryDatabase.TreatmentLists[TreatmentType.Recharge].AddLast(request);
            }
            else if (request is AirRequest)
            {
                InMemoryDatabase.TreatmentLists[TreatmentType.Inflate].AddLast(request);
            }
            else
            {
                throw new ArgumentException("Unsupported treatment request type.");
            }
        }
    }


    // Returns the first request that's vehicle is not "IntTreatment" or null if vehicle / list doesn't exist.
    public TreatmentRequest? FindFirstAvailableVehicleRequest(ITreatmentService treatmentService)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentLists.TryGetValue(type, out var list))
            return null;

        foreach (var request in list)
        {
            if (request.Vehicle.Status != Status.InTreatment)
                return request;
        }

        return null;
    }
    
    // Returns true if the request was found and successfully removed; false if it was not found or the list does not exist.
    public bool RemoveRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentLists.TryGetValue(type, out var list))
            return false;

        return list.Remove(request);
    }
    
}
