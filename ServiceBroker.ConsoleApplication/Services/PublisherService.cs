using System.Data;
using System.Runtime.InteropServices;
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
            command.CommandText = GetQuery(message);

            command.Transaction = connection.BeginTransaction();

            SqlParameter sqlParameter = new("@dialog_handle", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(sqlParameter);

            command.Parameters.AddWithValue("@message_body", message);
            command.Parameters.AddWithValue("@service_name", $"{DatabaseConfig.GetQueueName()}");

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
        string service = DatabaseConfig.GetServiceName();
        string contract = DatabaseConfig.GetContractName();
        string messageType = DatabaseConfig.GetMessageType();

        return $@"BEGIN DIALOG @dialog_handle
                        FROM SERVICE {service}
                        TO SERVICE '{service}', 'CURRENT DATABASE'
                        ON CONTRACT {contract}
                        WITH ENCRYPTION = OFF;
                    SEND ON CONVERSATION @dialog_handle
                        MESSAGE TYPE {messageType}(@message_body);";
    }
}