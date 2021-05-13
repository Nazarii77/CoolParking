using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        // GET: api/balance
        [HttpGet("balance")]
        public ActionResult<decimal> GetBalance()
        {
            return Ok(Startup.parkingService.GetBalance());
        }

        // GET: api/capacity
        [HttpGet("capacity")]
        public ActionResult<int> GetCapacity()
        {
            return Ok(Startup.parkingService.GetCapacity());
        }

        // GET: api/freePlaces
        [HttpGet("freePlaces")]
        public ActionResult<int> GetFreePlaces()
        {
            return Ok(Startup.parkingService.GetFreePlaces());
        }
    }
}