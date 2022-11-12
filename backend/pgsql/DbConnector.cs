using Npgsql;

namespace BackEndCSharp.Db;

class AccountDb : IDisposable
{
    private NpgsqlConnection connection;
    public string TableName;
    public AccountDb()
    {
        TableName = "user_account";
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }
    public void AddAccount(string username, string email, string encryptedPw)
    {
        string cmdText = $"INSERT INTO {TableName} VALUES ('{username}', '{email}', '{encryptedPw}')";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText);
        cmd.Connection = connection;
        cmd.ExecuteNonQueryAsync();        
    }
    public bool AuthenticateAccount(string username, string encryptedPw)
    {
        string cmdText = $"SELECT password FROM {TableName} WHERE username = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText);
        cmd.Connection = connection;

        NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        return reader.GetString(0) == encryptedPw;
    }
    public void Dispose() => connection.Dispose();
}