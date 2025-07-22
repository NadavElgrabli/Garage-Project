using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;
//TODO:
// 1) Factory design pattern (check program.cs) - do handlers
// 2) Lin ListProcessorService fix AddVehicleRequestsToMatchingList switch for loop
// To LINQ.
// 4) In all Handlers, add database as DI instead of receiving db in function
// 5) Garage state, check what to do with the nulls - done
// 6) In all exceptions (validation repository and GarageManagementService) dont use parameters
// in the exceptions you throw, use exception params instead.
// use https://www.ashtamkea.com/p/csharpexeptionhandling.html - done
// 7) Use IConfiguration for magic numbers (validation repository / services/ etc..)
// 8) In the tests, use DP for InMemoryDatabase instead of creating a new class?
// 9) check : enter before return - 
// 10) == null - 
// 11) no {} for 1 line ifs -
// 12) always create new var before returning
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
}