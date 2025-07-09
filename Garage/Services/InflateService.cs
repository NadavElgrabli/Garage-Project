using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class InflateService : ITreatmentService
{
    public async Task<float> TreatAsync(Vehicle vehicle, object data)
    {
        if (data is not List<float> desiredPressures)
            throw new ArgumentException("Invalid data for inflating tires");

        await GarageState.AirStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            float totalPrice = 0;
            vehicle.Status = Status.InTreatment;

            for (int i = 0; i < vehicle.Wheels.Count; i++)
            {
                var wheel = vehicle.Wheels[i];
                float target = desiredPressures[i];
                float pressureToAdd = MathF.Max(0, target - wheel.CurrentPressure);
                await Task.Delay((int)pressureToAdd * 500);

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

    public bool IsMatch(Vehicle vehicle) =>
        vehicle.TreatmentTypes.Contains(TreatmentType.Inflate);
}
