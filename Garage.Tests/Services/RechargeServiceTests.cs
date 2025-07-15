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
    public async Task TreatAsync_ShouldRechargeVehicleAndReturnCorrectPrice()
    {
        // Arrange
        var vehicle = new Car
        {
            Engine = new ElectricEngine { CurrentEnergy = 10, MaxEnergy = 100 },
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Recharge },
            Status = Status.Pending
        };

        var request = new ChargeRequest
        {
            Vehicle = vehicle,
            RequestedHoursToCharge = 5
        };

        var service = new RechargeService();

        // Pre-set semaphores to simulate availability
        GarageState.ChargeStationsRequestsSemaphore = new SemaphoreSlim(1);
        GarageState.WorkersSemaphore = new SemaphoreSlim(1);

        // Act
        float price = await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(50, price); // 5 hours * 10
        Assert.Equal(vehicle.Engine.MaxEnergy, vehicle.Engine.CurrentEnergy);
        Assert.DoesNotContain(TreatmentType.Recharge, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }
}