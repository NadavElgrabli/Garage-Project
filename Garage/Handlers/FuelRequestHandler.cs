using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Handlers;

public class FuelRequestHandler : ITreatmentRequestHandler
{
    private readonly IListRepository _listRepository;

    public FuelRequestHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
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

        _listRepository.AddRequest(TreatmentType.Refuel, request);
    }
}