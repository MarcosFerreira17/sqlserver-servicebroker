using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ServiceBroker.ConsoleApplication.Configurations;
public static class DatabaseConfig
{
    private static IConfiguration _configuration = GetConfiguration();

    private static IConfiguration GetConfiguration()
    {
        string appSettings = Path.GetFullPath("../../../appsettings.json");

        return new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
    }

    public static void StartQeueu()
    {
        string connectionString = GetConnectionString();

        using SqlConnection connection = new(connectionString);
        connection.Open();

        CreateQueue(connection, GetQueueName());
        CreateService(connection, GetServiceName());
        CreateMessageType(connection, GetMessageType());
        CreateContract(connection, GetContractName());
    }

    public static void CreateMessageType(SqlConnection connection, string messageTypeName)
    {
        if (!ExistsInDatabase(connection, "sys.service_message_types", "name", messageTypeName))
        {
            string createMessageTypeQuery = "CREATE MESSAGE TYPE " + messageTypeName + " VALIDATION = NONE;";

            ExecuteNonQuery(connection, createMessageTypeQuery);
        }
    }

    public static void CreateContract(SqlConnection connection, string contractName)
    {
        if (!ExistsInDatabase(connection, "sys.service_contracts", "name", contractName))
        {
            string createContractQuery = "CREATE CONTRACT " + contractName + " ([your_message_type_name] SENT BY ANY);";

            ExecuteNonQuery(connection, createContractQuery);
        }
    }

    public static void CreateService(SqlConnection connection, string serviceName)
    {
        if (!ExistsInDatabase(connection, "sys.services", "name", serviceName))
        {
            string createServiceQuery = "CREATE SERVICE " + serviceName + " ON QUEUE " + GetQueueName() + " ([DEFAULT]);";

            ExecuteNonQuery(connection, createServiceQuery);
        }
    }

    public static bool ExistsInDatabase(SqlConnection connection, string tableName, string columnName, string value)
    {
        string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Value";

        using SqlCommand command = connection.CreateCommand();
        command.CommandText = query;

        command.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;
        int count = (int)command.ExecuteScalar();

        return count > 0;
    }

    public static void ExecuteNonQuery(SqlConnection connection, string query)
    {
        using SqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.ExecuteNonQuery();
    }

    public static void CreateQueue(SqlConnection connection, string queueName)
    {
        string query = "SELECT COUNT(*) FROM sys.service_queues WHERE name = @QueueName";

        SqlCommand command = connection.CreateCommand();
        command.CommandText = query;

        command.Parameters.Add("@QueueName", SqlDbType.NVarChar).Value = queueName;
        int count = (int)command.ExecuteScalar();

        if (count == 0)
        {
            string enableQueueQuery = "ALTER DATABASE DatabaseName SET ENABLE_BROKER";

            command.CommandText = enableQueueQuery;
            command.ExecuteNonQuery();

            string createQueueQuery = "CREATE QUEUE " + queueName + " " +
                        "WITH STATUS = ON, " +
                        "RETENTION = ON, " +
                        "ACTIVATION (" +
                        "MAX_QUEUE_READERS = 2, " +
                        "EXECUTE AS SELF) " +
                        "ON [DEFAULT];";

            command.CommandText = createQueueQuery;
            command.ExecuteNonQuery();
        }
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
        string connection = _configuration["ConnectionStrings:DefaultConnection"]
                            ?? throw new ApplicationException("Database connection string not found.");

        return connection;
    }
}
