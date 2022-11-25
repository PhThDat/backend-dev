using Npgsql;
using NpgsqlTypes;
using BackEndCSharp.Model;

namespace BackEndCSharp.Db;

static class AccountTable
{
    private static NpgsqlConnection connection;
    public static string Name;
    public static readonly Column[] Columns;
    public static Column UsernameCol { get => Columns[0]; }
    public static Column EmailCol { get => Columns[1]; }
    public static Column PasswordCol { get => Columns[2]; }
    public static Column JWTKeyCol { get => Columns[3]; }
    public static Column[]? PrimaryKey;
    static AccountTable()
    {
        Name = "useraccount";
        Columns = new Column[4]
        {
            new Column("username", "VARCHAR(25)"),
            new Column("email", "VARCHAR(50)", isUnique: true),
            new Column("password", "CHAR(64)", isUnique: false),
            new Column("jwt", "BYTEA", isUnique: true)
        };
        PrimaryKey = new Column[]{ Columns[0] };

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
            AddUnique($"{Name}_{EmailCol.Name}_key", new Column[]{ EmailCol });
            AddUnique($"{Name}_{JWTKeyCol.Name}_key", new Column[]{ JWTKeyCol });
        }
        bool[]? buffer = null;
        if (!CorrectColumnIntegrity(ref buffer))
        {
            if (!uname)
            {
                AddColumn(UsernameCol);
                AddPrimaryKey($"{Name}_{UsernameCol.Name}_pkey", new Column[]{ UsernameCol });
            }
            if (!email)
            {
                AddColumn(EmailCol);
                AddUnique($"{Name}_{EmailCol.Name}_key", new Column[]{ EmailCol });
            }
            if (!pwd)
                AddColumn(PasswordCol);
            if (!jwt)
            {
                AddColumn(JWTKeyCol);
                AddUnique($"{Name}_{JWTKeyCol.Name}_key", new Column[]{ JWTKeyCol });
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
        string cmdText = $"SELECT {UsernameCol.Name} FROM {Name} WHERE {UsernameCol.Name} = '{username}'";
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
        string cmdText = $"SELECT {EmailCol.Name} FROM {Name} WHERE {EmailCol.Name} = '{email}'";
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
            string cmdText = $"INSERT INTO {Name} ({UsernameCol.Name}, {EmailCol.Name}, {PasswordCol.Name}) VALUES ('{username}', '{email}', '{encryptedPw}')";
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
            string cmdText = $"INSERT INTO {Name} ({UsernameCol.Name}, {EmailCol.Name}, {PasswordCol.Name}, {JWTKeyCol.Name}) VALUES ('{username}', '{email}', '{encryptedPw}', @key)";
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
            string cmdText = $"UPDATE {Name} SET {JWTKeyCol.Name} = @key WHERE {UsernameCol.Name} = '{username}'";
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
        string cmdText = $"SELECT {JWTKeyCol.Name} FROM {Name} WHERE {JWTKeyCol.Name} = @key";
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
        string cmdText = $"SELECT {PasswordCol.Name} FROM {Name} WHERE {UsernameCol.Name} = '{username}'";
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
            string cmdText = $"UPDATE {Name} SET {JWTKeyCol.Name} = NULL WHERE {UsernameCol.Name} = '{username}'";
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
        string cmdText = $"SELECT {JWTKeyCol.Name} FROM {Name} WHERE {UsernameCol.Name} = '{username}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        reader.Read();

        byte[] buffer = new byte[32];
        reader.GetBytes(0, 0, buffer, 0, 32);

        return buffer;
    }

    // DML & DDL
    /// <summary>
    /// Create the table to store data about accounts.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool CreateTable()
    {
        try
        {
            string cmdText = $"CREATE TABLE {Name} (";
            foreach (Column col in Columns)
                cmdText += $"{col.Name} {col.Type},";
            if (PrimaryKey != null)
            {
                cmdText += $"CONSTRAINT {Name}_";
                foreach (Column col in Columns)
                    cmdText += $"{col.Name}_";
                cmdText += "pkey PRIMARY KEY (";
                foreach (Column col in Columns)
                {
                    char seperator = col != Columns.Last() ? ',' : ')';
                    cmdText += $"{col.Name}{seperator}";
                }
            }
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
    /// Adds a single column with name, type. <br/>
    /// Equivalent to "ALTER TABLE useraccount ADD {name} {type}"
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddColumn(string name, string type)
    {
        try
        {
            string cmdText = $"ALTER TABLE {Name} ADD {name} {type}";
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
    /// Add a single column
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddColumn(Column column) => AddColumn(column.Name, column.Type);
    /// <summary>
    /// Adds new PRIMARY KEY constraint with constraintName and affected column name(s).
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    private static bool AddPrimaryKey(string constraintName, IEnumerable<Column> colNames)
    {
        try
        {
            string cmdText = $"ALTER TABLE {Name} ADD CONSTRAINT {constraintName} PRIMARY KEY (";
            foreach (Column col in colNames)
            {
                cmdText += col.Name;
                if (col.Name != colNames.Last().Name)
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
    private static bool AddUnique(string constraintName, IEnumerable<Column> colNames)
    {
        try
        {
            string cmdText = $"ALTER TABLE {Name} ADD CONSTRAINT {constraintName} UNIQUE (";
            foreach (Column col in colNames)
            {
                cmdText += col.Name;
                if (col.Name != colNames.Last().Name)
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
    /// Checks whether the table has already existed in the database.
    /// </summary>
    /// <returns>True on exists, otherwise false.</returns>
    private static bool TableExists()
    {
        string cmdText = $"SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = '{Name}'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    /// <summary>
    /// Checks whether the existing table meets the requirements or not.<br/>
    /// 4 arguments indicate which column has already existed after executing the method.
    /// </summary>
    /// <returns>True on meeting the requirements, otherwise false.</returns>
    private static bool CorrectColumnIntegrity(ref bool[]? boolBuffer)
    {
        char[]? buffer = new char[8];
        boolBuffer = new bool[Columns.Length];
        
        string cmdText = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{Name}' AND table_schema = 'public'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();

        int len = Columns.Length;
        while (reader.Read())
        {
            int count = (int)reader.GetChars(0, 0, buffer, 0, Columns.Max(col => col.Name.Length));
            string col = new string(buffer.Take(count).ToArray());

            for (int i = 0; i < len; i++)
                if (col == Columns[i].Name)
                    boolBuffer[i] = true;
        }
        return boolBuffer.All(col => col);
    }
}