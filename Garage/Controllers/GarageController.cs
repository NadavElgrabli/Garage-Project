using Garage.Data;
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

    public GarageController(GarageService garageService)
    {
        _garageService = garageService;
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
            await _garageService.CheckValidElectricCarInput(request);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
        var car = _garageService.CreateElectricCar(request);
        var chargeRequest = _garageService.CreateChargeRequest(car, request.HoursToCharge);
        var airRequest = _garageService.CreateAirRequest(car, request.DesiredWheelPressures);
        _garageService.AddVehicleToGarage(car);
        // TODO: Add the EnqueVehicle to the QueueProcessor service
        _garageService.EnqueVehicle(chargeRequest, airRequest);
        
        return Ok("Electric car added successfully");
    }

    [HttpPost("AddFuelCar")]
    public async Task<IActionResult> AddFuelCar([FromBody] AddFuelCarRequest request)
    {
        try
        {
            await _garageService.CheckValidFuelCarInput(request);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
        var car = _garageService.CreateFuelCar(request);
        var fuelRequest = _garageService.CreateFuelRequest(car, request.LitersToFuel);
        var airRequest = _garageService.CreateAirRequest(car, request.DesiredWheelPressures);
        _garageService.AddVehicleToGarage(car);
        _garageService.EnqueVehicle(fuelRequest, airRequest);
        
        return Ok("Fuel car added successfully");
    }
}