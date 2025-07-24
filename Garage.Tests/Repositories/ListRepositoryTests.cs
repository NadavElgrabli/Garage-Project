using Garage.Data;
using Garage.Enums;
using Garage.Models;
using Garage.Repositories;
using Garage.Services;
using Moq;

namespace Garage.Tests.Repositories;

using Xunit;

public class ListRepositoryTests
{
    [Fact]
    public void FindFirstAvailableVehicleRequest_ShouldReturnThirdRequest_WhenFirstTwoAreInTreatment()
    {
        // Arrange
        var treatmentService = new Mock<ITreatmentService>();
        treatmentService.Setup(s => s.GetTreatmentType()).Returns(TreatmentType.Recharge);

        var vehicle1 = new Car { Status = Status.InTreatment };
        var vehicle2 = new Car { Status = Status.InTreatment };
        var vehicle3 = new Car { Status = Status.Pending };
        
        var request1 = new ChargeRequest { Vehicle = vehicle1 };
        var request2 = new ChargeRequest { Vehicle = vehicle2 };
        var request3 = new ChargeRequest { Vehicle = vehicle3 };

        var list = new LinkedList<TreatmentRequest>();
        list.AddLast(request1); // Should be skipped
        list.AddLast(request2); // Should be skipped
        list.AddLast(request3);

        var db = new InMemoryDatabase();
        db.TreatmentLists[TreatmentType.Recharge] = list;
        
        var repo = new ListRepository(db);

        // Act
        var result = repo.FindFirstAvailableVehicleRequest(treatmentService.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(vehicle1, result!.Vehicle);
        Assert.NotEqual(vehicle2, result.Vehicle);
        Assert.Equal(vehicle3, result.Vehicle);
    }
}