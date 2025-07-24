using Garage.Enums;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Handlers;

public class ChargeRequestHandler : ITreatmentRequestHandler
{
    private readonly IListRepository _listRepository;

    public ChargeRequestHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
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

        _listRepository.AddRequest(TreatmentType.Recharge, request);
    }
}