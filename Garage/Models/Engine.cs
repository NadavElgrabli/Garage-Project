namespace Garage.Models;

public abstract class Engine
{
    public float CurrentEnergy { get; set; }
    public float MaxEnergy { get; set; }

    public abstract void Fill(float amount);
}