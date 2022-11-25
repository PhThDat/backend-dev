using Npgsql;
using NpgsqlTypes;
using BackEndCSharp.Model;

namespace BackEndCSharp.Db;

static class GoodsTable
{
    private static NpgsqlConnection connection;
    public static string Name;
    public static readonly Column[] Columns;
    public static Column IDCol { get => Columns[0]; }
    public static Column NameCol { get => Columns[1]; }
    public static Column AmountCol { get => Columns[2]; }
    public static Column DescriptionCol { get => Columns[3]; }
    public static Column ImagePathCol { get => Columns[4]; }
    public static Column[]? PrimaryKey;
    static GoodsTable()
    {
        Name = "goods";
        Columns = new Column[5]
        {
            new Column("id", "INT", isUnique: true),
            new Column("name", "VARCHAR(40)", isUnique: false),
            new Column("amount", "INT", isUnique: false),
            new Column("description", "VARCHAR(100)", isUnique: false),
            new Column("dir_path", "VARCHAR(70)")
        };
        PrimaryKey = new Column[] { Columns[0] };
        connection = new NpgsqlConnection("Server=172.20.0.2;Port=5432;Username=dat;Password=for-Thu;Database=db");
        connection.Open();
    }
    
}