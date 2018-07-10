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

namespace LaPlay.Api.Sources.Data
{
    public interface IData
    {
        void Save();
    }

    class AppData
    {
        List<StorageSpace> _storageSpaces;
    }

    class StorageSpace
    {
        String Name;
        String MainFolder;
        String MirrorFolder;
    }

    public class Data : IData
    {
        private IConfiguration _Configuration;

        private AppData _AppData;

        public Data(IConfiguration configuration)
        {
            _Configuration = configuration;
            String json = File.ReadAllText(_Configuration["AppDataFile"]);
            _AppData = JsonConvert.DeserializeObject<AppData>(json);
        }

        public void Save()
        {
            String json = JsonConvert.SerializeObject(_AppData);
            File.WriteAllText(_Configuration["AppDataFile"], json);
        }
    }
}