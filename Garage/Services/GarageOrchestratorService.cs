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

    public async Task AddElectricCar(AddElectricCarRequest request)
    {
        await _validationService.CheckValidElectricCarInput(request);
        var car = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateElectricCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }

    public async Task AddFuelCar(AddFuelCarRequest request)
    {
        await _validationService.CheckValidFuelCarInput(request);
        var car = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateFuelCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }
}
