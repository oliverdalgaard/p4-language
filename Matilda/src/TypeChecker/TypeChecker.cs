namespace Matilda;

public class TypeChecker
{
    public List<string> errors { get; }

    public bool HasErrors()
    {
        return errors.Count > 0;
    }

    public TypeChecker(Program program, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        errors = new List<string>();

        envVT.Bind("return", null);
        TopLevelDeclarationT(program.TopLevelDeclarations, envVT, envPT, envST);
        StmtT(program.Stmt, envVT, envPT, envST);
    }

    private void TopLevelDeclarationT(List<TopLevelDeclaration> topLevelDeclarations, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        foreach (TopLevelDeclaration topLevelDeclaration in topLevelDeclarations)
        {
            switch (topLevelDeclaration)
            {
                case SchemaDeclaration schemaDeclaration:

                    if (envVT.TryGet("return") != null)
                    {
                        errors.Add($"Line {schemaDeclaration.LineNumber}: Schemas can only be declared in the global scope.");
                        break;
                    }

                    if (envST.TryGet(schemaDeclaration.Identifier) != null)
                    {
                        errors.Add($"Line {schemaDeclaration.LineNumber}: Varibale '{schemaDeclaration.Identifier}' is already declared.");
                        break;
                    }

                    List<Column> columns = schemaDeclaration.Columns;

                    if (CompareSchema.ContainsDuplicate(columns))
                    {
                        errors.Add($"Line {schemaDeclaration.LineNumber}: Schema '{schemaDeclaration.Identifier}' may not contain duplicate identifiers.");
                        break;
                    }

                    foreach (Column column in columns)
                    {
                        if (column.Type != IntT.Instance &&
                            column.Type != StringT.Instance &&
                            column.Type != BoolT.Instance &&
                            column.Type != FloatT.Instance)
                        {
                            errors.Add($"Line {schemaDeclaration.LineNumber}: Schema declaration requires type of either 'int', 'string', 'bool', or 'float' but got '{column.Type}'");
                            break;
                        }
                    }

                    envST.Bind(schemaDeclaration.Identifier, columns);
                    break;

                case FunctionDeclaration f:

                    if (envVT.TryGet("return") != null)
                    {
                        errors.Add($"Line {f.LineNumber}: Functions can only be declared in the global scope.");
                    }

                    if (f.Identifier == null || f.Type == null)
                    {
                        errors.Add($"Line {f.LineNumber}: Invalid declaration.");
                        break;
                    }

                    if (envPT.TryGet(f.Identifier) != null)
                    {
                        errors.Add($"Line {f.LineNumber}: Function '{f.Identifier}' already declared.");
                        break;
                    }

                    // Register function
                    envPT.Bind(f.Identifier, new FunctionType(f));

                    // New local scope
                    EnvVT localScope = envVT.NewScope();
                    localScope.Bind("return", f.Type);

                    // Param 
                    foreach (Parameter param in f.Parameters)
                    {
                        if (localScope.TryGetLocal(param.Identifier) != null)
                        {
                            errors.Add($"Line {param.LineNumber}: Duplicate parameter '{param.Identifier}'.");
                        }
                        else
                        {
                            localScope.Bind(param.Identifier, param.Type);
                        }
                    }

                    StmtT(f.Body, localScope, envPT, envST);

                    if (localScope.TryGet("hasReturn") == null)
                    {
                        errors.Add($"Line {f.LineNumber}: Missing return in function {f.Identifier}.");
                    }
                    break;
            }
        }
    }

    private void StmtT(Stmt stmt, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        switch (stmt)
        {
            case Skip:
                break;

            case Print print:
                ExprT(print.Value, envVT, envPT, envST);
                break;

            case Comp comp:
                StmtT(comp.Stmt1, envVT, envPT, envST);
                StmtT(comp.Stmt2, envVT, envPT, envST);
                break;

            case If ifStmt:
                if (ifStmt.Condition != null)
                {
                    Type condT = ExprT(ifStmt.Condition, envVT, envPT, envST);

                    if (condT != BoolT.Instance)
                    {
                        errors.Add($"Line {ifStmt.LineNumber}: If statement requires a condition with type 'bool', but got '{condT}'.");
                    }
                }
                else
                {
                    errors.Add($"Line {ifStmt.LineNumber}: If statement requires a condition.");
                }

                // then, elseif, else branch
                if (ifStmt.ThenBody != null)
                {
                    StmtT(ifStmt.ThenBody, envVT, envPT, envST);
                }

                if (ifStmt.ElseIfStmts != null)
                {
                    foreach (If elseif in ifStmt.ElseIfStmts)
                    {
                        StmtT(elseif, envVT, envPT, envST);
                    }
                }

                if (ifStmt.ElseBody != null)
                {
                    StmtT(ifStmt.ElseBody, envVT, envPT, envST);
                }

                break;

            case Assign assign:
                // Check for null 
                if (assign.Identifier == null || assign.Value == null)
                {
                    errors.Add($"Line {assign.LineNumber}: Invalid assignment");
                    break;
                }

                // Check delclaration 
                if (envVT.TryGet(assign.Identifier) == null)
                {
                    errors.Add($"Line {assign.LineNumber}: Variable {assign.Identifier} is not declared.");
                }

                if (envVT.TryGetLocal(assign.Identifier) == null)
                {
                    errors.Add($"Line {assign.LineNumber}: Cannot reassign global varibale '{assign.Identifier}'.");
                    break;
                }


                Type expectedType = envVT.TryGet(assign.Identifier);
                Type actualType = ExprT(assign.Value, envVT, envPT, envST);

                // Check table type (unique)
                if (expectedType is TableT expectedTableType && actualType is TableT actualTableType)
                {
                    if (!CompareSchema.Compare(envST.TryGet(expectedTableType.SchemaId), envST.TryGet(actualTableType.SchemaId)))
                    {
                        errors.Add($"Line {assign.LineNumber}: Cannot assign table with schema '{actualTableType.SchemaId}' to table '{assign.Identifier}' with schema '{expectedTableType.SchemaId}'.");
                        break;
                    }
                    break;
                }

                // Check type match 
                if (expectedType != actualType)
                {
                    errors.Add($"Line {assign.LineNumber}: Cannot assign '{actualType}' to variable '{assign.Identifier}' of type '{expectedType}'.");
                    break;
                }

                break;

            case LocalDeclaration declaration:
                if (declaration.Identifier == null || declaration.Type == null)
                {
                    errors.Add($"Line {declaration.LineNumber}: Invalid declaration.");
                    break;
                }

                if (envVT.TryGetLocal(declaration.Identifier) != null)
                {
                    errors.Add($"Line {declaration.LineNumber}: Variable '{declaration.Identifier}' is already declared.");
                    break;
                }

                Type declarationExprType = ExprT(declaration.Expression, envVT, envPT, envST);

                if (declaration.Type is TableT declarationTableType)
                {
                    if (declarationExprType is not TableT)
                    {
                        errors.Add($"Line {declaration.LineNumber}: Declaration type does not match the type of the expression.");
                        break;
                    }

                    if (!CompareSchema.Compare(envST.TryGet(declarationTableType.SchemaId), envST.TryGet(((TableT)declarationExprType).SchemaId)))
                    {
                        errors.Add($"Line {declaration.LineNumber}: Declaration schema does not match the schema of the expression.");
                        break;
                    }
                }
                else if (declaration.Type != declarationExprType)
                {
                    errors.Add($"Line {declaration.LineNumber}: Declaration type does not match the type of the expression.");
                    break;
                }

                envVT.Bind(declaration.Identifier, declaration.Type);
                break;

            case TableDeclaration tableDeclaration:
                if (tableDeclaration.Identifier == null || tableDeclaration.Type is not TableT tableDeclarationType)
                {
                    errors.Add($"Line {tableDeclaration.LineNumber}: Invalid table declaration.");
                    break;
                }

                if (envST.TryGet(tableDeclarationType.SchemaId) == null)
                {
                    errors.Add($"Line {tableDeclaration.LineNumber}: Schema with identifier '{tableDeclarationType.SchemaId}' is not declared.");
                    break;
                }

                if (envVT.TryGetLocal(tableDeclaration.Identifier) != null)
                {
                    errors.Add($"Line {tableDeclaration.LineNumber}: Table '{tableDeclaration.Identifier}' is already declared.");
                    break;
                }

                envVT.Bind(tableDeclaration.Identifier, new TableT(tableDeclarationType.SchemaId));
                break;

            case While whileStmt:
                if (whileStmt.Condition == null)
                {
                    errors.Add($"Line {whileStmt.LineNumber}: While statement needs a valid condition.");
                }
                else
                {
                    Type condT = ExprT(whileStmt.Condition, envVT, envPT, envST);

                    if (condT != BoolT.Instance)
                    {
                        errors.Add($"Line {whileStmt.LineNumber}: While statement requires a condition with type 'bool', but got '{condT}'.");
                    }
                }

                if (whileStmt.Body != null)
                {
                    StmtT(whileStmt.Body, envVT, envPT, envST);
                }
                break;

            case Return r:
                if (r.Value == null)
                {
                    errors.Add($"Line {r.LineNumber}: 'return' needs a value.");
                    break;
                }
                Type currentType = ExprT(r.Value, envVT, envPT, envST);
                Type functionReturnType = envVT.TryGet("return");

                if (functionReturnType != null)
                {
                    // Inside function
                    if (currentType is TableT currentTableType && functionReturnType is TableT functionReturnTableType)
                    {
                        if (!CompareSchema.Compare(envST.TryGet(currentTableType.SchemaId), envST.TryGet(functionReturnTableType.SchemaId)))
                        {
                            errors.Add($"Line {r.LineNumber}: Return type schema '{currentTableType.SchemaId}' does not match function return type schema '{functionReturnTableType.SchemaId}'.");
                        }
                    }
                    else if (currentType != functionReturnType)
                    {
                        errors.Add($"Line {r.LineNumber}: Return type '{currentType}' does not match function return type '{functionReturnType}'.");
                    }

                    if (envVT.TryGet("hasReturn") == null)
                    {
                        envVT.Bind("hasReturn", BoolT.Instance);
                    }
                }
                else
                {
                    errors.Add($"Line {r.LineNumber}: Return outside of a function is not allowed.");
                }
                break;

            default: throw new Exception("Invalid statement");
        }
    }

    private Type ExprT(Expr expr, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        switch (expr)
        {
            case IntV: return IntT.Instance;

            case FloatV: return FloatT.Instance;

            case BoolV: return BoolT.Instance;

            case StringV: return StringT.Instance;

            case BinaryOp binaryOp:
                Type typeLeft = ExprT(binaryOp.ExprLeft, envVT, envPT, envST);
                Type typeRight = ExprT(binaryOp.ExprRight, envVT, envPT, envST);

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

                        // Return
                        if (typeLeft == IntT.Instance && typeRight == IntT.Instance)
                        {
                            return IntT.Instance;
                        }
                        else
                        {
                            return FloatT.Instance;
                        }

                    case BinaryOperators.SUB:
                        if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '-' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '-' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                        }

                        // Return
                        if (typeLeft == IntT.Instance && typeRight == IntT.Instance)
                        {
                            return IntT.Instance;
                        }
                        else
                        {
                            return FloatT.Instance;
                        }

                    case BinaryOperators.MUL:
                        if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '*' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '*' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                        }

                        // Return
                        if (typeLeft == IntT.Instance && typeRight == IntT.Instance)
                        {
                            return IntT.Instance;
                        }
                        else
                        {
                            return FloatT.Instance;
                        }

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

                        // Return
                        return FloatT.Instance;

                    case BinaryOperators.LT:
                        if (typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '<' expected a left operand of type 'int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '<' expected a right operand of type 'int' or 'float', but got '{typeRight}'.");
                        }

                        // Return
                        return BoolT.Instance;

                    case BinaryOperators.EQ:
                        if (typeLeft != BoolT.Instance && typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '==' expected a left operand of type 'bool','int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != BoolT.Instance && typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '==' expected a right operand of type 'bool','int' or 'float', but got '{typeRight}'.");
                        }

                        if (typeRight == BoolT.Instance && typeLeft != BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '==' expected a right and left operand of type 'bool', but got '{typeLeft}'.");
                        }

                        if (typeRight != BoolT.Instance && typeLeft == BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '==' expected a right and left operand of type 'bool', but got '{typeRight}'.");
                        }

                        // Return
                        return BoolT.Instance;

                    case BinaryOperators.NEQ:
                        if (typeLeft != BoolT.Instance && typeLeft != IntT.Instance && typeLeft != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '!=' expected a left operand of type 'bool','int' or 'float', but got '{typeLeft}'.");
                        }

                        if (typeRight != BoolT.Instance && typeRight != IntT.Instance && typeRight != FloatT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '!=' expected a right operand of type 'bool','int' or 'float', but got '{typeRight}'.");
                        }

                        if (typeRight == BoolT.Instance && typeLeft != BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '!=' expected a right and left operand of type 'bool', but got '{typeLeft}'.");
                        }

                        if (typeRight != BoolT.Instance && typeLeft == BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '!=' expected a right and left operand of type 'bool', but got '{typeRight}'.");
                        }

                        // Return
                        return BoolT.Instance;

                    case BinaryOperators.AND:
                        if (typeLeft != BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '&&' expected a left operand of type 'bool', but got '{typeLeft}'.");
                        }
                        if (typeRight != BoolT.Instance)
                        {

                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '&&' expected a right operand of type 'bool', but got '{typeRight}'.");
                        }

                        // Return
                        return BoolT.Instance;

                    case BinaryOperators.OR:
                        if (typeLeft != BoolT.Instance)
                        {
                            errors.Add($"Line {binaryOp.ExprLeft.LineNumber}: Operator '||' expected a left operand of type 'bool', but got '{typeLeft}'.");
                        }
                        if (typeRight != BoolT.Instance)
                        {

                            errors.Add($"Line {binaryOp.ExprRight.LineNumber}: Operator '||' expected a right operand of type 'bool', but got '{typeRight}'.");
                        }

                        // Return
                        return BoolT.Instance;

                    default: throw new Exception("Invalid binary operation");
                }

            case UnaryOp unaryOp:
                {
                    Type innertype = ExprT(unaryOp.Expr, envVT, envPT, envST);

                    switch (unaryOp.Op)
                    {
                        case UnaryOperators.NOT:
                            if (innertype != BoolT.Instance)
                            {
                                errors.Add($"Line {unaryOp.LineNumber}: Operator '!' expected a operand of type 'bool', but got '{innertype}'.");
                            }

                            // Return
                            return BoolT.Instance;

                        default:
                            throw new Exception("Unknown unary operator");
                    }
                }

            case FilterExpr filter:
                Type tableExprType = ExprT(filter.TableExpr, envVT, envPT, envST);

                if (tableExprType is not TableT)
                {
                    errors.Add($"Line {filter.LineNumber}: Argument 1 must be of type 'TableT'.");
                    return null;
                }

                TableT filterTable = (TableT)tableExprType;
                List<Column> filterTableSchema = envST.TryGet(filterTable.SchemaId);
                EnvVT rowEnv = envVT.NewScope();

                foreach (Column col in filterTableSchema)
                {
                    rowEnv.Bind(col.Id, col.Type);
                }

                if (ExprT(filter.Predicate, rowEnv, envPT, envST) != BoolT.Instance)
                {
                    errors.Add($"Line {filter.LineNumber}: Argument 2 must be of type 'BoolT'.");
                    return null;
                }

                return (TableT)tableExprType;

            case Ref r:
                if (envVT.TryGet(r.Name) == null)
                {
                    errors.Add($"Line {r.LineNumber}: Variable {r.Name} is not declared.");
                    return null;
                }
                return envVT.TryGet(r.Name);


            case FunctionRef functionRef:
                if (envPT.TryGet(functionRef.Name) == null)
                {
                    errors.Add($"Line {functionRef.LineNumber}: Function {functionRef.Name} is not declared.");
                    return null;
                }

                FunctionType funcType = envPT.TryGet(functionRef.Name);
                int parameterCount = funcType.Parameters.Count;

                if (functionRef.Arguments.Count != parameterCount)
                {
                    errors.Add($"Line {functionRef.LineNumber}: Function {functionRef.Name} argument count mismatch.");
                    return null;
                }

                //check type on param
                for (int i = 0; i < parameterCount; i++)
                {
                    Type argType = ExprT(functionRef.Arguments[i], envVT, envPT, envST);
                    Type paramType = funcType.Parameters[i];

                    if (argType is TableT currentTableType && paramType is TableT functionReturnTableType)
                    {
                        if (!CompareSchema.Compare(envST.TryGet(currentTableType.SchemaId), envST.TryGet(functionReturnTableType.SchemaId)))
                        {
                            errors.Add($"Line {functionRef.Arguments[i].LineNumber}: Function '{functionRef.Name}' expect parameter {i + 1} to have table with schema '{currentTableType.SchemaId}' but got '{functionReturnTableType.SchemaId}'.");
                        }
                    }
                    else if (argType != paramType)
                    {
                        errors.Add($"Line {functionRef.Arguments[i].LineNumber}: Function '{functionRef.Name}' expect parameter {i + 1} to have type '{paramType}' but got '{argType}'.");

                    }
                }
                return funcType.ReturnType;

            default: throw new Exception("Invalid expression");
        }
    }
}