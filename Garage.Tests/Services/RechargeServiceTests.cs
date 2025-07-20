using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Tests.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


public class RechargeServiceTests
{
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
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new RechargeService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(40, vehicle.TreatmentsPrice); // 4 hours * 10
        Assert.Equal(vehicle.Engine.MaxEnergy, vehicle.Engine.CurrentEnergy); //fully charged
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes); // removed treatment from list
        Assert.Equal(Status.Ready, vehicle.Status); // changed status to ready
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
            RequestedHoursToCharge = 10 // clearly overcharging (10 > 3 left)
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new RechargeService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.True(vehicle.Engine.CurrentEnergy <= vehicle.Engine.MaxEnergy, "Engine should not exceed max energy");
        Assert.True(vehicle.Engine.CurrentEnergy >= 2, "Engine should be at least the original energy");
        Assert.Equal(3 * 10 + 1500, vehicle.TreatmentsPrice); // base price + overflow penalty
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes); // treatment removed
        Assert.Equal(Status.Ready, vehicle.Status); // no more treatments left
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
            RequestedHoursToCharge = 5 // Partial charge: 2 + 5 = 7 < 10
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new RechargeService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(7, vehicle.Engine.CurrentEnergy); // 2 + 5
        Assert.Equal(50, vehicle.TreatmentsPrice); // 5 hours * 10
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status); // No treatments left
    }
}