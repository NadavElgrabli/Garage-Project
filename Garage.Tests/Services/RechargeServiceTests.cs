using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.Extensions.Configuration;

namespace Garage.Tests.Services;

public class RechargeServiceTests
{
    private IConfiguration GetTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Recharge:PricePerHour", "10"},
            {"Recharge:OverchargePenalty", "1500"},
            {"Recharge:MillisecondsPerHour", "1"}, // make test run faster
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task TreatAsync_ShouldRechargeVehicleFully_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var engine = new ElectricEngine { CurrentEnergy = 1, MaxEnergy = 5 };
        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedHoursToCharge = 4
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(40, vehicle.TreatmentsPrice); // 4 hours * 10
        Assert.Equal(engine.MaxEnergy, engine.CurrentEnergy); // Fully charged
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverChargeVehicle_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var engine = new ElectricEngine { CurrentEnergy = 2, MaxEnergy = 5 };
        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedHoursToCharge = 10
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.True(engine.CurrentEnergy <= engine.MaxEnergy, "Engine should not exceed max energy");
        Assert.True(engine.CurrentEnergy >= 2, "Engine should not be less than original");
        Assert.Equal(3 * 10 + 1500, vehicle.TreatmentsPrice); // 3 hours * 10 + penalty
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldPartiallyRechargeVehicle_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var engine = new ElectricEngine { CurrentEnergy = 2, MaxEnergy = 10 };
        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            Engine = engine,
            RequestedHoursToCharge = 5
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(7, engine.CurrentEnergy); // 2 + 5
        Assert.Equal(50, vehicle.TreatmentsPrice); // 5 hours * 10
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }
}

