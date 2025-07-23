using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.Extensions.Configuration;

namespace Garage.Tests.Services;

public class RefuelServiceTests
{
    [Fact]
    public async Task TreatAsync_ShouldRefuelVehicleFully_WhenVehicleIsPartiallyFueled()
    {
        // Arrange
        var engine = new FuelEngine { CurrentEnergy = 1, MaxEnergy = 5 };

        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Refuel },
            Status = Status.Pending
        };

        var request = new FuelRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedLiters = 4
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Refuel:FuelPricePerLiter", "5" },
            { "Refuel:SpillCleanupCost", "10" },
            { "Refuel:MillisecondsPerLiter", "1" }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var service = new RefuelService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(20, vehicle.TreatmentsPrice); // 4 * 5
        Assert.Equal(5, request.Engine.CurrentEnergy); // Engine now lives in request
        Assert.DoesNotContain(TreatmentType.Refuel, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    
    [Fact]
    public async Task TreatAsync_ShouldOverFuelVehicle_WhenVehicleIsPartiallyFueled()
    {
        // Arrange
        var engine = new FuelEngine { CurrentEnergy = 2, MaxEnergy = 5 };
        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Refuel, TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new FuelRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedLiters = 10 // clearly overcharging (10 > 3 left)
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Refuel:FuelPricePerLiter", "5" },
            { "Refuel:SpillCleanupCost", "25" },
            { "Refuel:MillisecondsPerLiter", "1" }
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var service = new RefuelService(garageState, config);
        
        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(engine.MaxEnergy, engine.CurrentEnergy); // Should be fully fueled
        Assert.Equal(3 * 5 + 25, vehicle.TreatmentsPrice); // 3 liters * 5 + 25 spill cost
        Assert.DoesNotContain(TreatmentType.Refuel, vehicle.TreatmentTypes); // treatment removed
        Assert.Equal(Status.Pending, vehicle.Status); // still has to inflate
    }

    [Fact]
    public async Task TreatAsync_ShouldPartiallyRefuelVehicle_WhenVehicleIsPartiallyFueled()
    {
        // Arrange
        var engine = new FuelEngine { CurrentEnergy = 2, MaxEnergy = 6 };
        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Refuel },
            Status = Status.Pending
        };

        var request = new FuelRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedLiters = 3 // Partial refuel: 2 + 3 = 5 < 6
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Refuel:FuelPricePerLiter", "5" },
            { "Refuel:SpillCleanupCost", "25" },
            { "Refuel:MillisecondsPerLiter", "1" }
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var service = new RefuelService(garageState, config);
        
        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(5, engine.CurrentEnergy); // 2 + 3 = 5
        Assert.Equal(15, vehicle.TreatmentsPrice); // 3 liters * 5 = 15
        Assert.DoesNotContain(TreatmentType.Refuel, vehicle.TreatmentTypes); // treatment removed
        Assert.Equal(Status.Ready, vehicle.Status); // no treatments left
    }
}