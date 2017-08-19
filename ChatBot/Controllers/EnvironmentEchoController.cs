using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatBot.Controllers
{
    [Route("api/[controller]")]
    public class EnvironmentEchoController : Controller
    {
        private readonly ChatBotOptions _chatBotOptions;

        public EnvironmentEchoController(IOptions<ChatBotOptions> chatBotOptions)
        {
            this._chatBotOptions = chatBotOptions.Value;
        }

        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "Return Bot App ID " + 
                this._chatBotOptions.AppID + 
                    " and secret " + this._chatBotOptions.Secret +
                    " from environment variables here.";
        }
    }
}
