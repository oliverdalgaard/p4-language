using System.Runtime.CompilerServices;

namespace Matilda
{
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
        private Stmt? stmt1;
        private Stmt? stmt2;

        public Comp(Stmt? stmt1, Stmt? stmt2)
        {
            this.stmt1 = stmt1;
            this.stmt2 = stmt2;
        }

        public override int LineNumber
        {
            get
            {
                if (stmt1 != null)
                {
                    return stmt1.LineNumber;
                }

                throw new Exception("Left statement of ';' is 'null'. Cannot get line number");

            }
        }
    }

    public class Declaration : Stmt
    {
        private Type? type;
        private string? identifier;

        public override int LineNumber { get; }

        public Declaration(Type? type, string? identifier, int lineNumber)
        {
            this.type = type;
            this.identifier = identifier;

            LineNumber = lineNumber;
        }
    }

    public class Assign : Stmt
    {
        private string? identifier;
        private Expr? value;

        public override int LineNumber { get; }

        public Assign(string? identifier, Expr? value, int lineNumber)
        {
            this.identifier = identifier;
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public class Print : Stmt
    {
        private Expr? value;

        public override int LineNumber { get; }

        public Print(Expr value, int lineNumber)
        {
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public class If : Stmt
    {
        private Expr? condition;
        private Stmt? thenBody;
        private Stmt? elseBody;

        public override int LineNumber { get; }

        public If(Expr condition, Stmt thenBody, Stmt elseBody, int lineNumber)
        {
            this.condition = condition;
            this.thenBody = thenBody;
            this.elseBody = elseBody;

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
}
