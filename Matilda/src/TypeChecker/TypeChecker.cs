namespace Matilda;

class TypeChecker
{
    public List<string> errors = new List<string>();

    public bool HasErrors()
    {
        return this.errors.Count > 0;
    }

    public TypeChecker(Stmt stmt)
    {
        StmtT(stmt);
    }

    private void StmtT(Stmt stmt)
    {
        switch (stmt)
        {
            case Skip:
                break;

            case Print print:
                ExprT(print.Value);
                break;

            case Comp comp:
                StmtT(comp.Stmt1);
                StmtT(comp.Stmt2);
                break;

            default: throw new Exception("Invalid statement");
        }
    }

    private Type ExprT(Expr expr)
    {
        switch (expr)
        {
            case IntV: return IntT.Instance;

            case FloatV: return FloatT.Instance;

            case BoolV: return BoolT.Instance;

            case StringV: return StringT.Instance;

            case BinaryOp binaryOp:
                Type typeLeft = ExprT(binaryOp.ExprLeft);
                Type typeRight = ExprT(binaryOp.ExprRight);

                switch (binaryOp.Op)
                {
                    case BinaryOperators.ADD:
                        if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '+' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '+' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                        }

                        if (typeLeft != typeRight)
                        {
                            errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                        }

                        break;

                    default: throw new Exception("Invalid binary operation");
                }

                switch (binaryOp.Op)
                {
                    case BinaryOperators.ADD:
                        if (typeLeft == IntT.Instance)
                        {
                            return IntT.Instance;
                        }
                        else
                        {
                            return FloatT.Instance;
                        }

                    default: throw new Exception("Invalid binary operation");
                }


            default: throw new Exception("Invalid expression");
        }
    }
}