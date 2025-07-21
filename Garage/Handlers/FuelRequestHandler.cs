using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class FuelRequestHandler : ITreatmentRequestHandler
{
    public bool IsMatching(TreatmentRequest request)
    {
        return request is FuelRequest;
    }

    public void Handle(TreatmentRequest request, InMemoryDatabase db)
    {
        if (request is not FuelRequest)
            throw new ArgumentException("Invalid request type for FuelRequestHandler");

        db.TreatmentLists[TreatmentType.Refuel].AddLast(request);
    }
}