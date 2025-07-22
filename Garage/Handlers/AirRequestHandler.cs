using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class AirRequestHandler : ITreatmentRequestHandler
{
    //TODO: instead of receiving db use DI
    public bool IsMatching(TreatmentRequest request)
    {
        // TODO: put in var and then return
        
        return request is AirRequest;
    }

    public void Handle(TreatmentRequest request, InMemoryDatabase db)
    {
        if (request is not AirRequest)
            throw new ArgumentException("Invalid request type for AirRequestHandler");

        db.TreatmentLists[TreatmentType.Inflate].AddLast(request);
    }
}