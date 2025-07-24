namespace Garage
{
    public static class ConfigurationKeys
    {
        public static class Validation
        {
            public const string MaxElectricCarEnergy = "Validation:MaxElectricCarEnergy";
            public const string MaxFuelCarEnergy = "Validation:MaxFuelCarEnergy";
            public const string MaxTruckEnergy = "Validation:MaxTruckEnergy";
            public const string MaxFuelMotorcycleEnergy = "Validation:MaxFuelMotorcycleEnergy";
            public const string MaxElectricMotorcycleEnergy = "Validation:MaxElectricMotorcycleEnergy";
            public const string MaxDroneEnergy = "Validation:MaxDroneEnergy";

            public const string MinimumNumberOfTreatments = "Validation:MinimumNumberOfTreatments";
            public const string MaximumNumberOfTreatments = "Validation:MaximumNumberOfTreatments";

            public const string NumberOfCarWheels = "Validation:NumberOfCarWheels";
            public const string NumberOfMotorcycleWheels = "Validation:NumberOfMotorcycleWheels";
            public const string NumberOfTruckWheels = "Validation:NumberOfTruckWheels";

            public const string CarWheelMaxPressure = "Validation:CarWheelMaxPressure";
            public const string MotorcycleWheelMaxPressure = "Validation:MotorcycleWheelMaxPressure";
            public const string TruckWheelMaxPressure = "Validation:TruckWheelMaxPressure";
        }
        
        public static class Refuel
        {
            public const string FuelPricePerLiter = "Refuel:FuelPricePerLiter";
            public const string SpillCleanupCost = "Refuel:SpillCleanupCost";
            public const string MillisecondsPerLiter = "Refuel:MillisecondsPerLiter";
        }
        
        public static class Recharge
        {
            public const string PricePerHour = "Recharge:PricePerHour";
            public const string OverchargePenalty = "Recharge:OverchargePenalty";
            public const string MillisecondsPerHour = "Recharge:MillisecondsPerHour";
        }
        
        public static class Inflate
        {
            public const string DelayPerPressureUnitInMilliseconds = "Inflate:DelayPerPressureUnitInMilliseconds";
            public const string ExplosionPenalty = "Inflate:ExplosionPenalty";
            public const string PricePerPressureUnit = "Inflate:PricePerPressureUnit";
        }
    }
}