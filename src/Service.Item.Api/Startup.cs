using HealthChecks.UI.Client;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Middlewares;
using Infrastructure.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Item.Api.Business;
using Service.Item.Api.Interfaces;
using Service.Item.Api.Repositories;
using Service.Item.Api.Services;

namespace Service.Item.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureBaseService(Configuration);

            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
            });

            services.AddGrpc();
            services.AddScoped<IDatabaseConnectionFactory, SqlDbConnectionFactory>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemNotFoundRepository, ItemNotFoundRepository>();
            services.AddScoped<ISpellRepository, SpellRepository>();
            services.AddScoped<IItemBusiness, ItemBusiness>();
            services.AddHostedService<ItemPullerService>();
            services.AddHostedService<SpellPullerService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureGlobal(env, "Service Item V1");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGrpcService<ItemService>();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}