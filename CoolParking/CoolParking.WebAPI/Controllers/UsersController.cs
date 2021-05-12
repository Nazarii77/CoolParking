using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Lecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET api/users
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {
        "user1",
        "user2"
      };
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"user{id}";
        }
    }
}