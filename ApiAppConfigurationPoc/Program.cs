using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiAppConfigurationPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var settings = config.Build();

                config.AddAzureAppConfiguration(options =>
                {
                    options.TrimKeyPrefix("TestApp:");
                    options.Connect(settings["ConnectionStrings:AppConfig"])
                    .Select("TestApp:*")
                    .Select("TestApp:ComLabel", "label01")
                    .ConfigureRefresh(refresh =>
                    {
                        refresh.SetCacheExpiration(TimeSpan.FromSeconds(5));
                        refresh.Register("TestApp:Sentinel", true);
                        refresh.Register("MasterSentinel", true);
                    });
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
