using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Repositories;
using Garage.Services;
using Moq;

namespace Garage.Tests.Services;

public class ListProcessorServiceTests
{
    [Fact]
    public async Task ProcessTreatmentListAsync_ShouldTreatSingleRequest_WhenRequestExists()
    {
        //Arrange
        var mockTreatmentService = new Mock<ITreatmentService>();
        var mockListRepository = new Mock<IListRepository>();

        var vehicle = new Car(); 
        var request = new ChargeRequest { Vehicle = vehicle };

        // Set up treatment type (needed for lock dictionary)
        mockTreatmentService.Setup(x => x.GetTreatmentType()).Returns(TreatmentType.Recharge);

        // Initialize the lock for this treatment type
        InMemoryDatabase.TreatmentLocks[TreatmentType.Recharge] = new object();

        // First call: return a request, Second call: return null to break the loop
        mockListRepository
            .SetupSequence(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object))
            .Returns(request)
            .Returns((TreatmentRequest)null);

        // Always successfully remove the request
        mockListRepository
            .Setup(r => r.RemoveRequest(mockTreatmentService.Object, request))
            .Returns(true);

        // Simulate TreatAsync working normally
        mockTreatmentService
            .Setup(t => t.TreatAsync(vehicle, request))
            .Returns(Task.CompletedTask);

        // Create the service under test
        var service = new ListProcessorService(new[] { mockTreatmentService.Object }, mockListRepository.Object);

        // Act
        // We'll run the method for a short time and let it process once
        var processTask = Task.Run(async () =>
        {
            await service.ProcessTreatmentListAsync(mockTreatmentService.Object);
        });

        await Task.Delay(200); // Let the loop run at least once

        // Assert
        mockListRepository.Verify(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object), Times.Exactly(2));
        mockListRepository.Verify(r => r.RemoveRequest(mockTreatmentService.Object, request), Times.Once);
        mockTreatmentService.Verify(t => t.TreatAsync(vehicle, request), Times.Once);
    }

    [Fact]
    public async Task ProcessTreatmentListAsync_ShouldTreatMultipleRequests_WhenRequestsExists()
    {
        // Arrange
        var mockTreatmentService = new Mock<ITreatmentService>();
        var mockListRepository = new Mock<IListRepository>();

        var vehicle1 = new Car();
        var vehicle2 = new Car();
        var vehicle3 = new Car();
        var request1 = new ChargeRequest { Vehicle = vehicle1 };
        var request2 = new ChargeRequest { Vehicle = vehicle2 };
        var request3 = new ChargeRequest { Vehicle = vehicle3 };

        mockTreatmentService.Setup(x => x.GetTreatmentType()).Returns(TreatmentType.Recharge);
        InMemoryDatabase.TreatmentLocks[TreatmentType.Recharge] = new object();

        mockListRepository
            .SetupSequence(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object))
            .Returns(request1)
            .Returns(request2)
            .Returns(request3)
            .Returns((TreatmentRequest)null); 

        mockListRepository
            .Setup(r => r.RemoveRequest(mockTreatmentService.Object, It.IsAny<TreatmentRequest>()))
            .Returns(true);

        mockTreatmentService
            .Setup(t => t.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()))
            .Returns(Task.CompletedTask);

        var service = new ListProcessorService(new[] { mockTreatmentService.Object }, mockListRepository.Object);

        // Act
        var task = Task.Run(async () => await service.ProcessTreatmentListAsync(mockTreatmentService.Object));
        await Task.Delay(400);

        // Assert
        mockListRepository.Verify(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object), Times.Exactly(4));
        mockListRepository.Verify(r => r.RemoveRequest(mockTreatmentService.Object, It.IsAny<TreatmentRequest>()), Times.Exactly(3));
        mockTreatmentService.Verify(t => t.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()), Times.Exactly(3));
    }


    [Fact]
    public async Task ProcessTreatmentListAsync_ShouldNotTreatSingleRequest_WhenRequestDoesNotExist()
    {
        // Arrange
        var mockTreatmentService = new Mock<ITreatmentService>();
        var mockListRepository = new Mock<IListRepository>();

        mockTreatmentService.Setup(x => x.GetTreatmentType()).Returns(TreatmentType.Recharge);
        InMemoryDatabase.TreatmentLocks[TreatmentType.Recharge] = new object();

        mockListRepository
            .Setup(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object))
            .Returns((TreatmentRequest)null);

        var service = new ListProcessorService(new[] { mockTreatmentService.Object }, mockListRepository.Object);

        // Act
        var task = Task.Run(async () => await service.ProcessTreatmentListAsync(mockTreatmentService.Object));
        await Task.Delay(300);

        // Assert
        mockListRepository.Verify(r => r.FindFirstAvailableVehicleRequest(mockTreatmentService.Object), Times.AtLeastOnce);
        mockListRepository.Verify(r => r.RemoveRequest(It.IsAny<ITreatmentService>(), It.IsAny<TreatmentRequest>()), Times.Never);
        mockTreatmentService.Verify(t => t.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()), Times.Never);
    }
}


    // private readonly Mock<IListRepository> _mockListRepository;
    // private readonly Mock<ITreatmentService> _mockTreatmentService1;
    // private readonly Mock<ITreatmentService> _mockTreatmentService2;
    // private readonly ListProcessorService _listProcessorService;
    //
    // public ListProcessorServiceTests()
    // {
    //     _mockListRepository = new Mock<IListRepository>();
    //     _mockTreatmentService1 = new Mock<ITreatmentService>();
    //     _mockTreatmentService2 = new Mock<ITreatmentService>();
    //
    //     var services = new List<ITreatmentService>
    //     {
    //         _mockTreatmentService1.Object,
    //         _mockTreatmentService2.Object
    //     };
    //
    //     _listProcessorService = new ListProcessorService(services, _mockListRepository.Object);
    // }
    //
    // [Fact]
    // public async Task StartProcessingAsync_ShouldStartProcessingForEachTreatmentService()
    // {
    //     // Arrange
    //     var tokenSource = new CancellationTokenSource();
    //     var token = tokenSource.Token;
    //
    //     // Force ProcessTreatmentListAsync to stop early after one loop
    //     var request = new Mock<TreatmentRequest>().Object;
    //     var vehicle = new Mock<Vehicle>().Object;
    //
    //     _mockTreatmentService1.Setup(x => x.GetTreatmentType()).Returns(TreatmentType.Recharge);
    //     _mockTreatmentService2.Setup(x => x.GetTreatmentType()).Returns(TreatmentType.Inflate);
    //
    //     InMemoryDatabase.TreatmentLocks[TreatmentType.Recharge] = new object();
    //     InMemoryDatabase.TreatmentLocks[TreatmentType.Inflate] = new object();
    //
    //     _mockListRepository
    //         .SetupSequence(r => r.FindFirstAvailableVehicleRequest(It.IsAny<ITreatmentService>()))
    //         .Returns(new TreatmentRequest { Vehicle = vehicle })
    //         .Returns((TreatmentRequest)null);
    //
    //     _mockListRepository
    //         .Setup(r => r.RemoveRequest(It.IsAny<ITreatmentService>(), It.IsAny<TreatmentRequest>()))
    //         .Returns(true);
    //
    //     _mockTreatmentService1
    //         .Setup(t => t.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()))
    //         .Returns(Task.CompletedTask);
    //
    //     _mockTreatmentService2
    //         .Setup(t => t.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()))
    //         .Returns(Task.CompletedTask);
    //
    //     // Act
    //     // Let it run briefly and cancel after 300ms
    //     var processingTask = _listProcessorService.StartProcessingAsync();
    //     await Task.Delay(300);
    //     tokenSource.Cancel();
    //
    //     // Assert
    //     _mockTreatmentService1.Verify(x => x.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()), Times.AtLeastOnce);
    //     _mockTreatmentService2.Verify(x => x.TreatAsync(It.IsAny<Vehicle>(), It.IsAny<TreatmentRequest>()), Times.AtLeastOnce);
    // }