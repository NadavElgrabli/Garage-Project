using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class ListRepository : IListRepository
{
    public void AddVehicleRequestToMatchingList(TreatmentRequest firstRequest, TreatmentRequest secondRequest)
    {
        foreach (var treatmentType in firstRequest.Vehicle.TreatmentTypes)
        {
            if (InMemoryDatabase.TreatmentLists.ContainsKey(treatmentType))
            {
                var list = InMemoryDatabase.TreatmentLists[treatmentType];
                list.AddLast(firstRequest);
            }
        }

        foreach (var treatmentType in secondRequest.Vehicle.TreatmentTypes)
        {
            if (InMemoryDatabase.TreatmentLists.ContainsKey(treatmentType))
            {
                var list = InMemoryDatabase.TreatmentLists[treatmentType];
                list.AddLast(secondRequest);
            }
        }
    }


// Attempts to get the first request in the treatment list without removing it.
// Returns true and sets 'request' if one exists; otherwise, returns false.
    public bool TryGetFirstRequest(ITreatmentService treatmentService, out TreatmentRequest? request)
    {
        request = null;
        var treatmentType = treatmentService.GetTreatmentType();

        if (InMemoryDatabase.TreatmentLists.ContainsKey(treatmentType))
        {
            var list = InMemoryDatabase.TreatmentLists[treatmentType];

            if (list.First != null)
            {
                request = list.First.Value;
                return true;
            }
        }

        return false;
    }


// Tries to remove and return the first request from the corresponding treatment linked list.
// Returns true and sets 'request' if successful; false if the list is empty.
    public bool TryRemoveFirstRequest(ITreatmentService treatmentService, out TreatmentRequest? request)
    {
        request = null;
        var type = treatmentService.GetTreatmentType();

        if (InMemoryDatabase.TreatmentLists.ContainsKey(type))
        {
            var list = InMemoryDatabase.TreatmentLists[type];

            if (list.First != null)
            {
                request = list.First.Value;
                list.RemoveFirst(); // Remove the first (oldest) request from the list
                return true;
            }
        }

        return false;
    }



    
// Removes all requests from the treatment list and returns them in the same order.
// Useful when temporarily rearranging the list (e.g., to reinsert a vehicle at the front).
    public Queue<TreatmentRequest> DrainTreatmentList(ITreatmentService treatmentService)
    {
        var drained = new Queue<TreatmentRequest>();
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentLists.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        var list = InMemoryDatabase.TreatmentLists[type];

        // Move all elements from the linked list to a temporary FIFO queue
        while (list.First != null)
        {
            drained.Enqueue(list.First.Value);
            list.RemoveFirst();
        }

        return drained;
    }


    
// Rebuilds the treatment linked list with a new order of requests.
// Useful after rearranging the list (e.g., for priority handling).
    public void RebuildTreatmentList(ITreatmentService treatmentService, IEnumerable<TreatmentRequest> reorderedRequests)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentLists.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        var targetList = InMemoryDatabase.TreatmentLists[type];
        targetList.Clear();

        foreach (var request in reorderedRequests)
        {
            targetList.AddLast(request);
        }
    }

    
    // Adds a new request to the end of the treatment list.
    public void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentLists.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        InMemoryDatabase.TreatmentLists[type].AddLast(request); // Append to the end of the list
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
