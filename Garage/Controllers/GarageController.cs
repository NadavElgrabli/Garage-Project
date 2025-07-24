using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;
//TODO's:
// 1) check: enter before return - 
// 2) check: "== null" - 
// 3) check: no {} for 1 line ifs -
// 4) check: always create new var before returning
// 5) In handlers, use repository when you call db
// 6) Create a new class (Under Garage) called ConfigurationKeys. This class will have the route
// for each of the appsettings.json key, save as const and then use as shortcut where they
// are used - done
// 7) In GarageManagementService, create handlers for generating treatmentRequests.
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