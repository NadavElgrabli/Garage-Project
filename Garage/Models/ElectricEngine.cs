namespace Garage.Models;

public class ElectricEngine : Engine
{
    public override void Fill(float amount)
    {
        if (CurrentEnergy + amount > MaxEnergy)
        {
            throw new InvalidOperationException("Too much charge");
        }
        CurrentEnergy += amount;
    }
}