using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public interface IGarageRepository
{
    void AddVehicleToGarage(Vehicle vehicle);
    void EnqueVehicle(TreatmentRequest firstRequest, TreatmentRequest secondRequest);
    
    Vehicle? GetVehicleByLicensePlate(string licensePlate);
    Task CheckValidElectricCarInput(AddElectricCarRequest request);
    Task CheckValidFuelCarInput(AddFuelCarRequest request);


    List <VehicleInfo> DisplayVehiclesByStatus(Status status);
}