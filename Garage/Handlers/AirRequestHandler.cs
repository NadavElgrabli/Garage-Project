using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class AirRequestHandler : ITreatmentRequestHandler
{
    public bool IsMatching(TreatmentRequest request)
    {
        return request is AirRequest;
    }

    public void Handle(TreatmentRequest request, InMemoryDatabase db)
    {
        if (request is not AirRequest)
            throw new ArgumentException("Invalid request type for AirRequestHandler");

        db.TreatmentLists[TreatmentType.Inflate].AddLast(request);
    }
}