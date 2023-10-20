using System.Data;
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
            string message = string.Empty;
            command.CommandText = GetQuery();

            SqlParameter dialogHandler = new("@dialog_handle", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter messageParam = new("@message", SqlDbType.NVarChar, -1)
            {
                Direction = ParameterDirection.Output
            };

            command.CommandTimeout = 60000;

            await command.ExecuteNonQueryAsync();

            if (messageParam.Value != DBNull.Value)
            {
                message = (string)messageParam.Value;

                if ((Guid)dialogHandler.Value != Guid.Empty)
                {
                    command.CommandText = "END CONVERSATION @dialog_handle";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@dialog_handle", dialogHandler.Value);
                    await command.ExecuteNonQueryAsync();
                }

                //trata mensagem
            }

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
        return "WAITFOR (RECEIVE TOP(1) @dialog_handle = conversation_handle, @message = message_body FROM dbo." + DatabaseConfig.GetQueueName() + ") TIMEOUT 60000";
    }
}
