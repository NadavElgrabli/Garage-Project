namespace Garage.Data;

public class GarageState
{
    public SemaphoreSlim? WorkersSemaphore { get; set; } 
    public SemaphoreSlim FuelStationsRequestsSemaphore { get; private set; }
    public SemaphoreSlim AirStationsRequestsSemaphore { get; private set; }
    public SemaphoreSlim ChargeStationsRequestsSemaphore { get; private set; }
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