public class ColumnV : Expr
{
    public string Id {get;}
    public Type Type { get; }

    public override int LineNumber { get; }

    public ColumnV(string id, Type type)
    {
        Id = id;
        Type = type;
    }
}