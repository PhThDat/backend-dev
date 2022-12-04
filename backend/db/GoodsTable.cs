using Npgsql;
using NpgsqlTypes;
using BackEndCSharp.Model;

namespace BackEndCSharp.Db;

static class GoodsTable
{
    private static NpgsqlConnection connection;
    public static string Name;
    public static readonly Column[] Columns;
    public static Column IDCol => Columns[0];
    public static Column NameCol => Columns[1];
    public static Column AmountCol => Columns[2];
    public static Column DescriptionCol => Columns[3];
    public static Column ImagePathCol => Columns[4];
    public static PrimaryKey? PKey;
    static GoodsTable()
    {
        Name = "goods";
        Columns = new Column[5]
        {
            new Column("id", "INT", isUnique: true),
            new Column("name", "VARCHAR(40)", isUnique: false),
            new Column("amount", "INT", isUnique: false),
            new Column("description", "VARCHAR(100)", isUnique: false),
            new Column("img_path", "VARCHAR(70)", isUnique: false)
        };
        PKey = new PrimaryKey(new Column[]{ Columns[0] });
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }

    // SQL
    /// <summary>
    /// Adds new goods based on id, name, amount, description, image path to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddGoods(int id, string name, int amount, string description, string pathToImg)
    {
        try
        {
            string cmdText = $"INSERT INTO {Name} ({IDCol.Name}, {NameCol.Name}, {AmountCol.Name}, {DescriptionCol.Name}, {ImagePathCol.Name}) VALUES ({id}, '{name}', {amount}, '{description}', '{pathToImg}')";
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
    /// Adds new goods based on a Goods object to the database.
    /// </summary>
    /// <returns>True on success, otherwise false.</returns>
    public static bool AddGoods(Goods goods, int amount) => AddGoods(goods.ID, goods.Name, amount, goods.Description, goods.ImagePath);
    /// <summary>
    /// Get random rows of good from the database.
    /// </summary>
    /// <returns>An array of goods</returns>
    public static GoodsInfo[] GetRandomGoods(int rowAmount)
    {
        string cmdText = $"SELECT g.{IDCol.Name}, g.{NameCol.Name}, g.{AmountCol.Name}, g.{DescriptionCol.Name}, g.{ImagePathCol.Name} FROM (SELECT {IDCol.Name} FROM {Name} ORDER BY RANDOM() LIMIT {rowAmount}) AS r INNER JOIN {Name} AS g ON g.{IDCol.Name} = r.{IDCol.Name}";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);
        using NpgsqlDataReader reader = cmd.ExecuteReader();

        GoodsInfo[] gInfos = new GoodsInfo[rowAmount];
        int count = 0;
        for (int i = 0; reader.Read(); i++)
        {
            Goods goods = new Goods()
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(3),
                ImagePath = reader.GetString(4),
            };
            gInfos[i] = new GoodsInfo()
            {
                Goods = goods,
                Amount = reader.GetInt32(2),
            };
            count++;
        }
        return gInfos.Take(count).ToArray();
    }


    // DML & DDL
    /// <summary>
    /// Makes sure the table including all of its columns and constraints are created.
    /// </summary>
    public static bool EnsureCreated()
    {
        try
        {
            if (!TableExists())
            {
                CreateTable();
                AddPrimaryKey($"{Name}_{PKey.Name()}_pkey", PKey.Keys);
                int len = Columns.Length;
                for (int i = 0; i < len; i++)
                    if (Columns[i].IsUnique)
                        AddUnique($"{Name}_{Columns[i].Name}_key", new Column[]{ Columns[i] });
            }
            bool[]? buffer = null;
            if (!CorrectColumnIntegrity(ref buffer))
            {
                AddPrimaryKey($"{Name}_{PKey.Name()}_pkey", PKey.Keys);
                int len = buffer.Length;
                for (int i = 0; i < len; i++)
                    if (!buffer[i])
                    {
                        AddColumn(Columns[i]);
                        if (Columns[i].IsUnique)
                            AddUnique($"{Name}_{Columns[i].Name}_key", new Column[]{ Columns[i] });
                    }
            }
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
            string cmdText = $"CREATE TABLE {Name} (";
            foreach (Column col in Columns)
                cmdText += $"{col.Name} {col.Type},";
            if (PKey != null)
            {
                cmdText += $"CONSTRAINT {Name}_";
                foreach (Column col in PKey.Keys)
                    cmdText += $"{col.Name}_";
                cmdText += "pkey PRIMARY KEY (";
                foreach (Column col in PKey.Keys)
                {
                    string seperator = col != PKey.Keys.Last() ? "," : "))";
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
        int maxNameLen = Columns.Max(col => col.Name.Length);
        char[]? buffer = new char[maxNameLen];
        boolBuffer = new bool[Columns.Length];
        
        string cmdText = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{Name}' AND table_schema = 'public'";
        using NpgsqlCommand cmd = new NpgsqlCommand(cmdText, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();

        int len = Columns.Length;
        while (reader.Read())
        {
            int count = (int)reader.GetChars(0, 0, buffer, 0, maxNameLen);
            string col = new string(buffer.Take(count).ToArray());

            for (int i = 0; i < len; i++)
                if (col == Columns[i].Name)
                    boolBuffer[i] = true;
        }
        return boolBuffer.All(col => col);
    }
}