using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog.Web;

namespace LaPlay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                buildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost buildWebHost(string[] args)
        {
            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddCommandLine(args)
                .Build();

            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configurationBuilder)
                
                // .ConfigureServices(
                //     servicesCollection =>
                //     {
                //         servicesCollection.AddSingleton<ICustomClass>(new CustomClass()
                //         {
                //             MyInt = myInt,
                //             MyString = myString
                //         });
                //     })
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: setup NLog for Dependency injection;

                return webHostBuilder.Build();

            // return new WebHostBuilder()
            // .UseKestrel()
            // .UseContentRoot(Directory.GetCurrentDirectory())
            // .ConfigureAppConfiguration((builderContext, config) =>
            // {
            //     IHostingEnvironment env = builderContext.HostingEnvironment;

            //     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            // })
            // .UseIISIntegration()
            // .UseDefaultServiceProvider((context, options) =>
            // {
            //     options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            // })
            // .UseStartup<Startup>()
            // .Build();


            // public Startup(IHostingEnvironment env)
            // {
            //     var builder = new ConfigurationBuilder()
            //         .SetBasePath(env.ContentRootPath)
            //         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //         .AddJsonFile($"config/myConfig.json", optional: false, reloadOnChange: true)
            //         .AddEnvironmentVariables();
            //     Configuration = builder.Build();
            // }



            //https://stackoverflow.com/questions/37365277/how-to-specify-the-port-an-asp-net-core-application-is-hosted-on
            //https://andrewlock.net/configuring-urls-with-kestrel-iis-and-iis-express-with-asp-net-core/
            //https://andrewlock.net/reloading-strongly-typed-options-when-appsettings-change-in-asp-net-core-rc2/
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: setup NLog for Dependency injection;
    }
}
