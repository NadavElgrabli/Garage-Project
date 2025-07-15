using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Repositories;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarageController : ControllerBase
{
    private readonly GarageService _garageService;
    private readonly ListProcessorService _listProcessorService;
    private readonly IValidationRepository _validationRepository;

    public GarageController(GarageService garageService,  ListProcessorService listProcessorService,  IValidationRepository validationRepository)
    {
        _garageService = garageService;
        _listProcessorService = listProcessorService;
        _validationRepository = validationRepository;
    }

    [HttpPost("InitializeGarage")]
    public IActionResult InitializeGarage([FromBody] GarageInit init)
    {
        if (init.Workers <= 0 || init.FuelStations <= 0 || init.AirStations <= 0 || init.ChargingStations <= 0)
        {
            return BadRequest("All values must be positive");
        }
        
        GarageState.Initialize(
            init.Workers,
            init.FuelStations,
            init.AirStations,
            init.ChargingStations);
        
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
        var v = _garageService.GetVehicleByLicensePlate(licensePlate);
        if (v == null)
        {
            return NotFound("Vehicle not found");
        }
        return Ok(v);
    }

    [HttpPost("AddElectricCar")]
    public async Task<IActionResult> AddElectricCar([FromBody] AddElectricCarRequest request)
    {
        try
        {
            await _validationRepository.CheckValidElectricCarInput(request);
            var (chargeRequest, airRequest) = _garageService.PrepareElectricCar(request);
            _listProcessorService.AddVehicleRequestsToMatchingList(chargeRequest, airRequest);
            return Ok("Electric car added successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("AddFuelCar")]
    public async Task<IActionResult> AddFuelCar([FromBody] AddFuelCarRequest request)
    {
        try
        {
            await _validationRepository.CheckValidFuelCarInput(request);
            var (fuelRequest, airRequest) = _garageService.PrepareFuelCar(request);
            _listProcessorService.AddVehicleRequestsToMatchingList(fuelRequest, airRequest);
            return Ok("Fuel car added successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}