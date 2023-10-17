using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ServiceBroker.ConsoleApplication.Configurations;
using ServiceBroker.ConsoleApplication.Interfaces;

namespace ServiceBroker.ConsoleApplication.Services;
public class SubscriberService : ISubscribeService
{
    private readonly ILogger<SubscriberService> _logger;
    public SubscriberService(ILogger<SubscriberService> logger)
    {
        _logger = logger;
    }

    public async Task SubscribeMessage()
    {
        using SqlConnection connection = new(DatabaseConfig.GetConnectionString());
        await connection.OpenAsync();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = connection.BeginTransaction();
        try
        {
            command.CommandText = GetQuery();
            await command.ExecuteNonQueryAsync();

            await command.Transaction.CommitAsync();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error on subscribe message.");
            await command.Transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on subscribe message.");
            await command.Transaction.RollbackAsync();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public string GetQuery()
    {
        return "WAITFOR (RECEIVE TOP(1) message_body FROM dbo." + DatabaseConfig.GetQueueName() + ");";
    }
}
