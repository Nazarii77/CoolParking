using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoolParking.WebAPI.ClientHttp
{

    class UserHttpClient
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main()
        {
            bool isExit = false;
            string id;
            int vehicleType;
            int balance;
            int sum;
            VehicleBody vb;
            while (!isExit)
            {
                char command = MainDisplay();
                Console.ReadKey();
                Console.Clear();
                switch (command)
                {
                    case '0': //capacity
                        Console.WriteLine("Capacity: " + await GetCapacity());
                        break;


                    case '1': //balance
                        Console.WriteLine("Balance: " + await GetBalance());
                        break;


                    case '2': //freeplaces
                        Console.WriteLine("Free places: " + await GetFreePlace());
                        break;


                    case '3': //last trans
                        Console.WriteLine("Last transactions: \n" + await GetLastTransactions());
                        break;


                    case '4': //all trans
                        Console.WriteLine("All transactions: \n" + await GetAllTransactions());
                        break;


                    case '5':// get vehicles
                        List<VehicleBody> list = await GetVehicles();
                        list.ForEach(vb =>
                        {
                            ShowVehicle(vb);
                            Console.WriteLine("------------");
                        });
                        break;


                    case '6': // get vehicles by id
                        Console.WriteLine("Enter your vehicle number");
                        id = Console.ReadLine();
                        ShowVehicle(await GetVehicle(id));
                        break;


                    case '7': //post vehicles
                        Console.WriteLine("Enter your vehicle number");
                        id = Console.ReadLine();
                        Console.WriteLine("Choose type of your vehicle");
                        Console.WriteLine("1 - PassengerCar, 2 - Truck, 3 - Bus, 4 - Motorcycle");
                        vehicleType = Convert.ToInt32(Console.ReadLine()) - 1;
                        Console.Write("Enter your balance - ");
                        balance = Convert.ToInt32(Console.ReadLine());
                        vb = new VehicleBody { Id = id, VehicleType = vehicleType, Balance = balance };
                        Console.WriteLine(await PostVehicles(vb));
                        break;


                    case '8': //delete vehicles
                        Console.WriteLine("Enter your vehicle number");
                        id = Console.ReadLine();
                        Console.WriteLine(await DeleteVehicle(id));
                        break;


                    case '9': //topupvehicles
                        Console.WriteLine("Enter your vehicle number");
                        id = Console.ReadLine();
                        Console.WriteLine("Enter top up amount");
                        sum = Convert.ToInt32(Console.ReadLine());
                        TransactionsTopUpVehicleBody tr = new TransactionsTopUpVehicleBody { Id = id, Sum = sum };
                        var vehicle = await PutTopUpVehicle(tr);
                        if (vehicle == null)
                        {
                            Console.WriteLine("Uncorrect data");
                        }
                        else
                        {
                            ShowVehicle(vehicle);
                        }
                        break;


                    case 'Q': //exit
                        isExit = true;
                        break;


                    default:
                        Console.WriteLine("Not found this command :(");
                        break;
                }
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }

        static void ShowVehicle(VehicleBody vb)
        {
            Console.WriteLine("Vehicle id: " + vb.Id);
            Console.WriteLine("Vehicle type: " + vb.VehicleType);
            Console.WriteLine("Vehicle balance: " + vb.Balance);
        }

        static char MainDisplay()
        {
            Console.Clear();
            char isChoised = ' ';
            Console.WriteLine("0. Current parking capacity");//capacity
            Console.WriteLine("1. Amount of money earned for the current period.");//balance
            Console.WriteLine("2. Amount free parking places.");//freeplace

            Console.WriteLine("3. Last Parking Transactions for the current period.");//last transactions
            Console.WriteLine("4. Transaction history.");//all transaction

            Console.WriteLine("5. List of Vehicles located in the Parking.");//get vehicles
            Console.WriteLine("6. Get Vehicle by id"); // get vehicle by id
            Console.WriteLine("7. Add Vehicles in the Parking");//post vehicle
            Console.WriteLine("8. Remove the Vehicle from Parking.");// delete vehicle

            Console.WriteLine("9. Top up the balance of Vehicle");//topupvehicle
            Console.WriteLine("For exit enter Q");
            isChoised = Console.ReadKey().KeyChar;

            return isChoised;
        }

        static async Task<string> GetBalance()
        {
            return await GetRequest("https://localhost:44326/api/parking/balance");
        }

        static async Task<string> GetCapacity()
        {
            return await GetRequest("https://localhost:44326/api/parking/capacity");
        }

        static async Task<string> GetFreePlace()
        {
            return await GetRequest("https://localhost:44326/api/parking/freePlaces");
        }

        static async Task<string> GetRequest(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string req = await response.Content.ReadAsStringAsync();
            return req;
        }

        static async Task<HttpStatusCode> PostVehicles(VehicleBody vb)
        {
            var url = "https://localhost:44326/api/vehicles";
            HttpContent req = new StringContent(JsonConvert.SerializeObject(vb), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(url, req);
            return result.StatusCode;
        }

        static async Task<VehicleBody> GetVehicle(string id)
        {
            var url = "https://localhost:44326/api/vehicles/" + id;
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            // string  = await response.Content.ReadAsStringAsync();
            var vehicle = JsonConvert.DeserializeObject<VehicleBody>(await response.Content.ReadAsStringAsync());
            return vehicle;
        }

        static async Task<List<VehicleBody>> GetVehicles()
        {
            var url = "https://localhost:44326/api/vehicles";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            // string  = await response.Content.ReadAsStringAsync();
            var vehicle = JsonConvert.DeserializeObject<List<VehicleBody>>(await response.Content.ReadAsStringAsync());
            return vehicle;
        }

        static async Task<HttpStatusCode> DeleteVehicle(string id)
        {
            var url = "https://localhost:44326/api/vehicles/" + id;
            HttpResponseMessage response = await client.DeleteAsync(url);
            return response.StatusCode;
        }

        static async Task<string> GetLastTransactions()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:44326/api/transactions/last");
            response.EnsureSuccessStatusCode();
            string lastTransactions = await response.Content.ReadAsStringAsync();
            return lastTransactions;
        }

        static async Task<string> GetAllTransactions()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:44326/api/transactions/all");
            response.EnsureSuccessStatusCode();
            string allTransactions = await response.Content.ReadAsStringAsync();
            return allTransactions;
        }

        static async Task<VehicleBody> PutTopUpVehicle(TransactionsTopUpVehicleBody tr)
        {
            var url = "https://localhost:44326/api/transactions/topUpVehicle";
            HttpContent req = new StringContent(JsonConvert.SerializeObject(tr), Encoding.UTF8, "application/json");
            var result = await client.PutAsync(url, req);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var vehicle = JsonConvert.DeserializeObject<VehicleBody>(await result.Content.ReadAsStringAsync());
                return vehicle;
            }
            else
            {
                return null;
            }

        }
    }
    public class TransactionsTopUpVehicleBody
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sum")]
        public decimal Sum { get; set; }
    }

    public class VehicleBody
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("vehicleType")]
        public int VehicleType { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

    }
}
