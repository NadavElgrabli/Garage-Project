using Garage.Data;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class QueueRepository : IQueueRepository
{
    public void EnqueVehicle(TreatmentRequest firstRequest, TreatmentRequest secondRequest)
    {
        foreach (var treatmentType in firstRequest.Vehicle.TreatmentTypes)
        {
            if (InMemoryDatabase.TreatmentQueues.ContainsKey(treatmentType))
            {
                var queue = InMemoryDatabase.TreatmentQueues[treatmentType];
                queue.Enqueue(firstRequest);
            }
        }

        foreach (var treatmentType in secondRequest.Vehicle.TreatmentTypes)
        {
            if (InMemoryDatabase.TreatmentQueues.ContainsKey(treatmentType))
            {
                var queue = InMemoryDatabase.TreatmentQueues[treatmentType];
                queue.Enqueue(secondRequest);
            }
        }
    }

    // Tries to get the first request in the corresponding queue without removing it.
    // Returns true if successful and out the request, false if the queue is empty.
    public bool TryPeekRequest(ITreatmentService treatmentService, out TreatmentRequest? request)
    {
        request = null;
        var treatmentType = treatmentService.GetTreatmentType();

        if (InMemoryDatabase.TreatmentQueues.ContainsKey(treatmentType))
        {
            var queue = InMemoryDatabase.TreatmentQueues[treatmentType];

            if (queue.TryPeek(out var peekedRequest))
            {
                request = peekedRequest;
                return true;
            }
        }
        return false;
    }


    // Tries to remove and return the first vehicle in the queue.
    // Returns true if successful (with the request), false if the queue is empty.
    public bool TryDequeueRequest(ITreatmentService treatmentService, out TreatmentRequest? request)
    {
        request = null;
        var type = treatmentService.GetTreatmentType();

        if (InMemoryDatabase.TreatmentQueues.ContainsKey(type))
        {
            var queue = InMemoryDatabase.TreatmentQueues[type];
            if (queue.TryDequeue(out var dequeuedRequest))
            {
                request = dequeuedRequest;
                return true;
            }
        }

        return false;
    }

    
    // Removes all  requests from the  queue and returns them in order.
    // Used when temporarily rearranging the queue (e.g., to insert a vehicle at the front).
    public Queue<TreatmentRequest> DrainQueue(ITreatmentService treatmentService)
    {
        var queue = new Queue<TreatmentRequest>();
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentQueues.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        var sourceQueue = InMemoryDatabase.TreatmentQueues[type];

        while (sourceQueue.TryDequeue(out var request))
        {
            queue.Enqueue(request);
        }

        return queue;
    }

    
    // Rebuilds the queue with a new order of requests.
    // Useful after rearranging queue logic (like cutting in line).
    public void RebuildQueue(ITreatmentService treatmentService, Queue<TreatmentRequest> reorderedQueue)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentQueues.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        var targetQueue = InMemoryDatabase.TreatmentQueues[type];

        foreach (var request in reorderedQueue)
        {
            targetQueue.Enqueue(request);
        }
    }
    
    // Adds a new request to the end of the queue.
    public void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        var type = treatmentService.GetTreatmentType();

        if (!InMemoryDatabase.TreatmentQueues.ContainsKey(type))
            throw new ArgumentException("Unsupported treatment type");

        InMemoryDatabase.TreatmentQueues[type].Enqueue(request);
    }

}
