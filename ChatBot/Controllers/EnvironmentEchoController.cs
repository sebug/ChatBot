using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    [Route("api/[controller]")]
    public class EnvironmentEchoController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "Return Bot App ID and secret from environment variables here.";
        }
    }
}
