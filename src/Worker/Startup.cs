using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Middlewares;
using Infrastructure.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Business;
using Worker.Interfaces;
using Worker.Repositories;
using Worker.Services;

namespace Worker
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGrpcService<ItemService>();
            });
        }
    }
}
