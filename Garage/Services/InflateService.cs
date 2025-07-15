using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public class InflateService : ITreatmentService
{
    public async Task TreatAsync(Vehicle vehicle, TreatmentRequest request)
    {
        if (request is not AirRequest airRequest)
            throw new ArgumentException("Invalid data type. Expected AirRequest.");

        List<float> desiredPressures = airRequest.DesiredWheelPressures;

        await GarageState.AirStationsRequestsSemaphore.WaitAsync();
        await GarageState.WorkersSemaphore.WaitAsync();

        try
        {
            float totalPrice = 0;
            vehicle.Status = Status.InTreatment;

            for (int i = 0; i < vehicle.Wheels.Count; i++)
            {
                var wheel = vehicle.Wheels[i];
                float target = desiredPressures[i]; // target is 10, current is 2, max is 8

                // Wheel exploded from over pressure, e.g: target is 10, current is 2, max pressure is 8
                if (target > wheel.MaxPressure)
                {
                    await Task.Delay((int)(wheel.MaxPressure - wheel.CurrentPressure) * 500);
                    wheel.CurrentPressure = 0;
                    totalPrice += 350;
                }
                //current pressure is 2, max pressure is 8, target is 8 or lower
                else
                {
                    float pressureToAdd = MathF.Max(0, target - wheel.CurrentPressure);
                    await Task.Delay((int)pressureToAdd * 500);
                    wheel.CurrentPressure += pressureToAdd;
                    totalPrice += pressureToAdd * 0.1f;
                }
            }

            vehicle.TreatmentsPrice = totalPrice;
            vehicle.TreatmentTypes.Remove(TreatmentType.Inflate);
            vehicle.Status = vehicle.TreatmentTypes.Count == 0 ? Status.Ready : Status.Pending;

        }
        finally
        {
            GarageState.WorkersSemaphore.Release();
            GarageState.AirStationsRequestsSemaphore.Release();
        }
    }

    public TreatmentType GetTreatmentType()
    {
        return TreatmentType.Inflate;
    }
}
