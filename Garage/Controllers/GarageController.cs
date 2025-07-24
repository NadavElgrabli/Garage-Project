using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;
//TODO's:
// 1) Factory design pattern (use handlers) in ListProcessorService - done
// 2) In ListProcessorService fix AddVehicleRequestsToMatchingList switch "for" loop To LINQ - check(?)
// 3) In all Handlers, add database as DI instead of receiving db in function - done
// 4) In all Handlers IsMatch create new var before returning - done
// 5) In all exceptions (validation repository and GarageManagementService) dont use parameters
// in the exceptions you throw, use exception params instead.
// use https://www.ashtamkea.com/p/csharpexeptionhandling.html - done
// 6) Use IConfiguration for magic numbers (validation repository / services/ etc..) - done
// 7) In the tests, use DP for InMemoryDatabase instead of creating a new class?
// 8) check: enter before return - 
// 9) check: "== null" - 
// 10) check: no {} for 1 line ifs -
// 11) check: always create new var before returning
// 12) move all treatments to handlers, they aren't services -
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
}