using System.Data;
using System.Data.SqlClient;

string connectionString = "Data Source=localhost,1433;Initial Catalog=pocDatabase;User ID=sa;Password=zwh83067;Integrated Security=True;Trusted_Connection=False; TrustServerCertificate=True";

using (SqlConnection connection = new(connectionString))
{
    connection.Open();

    using (SqlCommand command = new("BEGIN DIALOG CONVERSATION @dlgHandle FROM SERVICE pocService TO SERVICE 'pocService' ON CONTRACT pocContract;", connection))
    {
        SqlParameter dlgHandle = command.Parameters.Add("@dlgHandle", SqlDbType.UniqueIdentifier);
        dlgHandle.Direction = ParameterDirection.Output;
        command.ExecuteNonQuery();

        // Enviando a mensagem
        using (SqlCommand sendCommand = new("SEND ON CONVERSATION @dlgHandle MESSAGE TYPE [messageType] (@Mensagem);", connection))
        {
            sendCommand.Parameters.AddWithValue("@dlgHandle", dlgHandle.Value);
            sendCommand.Parameters.AddWithValue("@Mensagem", "Esta é a minha mensagem de exemplo.");
            sendCommand.ExecuteNonQuery();
        }
    }
}

Console.WriteLine("Mensagem enviada com sucesso!");

