using Infrastructure.Core.Extensions;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Net;
using Worker.Interfaces;
using Worker.Repositories;
using Worker.Services;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Local")
                    {
                        webBuilder.ConfigureKestrel(options =>
                        {
                            options.Listen(IPAddress.Any, 80, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });

                            options.Listen(IPAddress.Any, 6000, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                {
                    services.AddScoped<IDatabaseConnectionFactory, SqlDbConnectionFactory>();
                    services.AddScoped<IItemRepository, ItemRepository>();
                    services.AddScoped<IItemNotFoundRepository, ItemNotFoundRepository>();
                    services.AddHostedService<ItemPullerService>();
                    services.AddHostedService<SpellPullerService>();
                })
                .RegisterKeyVault();
    }
}
