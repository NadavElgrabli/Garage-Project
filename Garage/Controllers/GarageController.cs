using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarageController : ControllerBase
{
    private readonly GarageService _garageService;
    private readonly ListProcessorService _listProcessorService;
    private readonly ValidationService _validationService;

    public GarageController(GarageService garageService,  ListProcessorService listProcessorService,  ValidationService validationService)
    {
        _garageService = garageService;
        _listProcessorService = listProcessorService;
        _validationService = validationService;
    }

    [HttpPost("InitializeGarage")]
    public IActionResult InitializeGarage([FromBody] GarageInit init)
    {
        _garageService.InitializeGarage(init);
        return Ok("Garage initialized successfully");
    }


    [HttpPost("GetVehiclesByStatus")]
    public IActionResult GetVehiclesByStatus([FromQuery] Status status)
    {
        var vehicles = _garageService.DisplayVehiclesByStatus(status);
        return Ok(vehicles);
    }
    
    [HttpPost("GetVehicleByLicensePlate")]
    public IActionResult GetVehicleByLicensePlate([FromQuery] string licensePlate)
    {
        var vehicle = _garageService.GetVehicleByLicensePlateOrThrow(licensePlate);
        return Ok(vehicle);
    }

    
    [HttpPost("PickUpVehicleFromGarage")]
    public IActionResult PickUpVehicleFromGarage([FromQuery] string licensePlate)
    {
        var vehicle = _garageService.PickUpVehicle(licensePlate);
        return Ok(vehicle);
    }
    
    [HttpPost("AddElectricCar")]
    public async Task<IActionResult> AddElectricCar([FromBody] AddElectricCarRequest request)
    {
        await _validationService.CheckValidElectricCarInput(request);

        var (chargeRequest, airRequest) = _garageService.PrepareElectricCar(request);
        _listProcessorService.AddVehicleRequestsToMatchingList(chargeRequest, airRequest);

        return Ok("Electric car added successfully");
    }

    [HttpPost("AddFuelCar")]
    public async Task<IActionResult> AddFuelCar([FromBody] AddFuelCarRequest request)
    {
        await _validationService.CheckValidFuelCarInput(request);

        var (fuelRequest, airRequest) = _garageService.PrepareFuelCar(request);
        _listProcessorService.AddVehicleRequestsToMatchingList(fuelRequest, airRequest);

        return Ok("Fuel car added successfully");
    }

}