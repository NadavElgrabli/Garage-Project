using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public interface ITreatmentService
{
    Task TreatAsync(Vehicle vehicle, TreatmentRequest request);
    TreatmentType GetTreatmentType();
}
