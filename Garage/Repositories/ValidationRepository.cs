using Garage.Data;
using Garage.Enums;
using Garage.Models;

namespace Garage.Repositories;

public class ValidationRepository : IValidationRepository
{
    
    public Task CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(request.LicensePlate))
        {
            throw new InvalidOperationException("Car already in garage");
        }

        if (request.Wheels.Count != request.DesiredWheelPressures.Count)
        {
            throw new InvalidOperationException($"Number of wheels: {request.Wheels.Count} does not match the number of desired wheel pressures: {request.DesiredWheelPressures.Count}");
        }

        if (request.TreatmentTypes.Count is > 2 or < 1)
        {
            throw new InvalidOperationException("Number of treatments must be either 1 or 2");
        }
        
        if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
        {
            throw new InvalidOperationException("Engine has too much electricity");
        }
        
        if (request.TreatmentTypes.Contains(TreatmentType.Refuel))
        {
            throw new InvalidOperationException("Cannot have a refuel treatment for an electric car");
        }

        for (int i = 0; i < request.Wheels.Count; i++)
        {
            if (request.Wheels[i].CurrentPressure > request.DesiredWheelPressures[i])
            {
                throw new InvalidOperationException($"Wheel number: {i + 1} current pressure is above the desired pressure");
            }
        }
        
        return Task.CompletedTask;
    }
    
    
    public Task CheckValidFuelCarInput(AddFuelCarRequest request)
    {
        if (InMemoryDatabase.Vehicles.ContainsKey(request.LicensePlate))
        {
            throw new InvalidOperationException("Car already in garage");
        }

        if (request.Wheels.Count != request.DesiredWheelPressures.Count)
        {
            throw new InvalidOperationException($"Number of wheels: {request.Wheels.Count} does not match the number of desired wheel pressures: {request.DesiredWheelPressures.Count}");
        }

        if (request.TreatmentTypes.Count is > 2 or < 1)
        {
            throw new InvalidOperationException("Number of treatments must be either 1 or 2");
        }

        if (request.Engine.CurrentEnergy > request.Engine.MaxEnergy)
        {
            throw new InvalidOperationException("Engine has too much fuel");
        }

        if (request.TreatmentTypes.Contains(TreatmentType.Recharge))
        {
            throw new InvalidOperationException("Cannot have a recharge treatment for a car that runs on fuel");
        }
        
        for (int i = 0; i < request.Wheels.Count; i++)
        {
            if (request.Wheels[i].CurrentPressure > request.DesiredWheelPressures[i])
            {
                throw new InvalidOperationException($"Wheel number: {i + 1} current pressure is above the desired pressure");
            }
        }
        
        return Task.CompletedTask;
    }

}