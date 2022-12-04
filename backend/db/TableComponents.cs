namespace BackEndCSharp.Db;

class Column
{
    /// <summary>
    /// Column name.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// Column data type.
    /// </summary>
    public readonly string Type;
    /// <summary>
    /// Has UNIQUE constraint.
    /// </summary>
    public readonly bool IsUnique;
    public Column(string colName, string colType, bool isUnique = false)
    {
        Name = colName;
        Type = colType;
        IsUnique = isUnique;
    }
}

class PrimaryKey
{
    /// <summary>
    /// A set of columns that represents the PRIMARY KEY.
    /// </summary>
    public readonly Column[] Keys = null;
    /// <summary>
    /// Key's name used for constraint naming.
    /// </summary>
    public string Name()
    {
        string name = string.Empty;
        foreach (Column key in Keys)
            name += $"{key.Name}_";

        return name.TrimEnd('_');
    }
    public PrimaryKey(Column[] keys)
    {
        Keys = keys;
    }
}