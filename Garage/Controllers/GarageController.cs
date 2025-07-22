using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Controllers;
//TODO:
// 1) Factory design pattern (check program.cs) - done
// 2) ListRepository - move AddVehicleRequestsToMatchingList to
// ListProcessorService and use handlers - done
// 3) Garage state, check what to do with the nulls - done
// 4) In all exceptions (validation repository and GarageManagementService) dont use parameters
// in the exceptions you throw, use exception params instead. - done
// 5) In the tests, use DP for InMemoryDatabase instead of creating a new class - Need to create 
// a new interface just for InMEmoryDatabase in order to mock it?
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
}