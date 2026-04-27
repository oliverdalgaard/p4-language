namespace Matilda;

public class Column
{
    public string Id { get; }
    public Type Type { get; }

    public Column(string id, Type type)
    {
        Id = id;
        Type = type;
    }
}