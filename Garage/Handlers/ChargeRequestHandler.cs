using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class ChargeRequestHandler : ITreatmentRequestHandler
{
    private readonly InMemoryDatabase _db;

    public ChargeRequestHandler(InMemoryDatabase db)
    {
        _db = db;
    }
    
    public bool IsMatching(TreatmentRequest request)
    {
        var isChargeRequest = request is ChargeRequest;
        
        return isChargeRequest;
    }

    public void Handle(TreatmentRequest request)
    {
        if (request is not ChargeRequest)
            throw new ArgumentException("Invalid request type for ChargeRequestHandler");

        _db.TreatmentLists[TreatmentType.Recharge].AddLast(request);
    }
}