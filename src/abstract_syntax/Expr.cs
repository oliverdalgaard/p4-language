namespace Matilda
{

    public abstract class Expr
    {
        public abstract int LineNumber { get; }
    }

    public class UnaryOp : Expr
    {
        private UnaryOperators op;
        private Expr expr;

        public override int LineNumber { get; }

        public UnaryOp(UnaryOperators op, Expr expr, int lineNumber)
        {
            this.op = op;
            this.expr = expr;

            LineNumber = lineNumber;
        }
    }

    public class BinaryOp : Expr
    {
        private BinaryOperators op;
        private Expr exprLeft;
        private Expr exprRight;

        public override int LineNumber { get; }

        public BinaryOp(BinaryOperators op, Expr exprLeft, Expr exprRight, int lineNumber)
        {
            this.op = op;
            this.exprLeft = exprLeft;
            this.exprRight = exprRight;

            LineNumber = lineNumber;
        }
    }

    public class Ref : Expr
    {
        private string name;

        public override int LineNumber { get; }

        public Ref(string name, int lineNumber)
        {
            this.name = name;

            LineNumber = lineNumber;
        }
    }

    public class IntV : Expr
    {
        private int value;

        public override int LineNumber { get; }

        public IntV(int value, int lineNumber)
        {
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public class FloatV : Expr
    {
        private float value;

        public override int LineNumber { get; }

        public FloatV(float value, int lineNumber)
        {
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public class BoolV : Expr
    {
        private bool value;

        public override int LineNumber { get; }

        public BoolV(bool value, int lineNumber)
        {
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public class StringV : Expr
    {
        private string value;

        public override int LineNumber { get; }

        public StringV(string value, int lineNumber)
        {
            this.value = value;

            LineNumber = lineNumber;
        }
    }

    public enum UnaryOperators
    {
        NOT
    }

    public enum BinaryOperators
    {
        ADD, SUB
    }
}