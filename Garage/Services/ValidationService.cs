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
    
    public Task CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        return _validationRepository.CheckValidElectricCarInput(request);
    }
    
    public Task CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        return _validationRepository.CheckValidFuelCarInput(request);
    }
}