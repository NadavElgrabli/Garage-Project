using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarageController : ControllerBase
{
    private readonly GarageManagementService _garageManagementService;
    private readonly ListProcessorService _listProcessorService;
    private readonly ValidationService _validationService;

    public GarageController(GarageManagementService garageManagementService,  ListProcessorService listProcessorService,  ValidationService validationService)
    {
        _garageManagementService = garageManagementService;
        _listProcessorService = listProcessorService;
        _validationService = validationService;
    }

    [HttpPost("InitializeGarage")]
    public IActionResult InitializeGarage([FromBody] GarageInit init)
    {
        _garageManagementService.InitializeGarage(init);
        return Ok("Garage initialized successfully");
    }


    [HttpPost("GetVehiclesByStatus")]
    public IActionResult GetVehiclesByStatus([FromQuery] Status status)
    {
        var vehicles = _garageManagementService.DisplayVehiclesByStatus(status);
        return Ok(vehicles);
    }
    
    [HttpPost("GetVehicleByLicensePlate")]
    public IActionResult GetVehicleByLicensePlate([FromQuery] string licensePlate)
    {
        var vehicle = _garageManagementService.GetVehicleByLicensePlateOrThrow(licensePlate);
        return Ok(vehicle);
    }

    
    [HttpPost("PickUpVehicleFromGarage")]
    public IActionResult PickUpVehicleFromGarage([FromQuery] string licensePlate)
    {
        var vehicle = _garageManagementService.PickUpVehicle(licensePlate);
        return Ok(vehicle);
    }
    
    [HttpPost("AddElectricCar")]
    public async Task<IActionResult> AddElectricCar([FromBody] AddElectricCarRequest request)
    {
        await _validationService.CheckValidElectricCarInput(request);
        
        var car = _garageManagementService.CreateAndAddElectricCarToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateElectricCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);

        return Ok("Electric car added successfully");
    }
    

    [HttpPost("AddFuelCar")]
    public async Task<IActionResult> AddFuelCar([FromBody] AddFuelCarRequest request)
    {
        await _validationService.CheckValidFuelCarInput(request);
        
        var car =  _garageManagementService.CreateAndAddFuelCarToGarage(request);
        var treatmentRequests = _garageManagementService.GenerateFuelCarTreatmentRequests(car, request);
        _listProcessorService.AddVehicleRequestsToMatchingList(treatmentRequests);

        return Ok("Fuel car added successfully");
    }

}