namespace SecurityService.Service
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Shared.General;

    public class Program
    {
        public static void Main(String[] args)
        {
            var host = Program.BuildWebHost(args);

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            
        }

        public static IWebHost BuildWebHost(String[] args)
        {
            Console.Title = "Security Service";

            String environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: false)
                .AddJsonFile($"hosting.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            IWebHost host = new WebHostBuilder().UseKestrel().UseConfiguration(config).UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build();

            return host;
        }
    }
}
