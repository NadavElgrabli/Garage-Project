﻿namespace Garage.Models;

public class AddTruckRequest : Vehicle
{
    public Engine Engine { get; set; }
    public List<Wheel> Wheels { get; set; }
    public List<float> DesiredWheelPressures { get; set; }
    public bool CarryDangerousSubstances { get; set; }
    public float CargoVolume { get; set; }
    public float LitersToFuel { get; set; }
}