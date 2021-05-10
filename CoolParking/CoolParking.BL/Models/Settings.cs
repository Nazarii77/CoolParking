// TODO: implement class Settings.
//       Implementation details are up to you, they just have to meet the requirements of the home task.
using System.Collections.Generic;

namespace CoolParking.BL.Models
{
    public static class Settings
    {
        public static decimal initialParkingBalance { get; } = 0;
        public static int parkingCapacity { get; } = 10;
        public static int withdrawIntervalInMillis { get; } = 5000;
        public static float logIntervalInMillis { get; } = 12000f;
        public static decimal penaltyMultiplier { get; } = 2.5m;
        static public string LogFilePath { get; set; } = @"C:\Logs\log.txt";

        public static Dictionary<VehicleType, decimal> vehicleParkingTariff = new Dictionary<VehicleType, decimal>()
        {
            {VehicleType.PassengerCar, 2m},
            {VehicleType.Truck, 5m},
            {VehicleType.Bus, 3.5m},
            {VehicleType.Motorcycle, 1m}
        }; 
    }
}