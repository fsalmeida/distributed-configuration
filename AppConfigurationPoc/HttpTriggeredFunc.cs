using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace AppConfigurationPoc
{
    public static class HttpTriggeredFunc
    {
        private static IConfiguration Configuration { set; get; }
        public static IConfigurationRefresher ConfigurationRefresher { get; set; }

        static HttpTriggeredFunc()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                       .ConfigureRefresh(refreshOptions =>
                            refreshOptions.Register("TestApp:Settings:Message")
                                          .SetCacheExpiration(TimeSpan.FromSeconds(60))
                );
                ConfigurationRefresher = options.GetRefresher();
            });
            Configuration = builder.Build();
        }

        [FunctionName("HttpTriggeredFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await ConfigurationRefresher.Refresh();

            string keyName = "TestApp:Settings:Message";
            string message = Configuration[keyName];

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult($"Please create a key-value with the key '{keyName}' in App Configuration.");
        }
    }
}
