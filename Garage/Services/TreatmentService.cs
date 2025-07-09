using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class TreatmentService
{
    public async Task<float> RefuelAsync(Vehicle vehicle, float litersToFuel)
    {
        await GarageState.FuelStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;

            if (vehicle.Engine.CurrentEnergy == vehicle.Engine.MaxEnergy)
            {
                throw new Exception("Engine fully fueled already");
            }

            float missingFuelAmount = vehicle.Engine.MaxEnergy - vehicle.Engine.CurrentEnergy;
            float totalPrice = missingFuelAmount * 5;
            int milliseconds = (int)missingFuelAmount * 250;

            await Task.Delay(milliseconds);

            if (vehicle.Engine.CurrentEnergy + litersToFuel > vehicle.Engine.MaxEnergy)
            {
                totalPrice += 25; // Made a mess, spilled fuel, cost to clean is 25
            }

            vehicle.Engine.CurrentEnergy = vehicle.Engine.MaxEnergy;
            vehicle.TreatmentTypes.Remove(TreatmentType.Refuel);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;

            return totalPrice;
        }

        finally
        {
            GarageState.WorkersSemaphore.Release();
            GarageState.FuelStationsRequestsSemaphore.Release();
        }
    }

    public async Task<float> RechargeAsync(Vehicle vehicle, float hoursToCharge)
    {
        await GarageState.ChargeStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            vehicle.Status = Status.InTreatment;

            if (vehicle.Engine.CurrentEnergy == vehicle.Engine.MaxEnergy)
            {
                throw new Exception("Engine fully charged already");
            }

            float totalPrice = hoursToCharge *= 10;
            int milliseconds = (int)hoursToCharge * 10000;

            await Task.Delay(milliseconds);

            if (hoursToCharge > vehicle.Engine.MaxEnergy)
            {
                totalPrice += 1500; //burnt battery
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

    public async Task<float> InflateTiresAsync(Vehicle vehicle, List<float> desiredPressures)
    {
        await GarageState.AirStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            float totalPrice = 0;
            
            vehicle.Status =  Status.InTreatment;

            for (int i = 0; i < vehicle.Wheels.Count; i++)
            {
                var wheel = vehicle.Wheels[i];
                float target = desiredPressures[i];
                float pressureToAdd = MathF.Max(0, target - wheel.CurrentPressure);
                await Task.Delay((int) pressureToAdd * 500);

                if (target > wheel.MaxPressure)
                {
                    wheel.CurrentPressure = 0;
                    totalPrice += 350;
                }
                else
                {
                    wheel.CurrentPressure += pressureToAdd;
                    totalPrice += pressureToAdd * 0.1f;
                }
            }
            
            vehicle.TreatmentTypes.Remove(TreatmentType.Inflate);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;
            
            return totalPrice;
        }
        finally
        {
            GarageState.WorkersSemaphore.Release();
            GarageState.AirStationsRequestsSemaphore.Release();
        }
    }
}