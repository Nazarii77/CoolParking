// TODO: implement the ParkingService class from the IParkingService interface.
//       For try to add a vehicle on full parking InvalidOperationException should be thrown.
//       For try to remove vehicle with a negative balance (debt) InvalidOperationException should be thrown.
//       Other validation rules and constructor format went from tests.
//       Other implementation details are up to you, they just have to match the interface requirements
//       and tests, for example, in ParkingServiceTests you can find the necessary constructor format and validation rules.
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CoolParking.BL.Services
{
    public class ParkingService : IParkingService
    {
        private Parking Park { get; set; } = Parking.GetParking();
        public ILogService LogFileService { get; set; }
        ITimerService WithdrawTimer { get; set; }
        public ParkingService(bool IsStartTimer) {
            if (IsStartTimer)
            {
                WithdrawTimer = new TimeService();
                WithdrawTimer.Start();
            }
            LogFileService = new LogService(Settings.LogFilePath);
        }

        public ParkingService(ITimerService withdrawTimer, ITimerService logTimer, ILogService logService)
        {
            LogFileService = logService;
            WithdrawTimer = withdrawTimer;
        }        
        public void AddVehicle(Vehicle vehicle)
        {

            if (Park.Vehicles.Count > Settings.parkingCapacity)
                throw new System.InvalidOperationException("Parking is full");
            if (IsNotEqualId(vehicle.Id))
            {
                Park.Vehicles.Add(vehicle);
                Park.ParkingPlaces[Park.GetFreeParkingPlace()] = false;
                vehicle.ParkingPlaceNumber = Park.GetFreeParkingPlace();
                return;
            }
            throw new System.ArgumentException("argument exception!");

        }
        private bool IsNotEqualId(string vehicleId) => !Park.Vehicles.Any(Vehicle => Vehicle.Id == vehicleId);
        public void Dispose()
        {
            Park.Balance = Settings.initialParkingBalance;
            Park.ParkingPlaces.Clear();
            for (int i = 0; i < Settings.parkingCapacity; i++)
                Park.ParkingPlaces.Add(i, true);
            Park.Vehicles.Clear();
            WithdrawTimer.Dispose();
            if (System.IO.File.Exists(Settings.LogFilePath))
                System.IO.File.Delete(Settings.LogFilePath);
            System.GC.Collect();
        }

        public decimal GetBalance()
        {
            return Park.Balance;
        }

        public int GetCapacity()
        {
            return Settings.parkingCapacity;
        } 

        public int GetFreePlaces()
        {
            return GetCapacity() - Park.Vehicles.Count;
        }

        public TransactionInfo[] GetLastParkingTransactions()
        {
            
            return Park.TransactionInfos.ToArray();
        }

        public ReadOnlyCollection<Vehicle> GetVehicles()
        {
            ReadOnlyCollection<Vehicle> collection = new ReadOnlyCollection<Vehicle>(Park.Vehicles);
            return collection;
        }

        public string ReadFromLog()
        {
            LogService log = new(Settings.LogFilePath);
            return log.Read();
        }

        public void RemoveVehicle(string vehicleId)
        {
            if (!IsNotEqualId(vehicleId))
            {
                if (Vehicle.GetVehicleById(vehicleId).Balance < 0)
                    throw new System.InvalidOperationException("Remove transport with negative balance not possible");
                Park.ParkingPlaces[Vehicle.GetVehicleById(vehicleId).ParkingPlaceNumber] = true;
                Park.Vehicles.Remove(Vehicle.GetVehicleById(vehicleId));
                return;
            }
            throw new System.ArgumentException("Vehicle ID does not exist");
        }

        public void TopUpVehicle(string vehicleId, decimal sum)
        {
            if (sum <= 0)
                throw new System.ArgumentException("The sum <= 0");
            Vehicle vehicle = Vehicle.GetVehicleById(vehicleId);
            if (vehicle != null)
            {
                vehicle.Balance += sum;
                System.Console.WriteLine("This transport has " + vehicle.Balance);
                return;
            }
            throw new System.ArgumentException("Vehicle ID does not exist");
        }
    }
}