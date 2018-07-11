using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using LaPlay.Tools;
using LaPlay.AppData;

using Microsoft.Extensions.Logging;

namespace LaPlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private readonly ILogger<v1Controller> _logger;

        public v1Controller(ILogger<v1Controller> logger, IAppData appData)
        {
            _logger = logger;
        }

        // /api/v1/route/XXXXX
        [HttpGet("route/{id}")]
        public string Get(string id)
        {
            _logger.LogTrace("get get get !!!!!");

            return id;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
