using System.Data;
using System.Data.SqlClient;

string connectionString = "Data Source=localhost,1433;Initial Catalog=pocDatabase;User ID=sa;Password=zwh83067;Integrated Security=True;Trusted_Connection=False; TrustServerCertificate=True";

using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();

    // Recebendo a mensagem
    using (SqlCommand receiveCommand = new("RECEIVE TOP(1) @dlgHandle = conversation_handle, @Mensagem = message_body FROM pocQueue;", connection))
    {
        SqlParameter dlgHandle = receiveCommand.Parameters.Add("@dlgHandle", SqlDbType.UniqueIdentifier);
        dlgHandle.Direction = ParameterDirection.Output;
        SqlParameter mensagemParam = receiveCommand.Parameters.Add("@Mensagem", SqlDbType.NVarChar, -1);
        mensagemParam.Direction = ParameterDirection.Output;

        receiveCommand.ExecuteNonQuery();

        string mensagem = mensagemParam.Value.ToString();
        Console.WriteLine("Mensagem recebida: " + mensagem);
    }
}
