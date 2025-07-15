using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class RechargeService : ITreatmentService
{
    public async Task<float> TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not ChargeRequest chargeRequest)
            throw new ArgumentException("Invalid data type. Expected ChargeRequest.");

        float hoursToCharge = chargeRequest.RequestedHoursToCharge;

        await GarageState.ChargeStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;

            if (vehicle.Engine.CurrentEnergy == vehicle.Engine.MaxEnergy)
                throw new Exception("Engine fully charged already");

            float totalPrice = hoursToCharge * 10;
            int milliseconds = (int)(hoursToCharge * 10000);
            
            // Overflow of charge
            if (hoursToCharge > vehicle.Engine.MaxEnergy - vehicle.Engine.CurrentEnergy)
            {
                totalPrice += 1500;
                var random = new Random();
                vehicle.Engine.CurrentEnergy = (float)random.NextDouble() * (vehicle.Engine.MaxEnergy - 1) + 1;
                int randomDelay = random.Next(0, milliseconds + 1);
                await Task.Delay(randomDelay);

            }
            else
            {
                vehicle.Engine.CurrentEnergy += hoursToCharge;
                await Task.Delay(milliseconds);
            }
            
            vehicle.TreatmentsPrice = totalPrice;
            vehicle.TreatmentTypes.Remove(TreatmentType.Recharge);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;

            return totalPrice;
        }
        finally
        {
            GarageState.WorkersSemaphore.Release();
            GarageState.ChargeStationsRequestsSemaphore.Release();
        }
    }

    public TreatmentType GetTreatmentType()
    {
        return TreatmentType.Recharge;
    }
}