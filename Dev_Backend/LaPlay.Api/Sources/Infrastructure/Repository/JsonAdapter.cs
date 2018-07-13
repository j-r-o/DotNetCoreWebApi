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
    public class JsonAdapter : IRepositoryContract
    {
        private readonly IConfiguration _Configuration;
        private readonly IHostingEnvironment _HostingEnvironment;

        private JObject _JsonData;

        private void Save()
        {
            File.WriteAllText(_HostingEnvironment.ContentRootPath + "/" + _Configuration["AppDataFile"], _JsonData.ToString());
        }

        public JsonAdapter(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _Configuration = configuration;
            _HostingEnvironment = hostingEnvironment;
            
            String Path = _HostingEnvironment.ContentRootPath + "/" + _Configuration["AppDataFile"];

            _JsonData = JObject.Parse(File.ReadAllText(Path));
        }

        public void CreateStorageSpace(StorageSpace storageSpace)
        {
            ((JArray) _JsonData["StorageSpaces"]).Add(JToken.FromObject(storageSpace));
            Save();
        }

        public StorageSpace ReadStorageSpace(Guid id)
        {
            return (StorageSpace) from a in JsonConvert.DeserializeObject<List<StorageSpace>>(_JsonData["StorageSpaces"].ToString())
                                  where a.Id.Equals(id)
                                  select a;
        }

        public List<StorageSpace> ReadStorageSpaces()
        {
            return JsonConvert.DeserializeObject<List<StorageSpace>>(_JsonData["StorageSpaces"].ToString());
        }

        public void UpdateStorageSpace(StorageSpace storageSpace)
        {
            _JsonData.SelectToken("$.StorageSpaces[?(@.Id == '" + storageSpace.Id + "')]").Replace(JToken.FromObject(storageSpace));
            Save();
        }

        public void DeleteStorageSpaces(Guid id)
        {
            _JsonData.SelectToken("$.StorageSpaces[?(@.Id == '" + id + "')]").Remove();
            Save();
        }
    }
}