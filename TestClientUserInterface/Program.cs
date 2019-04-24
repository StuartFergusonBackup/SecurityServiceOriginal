using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestClientUserInterface
{
    public class Program
    {
        public static void Main(String[] args)
        {
            var host = BuildWebHost(args);

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public static IWebHost BuildWebHost(String[] args)
        {
            Console.Title = "Security Service";

            String environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                  .AddJsonFile("hosting.json", optional: false)
                                                                  .AddJsonFile($"hosting.{environmentName}.json", optional: true)
                                                                  .Build();

            IWebHost host = new WebHostBuilder().UseKestrel().UseConfiguration(config).UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build();

            return host;
        }
    }
}
