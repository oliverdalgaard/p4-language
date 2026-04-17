namespace Matilda;

public static class Interpreter
{
    public static void EvalStmt(Stmt stmt, EnvV envV, EnvP envP)
    {
        switch (stmt)
        {
            case Skip:
                Console.WriteLine("Skipped");
                break;

            case Comp comp:
                EvalStmt(comp.Stmt1, envV, envP);
                if (envV.TryGet("return") == null)
                {
                    EvalStmt(comp.Stmt2, envV, envP);
                }
                break;

            case Print print:
                Val value = EvalExpr(print.Value, envV, envP);
                Console.WriteLine(value.ToString());
                break;

            case Declaration declaration:
                envV.Bind(declaration.Identifier, null);
                break;

            case Assign assign:
                envV.Set(assign.Identifier, EvalExpr(assign.Value, envV, envP));
                break;

            case FunctionDeclaration functionDeclaration:
                envP.Bind(functionDeclaration);
                break;

            case Return returnVal:
                envV.Bind("return", EvalExpr(returnVal.Value, envV, envP));
                break;

            case If ifStmt:
                EnvV localScope = envV.NewScope();

                bool runElse = true;

                Val condition = EvalExpr(ifStmt.Condition, localScope, envP);
                if (condition.AsBool())
                {
                    runElse = false;
                    EvalStmt(ifStmt.ThenBody, localScope, envP);
                }
                else if (ifStmt.ElseIfStmts.Any())
                {
                    foreach (If elseIfStmt in ifStmt.ElseIfStmts)
                    {
                        Val elseIfStmtCondition = EvalExpr(elseIfStmt.Condition, localScope, envP);
                        if (elseIfStmtCondition.AsBool())
                        {
                            EvalStmt(elseIfStmt.ThenBody, localScope, envP);
                            runElse = false;
                            break;
                        }
                    }
                }

                if (runElse)
                {
                    EvalStmt(ifStmt.ElseBody, localScope, envP);
                }

                if (localScope.TryGet("return") != null)
                {
                    envV.Bind("return", localScope.TryGet("return"));
                }

                break;

            default:
                throw new Exception("Not valid statement");

        }
    }

    private static bool IsEqual(Val a, Val b)
    {
        if (a is IntVal ai && b is IntVal bi)
        {
            return ai.AsInt() == bi.AsInt();
        }
        else if (a is FloatVal af && b is FloatVal bf)
        {
            return af.AsFloat() == bf.AsFloat();
        }
        else if (a is BoolVal ab && b is BoolVal bb)
        {
            return ab.AsBool() == bb.AsBool();
        }
        return false;
    }

    public static Val EvalExpr(Expr expr, EnvV envV, EnvP envP)
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

            case FunctionRef functionRef:
                FunctionDeclaration function = envP.TryGet(functionRef.Name);

                if (functionRef.Arguments.Count != function.Parameters.Count)
                {
                    throw new Exception("Number of arguments do not match the amount of parameters.");
                }

                EnvV localScope = envV.NewScope();

                for (int i = 0; i < functionRef.Arguments.Count; i++)
                {
                    string parameterName = function.Parameters[i].Identifier;
                    Val value = EvalExpr(functionRef.Arguments[i], envV, envP);

                    localScope.Bind(parameterName, value);
                }

                foreach (Stmt stmt in function.Body)
                {
                    EvalStmt(stmt, localScope, envP);
                    if (localScope.TryGet("return") != null)
                    {
                        break;
                    }
                }


                return localScope.TryGet("return");

            case BinaryOp binaryOp:
                Val v1 = EvalExpr(binaryOp.ExprLeft, envV, envP);
                Val v2 = EvalExpr(binaryOp.ExprRight, envV, envP);

                switch (binaryOp.Op)
                {
                    case BinaryOperators.OR:
                        return new BoolVal(v1.AsBool() || v2.AsBool());

                    case BinaryOperators.AND:
                        return new BoolVal(v1.AsBool() && v2.AsBool());

                    case BinaryOperators.EQ:
                        return new BoolVal(IsEqual(v1, v2));

                    case BinaryOperators.NEQ:
                        return new BoolVal(!IsEqual(v1, v2));

                    case BinaryOperators.LT:
                        {
                            if (v1 is IntVal ai && v2 is IntVal bi)
                            {
                                return new BoolVal(ai.AsInt() < bi.AsInt());
                            }
                            else if (v1 is FloatVal af && v2 is FloatVal bf)
                            {
                                return new BoolVal(af.AsFloat() < bf.AsFloat());
                            }
                            else if (v1 is FloatVal af2 && v2 is IntVal bi2)
                            {
                                return new BoolVal(af2.AsFloat() < bi2.AsInt());
                            }
                            else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
                            {
                                return new BoolVal(ai2.AsInt() < bf2.AsFloat());
                            }
                            throw new Exception("Type error: '<' supports only numeric types (int/float)");
                        }

                    case BinaryOperators.ADD:
                        {
                            if (v1 is IntVal ai && v2 is IntVal bi)
                            {
                                return new FloatVal(ai.AsInt() + bi.AsInt());
                            }
                            else if (v1 is FloatVal af && v2 is FloatVal bf)
                            {
                                return new FloatVal(af.AsFloat() + bf.AsFloat());
                            }
                            else if (v1 is FloatVal af2 && v2 is IntVal bi2)
                            {
                                return new FloatVal(af2.AsFloat() + bi2.AsInt());
                            }
                            else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
                            {
                                return new FloatVal(ai2.AsInt() + bf2.AsFloat());
                            }
                            throw new Exception("Type error: '+' supports only numeric types (int/float)");
                        }

                    case BinaryOperators.SUB:
                        {
                            if (v1 is IntVal ai && v2 is IntVal bi)
                            {
                                return new FloatVal(ai.AsInt() - bi.AsInt());
                            }
                            else if (v1 is FloatVal af && v2 is FloatVal bf)
                            {
                                return new FloatVal(af.AsFloat() - bf.AsFloat());
                            }
                            else if (v1 is FloatVal af2 && v2 is IntVal bi2)
                            {
                                return new FloatVal(af2.AsFloat() - bi2.AsInt());
                            }
                            else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
                            {
                                return new FloatVal(ai2.AsInt() - bf2.AsFloat());
                            }
                            throw new Exception("Type error: '-' supports only numeric types (int/float)");
                        }

                    case BinaryOperators.MUL:
                        {
                            if (v1 is IntVal ai && v2 is IntVal bi)
                            {
                                return new FloatVal(ai.AsInt() * bi.AsInt());
                            }
                            else if (v1 is FloatVal af && v2 is FloatVal bf)
                            {
                                return new FloatVal(af.AsFloat() * bf.AsFloat());
                            }
                            else if (v1 is FloatVal af2 && v2 is IntVal bi2)
                            {
                                return new FloatVal(af2.AsFloat() * bi2.AsInt());
                            }
                            else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
                            {
                                return new FloatVal(ai2.AsInt() * bf2.AsFloat());
                            }
                            throw new Exception("Type error: '*' supports only numeric types (int/float)");
                        }

                    case BinaryOperators.DIV:
                        {
                            if (v1 is IntVal ai && v2 is IntVal bi)
                            {
                                return new FloatVal(ai.AsInt() / bi.AsInt());
                            }
                            else if (v1 is FloatVal af && v2 is FloatVal bf)
                            {
                                return new FloatVal(af.AsFloat() / bf.AsFloat());
                            }
                            else if (v1 is FloatVal af2 && v2 is IntVal bi2)
                            {
                                return new FloatVal(af2.AsFloat() / bi2.AsInt());
                            }
                            else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
                            {
                                return new FloatVal(ai2.AsInt() / bf2.AsFloat());
                            }
                            throw new Exception("Type error: '/' supports only numeric types (int/float)");
                        }

                    default: throw new Exception("Not a valid binaryOp expression");
                }

            case UnaryOp unaryOp:
                Val val = EvalExpr(unaryOp.Expr, envV, envP);

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