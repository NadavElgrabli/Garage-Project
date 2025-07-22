using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Handlers;

public class FuelRequestHandler : ITreatmentRequestHandler
{
    private readonly InMemoryDatabase _db;

    public FuelRequestHandler(InMemoryDatabase db)
    {
        _db = db;
    }
    
    public bool IsMatching(TreatmentRequest request)
    {
        var isFuelRequest = request is FuelRequest;
        
        return isFuelRequest;
    }

    public void Handle(TreatmentRequest request)
    {
        if (request is not FuelRequest)
            throw new ArgumentException("Invalid request type for FuelRequestHandler");

        _db.TreatmentLists[TreatmentType.Refuel].AddLast(request);
    }
}