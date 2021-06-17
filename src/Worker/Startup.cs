using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Interfaces;
using Worker.Repositories;
using Worker.Services;

namespace Worker
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddScoped<IDatabaseConnectionFactory, SqlDbConnectionFactory>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemNotFoundRepository, ItemNotFoundRepository>();
            services.AddScoped<ISpellRepository, SpellRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ItemService>();
            });
        }
    }
}
