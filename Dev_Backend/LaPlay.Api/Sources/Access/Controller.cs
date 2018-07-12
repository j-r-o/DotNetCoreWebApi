using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        // /api/v1/CreateStorageSpace
        [HttpPost("CreateStorageSpace")]
        public void CreateStorageSpace([FromBody] StorageSpace storageSpace)
        {
            _StorageSpace.CreateStorageSpace(storageSpace);
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
