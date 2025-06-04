namespace Garage.Data;

public class GarageState
{
    public static SemaphoreSlim WorkersSemaphore;
    public static SemaphoreSlim FuelStationsRequestsSemaphore;
    public static SemaphoreSlim AirStationsRequestsSemaphore;
    public static SemaphoreSlim ChargeStationsRequestsSemaphore;

    public static void Initialize(int workers, int fuelStations, int airStations, int chargeStations)
    {
        WorkersSemaphore = new SemaphoreSlim(workers, workers);
        FuelStationsRequestsSemaphore = new SemaphoreSlim(fuelStations, fuelStations);
        AirStationsRequestsSemaphore = new SemaphoreSlim(airStations, airStations);
        ChargeStationsRequestsSemaphore = new SemaphoreSlim(chargeStations, chargeStations);
    }
}