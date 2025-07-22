using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class AirRequestHandler : ITreatmentRequestHandler
{
    private readonly InMemoryDatabase _db;

    public AirRequestHandler(InMemoryDatabase db)
    {
        _db = db;
    }
    
    public bool IsMatching(TreatmentRequest request)
    {
        var isAirRequest = request is AirRequest;
        
        return isAirRequest;
    }

    public void Handle(TreatmentRequest request)
    {
        if (request is not AirRequest)
            throw new ArgumentException("Invalid request type for AirRequestHandler");

        _db.TreatmentLists[TreatmentType.Inflate].AddLast(request);
    }
}