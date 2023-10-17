using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceBroker.ConsoleApplication.Configurations;
using ServiceBroker.ConsoleApplication.Interfaces;

namespace ServiceBroker.ConsoleApplication.Services;
public class PublisherService : IPublisherService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublisherService> _logger;
    public PublisherService(IConfiguration configuration, ILogger<PublisherService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishMessage(string message = "Hello World!")
    {
        string connectionString = GetConnectionString();

        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        SqlCommand command = connection.CreateCommand();

        try
        {
            command.Transaction = connection.BeginTransaction();
            command.CommandText = GetQuery(message);

            await command.ExecuteNonQueryAsync();

            await command.Transaction.CommitAsync();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error on publish message.");
            await command.Transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on publish message.");
            await command.Transaction.RollbackAsync();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static string GetConnectionString()
    {
        return DatabaseConfig.GetConnectionString();
    }

    private string GetQuery(string message)
    {
        string queue = DatabaseConfig.GetQueueName();
        string service = DatabaseConfig.GetServiceName();
        string contract = DatabaseConfig.GetContractName();
        string messageType = DatabaseConfig.GetMessageType();

        return $@"DECLARE @conversationHandle UNIQUEIDENTIFIER;
                    BEGIN DIALOG CONVERSATION @conversationHandle
                        FROM SERVICE {service}
                        TO SERVICE '{service}', '{queue}'
                        ON CONTRACT {contract}
                        WITH ENCRYPTION = OFF;
                    SEND ON CONVERSATION @conversationHandle
                        MESSAGE TYPE {messageType}('{message}');";
    }
}