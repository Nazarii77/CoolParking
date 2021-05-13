using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoolParking.BL.Models;
using CoolParking.BL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        // GET: api/Vehicles
        [HttpGet]
        public ActionResult<VehicleBody> Get()
        {
            return Ok(Startup.parkingService.GetVehicles());
        }

        // POST: api/Vehicles
        [HttpPost]
        public ActionResult<VehicleBody> Post([FromBody] VehicleBody vehicleBody)//[FromBody] string json)
        {

            if (vehicleBody == null)
            {
                return BadRequest();
            }
            try
            {
                Vehicle vehicle = new Vehicle(vehicleBody.Id, (VehicleType)vehicleBody.VehicleType, vehicleBody.Balance);

                Startup.parkingService.AddVehicle(vehicle);
                return CreatedAtAction("Post", vehicleBody);
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        //GET api/Vehicles/id
        [HttpGet("{id}")]
        public ActionResult GetId([FromRoute] string id)
        {
            try
            {
                Vehicle vehicle = Startup.parkingService.GetVehicle(id);
                VehicleBody vb = new VehicleBody
                {
                    Id = vehicle.Id,
                    VehicleType = (int)vehicle.VehicleType,
                    Balance = vehicle.Balance
                };
                return Ok(vb);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception)//InvalidDataException InvalidOperationException. We catch all errors so that the server does not fall
            {
                return BadRequest();
            }
        }

        // DELETE api/vehicles/id
        [HttpDelete("{id}")]
        public ActionResult Dell([FromRoute] string id)
        {

            try
            {
                Startup.parkingService.RemoveVehicle(id);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception)//InvalidDataException, InvalidOperationException and others. We catch all errors so that the server does not fall
            {
                return BadRequest();
            }
            ////
            /*try
            {
                Startup.parkingService.RemoveVehicle(id);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }*/
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
}
