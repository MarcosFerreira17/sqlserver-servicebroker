using Microsoft.Extensions.Configuration;

namespace ServiceBroker.ConsoleApplication.Configurations;
public static class DatabaseConfig
{
    private static IConfiguration _configuration = GetConfiguration();
    private static IConfiguration GetConfiguration()
    {
        return _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }
    public static string GetQueueName()
    {
        return _configuration["ServiceBroker:Queue"]
                ?? throw new ApplicationException("Queue name not found.");
    }

    public static string GetServiceName()
    {
        return _configuration["ServiceBroker:Service"]
                ?? throw new ApplicationException("Service name not found.");
    }

    public static string GetContractName()
    {
        return _configuration["ServiceBroker:Contract"]
                ?? throw new ApplicationException("Contract name not found.");
    }

    public static string GetMessageType()
    {
        return _configuration["ServiceBroker:MessageType"]
                ?? throw new ApplicationException("Message type not found.");
    }

    public static string GetConnectionString()
    {
        string connection = _configuration.GetConnectionString("DefaultConnection")
                            ?? throw new ApplicationException("Database connection string not found.");

        return connection;
    }
}
