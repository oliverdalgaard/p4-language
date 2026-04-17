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

    private Skip() { }

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

public class Declaration : Stmt
{
    public Type? Type { get; }
    public string? Identifier { get; }

    public override int LineNumber { get; }

    public Declaration(Type? type, string? identifier, int lineNumber)
    {
        Type = type;
        Identifier = identifier;

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

public class FunctionDeclaration : Stmt
{
    public Type Type { get; }
    public string Identifier { get; }
    public List<Declaration> Parameters { get; }
    public List<Stmt> Body { get; }

    public override int LineNumber { get; }

    public FunctionDeclaration(Type type, string identifier, List<Declaration> parameters, List<Stmt> body, int lineNumber)
    {
        Type = type;
        Identifier = identifier;
        Parameters = parameters;
        Body = body;

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

public class Print : Stmt
{
    public Expr Value { get; }

    public override int LineNumber { get; }

    public Print(Expr value, int lineNumber)
    {
        this.Value = value;

        LineNumber = lineNumber;
    }
}

public class If : Stmt
{
    public Expr Condition { get; }
    public Stmt ThenBody { get; }
    public List<If>? ElseIfStmts { get; }
    public Stmt ElseBody { get; }

    public override int LineNumber { get; }

    public If(Expr condition, Stmt thenBody, List<If> elseIfStmts, Stmt elseBody, int lineNumber)
    {
        Condition = condition;
        ThenBody = thenBody;
        ElseIfStmts = elseIfStmts;
        ElseBody = elseBody;

        LineNumber = lineNumber;
    }
}

public class While : Stmt
{
    private Expr? condition;
    private Stmt? body;

    public override int LineNumber { get; }

    public While(Expr? condition, Stmt? body, int lineNumber)
    {
        this.condition = condition;
        this.body = body;

        LineNumber = lineNumber;
    }

}