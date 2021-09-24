using Infrastructure.Core.HostBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args) => CoreHostBuilder<Startup>.BuildHost(args).Build().Run();
    }
}
