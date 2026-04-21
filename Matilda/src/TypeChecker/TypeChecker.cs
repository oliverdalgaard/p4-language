namespace Matilda
{
    class TypeChecker
    {
        public List<string> errors = new List<string>();
        private Dictionary<string, Type> env = new Dictionary<string, Type>();

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

                case If ifStmt:
                    if (ifStmt.Condition != null)
                    {
                        Type condT = ExprT(ifStmt.Condition);

                        if (condT != BoolT.Instance)
                        {
                            errors.Add($"Line {ifStmt.LineNumber} If statement requires a condition with type 'bool', but got '{condT}'.");
                        }
                    }
                    //then, elseif, else branch

                    if (ifStmt.ThenBody != null)
                    {
                        StmtT(ifStmt.ThenBody);
                    }

                    if (ifStmt.ElseIfStmts != null)
                    {
                        foreach (var elseif in ifStmt.ElseIfStmts)
                        {
                            StmtT(elseif);
                        }
                    }

                    if (ifStmt.ElseBody != null)
                    {
                        StmtT(ifStmt.ElseBody);
                    }

                    break;

                case Assign assign:

                    // check for null 
                    if (assign.Identifier == null || assign.Value == null)
                    {
                        errors.Add($"Line {assign.LineNumber} invalid assignment");
                        break;
                    }
                    // check delclaration 
                    if (!env.ContainsKey(assign.Identifier))
                    {
                        errors.Add($"Line {assign.LineNumber} varibale {assign.Identifier} is not declared.");
                    }
                    else
                    {
                        // check type match 
                        Type expectedType = env[assign.Identifier];
                        Type actualType = ExprT(assign.Value);

                        if (expectedType != actualType)
                        {
                            errors.Add($"Line {assign.LineNumber}: Cannot assign '{actualType}' to variable '{assign.Identifier}' of type '{expectedType}'.");
                        }
                    }

                    break;

                case Declaration declaration:
                    if (declaration.Identifier == null || declaration.Type == null)
                    {
                        errors.Add($"Line {declaration.LineNumber} invalid declaration.");
                        break;
                    }

                    if (env.ContainsKey(declaration.Identifier))
                    {
                        errors.Add($"Line {declaration.LineNumber} varibale '{declaration.Identifier}' is already declared.");
                        break;
                    }

                    env[declaration.Identifier] = declaration.Type;
                    break;

                case While whileStmt:
                    if (whileStmt.Condition == null)
                    {
                        errors.Add($"Line {whileStmt.LineNumber} while statement need a valid condition.");
                    }
                    else
                    {
                        Type condT = ExprT(whileStmt.Condition);

                        if (condT != BoolT.Instance)
                        {
                            errors.Add($"Line {whileStmt.LineNumber} while statement requires a condition with type 'bool', but got '{condT}'.");
                        }
                    }

                    if (whileStmt.Body != null)
                    {
                        StmtT(whileStmt.Body);
                    }
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

                        case BinaryOperators.SUB:
                            if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '-' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '-' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                            }

                            if (typeLeft != typeRight)
                            {
                                errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                            }

                            break;

                        case BinaryOperators.MUL:
                            if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '*' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '*' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                            }

                            if (typeLeft != typeRight)
                            {
                                errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                            }

                            break;

                        case BinaryOperators.DIV:
                            if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '/' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '/' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                            }

                            if (binaryOp.ExprRight is IntV intVal && intVal.Value == 0)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Division by zero.");
                            }

                            if (binaryOp.ExprRight is FloatV floatVal && floatVal.Value == 0.0)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Division by zero.");
                            }

                            if (typeLeft != typeRight)
                            {
                                errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                            }

                            break;

                        case BinaryOperators.LT:
                            if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '<' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '<' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                            }

                            break;

                        case BinaryOperators.EQ:
                            if (typeLeft != BoolT.Instance && typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '==' expected a left operand of type 'boolean','int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != BoolT.Instance && typeRight != IntT.Instance && typeRight != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '==' expected a right operand of type 'Boolean','int' or 'float', but got '{typeRight}'.");
                            }

                            if (typeLeft != typeRight)
                            {
                                errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                            }

                            break;

                        case BinaryOperators.NEQ:
                            if (typeLeft != BoolT.Instance && typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '==' expected a left operand of type 'boolean','int' or 'float', but got '{typeLeft}'.");
                            }

                            if (typeRight != BoolT.Instance && typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '==' expected a right operand of type 'Boolean','int' or 'float', but got '{typeRight}'.");
                            }

                            if (typeLeft != typeRight)
                            {
                                errors.Add($"Line {binaryOp.LineNumber}: Type mismatch.");
                            }

                            break;

                        case BinaryOperators.AND:
                            if (typeLeft != BoolT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '&&' expected a left operand of type 'Boolean', but got '{typeLeft}'.");
                            }
                            if (typeRight != BoolT.Instance)
                            {

                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '&&' expected a right operand of type 'Boolean', but got '{typeRight}'.");
                            }

                            break;

                        case BinaryOperators.OR:
                            if (typeLeft != BoolT.Instance)
                            {
                                errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '||' expected a left operand of type 'Boolean', but got '{typeLeft}'.");
                            }
                            if (typeRight != BoolT.Instance)
                            {

                                errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '||' expected a right operand of type 'Boolean', but got '{typeRight}'.");
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

                        case BinaryOperators.SUB:
                            if (typeLeft == IntT.Instance)
                            {
                                return IntT.Instance;
                            }
                            else
                            {
                                return FloatT.Instance;
                            }

                        case BinaryOperators.MUL:
                            if (typeLeft == IntT.Instance)
                            {
                                return IntT.Instance;
                            }
                            else
                            {
                                return FloatT.Instance;
                            }

                        case BinaryOperators.DIV:
                            return FloatT.Instance;

                        case BinaryOperators.LT:
                            return BoolT.Instance;

                        case BinaryOperators.EQ:
                            return BoolT.Instance;

                        case BinaryOperators.NEQ:
                            return BoolT.Instance;

                        case BinaryOperators.AND:
                            return BoolT.Instance;

                        case BinaryOperators.OR:
                            return BoolT.Instance;


                        default: throw new Exception("Invalid binary operation");
                    }

                case Ref r:
                    if (!env.ContainsKey(r.Name))
                    {
                        errors.Add($"Line {r.LineNumber}: variable {r.Name} is not declared.");
                        return IntT.Instance;
                    }
                    return env[r.Name];

                default: throw new Exception("Invalid expression");
            }
        }
    }
}