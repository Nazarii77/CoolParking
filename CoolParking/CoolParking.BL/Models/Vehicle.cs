// TODO: implement class Vehicle.
//       Properties: Id (string), VehicleType (VehicleType), Balance (decimal).
//       The format of the identifier is explained in the description of the home task.
//       Id and VehicleType should not be able for changing.
//       The Balance should be able to change only in the CoolParking.BL project.
//       The type of constructor is shown in the tests and the constructor should have a validation, which also is clear from the tests.
//       Static method GenerateRandomRegistrationPlateNumber should return a randomly generated unique identifier.

using System;
using System.Text.RegularExpressions;

namespace CoolParking.BL.Models
{
    public class Vehicle
    {
        public int ParkingPlaceNumber { get; set; }
        public string Id { get; private set; }
        public VehicleType VehicleType { get; private set; }
        public decimal Balance { get; set; }
        public Vehicle(string id, VehicleType vehicle, decimal money)
        {
            Regex regex = new Regex(@"[A-Z]{2}-\d{4}-[A-Z]{2}");
            if (!(regex.IsMatch(id)) || money < 0)
                throw new ArgumentException("Master data is not correct");
            Id = id;
            VehicleType = vehicle;
            Balance = money;
        }
        public Vehicle(VehicleType typeOfVehicle, decimal balance)
        {
            Id = GenerateRandomRegistrationPlateNumber();
            VehicleType = typeOfVehicle;
            Balance = balance;
        }
        public static string GenerateRandomRegistrationPlateNumber()
        {
            var numbers = GenerateRandomNumbersSet();
            var charactersBegin = GenerateRandomCharactersSet();
            var charactersEnd = GenerateRandomCharactersSet();

            return String.Format("{0}-{1}-{2}", charactersBegin, numbers, charactersEnd);
        }

        public static string GenerateRandomCharactersSet()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var randomCharacter = new Random();
            var stringChars = new char[2];

            for (int i = 0; i < 2; i++)
            {
                stringChars[i] = chars[randomCharacter.Next(chars.Length)];
            }

            string result = new string(stringChars);
            return result;
        }

        private static string GenerateRandomNumbersSet()
        {
            Random r = new Random();
            int rInt = r.Next(1000, 9999); //for ints
            return rInt.ToString();
        }
        public static Vehicle GetVehicleById(string Id)
        {
            Parking parking = Parking.GetParking();
            for (int i = 0; i < parking.Vehicles.Count; i++)
            {
                if (parking.Vehicles[i].Id == Id)
                    return parking.Vehicles[i];
            }
            return null;
        }


        public void Withdraw(decimal sum)
        {
            if (sum > 0) Balance -= sum;
            else throw new ArgumentException();
        }


    }
}