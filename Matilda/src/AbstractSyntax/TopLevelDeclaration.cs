namespace Matilda;

public abstract class TopLevelDeclaration
{
    public abstract int LineNumber { get; }
}

public class SchemaDeclaration : TopLevelDeclaration
{
    public string Identifier { get; }
    public List<Column> Columns { get; }

    public override int LineNumber { get; }


    public SchemaDeclaration(string identifier, List<Column> columns, int lineNumber)
    {
        Identifier = identifier;
        Columns = columns;

        LineNumber = lineNumber;
    }
}

public class FunctionDeclaration : TopLevelDeclaration
{
    public Type Type { get; }
    public string Identifier { get; }
    public List<Parameter> Parameters { get; }
    public Stmt Body { get; }

    public override int LineNumber { get; }

    public FunctionDeclaration(Type type, string identifier, List<Parameter> parameters, Stmt body, int lineNumber)
    {
        Type = type;
        Identifier = identifier;
        Parameters = parameters;
        Body = body;

        LineNumber = lineNumber;
    }
}