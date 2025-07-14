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
            int milliseconds = (int)hoursToCharge * 10000;

            await Task.Delay(milliseconds);

            if (hoursToCharge > vehicle.Engine.MaxEnergy)
            {
                totalPrice += 1500;
                var random = new Random();
                vehicle.Engine.CurrentEnergy = (float)random.NextDouble() * (vehicle.Engine.MaxEnergy - 1) + 1;
            }

            vehicle.Engine.CurrentEnergy = vehicle.Engine.MaxEnergy;
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