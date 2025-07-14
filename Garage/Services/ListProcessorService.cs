using Garage.Data;
using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class ListProcessorService
{
    private readonly IEnumerable<ITreatmentService> _treatmentServices;
    private readonly IListRepository _listRepository;


    public ListProcessorService(
        IEnumerable<ITreatmentService> treatmentServices,
        IListRepository listRepository)
    {
        _treatmentServices = treatmentServices;
        _listRepository = listRepository;
    }

    
    public async Task StartProcessingAsync()
    {
        var processingTasks = _treatmentServices
            .Select(service => Task.Run(() => ProcessTreatmentListAsync(service)))
            .ToList();

        await Task.WhenAll(processingTasks);
    }

    public void AddVehicleRequestsToMatchingList(TreatmentRequest firstRequest, TreatmentRequest secondRequest)
    {
        _listRepository.AddVehicleRequestToMatchingList(firstRequest, secondRequest);
    }
    
    public async Task ProcessTreatmentListAsync(ITreatmentService treatmentService)
    {
        var treatmentType = treatmentService.GetTreatmentType();

        while (true)
        {
            TreatmentRequest? request;

            // Lock only to safely find & remove the first available request
            lock (InMemoryDatabase.TreatmentLocks[treatmentType])
            {
                request = _listRepository.FindFirstAvailableVehicleRequest(treatmentService);

                if (request == null || !_listRepository.RemoveRequest(treatmentService, request))
                {
                    goto WaitAndRetry;
                }
            }

            // Outside the lock and free to treat in parallel
            try
            {
                float price = await treatmentService.TreatAsync(request.Vehicle, request);
                request.Vehicle.TreatmentsPrice += price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Treatment error: {ex.Message}");
            }
            
            // await Task.Delay(100) gives the thread a chance to pause and reduce CPU usage
            WaitAndRetry:
            await Task.Delay(100);
        }
    }
}