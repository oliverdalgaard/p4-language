namespace Matilda
{

    public abstract class Expr
    {
        public abstract int LineNumber { get; }
    }

    public class UnaryOp : Expr
    {
        public UnaryOperators Op { get; }
        public Expr Expr { get; }

        public override int LineNumber { get; }

        public UnaryOp(UnaryOperators op, Expr expr, int lineNumber)
        {
            Op = op;
            Expr = expr;

            LineNumber = lineNumber;
        }
    }

    public class BinaryOp : Expr
    {
        public BinaryOperators Op { get; }
        public Expr ExprLeft { get; }
        public Expr ExprRight { get; }

        public override int LineNumber { get; }

        public BinaryOp(BinaryOperators op, Expr exprLeft, Expr exprRight, int lineNumber)
        {
            Op = op;
            ExprLeft = exprLeft;
            ExprRight = exprRight;

            LineNumber = lineNumber;
        }
    }

    public class Ref : Expr
    {
        public string Name { get; }

        public override int LineNumber { get; }

        public Ref(string name, int lineNumber)
        {
            Name = name;

            LineNumber = lineNumber;
        }
    }

    public class FunctionRef : Expr
    {
        public string Name { get; }

        public List<Expr> Arguments { get; }

        public override int LineNumber { get; }

        public FunctionRef(string name, List<Expr> arguments, int lineNumber)
        {
            Name = name;
            Arguments = arguments;

            LineNumber = lineNumber;
        }
    }

    public class IntV : Expr
    {
        public int Value { get; }

        public override int LineNumber { get; }

        public IntV(int value, int lineNumber)
        {
            Value = value;

            LineNumber = lineNumber;
        }
    }

    public class FloatV : Expr
    {
        public float Value { get; }

        public override int LineNumber { get; }

        public FloatV(float value, int lineNumber)
        {
            Value = value;

            LineNumber = lineNumber;
        }
    }

    public class BoolV : Expr
    {
        public bool Value { get; }

        public override int LineNumber { get; }

        public BoolV(bool value, int lineNumber)
        {
            Value = value;

            LineNumber = lineNumber;
        }
    }

    public class StringV : Expr
    {
        public string Value { get; }

        public override int LineNumber { get; }

        public StringV(string value, int lineNumber)
        {
            Value = value;

            LineNumber = lineNumber;
        }
    }

    public enum UnaryOperators
    {
        NOT
    }

    public enum BinaryOperators
    {
        ADD, SUB, MUL, DIV, LT, EQ, NEQ, AND, OR
    }
}