using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaPubSubSample.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    IHostEnvironment env = hostContext.HostingEnvironment;
                    config.AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json")
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<KafkaSettings>(hostContext.Configuration.GetSection(nameof(KafkaSettings)));
                    services.AddHostedService<Producer>();
                })
                .Build()
                .Run();
        }
    }
}