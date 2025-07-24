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
        var vehicles = _garageManagementService.DisplayVehiclesByStatus(status);
        
        return vehicles;
    }

    public Vehicle GetVehicleByLicensePlate(string licensePlate)
    {
        var vehicle = _garageManagementService.GetVehicleByLicensePlateOrThrow(licensePlate);
        
        return vehicle;
    }

    public Vehicle PickUpVehicle(string licensePlate)
    {
        var vehicle = _garageManagementService.PickUpVehicle(licensePlate);
        
        return vehicle;
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
    
    public void AddFuelMotorcycle(AddFuelMotorcycleRequest request)
    {
        _validationService.CheckValidFuelMotorcycleInput(request);
        var motorcycle = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateFuelMotorcycleTreatmentRequests(motorcycle, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }
    
    public void AddElectricMotorcycle(AddElectricMotorcycleRequest request)
    {
        _validationService.CheckValidElectricMotorcycleInput(request);
        var motorcycle = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateElectricMotorcycleTreatmentRequests(motorcycle, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }

    public void AddTruck(AddTruckRequest request)
    {
        _validationService.CheckValidTruckInput(request);
        var truck = _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateTruckTreatmentRequests(truck, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }

    public void AddDrone(AddDroneRequest request)
    {
        _validationService.CheckValidDroneInput(request);
        var drone =  _garageManagementService.CreateAndAddVehicleToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateDroneTreatmentRequest(drone, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);
    }
}
