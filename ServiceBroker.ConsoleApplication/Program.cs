using Microsoft.Extensions.Hosting;
using ServiceBroker.ConsoleApplication.Configurations;

class Program
{
    static async Task Main(string[] args)
    {
        DatabaseConfig.StartQeueu();
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.ResolveDependencies();
            });
}