using Microsoft.VisualBasic.FileIO;

namespace Matilda;

public static class Interpreter
{
    public static void EvalStmt(Stmt stmt, EnvV envV, EnvP envP, EnvS envS)
    {
        switch (stmt)
        {
            case Skip:
                Console.WriteLine("Skipped");
                break;

            case Comp comp:
                EvalStmt(comp.Stmt1, envV, envP, envS);
                if (envV.TryGet("return") == null)
                {
                    EvalStmt(comp.Stmt2, envV, envP, envS);
                }
                break;

            case Print print:
                Val value = EvalExpr(print.Value, envV, envP, envS);
                Console.WriteLine(value.ToString());
                break;

            case Parameter parameter:
                envV.Bind(parameter.Identifier, null);
                break;

            case Declaration declaration:
                envV.Bind(declaration.Identifier, EvalExpr(declaration.Expression, envV, envP, envS));
                break;

            case Assign assign:
                envV.Set(assign.Identifier, EvalExpr(assign.Value, envV, envP, envS));
                break;

            case SchemaDeclaration schemaDeclaration:
                envS.Bind(schemaDeclaration.Identifier, schemaDeclaration.Columns);
                break;

            case TableDeclaration tableDeclaration:
                Val expr = EvalExpr(tableDeclaration.Expr, envV, envP, envS);
                if (expr is RowVal)
                {
                    Table table = new Table(tableDeclaration.Identifier, envS.TryGet(tableDeclaration.SchemaId), expr.AsRow());

                    table.ParseTypes();
                    TableVal parsedTable = new TableVal(table);
                    envV.Bind(tableDeclaration.Identifier, parsedTable);
                }
                else
                {
                    envV.Bind(tableDeclaration.Identifier, expr);
                }
                break;

            case FunctionDeclaration functionDeclaration:
                envP.Bind(functionDeclaration);
                break;

            case Return returnVal:
                envV.Bind("return", EvalExpr(returnVal.Value, envV, envP, envS));
                break;

            case If ifStmt:
                EnvV ifLocalScope = envV.NewScope();

                bool runElse = true;

                Val condition = EvalExpr(ifStmt.Condition, ifLocalScope, envP, envS);
                if (condition.AsBool())
                {
                    runElse = false;
                    EvalStmt(ifStmt.ThenBody, ifLocalScope, envP, envS);
                }
                else if (ifStmt.ElseIfStmts.Any())
                {
                    foreach (If elseIfStmt in ifStmt.ElseIfStmts)
                    {
                        Val elseIfStmtCondition = EvalExpr(elseIfStmt.Condition, ifLocalScope, envP, envS);
                        if (elseIfStmtCondition.AsBool())
                        {
                            EvalStmt(elseIfStmt.ThenBody, ifLocalScope, envP, envS);
                            runElse = false;
                            break;
                        }
                    }
                }

                if (runElse)
                {
                    EvalStmt(ifStmt.ElseBody, ifLocalScope, envP, envS);
                }

                if (ifLocalScope.TryGet("return") != null)
                {
                    envV.Bind("return", ifLocalScope.TryGet("return"));
                }

                break;

            case While whileStmt:
                {
                    while (EvalExpr(whileStmt.Condition, envV, envP, envS).AsBool())
                    {
                        EvalStmt(whileStmt.Body, envV, envP, envS);
                    }
                    break;
                }

            default:
                throw new Exception("Not valid statement");

        }
    }

    public static Val EvalExpr(Expr expr, EnvV envV, EnvP envP, EnvS envS)
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

            case Read read:
                List<string[]> rows = new List<string[]>();
                // Open file with filename "" removed
                using (TextFieldParser textFieldParser = new TextFieldParser(read.FilePath))
                {
                    textFieldParser.TextFieldType = FieldType.Delimited;
                    textFieldParser.SetDelimiters(",");
                    while (!textFieldParser.EndOfData)
                    {
                        rows.Add(textFieldParser.ReadFields());
                    }
                }
                return new RowVal(rows);

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
                    Val value = EvalExpr(functionRef.Arguments[i], envV, envP, envS);

                    localScope.Bind(parameterName, value);
                }

                foreach (Stmt stmt in function.Body)
                {
                    EvalStmt(stmt, localScope, envP, envS);
                    if (localScope.TryGet("return") != null)
                    {
                        break;
                    }
                }


                return localScope.TryGet("return");

            case FilterExpr filterExpr:
                {
                    Val tableValue = EvalExpr(filterExpr.TableExpr, envV, envP, envS);
                    Table inputTable = tableValue.AsTable();

                    List<string[]> filteredFile = new List<string[]>();

                    // Add header row
                    filteredFile.Add(inputTable.File[0]);

                    for (int rowIndex = 0; rowIndex < inputTable.Records.Count; rowIndex++)
                    {
                        TableRecord record = inputTable.Records[rowIndex];

                        EnvV rowScope = envV.NewScope();

                        for (int colIndex = 0; colIndex < inputTable.Headers.Count; colIndex++)
                        {
                            string columnName = inputTable.Headers[colIndex].Identifier;
                            Val columnValue = record.Values[colIndex];

                            rowScope.Bind(columnName, columnValue);
                        }

                        Val predicateResult = EvalExpr(filterExpr.Predicate, rowScope, envP, envS);

                        if (predicateResult.AsBool())
                        {
                            filteredFile.Add(inputTable.File[rowIndex + 1]);
                        }
                    }

                    Table resultTable = new Table(inputTable.Identifier, inputTable.Schema, filteredFile);
                    resultTable.ParseTypes();

                    return new TableVal(resultTable);
                }

            case BinaryOp binaryOp:
                Val v1 = EvalExpr(binaryOp.ExprLeft, envV, envP, envS);
                Val v2 = EvalExpr(binaryOp.ExprRight, envV, envP, envS);

                switch (binaryOp.Op)
                {
                    case BinaryOperators.OR:
                        return new BoolVal(v1.AsBool() || v2.AsBool());

                    case BinaryOperators.AND:
                        return new BoolVal(v1.AsBool() && v2.AsBool());

                    case BinaryOperators.EQ:
                        return new BoolVal(InterpreterHelperFunction.IsEqual(v1, v2));

                    case BinaryOperators.NEQ:
                        return new BoolVal(!InterpreterHelperFunction.IsEqual(v1, v2));

                    case BinaryOperators.LT:
                        return new BoolVal(InterpreterHelperFunction.HelperFunctionLT(v1, v2));

                    case BinaryOperators.ADD:
                        return InterpreterHelperFunction.HelperFunctionADD(v1, v2);

                    case BinaryOperators.SUB:
                        return InterpreterHelperFunction.HelperFunctionSUB(v1, v2);

                    case BinaryOperators.MUL:
                        return InterpreterHelperFunction.HelperFunctionMUL(v1, v2);

                    case BinaryOperators.DIV:
                        return InterpreterHelperFunction.HelperFunctionDIV(v1, v2);

                    default: throw new Exception("Not a valid binaryOp expression");
                }

            case UnaryOp unaryOp:
                Val val = EvalExpr(unaryOp.Expr, envV, envP, envS);

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