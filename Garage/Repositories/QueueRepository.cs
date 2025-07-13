using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Repositories;

public class QueueRepository : IQueueRepository
{
    //Fuel
    // Tries to get the first vehicle in the fuel queue without removing it.
    // Returns true if successful, false if the queue is empty.
    public bool TryPeekFuelRequest(out FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.TryPeek(out request);

    // Tries to remove and return the first vehicle in the fuel queue.
    // Returns true if successful, false if the queue is empty.
    public bool TryDequeueFuelRequest(out FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.TryDequeue(out request);

    // Adds a new fuel request to the end of the fuel queue.
    public void EnqueueFuelRequest(FuelRequest request) =>
        InMemoryDatabase.FuelStationRequests.Enqueue(request);

    // Removes all fuel requests from the fuel queue and returns them in order.
    // Used when temporarily rearranging the queue (e.g., to insert a vehicle at the front).
    public Queue<FuelRequest> DrainFuelQueue()
    {
        var temp = new Queue<FuelRequest>();
        while (InMemoryDatabase.FuelStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    // Rebuilds the fuel queue with a new order of requests.
    // Useful after rearranging queue logic (like cutting in line).
    public void RebuildFuelQueue(Queue<FuelRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.FuelStationRequests.Enqueue(r);
    }


    // Charge
    public bool TryPeekChargeRequest(out ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.TryPeek(out request);

    public bool TryDequeueChargeRequest(out ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.TryDequeue(out request);

    public void EnqueueChargeRequest(ChargeRequest request) =>
        InMemoryDatabase.ChargeStationRequests.Enqueue(request);

    public Queue<ChargeRequest> DrainChargeQueue()
    {
        var temp = new Queue<ChargeRequest>();
        while (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    public void RebuildChargeQueue(Queue<ChargeRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.ChargeStationRequests.Enqueue(r);
    }

    // Air 
    public bool TryPeekAirRequest(out AirRequest request) =>
        InMemoryDatabase.AirStationRequests.TryPeek(out request);

    public bool TryDequeueAirRequest(out AirRequest request) =>
        InMemoryDatabase.AirStationRequests.TryDequeue(out request);

    public void EnqueueAirRequest(AirRequest request) =>
        InMemoryDatabase.AirStationRequests.Enqueue(request);

    public Queue<AirRequest> DrainAirQueue()
    {
        var temp = new Queue<AirRequest>();
        while (InMemoryDatabase.AirStationRequests.TryDequeue(out var r))
            temp.Enqueue(r);
        return temp;
    }

    public void RebuildAirQueue(Queue<AirRequest> reorderedQueue)
    {
        foreach (var r in reorderedQueue)
            InMemoryDatabase.AirStationRequests.Enqueue(r);
    }
    
    // All types 
    // Tries to get the first request in the corresponding queue without removing it.
    // Returns true if successful and out the request, false if the queue is empty.
    public bool TryPeekRequest(ITreatmentService treatmentService, out TreatmentRequest request)
    {
        request = null;

        switch (treatmentService.GetTreatmentType())
        {
            case TreatmentType.Refuel:
                if (InMemoryDatabase.FuelStationRequests.TryPeek(out var fuelReq))
                {
                    request = fuelReq;
                    return true;
                }
                break;

            case TreatmentType.Recharge:
                if (InMemoryDatabase.ChargeStationRequests.TryPeek(out var chargeReq))
                {
                    request = chargeReq;
                    return true;
                }
                break;

            case TreatmentType.Inflate:
                if (InMemoryDatabase.AirStationRequests.TryPeek(out var airReq))
                {
                    request = airReq;
                    return true;
                }
                break;
        }

        return false;
    }

    public bool TryDequeueRequest(ITreatmentService treatmentService, out TreatmentRequest request)
    {
        request = null;

        switch (treatmentService.GetTreatmentType())
        {
            case TreatmentType.Refuel:
                if (InMemoryDatabase.FuelStationRequests.TryDequeue(out var fuelReq))
                {
                    request = fuelReq;
                    return true;
                }
                break;

            case TreatmentType.Recharge:
                if (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var chargeReq))
                {
                    request = chargeReq;
                    return true;
                }
                break;

            case TreatmentType.Inflate:
                if (InMemoryDatabase.AirStationRequests.TryDequeue(out var airReq))
                {
                    request = airReq;
                    return true;
                }
                break;
        }

        return false;
    }
    
    public Queue<TreatmentRequest> DrainQueue(ITreatmentService treatmentService)
    {
        var queue = new Queue<TreatmentRequest>();

        switch (treatmentService.GetTreatmentType())
        {
            case TreatmentType.Refuel:
                while (InMemoryDatabase.FuelStationRequests.TryDequeue(out var fuelRequest))
                    queue.Enqueue(fuelRequest);
                break;

            case TreatmentType.Recharge:
                while (InMemoryDatabase.ChargeStationRequests.TryDequeue(out var chargeRequest))
                    queue.Enqueue(chargeRequest);
                break;

            case TreatmentType.Inflate:
                while (InMemoryDatabase.AirStationRequests.TryDequeue(out var airRequest))
                    queue.Enqueue(airRequest);
                break;

            default:
                throw new ArgumentException("Unsupported treatment type");
        }

        return queue;
    }
    
    public void RebuildQueue(ITreatmentService treatmentService, Queue<TreatmentRequest> reorderedQueue)
    {
        switch (treatmentService.GetTreatmentType())
        {
            case TreatmentType.Refuel:
                foreach (var r in reorderedQueue)
                    InMemoryDatabase.FuelStationRequests.Enqueue((FuelRequest)r);
                break;

            case TreatmentType.Recharge:
                foreach (var r in reorderedQueue)
                    InMemoryDatabase.ChargeStationRequests.Enqueue((ChargeRequest)r);
                break;

            case TreatmentType.Inflate:
                foreach (var r in reorderedQueue)
                    InMemoryDatabase.AirStationRequests.Enqueue((AirRequest)r);
                break;

            default:
                throw new ArgumentException("Unsupported treatment type");
        }
    }

    public void EnqueueRequest(ITreatmentService treatmentService, TreatmentRequest request)
    {
        switch (treatmentService.GetTreatmentType())
        {
            case TreatmentType.Refuel:
                InMemoryDatabase.FuelStationRequests.Enqueue((FuelRequest)request);
                break;

            case TreatmentType.Recharge:
                InMemoryDatabase.ChargeStationRequests.Enqueue((ChargeRequest)request);
                break;

            case TreatmentType.Inflate:
                InMemoryDatabase.AirStationRequests.Enqueue((AirRequest)request);
                break;

            default:
                throw new ArgumentException("Unsupported treatment type");
        }
    }


}
