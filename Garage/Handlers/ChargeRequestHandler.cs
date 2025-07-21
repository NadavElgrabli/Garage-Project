using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class ChargeRequestHandler : ITreatmentRequestHandler
{
    public bool IsMatching(TreatmentRequest request)
    {
        return request is ChargeRequest;
    }

    public void Handle(TreatmentRequest request, InMemoryDatabase db)
    {
        if (request is not ChargeRequest)
            throw new ArgumentException("Invalid request type for ChargeRequestHandler");

        db.TreatmentLists[TreatmentType.Recharge].AddLast(request);
    }
}