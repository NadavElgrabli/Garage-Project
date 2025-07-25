﻿using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class InflateService : ITreatmentService
{
    private readonly GarageState _garageState;
    
    private readonly int _delayPerPressureUnitInMilliseconds;
    private readonly float _explosionPenalty;
    private readonly float _pricePerPressureUnit;

    public InflateService(GarageState garageState, IConfiguration config)
    {
        _garageState = garageState;
        
        _delayPerPressureUnitInMilliseconds = config.GetValue<int>(ConfigurationKeys.Inflate.DelayPerPressureUnitInMilliseconds);
        _explosionPenalty = config.GetValue<float>(ConfigurationKeys.Inflate.ExplosionPenalty);
        _pricePerPressureUnit = config.GetValue<float>(ConfigurationKeys.Inflate.PricePerPressureUnit);
    }
    
    public async Task TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not AirRequest airRequest)
            throw new ArgumentException("Invalid data type. Expected AirRequest.");

        List<float> desiredPressures = airRequest.DesiredWheelPressures;

        await _garageState.AirStationsRequestsSemaphore.WaitAsync();
        await _garageState.WorkersSemaphore.WaitAsync();

        try
        {
            float totalPrice = 0;
            vehicle.Status = Status.InTreatment;

            for (int i = 0; i < airRequest.Wheels.Count; i++)
            {
                var wheel = airRequest.Wheels[i];
                float target = desiredPressures[i];

                if (target > wheel.MaxPressure)
                {
                    await Task.Delay((int)(wheel.MaxPressure - wheel.CurrentPressure) * _delayPerPressureUnitInMilliseconds);
                    wheel.CurrentPressure = 0;
                    totalPrice += _explosionPenalty;
                }
                else
                {
                    float pressureToAdd = MathF.Max(0, target - wheel.CurrentPressure);
                    await Task.Delay((int)pressureToAdd * _delayPerPressureUnitInMilliseconds);
                    wheel.CurrentPressure += pressureToAdd;
                    totalPrice += pressureToAdd * _pricePerPressureUnit;
                }
            }

            vehicle.TreatmentsPrice += totalPrice;
            vehicle.TreatmentTypes.Remove(TreatmentType.Inflate);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;
        }
        finally
        {
            _garageState.WorkersSemaphore.Release();
            _garageState.AirStationsRequestsSemaphore.Release();
        }
    }

    public TreatmentType GetTreatmentType()
    {
        var treatmentType = TreatmentType.Inflate;
        
        return treatmentType;
    }
}
