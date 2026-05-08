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

    public override bool Equals(object? obj)
    {
        return obj is Column columnObj && columnObj.Id == Id && columnObj.Type == Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(nameof(Column), Id, Type);
    }
}