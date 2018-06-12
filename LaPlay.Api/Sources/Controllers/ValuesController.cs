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
    public class ValuesController : ControllerBase
    {
        // GET api/values
        // [HttpGet]
        // public ActionResult<IEnumerable<string>> Get()
        // {
        //     return new string[] { "value1", "value2" };
        // }

        private Tools.Tools _tools = new Tools.Tools();

        // GET api/values
        [HttpGet("OK")]
        public ActionResult<string> Get()
        {
            // LaPlay.Api.Sources.Tools.Tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\"");
            Console.WriteLine(_tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\""));

            return "OK OK";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
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
