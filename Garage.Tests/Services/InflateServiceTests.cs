using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;

namespace Garage.Tests.Services;

using System.Threading.Tasks;
using Xunit;
public class InflateServiceTests
{
    [Fact]
    public async Task TreatAsync_ShouldInflateWheelsFully()
    {
        // Arrange
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 2, MaxPressure = 8 },
            new Wheel { CurrentPressure = 3, MaxPressure = 8 },
            new Wheel { CurrentPressure = 4, MaxPressure = 8 },
            new Wheel { CurrentPressure = 5, MaxPressure = 8 }
        };

        var desiredPressures = new List<float> { 8, 8, 8, 8 }; 

        var vehicle = new Car
        {
            Wheels = wheels,
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new InflateService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        for (int i = 0; i < wheels.Count; i++)
        {
            Assert.Equal(desiredPressures[i], wheels[i].CurrentPressure);
        }

        // Total added pressure = 6 + 5 + 4 + 3 = 18
        // Price = 18 * 0.1 = 1.8
        Assert.Equal(1.8f, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverInflateCertainWheels()
    {
        // Arrange
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 3, MaxPressure = 6 }, // Will explode (target 8)
            new Wheel { CurrentPressure = 3, MaxPressure = 6 }, 
            new Wheel { CurrentPressure = 4, MaxPressure = 8 }, // Will explode (target 10)
            new Wheel { CurrentPressure = 4, MaxPressure = 8 }  
        };

        var desiredPressures = new List<float> { 8, 6, 10, 8 };

        var vehicle = new Car
        {
            Wheels = wheels,
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new InflateService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        Assert.Equal(0, wheels[0].CurrentPressure); // Exploded
        Assert.Equal(6, wheels[1].CurrentPressure); // Inflated normally
        Assert.Equal(0, wheels[2].CurrentPressure); // Exploded
        Assert.Equal(8, wheels[3].CurrentPressure); // Inflated normally

        // Price:
        // Wheel 0: exploded = +350
        // Wheel 1: 3 * 0.1 = 0.3
        // Wheel 2: exploded = +350
        // Wheel 3: 4 * 0.1 = 0.4
        float expectedPrice = 350 + 0.3f + 350 + 0.4f;
        Assert.Equal(expectedPrice, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverInflateAllWheels()
    {
        // Arrange
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 1, MaxPressure = 5 },
            new Wheel { CurrentPressure = 2, MaxPressure = 6 },
            new Wheel { CurrentPressure = 3, MaxPressure = 7 },
            new Wheel { CurrentPressure = 4, MaxPressure = 6 }
        };

        var desiredPressures = new List<float> { 10, 10, 10, 10 }; // All targets are over max

        var vehicle = new Car
        {
            Wheels = wheels,
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new InflateService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        foreach (var wheel in wheels)
        {
            Assert.Equal(0, wheel.CurrentPressure); // All exploded
        }

        // Each explosion costs 350. 4 wheels = 4 * 350 = 1400
        Assert.Equal(1400, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldPartiallyInflateAllWheels()
    {
        // Arrange
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 2, MaxPressure = 10 },
            new Wheel { CurrentPressure = 3, MaxPressure = 10 },
            new Wheel { CurrentPressure = 4, MaxPressure = 10 },
            new Wheel { CurrentPressure = 5, MaxPressure = 10 }
        };

        var desiredPressures = new List<float> { 6, 6, 6, 6 }; // All MaxPressure > desiredPressures

        var vehicle = new Car
        {
            Wheels = wheels,
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1); // initialize with 1 worker and 1 of each station
        var service = new InflateService(garageState);

        // Act
        await service.TreatAsync(vehicle, request);

        // Assert
        for (int i = 0; i < wheels.Count; i++)
        {
            Assert.Equal(desiredPressures[i], wheels[i].CurrentPressure);
        }

        // Pressure added: 4 + 3 + 2 + 1 = 10
        // Price = 10 * 0.1 = 1.0
        Assert.Equal(1.0f, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }



}