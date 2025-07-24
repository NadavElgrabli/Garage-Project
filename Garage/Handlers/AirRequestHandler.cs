using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Handlers;

public class AirRequestHandler : ITreatmentRequestHandler
{
    private readonly IListRepository _listRepository;

    public AirRequestHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
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

        _listRepository.AddRequest(TreatmentType.Inflate, request);
    }
}