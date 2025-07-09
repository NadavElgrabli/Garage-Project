using Garage.Enums;
using Garage.Models;

namespace Garage.Services;

public interface ITreatmentService
{
    Task<float> TreatAsync(Vehicle vehicle, object data);
    bool IsMatch(Vehicle vehicle);
}
