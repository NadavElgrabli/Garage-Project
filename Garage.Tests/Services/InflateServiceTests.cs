using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Services;
using Microsoft.Extensions.Configuration;

namespace Garage.Tests.Services;

public class InflateServiceTests
{
    private IConfiguration CreateInflateTestConfig()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Inflate:DelayPerPressureUnitInMilliseconds", "0"},
            {"Inflate:ExplosionPenalty", "350"},
            {"Inflate:PricePerPressureUnit", "0.1"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task TreatAsync_ShouldInflateWheelsFully()
    {
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
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            Wheels = wheels,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = CreateInflateTestConfig();
        var service = new InflateService(garageState, config);

        await service.TreatAsync(vehicle, request);

        for (int i = 0; i < wheels.Count; i++)
            Assert.Equal(desiredPressures[i], wheels[i].CurrentPressure);

        Assert.Equal(1.8f, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverInflateCertainWheels()
    {
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 3, MaxPressure = 6 },
            new Wheel { CurrentPressure = 3, MaxPressure = 6 },
            new Wheel { CurrentPressure = 4, MaxPressure = 8 },
            new Wheel { CurrentPressure = 4, MaxPressure = 8 }
        };

        var desiredPressures = new List<float> { 8, 6, 10, 8 };

        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            Wheels = wheels,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = CreateInflateTestConfig();
        var service = new InflateService(garageState, config);

        await service.TreatAsync(vehicle, request);

        Assert.Equal(0, wheels[0].CurrentPressure);
        Assert.Equal(6, wheels[1].CurrentPressure);
        Assert.Equal(0, wheels[2].CurrentPressure);
        Assert.Equal(8, wheels[3].CurrentPressure);

        float expectedPrice = 350 + 0.3f + 350 + 0.4f;
        Assert.Equal(expectedPrice, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldOverInflateAllWheels()
    {
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 1, MaxPressure = 5 },
            new Wheel { CurrentPressure = 2, MaxPressure = 6 },
            new Wheel { CurrentPressure = 3, MaxPressure = 7 },
            new Wheel { CurrentPressure = 4, MaxPressure = 6 }
        };

        var desiredPressures = new List<float> { 10, 10, 10, 10 };

        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            Wheels = wheels,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = CreateInflateTestConfig();
        var service = new InflateService(garageState, config);

        await service.TreatAsync(vehicle, request);

        foreach (var wheel in wheels)
            Assert.Equal(0, wheel.CurrentPressure);

        Assert.Equal(1400, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }

    [Fact]
    public async Task TreatAsync_ShouldPartiallyInflateAllWheels()
    {
        var wheels = new List<Wheel>
        {
            new Wheel { CurrentPressure = 2, MaxPressure = 10 },
            new Wheel { CurrentPressure = 3, MaxPressure = 10 },
            new Wheel { CurrentPressure = 4, MaxPressure = 10 },
            new Wheel { CurrentPressure = 5, MaxPressure = 10 }
        };

        var desiredPressures = new List<float> { 6, 6, 6, 6 };

        var vehicle = new Car
        {
            TreatmentTypes = new List<TreatmentType> { TreatmentType.Inflate },
            Status = Status.Pending
        };

        var request = new AirRequest
        {
            Vehicle = vehicle,
            Wheels = wheels,
            DesiredWheelPressures = desiredPressures
        };

        var garageState = new GarageState();
        garageState.Initialize(1, 1, 1, 1);
        var config = CreateInflateTestConfig();
        var service = new InflateService(garageState, config);

        await service.TreatAsync(vehicle, request);

        for (int i = 0; i < wheels.Count; i++)
            Assert.Equal(desiredPressures[i], wheels[i].CurrentPressure);

        Assert.Equal(1.0f, vehicle.TreatmentsPrice);
        Assert.DoesNotContain(TreatmentType.Inflate, vehicle.TreatmentTypes);
        Assert.Equal(Status.Ready, vehicle.Status);
    }
}