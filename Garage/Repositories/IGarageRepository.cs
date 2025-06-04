using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public interface IGarageRepository
{
    void AddAndEnqueFuelVehicle(FuelRequest fuelRequest,  AirRequest airRequest);
    void AddAndEnqueElectricVehicle(ChargeRequest chargeRequest, AirRequest airRequest);


    Vehicle? GetVehicleByLicensePlate(string licensePlate);
    Task CheckValidElectricCarInput(AddElectricCarRequest request);
    Task CheckValidFuelCarInput(AddFuelCarRequest request);


    List <VehicleInfo> DisplayVehiclesByStatus(Status status);
}