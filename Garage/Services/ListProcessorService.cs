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
        while (true)
        {
            // Find the first available vehicle in the list
            var request = _listRepository.FindFirstAvailableVehicleRequest(treatmentService);

            if (request == null)
            {
                // No available vehicle found (empty or all in treatment)
                await Task.Delay(100);
                continue;
            }

            var vehicle = request.Vehicle;

            // Try to remove the found request (may have been treated by another task)
            if (_listRepository.RemoveRequest(treatmentService, request))
            {
                try
                {
                    float price = await treatmentService.TreatAsync(vehicle, request);
                    vehicle.TreatmentsPrice += price;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Treatment error: {ex.Message}");
                }
            }

        }
    }
}

    //
    // public async Task ProcessTreatmentListAsync(ITreatmentService treatmentService)
    // {
    //     while (true)
    //     {
    //         if (!_queueRepository.TryGetFirstRequest(treatmentService, out var request))
    //         {
    //             //If the linked list is empty: wait 100ms and try again.
    //             await Task.Delay(100);
    //             continue;
    //         }
    //
    //         var vehicle = request.Vehicle;
    //         
    //         // if its available for treatment
    //         // TODO: Check how to avoid two tasks to enter this "if" at the same time
    //         if (vehicle.Status != Status.InTreatment)
    //         {
    //             // remove from the linked list and begin treatment 
    //             if (_queueRepository.TryRemoveFirstRequest(treatmentService, out var readyRequest))
    //             {
    //                 try
    //                 {
    //                     float price = await treatmentService.TreatAsync(readyRequest.Vehicle, 
    //                         readyRequest);
    //                     readyRequest.Vehicle.TreatmentsPrice += price;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Console.WriteLine($"Error in providing service: {treatmentService.GetTreatmentType()} error: {ex.Message}");
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // can't treat now - first vehicle (currentRequest) is currently in another treatment
    //             if (_queueRepository.TryRemoveFirstRequest(treatmentService, out var currentRequest))
    //             {
    //                 bool requeued = false;
    //                 
    //                 // if there is a next vehicle in the queue that can cut the current vehicle
    //                 if (_queueRepository.TryGetFirstRequest(treatmentService, out var nextRequest) &&
    //                     nextRequest.Vehicle.Status != Status.InTreatment)
    //                 {
    //                     // cut in line TODO: 2 tasks can enter the if at the same time, and both will go through treatment
    //                     if (_queueRepository.TryRemoveFirstRequest(treatmentService, out var cutterRequest))
    //                     {
    //                         try
    //                         {
    //                             float price = await treatmentService.TreatAsync(cutterRequest.Vehicle, cutterRequest);
    //                             cutterRequest.Vehicle.TreatmentsPrice += price;
    //                         }
    //                         catch (Exception ex)
    //                         {
    //                             Console.WriteLine($"Treatment error (cutter): {ex.Message}");
    //                         }
    //                     }
    //                     //TODO: the whole following block part should be in a seperate function called something like: Put back the request in the first place in the que
    //                         
    //                     // Re-queue the original busy vehicle at the front
    //                     var tempQueue = new Queue<TreatmentRequest>();
    //                     tempQueue.Enqueue(currentRequest);
    //
    //                     // Drain the rest of the Requests queue into the temp queue, in order
    //                     var drainedQueue = _queueRepository.DrainTreatmentList(treatmentService);
    //                     foreach (var item in drainedQueue)
    //                         tempQueue.Enqueue(item);
    //
    //                     // Rebuild the Requests queue with the updated order.
    //                     _queueRepository.RebuildTreatmentList(treatmentService, tempQueue);
    //
    //                     requeued = true;
    //                 }
    //                 
    //
    //                 // If there was no cutter, just put the current vehicle back at the end, wait a bit, and retry.
    //                 if (!requeued)
    //                 {
    //                     _queueRepository.EnqueueRequest(treatmentService, currentRequest);
    //                     await Task.Delay(100);
    //                 }
    //             }
    //             
    //         }
    //     }
    // }


