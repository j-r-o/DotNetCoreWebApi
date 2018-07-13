using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

using LaPlay.Business;
using LaPlay.Business.Model;

using Microsoft.Extensions.Logging;

namespace LaPlay.Access
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private readonly ILogger<v1Controller> _Logger;
        private readonly IStorageSpaceContract _StorageSpace;

        public v1Controller(ILogger<v1Controller> logger, IStorageSpaceContract storageSpace)
        {
            _Logger = logger;
            _StorageSpace = storageSpace;
        }

        /* TESTED OK */

        // /api/v1/fixedString
        [HttpGet("fixedString")]
        public string a()
        {
            return "fixedString";
        }

        // /api/v1/queryParams?p1=v1&p2=v2
        [HttpGet("queryParams")]
        public string queryParams([FromQuery]string p1, [FromQuery]string p2)
        {
            return "# " + p1 + " # " + p2 + " # ";
        }

        // /api/v1/urlParam/value
        [HttpGet("urlParam/{param}")]
        public string urlParam(String param){
            return param + " # ";
        }

        // curl https://localhost:5001/api/v1/4 -k
        // GET api/v1/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a = new {f1 = id};
            return StatusCode(StatusCodes.Status200OK, a);
        }

        // curl -X POST -H "Content-Type: application/json" -d '{"Id":"c7a368d6-12f3-4957-a0c1-81b644ac5e3d", "Name": "Name1", "MainFolder": "MF1", "MirrorFolder": "MF1"}' https://localhost:5001/api/v1/postSimple -k
        [HttpPost("postSimple")]
        public IActionResult PostSimple([FromBody] StorageSpace storageSpace)
        {
           _StorageSpace.CreateStorageSpace(storageSpace);

            return StatusCode(StatusCodes.Status200OK);
        }

        /* KO */


        /* NOT TESTED */
        
        // /api/v1/CreateStorageSpace
        [HttpGet("CreateStorageSpace")]
        public void CreateStorageSpace([FromBody] StorageSpace storageSpace)
        {
            _StorageSpace.CreateStorageSpace(storageSpace);
        }

        // /api/values
        [HttpGet("a")]
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
