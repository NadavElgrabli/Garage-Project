namespace Garage.Data;

public class GarageState
{
    public SemaphoreSlim WorkersSemaphore { get; private set; } = new (1, 1);
    public SemaphoreSlim FuelStationsRequestsSemaphore { get; private set; } = new (1, 1);
    public SemaphoreSlim AirStationsRequestsSemaphore { get; private set; } = new (1, 1);
    public SemaphoreSlim ChargeStationsRequestsSemaphore { get; private set; } = new (1, 1);
    public bool IsInitialized { get; private set; }


    public void Initialize(int workers, int fuelStations, int airStations, int chargeStations)
    {
        WorkersSemaphore = new SemaphoreSlim(workers, workers);
        FuelStationsRequestsSemaphore = new SemaphoreSlim(fuelStations, fuelStations);
        AirStationsRequestsSemaphore = new SemaphoreSlim(airStations, airStations);
        ChargeStationsRequestsSemaphore = new SemaphoreSlim(chargeStations, chargeStations);
        IsInitialized = true;
    }
}