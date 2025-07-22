using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class ValidationService
{
    private readonly IValidationRepository _validationRepository;

    public ValidationService(IValidationRepository validationRepository)
    {
        _validationRepository = validationRepository;
    }
    
    public void CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        _validationRepository.CheckValidElectricCarInput(request);
    }
    
    public void CheckValidFuelCarInput(AddFuelCarRequest request)
    { 
        _validationRepository.CheckValidFuelCarInput(request);
    }

    public void CheckValidTruckInput(AddTruckRequest request)
    {
        
    }
}