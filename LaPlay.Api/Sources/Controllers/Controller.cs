using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using LaPlay.Api.Sources.Tools;

namespace LaPlay.Api.Sources.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private Tools.Tools _tools = new Tools.Tools();

        // api/v1/route/XXXXX
        [HttpGet("route/{id}")]
        public string Get(string id)
        {
            return id;
        }

        // api/v1/test
        [HttpGet("test")]
        public ActionResult<string> ok()
        {
            // LaPlay.Api.Sources.Tools.Tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\"");
            Console.WriteLine(_tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\""));

            return (_tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\""));

            return "OK OK";
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
