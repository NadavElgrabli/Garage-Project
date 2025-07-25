﻿using Garage.Models;
using Garage.Repositories;

namespace Garage.Services;

public class ValidationService
{
    private readonly IValidationRepository _validationRepository;

    public ValidationService(IValidationRepository validationRepository)
    {
        _validationRepository = validationRepository;
    }
    
    public void CheckValidElectricCarInput(AddElectricCarRequest request)
    {
        _validationRepository.CheckValidElectricCarInput(request);
    }
    
    public void CheckValidFuelCarInput(AddFuelCarRequest request)
    { 
        _validationRepository.CheckValidFuelCarInput(request);
    }

    public void CheckValidTruckInput(AddTruckRequest request)
    {
        _validationRepository.CheckValidTruckInput(request);
    }

    public void CheckValidDroneInput(AddDroneRequest request)
    {
        _validationRepository.CheckValidDroneInput(request);
    }

    public void CheckValidFuelMotorcycleInput(AddFuelMotorcycleRequest request)
    {
        _validationRepository.CheckValidFuelMotorcycleInput(request);
    }

    public void CheckValidElectricMotorcycleInput(AddElectricMotorcycleRequest request)
    {
        _validationRepository.CheckValidElectricMotorcycleInput(request);
    }
}