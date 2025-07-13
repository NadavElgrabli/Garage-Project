using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public interface ITreatmentService
{
    Task<float> TreatAsync(Vehicle vehicle, TreatmentRequest request);
    bool IsMatch(Vehicle vehicle);
    TreatmentType GetTreatmentType();
}
