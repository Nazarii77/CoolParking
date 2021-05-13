using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using CoolParking.BL.Models;
using CoolParking.BL.Services;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Razor.Language;
using System.IO;

namespace CoolParking.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        // GET: api/Transactions/last
        [HttpGet("last")]
        public ActionResult GetLastTransaction()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            TransactionInfo[] lastTransactions = Startup.parkingService.GetLastParkingTransactions();
            TransactionsBody[] lastTransactionsBody = new TransactionsBody[lastTransactions.Length];
            int i = 0;
            foreach (TransactionInfo item in lastTransactions)
            {
                TransactionsBody tb = new TransactionsBody();
                tb.VehicleId = item.VehicleId;
                tb.Sum = item.Sum;
                tb.TransactionDate = item.TimeTransaction;
                lastTransactionsBody[i] = tb;
                i++;
            }
            return Ok(lastTransactionsBody);
        }

        // GET: api/Transactions/all
        [HttpGet("all")]
        public ActionResult<string> GetAllTransaction()
        {
            try
            {
                return Ok(Startup.parkingService.ReadFromLog());
            }
            catch (Exception) // We catch all errors so that the server does not fall
            {
                return NotFound();
            }
        }

        // PUT: api/Transactions/topUpVehicle
        [HttpPut("topUpVehicle")]
        public ActionResult PutTopUpVehicle([FromBody] TransactionsTopUpVehicleBody transactionsTopUpVehicleBody)
        {
            if (transactionsTopUpVehicleBody == null)
            {
                return BadRequest();
            }
            try
            {
                Vehicle vehicle = Startup.parkingService.GetVehicle(transactionsTopUpVehicleBody.Id);
                Startup.parkingService.TopUpVehicle(transactionsTopUpVehicleBody.Id, transactionsTopUpVehicleBody.Sum);
                vehicle = Startup.parkingService.GetVehicle(transactionsTopUpVehicleBody.Id);
                return Ok(JsonConvert.SerializeObject(vehicle));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception) //InvalidDataException and other. We catch all errors so that the server does not fall
            {
                return BadRequest();
            }
        }

    }

    public class TransactionsBody
    {
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("sum")]
        public decimal Sum { get; set; }
        [JsonProperty("transactionDate")]
        public DateTime TransactionDate { get; set; }
    }
    public class TransactionsTopUpVehicleBody
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sum")]
        public decimal Sum { get; set; }
    }

}