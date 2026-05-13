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
                    if (envST.TryGet(schemaDeclaration.Identifier) != null)
                    {
                        errors.Add($"Line {schemaDeclaration.LineNumber}: Schema '{schemaDeclaration.Identifier}' is already declared.");
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
                    EnvVT localScope = envVT.NewFunctionScope(f.Type);

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

                    if (!localScope.HasReturn)
                    {
                        errors.Add($"Line {f.LineNumber}: Not all paths return a value in function '{f.Identifier}'.");
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

            case Comp comp:
                StmtT(comp.Stmt1, envVT, envPT, envST);
                StmtT(comp.Stmt2, envVT, envPT, envST);
                break;

            case If ifStmt:

                EnvVT thenScope;
                EnvVT elseScope;

                if (envVT.FunctionReturnType != null)
                {
                    thenScope = envVT.NewFunctionScope(envVT.FunctionReturnType);
                    elseScope = envVT.NewFunctionScope(envVT.FunctionReturnType);
                }
                else
                {
                    thenScope = envVT.NewScope();
                    elseScope = envVT.NewScope();
                }

                if (ifStmt.Condition != null)
                {
                    Type? condT = ExprT(ifStmt.Condition, envVT, envPT, envST);

                    if (condT != BoolT.Instance)
                    {
                        errors.Add($"Line {ifStmt.LineNumber}: If statement requires a condition with type 'bool', but got '{condT}'.");
                    }
                }
                else
                {
                    errors.Add($"Line {ifStmt.LineNumber}: If statement requires a condition.");
                }

                // then & else branch
                if (ifStmt.ThenBody != null)
                {
                    StmtT(ifStmt.ThenBody, thenScope, envPT, envST);
                }

                if (ifStmt.ElseBody != null)
                {
                    StmtT(ifStmt.ElseBody, elseScope, envPT, envST);
                }

                if (envVT.FunctionReturnType != null)
                {
                    if (thenScope.HasReturn && elseScope.HasReturn)
                    {
                        envVT.HasReturn = true;
                    }
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
                    break;
                }

                Type? expectedType = envVT.TryGet(assign.Identifier);
                Type? actualType = ExprT(assign.Value, envVT, envPT, envST);

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

                Type? declarationExprType = ExprT(declaration.Expression, envVT, envPT, envST);

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

            case Return r:
                if (r.Value == null)
                {
                    errors.Add($"Line {r.LineNumber}: 'return' needs a value.");
                    break;
                }
                Type? currentType = ExprT(r.Value, envVT, envPT, envST);
                Type? functionReturnType = envVT.FunctionReturnType;

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

                    envVT.HasReturn = true;
                }
                else
                {
                    errors.Add($"Line {r.LineNumber}: Return outside of a function is not allowed.");
                }
                break;

            default: throw new Exception("Invalid statement");
        }
    }

    private Type? ExprT(Expr expr, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        switch (expr)
        {
            case IntV: return IntT.Instance;

            case FloatV: return FloatT.Instance;

            case BoolV: return BoolT.Instance;

            case StringV: return StringT.Instance;

            case BinaryOp binaryOp:
                Type? typeLeft = ExprT(binaryOp.ExprLeft, envVT, envPT, envST);
                Type? typeRight = ExprT(binaryOp.ExprRight, envVT, envPT, envST);

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
                    Type? innertype = ExprT(unaryOp.Expr, envVT, envPT, envST);

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

            case Filter filter:
                Type? filterTableExprType = ExprT(filter.TableExpr, envVT, envPT, envST);

                if (filterTableExprType is not TableT)
                {
                    errors.Add($"Line {filter.LineNumber}: Argument 1 must be of type 'TableT'.");
                    return null;
                }

                TableT filterTable = (TableT)filterTableExprType;
                List<Column> filterTableSchema = envST.TryGet(filterTable.SchemaId)!;
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

                return (TableT)filterTableExprType;

            case Sum sum:
                Type? sumTableExprType = ExprT(sum.TableExpr, envVT, envPT, envST);

                if (sumTableExprType is not TableT)
                {
                    errors.Add($"Line {sum.LineNumber}: Argument 1 must be of type 'TableT'.");
                    return null;
                }

                TableT sumExprTable = (TableT)sumTableExprType;

                List<Column>? sumResultSchema = envST.TryGet(sum.ResultSchemaId);

                if (sumResultSchema == null)
                {
                    errors.Add($"Line {sum.LineNumber}: Result schema '{sum.ResultSchemaId}' has not been defined.");
                    return null;
                }

                if (sumResultSchema.Count != 2)
                {
                    errors.Add($"Line {sum.LineNumber}: Result schema '{sum.ResultSchemaId}' may only contain two columns but has {sumResultSchema.Count} columns.");
                    return null;
                }

                List<Column> sumTableExprSchema = envST.TryGet(sumExprTable.SchemaId)!;

                if (!sumTableExprSchema.Contains(sumResultSchema[0]))
                {
                    errors.Add($"Line {sum.LineNumber}: The column '{sumResultSchema[0].Id}' does not exist in schema '{sumExprTable.SchemaId}'.");
                    return null;
                }

                if (!sumTableExprSchema.Contains(sumResultSchema[1]))
                {
                    errors.Add($"Line {sum.LineNumber}: The column '{sumResultSchema[1].Id}' does not exist in schema '{sumExprTable.SchemaId}'.");
                    return null;
                }

                Column sumColumn;

                if (sumResultSchema[0].Id == sum.SumColumn)
                {
                    sumColumn = sumResultSchema[0];
                }
                else
                {
                    sumColumn = sumResultSchema[1];

                }

                if (sumColumn.Type != IntT.Instance && sumColumn.Type != FloatT.Instance)
                {
                    errors.Add($"Line {sum.LineNumber}: The column '{sumColumn.Id}' must be of type 'IntT' or 'FloatT', but got '{sumColumn.Type}'.");
                    return null;
                }

                return new TableT(sum.ResultSchemaId);

            case Join join:
                Type? joinOnTableType = ExprT(join.JoinOnTableExpr, envVT, envPT, envST);
                Type? joinFromTableType = ExprT(join.JoinFromTableExpr, envVT, envPT, envST);

                if (joinOnTableType is not TableT)
                {
                    errors.Add($"Line {join.LineNumber}: Argument 1 must be of type 'TableT'.");
                    return null;
                }

                if (joinFromTableType is not TableT)
                {
                    errors.Add($"Line {join.LineNumber}: Argument 2 must be of type 'TableT'.");
                    return null;
                }

                TableT joinOnTable = (TableT)joinOnTableType;
                TableT joinFromTable = (TableT)joinFromTableType;

                List<Column>? joinResultSchema = envST.TryGet(join.ResultSchemaId);
                List<Column> joinOnTableSchema = envST.TryGet(joinOnTable.SchemaId)!;
                List<Column> joinFromTableSchema = envST.TryGet(joinFromTable.SchemaId)!;

                if (joinResultSchema == null)
                {
                    errors.Add($"Line {join.LineNumber}: Result schema '{join.ResultSchemaId}' has not been defined.");
                    return null;
                }

                if (joinResultSchema.Count != joinOnTableSchema.Count + joinFromTableSchema.Count - 1)
                {
                    errors.Add($"Line {join.LineNumber}: Result schema '{join.ResultSchemaId}' may only contain {joinOnTableSchema.Count + joinFromTableSchema.Count - 1} columns but has {joinResultSchema.Count} columns.");
                    return null;
                }

                Column? joinOnReferenceColumn = null;
                Column? joinFromReferenceColumn = null;

                foreach (Column col in joinOnTableSchema)
                {
                    if (col.Id == join.KeyColumn1)
                    {
                        joinOnReferenceColumn = col;
                    }
                }

                foreach (Column col in joinFromTableSchema)
                {
                    if (col.Id == join.KeyColumn2)
                    {
                        joinFromReferenceColumn = col;
                    }
                }

                if (joinOnReferenceColumn == null)
                {
                    errors.Add($"Line {join.LineNumber}: Join schema '{joinOnTable.SchemaId}' must contain '{join.KeyColumn1}'.");
                    return null;
                }

                if (joinFromReferenceColumn == null)
                {
                    errors.Add($"Line {join.LineNumber}: Join schema '{joinFromTable.SchemaId}' must contain '{join.KeyColumn2}'.");
                    return null;
                }

                if (joinOnReferenceColumn.Type == IntT.Instance || joinOnReferenceColumn.Type == FloatT.Instance)
                {
                    if (joinFromReferenceColumn.Type != IntT.Instance && joinFromReferenceColumn.Type != FloatT.Instance)
                    {
                        errors.Add($"Line {join.LineNumber}: Schemas have uncompatible column types '{joinOnReferenceColumn.Type}' and '{joinFromReferenceColumn.Type}'.");
                        return null;
                    }
                }
                else if (joinOnReferenceColumn.Type != joinFromReferenceColumn.Type)
                {
                    errors.Add($"Line {join.LineNumber}: Schemas have uncompatible column types '{joinOnReferenceColumn.Type}' and '{joinFromReferenceColumn.Type}'.");
                    return null;
                }

                if (joinResultSchema.Contains(joinFromReferenceColumn) && !joinOnTableSchema.Contains(joinFromReferenceColumn))
                {
                    errors.Add($"Line {join.LineNumber}: Result schema '{join.ResultSchemaId}' may not contain column with id '{join.KeyColumn2}'.");
                    return null;
                }

                foreach (Column col in joinResultSchema)
                {
                    if (!joinOnTableSchema.Contains(col) && !joinFromTableSchema.Contains(col))
                    {
                        errors.Add($"Line {join.LineNumber}: Result schema '{join.ResultSchemaId}' may not contain a column '{col.Id}' that does not exist in schema '{joinOnTable.SchemaId}' or '{joinFromTable.SchemaId}'.");
                        return null;
                    }
                }

                return new TableT(join.ResultSchemaId);

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

                FunctionType funcType = envPT.TryGet(functionRef.Name)!;
                int parameterCount = funcType.Parameters.Count;

                if (functionRef.Arguments.Count != parameterCount)
                {
                    errors.Add($"Line {functionRef.LineNumber}: Function {functionRef.Name} argument count mismatch.");
                    return null;
                }

                //check type on param
                for (int i = 0; i < parameterCount; i++)
                {
                    Type? argType = ExprT(functionRef.Arguments[i], envVT, envPT, envST);
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