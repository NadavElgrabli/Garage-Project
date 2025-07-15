using Garage.Models;

namespace Garage.Repositories;

public interface IValidationRepository
{
    Task CheckValidElectricCarInput(AddElectricCarRequest request);
    Task CheckValidFuelCarInput(AddFuelCarRequest request);
}