﻿using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RechargeService : ITreatmentService
{
    private readonly GarageState _garageState;
    private readonly float _pricePerHour;
    private readonly float _overchargePenalty;
    private readonly int _millisecondsPerHour;

    public RechargeService(GarageState garageState, IConfiguration config)
    {
        _garageState = garageState;
        
        _pricePerHour = config.GetValue<float>(ConfigurationKeys.Recharge.PricePerHour);
        _overchargePenalty = config.GetValue<float>(ConfigurationKeys.Recharge.OverchargePenalty);
        _millisecondsPerHour = config.GetValue<int>(ConfigurationKeys.Recharge.MillisecondsPerHour);
    }
    
    public async Task TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not ChargeRequest chargeRequest)
            throw new ArgumentException("Invalid data type. Expected ChargeRequest.");

        float hoursToCharge = chargeRequest.RequestedHoursToCharge;

        await _garageState.ChargeStationsRequestsSemaphore.WaitAsync();
        await _garageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;
            float energyToFullCharge = chargeRequest.Engine.MaxEnergy - chargeRequest.Engine.CurrentEnergy;
            float totalPrice = hoursToCharge * _pricePerHour;
            
            if (chargeRequest.Engine.CurrentEnergy + hoursToCharge > chargeRequest.Engine.MaxEnergy)
            {
                totalPrice = (energyToFullCharge) * _pricePerHour;
                totalPrice += _overchargePenalty;
                int timeToFullyCharge = (int)(energyToFullCharge) * (_millisecondsPerHour);
                
                await Task.Delay(timeToFullyCharge);
                
                var random = new Random();
                float min = chargeRequest.Engine.CurrentEnergy;
                float max = chargeRequest.Engine.MaxEnergy;
                
                chargeRequest.Engine.CurrentEnergy = (float)(random.NextDouble() * (max - min) + min);
            }
            else
            {
                int milliseconds = (int)(hoursToCharge * _millisecondsPerHour);
                chargeRequest.Engine.CurrentEnergy += hoursToCharge;
                await Task.Delay(milliseconds);
            }
            
            vehicle.TreatmentsPrice += totalPrice;
            vehicle.TreatmentTypes.Remove(TreatmentType.Recharge);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;
        }
        finally
        {
            _garageState.WorkersSemaphore.Release();
            _garageState.ChargeStationsRequestsSemaphore.Release();
        }
    }

    public TreatmentType GetTreatmentType()
    {
        var treatmentType = TreatmentType.Recharge;
        
        return treatmentType;
    }
}