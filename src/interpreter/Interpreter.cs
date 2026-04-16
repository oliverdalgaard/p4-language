namespace Matilda
{

    public static class Interpreter
    {
        public static void EvalStmt(Stmt stmt, EnvV envV)
        {

            switch (stmt)
            {
                case Skip:
                    Console.WriteLine("Skipped");
                    break;

                case Comp comp:
                    EvalStmt(comp.Stmt1, envV);
                    EvalStmt(comp.Stmt2, envV);
                    break;

                case Print print:
                    Val value = EvalExpr(print.Value, envV);
                    Console.WriteLine(value.ToString());
                    break;

                case Declaration declaration:
                    envV.Bind(declaration.Identifier, null);
                    break;

                case Assign assign:
                    envV.Set(assign.Identifier, EvalExpr(assign.Value, envV));
                    break;

                case If ifStmt:
                    Val condition = EvalExpr(ifStmt.Condition, envV);
                    if (condition.AsBool())
                    {
                        EvalStmt(ifStmt.ThenBody, envV);
                    }
                    else if (ifStmt.ElseIfStmts.Any())
                    {
                        foreach (If elseIfStmt in ifStmt.ElseIfStmts)
                        {
                            Val elseIfStmtCondition = EvalExpr(elseIfStmt.Condition, envV);
                            if (elseIfStmtCondition.AsBool())
                            {
                                EvalStmt(elseIfStmt.ThenBody, envV);
                                break;
                            }
                        }
                    }
                    else
                    {
                        EvalStmt(ifStmt.ElseBody, envV);
                    }
                    break;



                default:
                    throw new Exception("Not valid statement");

            }
        }

        public static Val EvalExpr(Expr expr, EnvV envV)
        {
            switch (expr)
            {
                case IntV intv:
                    return new IntVal(intv.Value);

                case FloatV floatv:
                    return new FloatVal(floatv.Value);

                case BoolV boolv:
                    return new BoolVal(boolv.Value);

                case StringV stringv:
                    return new StringVal(stringv.Value);

                case Ref reference:
                    return envV.TryGet(reference.Name);

                case BinaryOp binaryOp:
                    Val v1 = EvalExpr(binaryOp.ExprLeft, envV);
                    Val v2 = EvalExpr(binaryOp.ExprRight, envV);

                    switch (binaryOp.Op)
                    {
                        case BinaryOperators.OR:
                            return new BoolVal(v1.AsBool() || v2.AsBool());

                        case BinaryOperators.AND:
                            return new BoolVal(v1.AsBool() && v2.AsBool());

                        case BinaryOperators.EQ:
                            return new BoolVal(v1.AsBool() == v2.AsBool());

                        case BinaryOperators.NEQ:
                            return new BoolVal(v1.AsBool() != v2.AsBool());

                        case BinaryOperators.LT:
                            return new BoolVal(v1.AsInt() < v2.AsInt());

                        case BinaryOperators.ADD:
                            return new IntVal(v1.AsInt() + v2.AsInt());

                        case BinaryOperators.SUB:
                            return new IntVal(v1.AsInt() - v2.AsInt());

                        case BinaryOperators.MUL:
                            return new IntVal(v1.AsInt() * v2.AsInt());

                        case BinaryOperators.DIV:
                            return new IntVal(v1.AsInt() / v2.AsInt());

                        default: throw new Exception("Not a valid binaryOp expression");
                    }

                case UnaryOp unaryOp:
                    Val val = EvalExpr(unaryOp.Expr, envV);

                    switch (unaryOp.Op)
                    {
                        case UnaryOperators.NOT:
                            return new BoolVal(!val.AsBool());

                        default: throw new Exception("Not a valid unaryOp expression");
                    }

                default:
                    throw new Exception("Not a valid expression");
            }
        }
    }
}