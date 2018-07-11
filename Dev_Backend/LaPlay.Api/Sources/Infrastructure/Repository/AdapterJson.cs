using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using LaPlay.Business.Model;

namespace LaPlay.Infrastructure.Repository
{
    public class AdapterJson : IRepositoryContract
    {
        private readonly IConfiguration _Configuration;

        private JObject _JsonData;

        private List<StorageSpace> StorageSpaces;

        public AdapterJson(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _Configuration = configuration;
            
            _JsonData = JObject.Parse(File.ReadAllText(hostingEnvironment.ContentRootPath + "/" + _Configuration["AppDataFile"]));

            StorageSpaces = JsonConvert.DeserializeObject<List<StorageSpace>>(jsonData["StorageSpaces"].ToString());
        }

        private void Save()
        {
            String json = JsonConvert.SerializeObject(StorageSpaces);
            File.WriteAllText(_Configuration["AppDataFile"], json);
        }

        public StorageSpace readStorageSpace(Guid id)
        {
            return (StorageSpace) from a in JsonConvert.DeserializeObject<List<StorageSpace>>(_JsonData["StorageSpaces"].ToString())
                                  where a.Id.Equals(id)
                                  select a;
        }

        public List<StorageSpace> readStorageSpaces()
        {
            return JsonConvert.DeserializeObject<List<StorageSpace>>(_JsonData["StorageSpaces"].ToString());
        }

        public StorageSpace createStorageSpace(StorageSpace storageSpace)
        {
            ((JArray) _JsonData["StorageSpaces"]).Add(JsonConvert.SerializeObject(storageSpace));
            Save();
            return readStorageSpace(storageSpace.Id);
        }
    }
}