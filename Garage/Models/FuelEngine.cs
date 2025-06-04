using Garage.Enums;

namespace Garage.Models;

public class FuelEngine : Engine
{
    public FuelType FuelType {get; set;}

    public override void Fill(float amount)
    {
        if (CurrentEnergy + amount > MaxEnergy)
        {
            throw new InvalidOperationException("Too much fuel");
        }
        CurrentEnergy += amount;
    }
    
}