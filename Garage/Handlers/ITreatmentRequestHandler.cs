using Garage.Data;
using Garage.Models;

namespace Garage.Handlers;

public interface ITreatmentRequestHandler
{
    bool IsMatching(TreatmentRequest request);
    void Handle(TreatmentRequest request, InMemoryDatabase db);
}