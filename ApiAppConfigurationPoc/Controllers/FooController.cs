using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiAppConfigurationPoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public FooController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public string Bar([FromQuery]string key)
        {
            return configuration[key ?? "Huhu"];
        }
    }
}
