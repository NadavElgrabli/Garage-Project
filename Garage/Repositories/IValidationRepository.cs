using Garage.Models;

namespace Garage.Repositories;

public interface IValidationRepository
{
    void CheckValidElectricCarInput(AddElectricCarRequest request);
    void CheckValidFuelCarInput(AddFuelCarRequest request);
    void CheckValidFuelMotorcycleInput(AddFuelMotorcycleRequest request);
    void CheckValidElectricMotorcycleInput(AddElectricMotorcycleRequest request);
    void CheckValidTruckInput(AddTruckRequest request);
    void CheckValidDroneInput(AddDroneRequest request);
}