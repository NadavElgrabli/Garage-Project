namespace Garage.Services;

public class GarageOrchestratorService
{
    private readonly GarageManagementService _garageManagementService;
    private readonly ListProcessorService _listProcessorService;
    private readonly ValidationService _validationService;

    public GarageOrchestratorService(GarageManagementService garageManagementService,  ListProcessorService listProcessorService,  ValidationService validationService)
    {
        _garageManagementService = garageManagementService;
        _listProcessorService = listProcessorService;
        _validationService = validationService;
    }
}