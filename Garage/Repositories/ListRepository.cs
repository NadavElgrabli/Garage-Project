using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class ListRepository : IListRepository
{
    public void AddVehicleRequestToMatchingList(TreatmentRequest firstRequest, TreatmentRequest secondRequest)
    {
        AddToMatchingList(firstRequest);
        AddToMatchingList(secondRequest);
    }
    
    public void AddToMatchingList(TreatmentRequest request)
    {
        TreatmentType treatmentType = request switch
        {
            FuelRequest => TreatmentType.Refuel,
            ChargeRequest => TreatmentType.Recharge,
            AirRequest => TreatmentType.Inflate,
            _ => throw new ArgumentException("Unsupported request type")
        };

        if (InMemoryDatabase.TreatmentLists.TryGetValue(treatmentType, out var list))
        {
            list.AddLast(request);
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
