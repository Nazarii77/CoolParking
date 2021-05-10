// TODO: implement class Parking.
//       Implementation details are up to you, they just have to meet the requirements 
//       of the home task and be consistent with other classes and tests.
using System.Collections.Generic;
namespace CoolParking.BL.Models
{
    public class Parking 
    {
        public decimal CurentBalance { get; set; } = Settings.initialParkingBalance;
        private static Parking parking;
        public IList<TransactionInfo> TransactionInfos { get; set; }
        public decimal Balance { get; set; } = Settings.initialParkingBalance;
        public IList<Vehicle> Vehicles { get; set; }    
        public Dictionary<int, bool> ParkingPlaces { get; set; }

        private Parking() {
            Initialization();
        }

        public static Parking GetParking()
        {
            if (parking == null)
                parking = new Parking();
            return parking;
        }

        private void Initialization()
        {
            ParkingPlaces = new Dictionary<int, bool>();
            for (int i = 0; i < Settings.parkingCapacity; i++)
                ParkingPlaces.Add(i, true);
            Vehicles = new List<Vehicle>();
            TransactionInfos = new List<TransactionInfo>();
        }      
        
        public int GetFreeParkingPlace()
        {
            for (int i = 0; i < ParkingPlaces.Count; i++)
            {
                if (ParkingPlaces[i])
                    return i;
            }
            throw new System.InvalidOperationException("No parking place");
        }
    }
}