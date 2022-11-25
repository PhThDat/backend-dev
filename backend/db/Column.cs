namespace BackEndCSharp.Db;

class Column
{
    public readonly string Name;
    public readonly string Type;
    public readonly bool IsUnique;
    public Column(string colName, string colType, bool isUnique = false)
    {
        Name = colName;
        Type = colType;
        IsUnique = isUnique;
    }
}