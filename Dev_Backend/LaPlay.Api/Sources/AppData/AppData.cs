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

namespace LaPlay.AppData
{
    public interface IAppData
    {
        void Save();
    }

    class StorageSpace
    {
        public String Id;
        public String Name;
        public String MainFolder;
        public String MirrorFolder;
    }

    public class AppData : IAppData
    {
        private readonly IConfiguration _configuration;

        private List<StorageSpace> StorageSpaces;

        public AppData(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            
            JObject jsonData = JObject.Parse(File.ReadAllText(hostingEnvironment.ContentRootPath + "/" + _configuration["AppDataFile"]));

            StorageSpaces = JsonConvert.DeserializeObject<List<StorageSpace>>(jsonData["StorageSpaces"].ToString());
        }

        public void Save()
        {
            String json = JsonConvert.SerializeObject(StorageSpaces);
            File.WriteAllText(_configuration["AppDataFile"], json);
        }
    }
}