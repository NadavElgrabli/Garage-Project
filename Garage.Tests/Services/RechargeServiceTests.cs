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
            {"Recharge:MillisecondsPerHour", "10000"},
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task TreatAsync_ShouldRechargeVehicleFully_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var vehicle = new Car
        {
            Engine = new ElectricEngine { CurrentEnergy = 1, MaxEnergy = 5 },
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = 4
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(40, vehicle.TreatmentsPrice);
        Assert.Equal(vehicle.Engine.MaxEnergy, vehicle.Engine.CurrentEnergy);
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverChargeVehicle_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var vehicle = new Car
        {
            Engine = new ElectricEngine { CurrentEnergy = 2, MaxEnergy = 5 },
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = 10
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.True(vehicle.Engine.CurrentEnergy <= vehicle.Engine.MaxEnergy, "Engine should not exceed max energy");
        Assert.True(vehicle.Engine.CurrentEnergy >= 2, "Engine should be at least the original energy");
        Assert.Equal(3 * 10 + 1500, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldPartiallyRechargeVehicle_WhenVehicleIsNotFullyCharged()
    {
        // Arrange
        var vehicle = new Car
        {
            Engine = new ElectricEngine { CurrentEnergy = 2, MaxEnergy = 10 },
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = 5
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = GetTestConfiguration();

        var service = new RechargeService(garageState, config);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(7, vehicle.Engine.CurrentEnergy);
        Assert.Equal(50, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }
}
