using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class GarageOrchestratorService
{
    private readonly GarageManagementService _garageManagementService;
    private readonly ListProcessorService _listProcessorService;
    private readonly ValidationService _validationService;

    public GarageOrchestratorService(GarageManagementService garageManagementService, ListProcessorService listProcessorService, ValidationService validationService)
    {
        _garageManagementService = garageManagementService;
        _listProcessorService = listProcessorService;
        _validationService = validationService;
    }

    public void InitializeGarage(GarageInit init)
    {
        _garageManagementService.InitializeGarage(init);
    }

    public IEnumerable<VehicleInfo> GetVehiclesByStatus(Status status)
    {
        return _garageManagementService.DisplayVehiclesByStatus(status);
    }

    public Vehicle GetVehicleByLicensePlate(string licensePlate)
    {
        return _garageManagementService.GetVehicleByLicensePlateOrThrow(licensePlate);
    }

    public Vehicle PickUpVehicle(string licensePlate)
    {
        return _garageManagementService.PickUpVehicle(licensePlate);
    }

    public void AddElectricCar(AddElectricCarRequest request)
    {
        _validationService.CheckValidElectricCarInput(request);
        var car = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateElectricCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }

    public void AddFuelCar(AddFuelCarRequest request)
    {
        _validationService.CheckValidFuelCarInput(request);
        var car = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateFuelCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }

    public void AddTruck(AddTruckRequest request)
    {
        _validationService.CheckValidTruckInput(request);
        var truck = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateTruckTreatmentRequests(truck, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }
}
