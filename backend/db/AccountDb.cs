using Npgsql;
using NpgsqlTypes;
using BackEndCSharp.Model;
using System.Diagnostics;

namespace BackEndCSharp.Db;

static class AccountDb
{
    private static NpgsqlConnection connection;
    public static string TableName;
    public static string UsernameCol;
    public static string EmailCol;
    public static string PasswordCol;
    public static string JWTKeyCol;
    static AccountDb()
    {
        TableName = "user_account";
        UsernameCol = "username";
        EmailCol = "email";
        PasswordCol = "password";
        JWTKeyCol = "jwt";
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }
    public static bool UsernameExists(string username)
    {
        string cmdText = $"SELECT ({UsernameCol} = '{username}') FROM {TableName} WHERE {UsernameCol} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    public static void AddAccount(string username, string email, string encryptedPw)
    {
        string cmdText = $"INSERT INTO {TableName} ({UsernameCol}, {EmailCol}, {PasswordCol}) VALUES ('{username}', '{email}', '{encryptedPw}')";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
        cmd.ExecuteNonQuery();
    }
    public static void AddAccount(UserAccount user) => AddAccount(user.Username, user.Email, user.Password);
    public static bool AddJWTKey(string username, byte[] key)
    {
        if (UsernameExists(username) && !JWTKeyExist(key))
        {
            string cmdText = $"UPDATE {TableName} SET {JWTKeyCol} = @key WHERE {UsernameCol} = '{username}'";
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.Parameters.Add("key", NpgsqlDbType.Bytea).Value = (object?)key ?? DBNull.Value;
            cmd.ExecuteNonQuery();
            return true;
        }

        return false;
    }
    public static bool JWTKeyExist(byte[] key)
    {
        string cmdText = $"SELECT {JWTKeyCol} FROM {TableName} WHERE {JWTKeyCol} = @key";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
        cmd.Parameters.Add("key", NpgsqlDbType.Bytea).Value = (object?)key ?? DBNull.Value;

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    public static bool AuthenticateAccount(string username, string encryptedPw)
    {
        string cmdText = $"SELECT {PasswordCol} FROM {TableName} WHERE {UsernameCol} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        return reader.GetString(0) == encryptedPw;
    }
}