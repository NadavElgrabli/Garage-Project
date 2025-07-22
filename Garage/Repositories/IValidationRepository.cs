using Garage.Models;

namespace Garage.Repositories;

public interface IValidationRepository
{
    void CheckValidElectricCarInput(AddElectricCarRequest request);
    void CheckValidFuelCarInput(AddFuelCarRequest request);
}