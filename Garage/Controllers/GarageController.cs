using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarageController : ControllerBase
{
    private readonly GarageOrchestratorService _garageOrchestratorService;

    public GarageController(GarageOrchestratorService garageOrchestratorService)
    {
        _garageOrchestratorService = garageOrchestratorService;
    }

    [HttpPost("InitializeGarage")]
    public IActionResult InitializeGarage([FromBody] GarageInit init)
    {
        _garageOrchestratorService.InitializeGarage(init);
        
        return Ok("Garage initialized successfully");
    }

    [HttpPost("GetVehiclesByStatus")]
    public IActionResult GetVehiclesByStatus([FromQuery] Status status)
    {
        var vehicles = _garageOrchestratorService.GetVehiclesByStatus(status);
        
        return Ok(vehicles);
    }
    
    [HttpPost("GetVehicleByLicensePlate")]
    public IActionResult GetVehicleByLicensePlate([FromQuery] string licensePlate)
    {
        var vehicle = _garageOrchestratorService.GetVehicleByLicensePlate(licensePlate);
        
        return Ok(vehicle);
    }
    
    [HttpPost("PickUpVehicleFromGarage")]
    public IActionResult PickUpVehicleFromGarage([FromQuery] string licensePlate)
    {
        var vehicle = _garageOrchestratorService.PickUpVehicle(licensePlate);
        
        return Ok(vehicle);
    }
    
    [HttpPost("AddElectricCar")]
    public IActionResult AddElectricCar([FromBody] AddElectricCarRequest request)
    {
        _garageOrchestratorService.AddElectricCar(request);
        
        return Ok("Electric car added successfully");
    }
    

    [HttpPost("AddFuelCar")]
    public IActionResult AddFuelCar([FromBody] AddFuelCarRequest request)
    {
        _garageOrchestratorService.AddFuelCar(request);
        
        return Ok("Fuel car added successfully");
    }
    
    [HttpPost("AddTruck")]
    public IActionResult AddTruck([FromBody] AddTruckRequest request)
    {
        _garageOrchestratorService.AddTruck(request);
        
        return Ok("Truck added successfully");
    }

    [HttpPost("AddDrone")]
    public IActionResult AddDrone([FromBody] AddDroneRequest request)
    {
        _garageOrchestratorService.AddDrone(request);
        
        return Ok("Drone added successfully");
    }
    
    [HttpPost("AddFuelMotorcycle")]
    public IActionResult AddFuelMotorcycle([FromBody] AddFuelMotorcycleRequest request)
    {
        _garageOrchestratorService.AddFuelMotorcycle(request);
        
        return Ok("Fuel motorcycle added successfully");
    }
    
    [HttpPost("AddElectricMotorcycle")]
    public IActionResult AddElectricMotorcycle([FromBody] AddElectricMotorcycleRequest request)
    {
        _garageOrchestratorService.AddElectricMotorcycle(request);
        
        return Ok("Electric motorcycle added successfully");
    }
}