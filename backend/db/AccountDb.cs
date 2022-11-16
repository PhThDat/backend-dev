using Npgsql;
using BackEndCSharp.Model;

namespace BackEndCSharp.Db;

class AccountDb : IDisposable
{
    private static NpgsqlConnection connection;
    public static string TableName;
    static AccountDb()
    {
        TableName = "user_account";
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }
    public static bool UsernameExists(string uname)
    {
        string cmdText = $"SELECT username FROM {TableName} WHERE username = '{uname}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        return reader.HasRows;
    }
    public static void AddAccount(string username, string email, string encryptedPw)
    {
        string cmdText = $"INSERT INTO {TableName} VALUES ('{username}', '{email}', '{encryptedPw}')";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
        cmd.ExecuteNonQueryAsync();
    }
    public static void AddAccount(UserAccount user) => AddAccount(user.Username, user.Email, user.Password);
    public static bool AuthenticateAccount(string username, string encryptedPw)
    {
        string cmdText = $"SELECT password FROM {TableName} WHERE username = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        
        reader.Read();

        return reader.GetString(0) == encryptedPw;
    }
    public void Dispose() => connection.Dispose();
}