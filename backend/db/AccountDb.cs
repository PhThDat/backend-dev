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
        TableName = "useraccount";
        UsernameCol = "username";
        EmailCol = "email";
        PasswordCol = "password";
        JWTKeyCol = "jwt";
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }
    /// <summary>
    /// Makes sure the table including all of its columns and constraints are created.
    /// </summary>
    public static void EnsureCreated()
    {
        if (!TableExists())
        {
            CreateTable();
            AddUnique($"{TableName}_{EmailCol}_key", new string[]{ EmailCol });
            AddUnique($"{TableName}_{JWTKeyCol}_key", new string[]{ JWTKeyCol });
        }
        bool uname, email, pwd, jwt;
        uname = email = pwd = jwt = false;
        if (!CorrectColumnIntegrity(ref uname, ref email, ref pwd, ref jwt))
        {
            if (!uname)
            {
                AddColumn(UsernameCol, "VARCHAR(25)");
                AddPrimaryKey($"{TableName}_{UsernameCol}_pkey", new string[]{ UsernameCol });
            }
            if (!email)
            {
                AddColumn(EmailCol, "VARCHAR(50)");
                AddUnique($"{TableName}_{EmailCol}_key", new string[]{ EmailCol });
            }
            if (!pwd)
                AddColumn(PasswordCol, "VARCHAR(35)");
            if (!jwt)
            {
                AddColumn(JWTKeyCol, "BYTEA");
                AddUnique($"{TableName}_{JWTKeyCol}_key", new string[]{ JWTKeyCol });
            }
        }
    }
    // SQL
    /// <summary>
    /// Checks whether a username has already existed in the database or not.
    /// </summary>
    /// <returns>True on exists, otherwise false.</returns>
    public static bool UsernameExists(string username)
    {
        string cmdText = $"SELECT {UsernameCol} FROM {TableName} WHERE {UsernameCol} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    /// <summary>
    /// Checks whether an email has already existed in the database or not.
    /// </summary>
    /// <returns>True on exists, otherwise false.</returns>
    public static bool EmailExist(string email)
    {
        string cmdText = $"SELECT {EmailCol} FROM {TableName} WHERE {EmailCol} = '{email}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    /// <summary>
    /// Adds new account based on username, email, encrypted password to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddAccount(string username, string email, string encryptedPw)
    {
        try
        {
            string cmdText = $"INSERT INTO {TableName} ({UsernameCol}, {EmailCol}, {PasswordCol}) VALUES ('{username}', '{email}', '{encryptedPw}')";
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Adds new account based on username, email, encrypted password, JWT key to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddAccount(string username, string email, string encryptedPw, byte[] jwtKey)
    {
        try
        {
            string cmdText = $"INSERT INTO {TableName} ({UsernameCol}, {EmailCol}, {PasswordCol}, {JWTKeyCol}) VALUES ('{username}', '{email}', '{encryptedPw}', @key)";
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.Parameters.Add("key", NpgsqlDbType.Bytea).Value = jwtKey;
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Adds new account based on a UserAccount object to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddAccount(UserAccount account) => AddAccount(account.Username, account.Email, account.Password);
    /// <summary>
    /// Adds new account based on a UserAccount object and JWT key to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddAccount(UserAccount account, byte[] key) => AddAccount(account.Username, account.Email, account.Password, key);
    /// <summary>
    /// Add new JWT key to an account based on username.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
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
    /// <summary>
    /// Checks whether a JWT key has already existed in the database.
    /// </summary>
    /// <returns>True on exists, otherwise false.</returns>
    public static bool JWTKeyExist(byte[] key)
    {
        string cmdText = $"SELECT {JWTKeyCol} FROM {TableName} WHERE {JWTKeyCol} = @key";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
        cmd.Parameters.Add("key", NpgsqlDbType.Bytea).Value = (object?)key ?? DBNull.Value;

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    /// <summary>
    /// Checks whether an encrypted password matches the username.
    /// </summary>
    /// <returns>True on matching, otherwise false.</returns>
    public static bool AuthenticateAccount(string username, string encryptedPw)
    {
        string cmdText = $"SELECT {PasswordCol} FROM {TableName} WHERE {UsernameCol} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        return reader.GetString(0) == encryptedPw;
    }
    /// <summary>
    /// Remove the JWT key from an account based on username.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool DeleteJWTKey(string username)
    {
        try
        {
            string cmdText = $"UPDATE {TableName} SET {JWTKeyCol} = NULL WHERE {UsernameCol} = '{username}'";
            NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Gets the JWT key in bytes from an account based on username.
    /// </summary>
    /// <returns>A 32-byte array of retrieved JWT key</returns>
    public static byte[] GetJWTKey(string username)
    {
        string cmdText = $"SELECT {JWTKeyCol} FROM {TableName} WHERE {UsernameCol} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        byte[] buffer = new byte[32];
        reader.GetBytes(0, 0, buffer, 0, 32);

        return buffer;
    }

    // DML & DDL
    /// <summary>
    /// Adds a single column with name, type. <br/>
    /// Equivalent to "ALTER TABLE useraccount ADD {name} {type}"
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddColumn(string name, string type)
    {
        try
        {
            string cmdText = $"ALTER TABLE {TableName} ADD {name} {type}";
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Adds new PRIMARY KEY constraint with constraintName and affected column name(s).
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddPrimaryKey(string constraintName, IEnumerable<string> colNames)
    {
        try
        {
            string cmdText = $"ALTER TABLE {TableName} ADD CONSTRAINT {constraintName} PRIMARY KEY (";
            foreach (string col in colNames)
            {
                cmdText += col;
                if (col != colNames.Last())
                    cmdText += ',';
            }
            cmdText += ')';
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Adds new UNIQUE constraint with constraintName and affected column name(s).
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddUnique(string constraintName, IEnumerable<string> colNames)
    {
        try
        {
            string cmdText = $"ALTER TABLE {TableName} ADD CONSTRAINT {constraintName} UNIQUE (";
            foreach (string col in colNames)
            {
                cmdText += col;
                if (col != colNames.Last())
                    cmdText += ',';
            }
            cmdText += ')';
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Create the table to store data about accounts.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool CreateTable()
    {
        try
        {
            string cmdText = $"CREATE TABLE {TableName} ({UsernameCol} VARCHAR(25), {EmailCol} VARCHAR(50), {PasswordCol} VARCHAR(35), {JWTKeyCol} BYTEA, CONSTRAINT {TableName}_{UsernameCol}_pkey PRIMARY KEY ({UsernameCol}))";
            using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Checks whether the table has already existed in the database.
    /// </summary>
    /// <returns>True on exists, otherwise false.</returns>
    private static bool TableExists()
    {
        string cmdText = $"SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = '{TableName}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    /// <summary>
    /// Checks whether the existing table meets the requirements or not.<br/>
    /// 4 arguments indicate which column has already existed after executing the method.
    /// </summary>
    /// <returns>True on meeting the requirements, otherwise false.</returns>
    private static bool CorrectColumnIntegrity(ref bool uname, ref bool email, ref bool password, ref bool jwt)
    {
        char[]? buffer = new char[8];
        uname = email = password = jwt = false;
        
        string cmdText = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{TableName}' AND table_schema = 'public'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int count = (int)reader.GetChars(0, 0, buffer, 0, 8);
            string col = new string(buffer.Take(count).ToArray());
            if (col == UsernameCol)
                uname = true;
            else if (col == EmailCol)
                email = true;
            else if (col == PasswordCol)
                password = true;
            else if (col == JWTKeyCol)
                jwt = true;
        }
        return uname && email && password && jwt;
    }
}