using Infrastructure.Core.Extensions;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                {
                    services.AddScoped<IDatabaseConnectionFactory, SqlDbConnectionFactory>();
                    services.AddScoped<IItemRepository, ItemRepository>();
                    services.AddScoped<IItemNotFoundRepository, ItemNotFoundRepository>();
                    services.AddHostedService<ItemPullerService>();
                })
                .RegisterKeyVault();
    }
}
