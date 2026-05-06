namespace Matilda;

public abstract class Stmt
{
    public abstract int LineNumber { get; }
}

public class Skip : Stmt
{
    public override int LineNumber
    {
        get { throw new Exception("Skip does not have a line number!"); }
    }

    public static readonly Skip Instance = new Skip();

    public Skip() { }

}

public class Comp : Stmt
{
    public Stmt? Stmt1 { get; }
    public Stmt? Stmt2 { get; }

    public Comp(Stmt? stmt1, Stmt? stmt2)
    {
        Stmt1 = stmt1;
        Stmt2 = stmt2;
    }

    public override int LineNumber
    {
        get
        {
            if (Stmt1 != null)
            {
                return Stmt1.LineNumber;
            }

            throw new Exception("Left statement of ';' is 'null'. Cannot get line number");

        }
    }
}

public class Parameter : Stmt
{
    public Type Type { get; }
    public string Identifier { get; }

    public override int LineNumber { get; }

    public Parameter(Type type, string identifier, int lineNumber)
    {
        Type = type;
        Identifier = identifier;

        LineNumber = lineNumber;
    }
}

public class LocalDeclaration : Stmt
{
    public Type Type { get; }
    public string Identifier { get; }

    public Expr Expression { get; }

    public override int LineNumber { get; }

    public LocalDeclaration(Type type, string identifier, Expr expression, int lineNumber)
    {
        Type = type;
        Identifier = identifier;
        Expression = expression;

        LineNumber = lineNumber;
    }
}

public class Assign : Stmt
{
    public string? Identifier { get; }
    public Expr? Value { get; }

    public override int LineNumber { get; }

    public Assign(string? identifier, Expr? value, int lineNumber)
    {
        Identifier = identifier;
        Value = value;

        LineNumber = lineNumber;
    }
}

public class TableDeclaration : Stmt
{
    public Type Type { get; }
    public string Identifier { get; }
    public string FilePath { get; }

    public override int LineNumber { get; }


    public TableDeclaration(Type type, string identifier, string filePath, int lineNumber)
    {
        Type = type;
        Identifier = identifier;
        FilePath = filePath;

        LineNumber = lineNumber;
    }
}

public class Return : Stmt
{
    public Expr Value { get; }

    public override int LineNumber { get; }

    public Return(Expr value, int lineNumber)
    {
        Value = value;

        LineNumber = lineNumber;
    }
}

public class If : Stmt
{
    public Expr Condition { get; }
    public Stmt ThenBody { get; }
    public Stmt ElseBody { get; }

    public override int LineNumber { get; }

    public If(Expr condition, Stmt thenBody, Stmt elseBody, int lineNumber)
    {
        Condition = condition;
        ThenBody = thenBody;
        ElseBody = elseBody;

        LineNumber = lineNumber;
    }
}

public class While : Stmt
{
    public Expr Condition { get; }
    public Stmt Body { get; }

    public override int LineNumber { get; }

    public While(Expr condition, Stmt body, int lineNumber)
    {
        Condition = condition;
        Body = body;

        LineNumber = lineNumber;
    }

}