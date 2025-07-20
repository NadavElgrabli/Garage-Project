using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RechargeService : ITreatmentService
{
    private readonly GarageState _garageState;

    public RechargeService(GarageState garageState)
    {
        _garageState = garageState;
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
            float energyToFullCharge = vehicle.Engine.MaxEnergy - vehicle.Engine.CurrentEnergy;
            float totalPrice = hoursToCharge * 10;
            
            // Overflow of charge (for example current energy was 2, max energy is 5, but requested to charge 10 hours)
            if (vehicle.Engine.CurrentEnergy + hoursToCharge > vehicle.Engine.MaxEnergy)
            {
                // total price will be of 3 hours
                totalPrice = (energyToFullCharge) * 10;
                // penalty for overcharge
                totalPrice += 1500;
                // we will wait the time to fully charge 3 hours (until max energy which is 5)
                int timeToFullyCharge = (int)(energyToFullCharge) * (10000);
                await Task.Delay(timeToFullyCharge);
                var random = new Random();
                // the current energy will be random between the current energy and max energy because the engine is overflowed and messed up
                float min = vehicle.Engine.CurrentEnergy;
                float max = vehicle.Engine.MaxEnergy;

                vehicle.Engine.CurrentEnergy = (float)(random.NextDouble() * (max - min) + min);
            }
            // We recharge it to the maximum or less
            else
            {
                int milliseconds = (int)(hoursToCharge * 10000);
                vehicle.Engine.CurrentEnergy += hoursToCharge;
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
        return TreatmentType.Recharge;
    }
}