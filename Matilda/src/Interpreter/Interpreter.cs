using Microsoft.VisualBasic.FileIO;

namespace Matilda;

public static class Interpreter
{
    public static void EvalTopLevelDeclarations(List<TopLevelDeclaration> topLevelDeclarations, EnvP envP, EnvS envS)
    {
        foreach (TopLevelDeclaration topLevelDeclaration in topLevelDeclarations)
        {
            switch (topLevelDeclaration)
            {
                case SchemaDeclaration schemaDeclaration:
                    envS.Bind(schemaDeclaration.Identifier, schemaDeclaration.Columns);
                    break;

                case FunctionDeclaration functionDeclaration:
                    envP.Bind(functionDeclaration);
                    break;

                default:
                    throw new Exception("Not a valid TopLevelDeclaration");
            }
        }
    }

    public static void EvalStmt(Stmt stmt, EnvV envV, EnvP envP, EnvS envS)
    {
        switch (stmt)
        {
            case Skip:
                break;

            case Comp comp:
                EvalStmt(comp.Stmt1, envV, envP, envS);
                if (envV.FunctionReturnValue == null)
                {
                    EvalStmt(comp.Stmt2, envV, envP, envS);
                }
                break;

            case Parameter parameter:
                envV.Bind(parameter.Identifier, null);
                break;

            case LocalDeclaration declaration:
                envV.Bind(declaration.Identifier, EvalExpr(declaration.Expression, envV, envP, envS));
                break;

            case Assign assign:
                envV.Set(assign.Identifier, EvalExpr(assign.Value, envV, envP, envS));
                break;

            case TableDeclaration tableDeclaration:
                List<string[]> rows = new List<string[]>();
                // Open file with filename "" removed
                using (TextFieldParser textFieldParser = new TextFieldParser(tableDeclaration.FilePath))
                {
                    textFieldParser.TextFieldType = FieldType.Delimited;
                    textFieldParser.SetDelimiters(",");
                    while (!textFieldParser.EndOfData)
                    {
                        rows.Add(textFieldParser.ReadFields());
                    }
                }

                Table table = new Table(tableDeclaration.Identifier, envS.TryGet(((TableT)tableDeclaration.Type).SchemaId), rows);
                table.ParseTypes();

                TableVal parsedTable = new TableVal(table);
                envV.Bind(tableDeclaration.Identifier, parsedTable);

                break;

            case Return returnVal:
                envV.FunctionReturnValue = EvalExpr(returnVal.Value, envV, envP, envS);
                break;

            case If ifStmt:
                EnvV thenScope = envV.NewScope(envV.IsFunctionScope);
                EnvV elseScope = envV.NewScope(envV.IsFunctionScope);

                Val condition = EvalExpr(ifStmt.Condition, envV, envP, envS);
                if (condition.AsBool())
                {
                    EvalStmt(ifStmt.ThenBody, thenScope, envP, envS);
                    if (envV.IsFunctionScope)
                    {
                        envV.FunctionReturnValue = thenScope.FunctionReturnValue;
                    }
                }
                else
                {
                    EvalStmt(ifStmt.ElseBody, elseScope, envP, envS);
                    if (envV.IsFunctionScope)
                    {
                        envV.FunctionReturnValue = elseScope.FunctionReturnValue;
                    }
                }

                break;

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

            case FunctionRef functionRef:
                FunctionDeclaration function = envP.TryGet(functionRef.Name);

                if (functionRef.Arguments.Count != function.Parameters.Count)
                {
                    throw new Exception("Number of arguments do not match the amount of parameters.");
                }

                EnvV localScope = envV.NewScope(true);

                for (int i = 0; i < functionRef.Arguments.Count; i++)
                {
                    string parameterName = function.Parameters[i].Identifier;
                    Val value = EvalExpr(functionRef.Arguments[i], envV, envP, envS);

                    if (value is TableVal tVal)
                    {
                        localScope.Bind(parameterName, new TableVal(tVal.AsTable().Clone()));
                    }
                    else
                    {
                        localScope.Bind(parameterName, value);
                    }
                }

                EvalStmt(function.Body, localScope, envP, envS);

                return localScope.FunctionReturnValue;

            case Filter filter:
                {
                    Val tableValue = EvalExpr(filter.TableExpr, envV, envP, envS);
                    Table inputTable = tableValue.AsTable();

                    Table filteredTable = new Table(inputTable.Identifier, inputTable.Schema, inputTable.Headers, new List<TableRecord>());

                    for (int rowIndex = 0; rowIndex < inputTable.Records.Count; rowIndex++)
                    {
                        TableRecord record = inputTable.Records[rowIndex];

                        EnvV rowScope = envV.NewScope();

                        for (int valIndex = 0; valIndex < inputTable.Headers.Count; valIndex++)
                        {
                            string columnName = inputTable.Headers[valIndex].Identifier;
                            Val columnValue = record.Values[valIndex];

                            rowScope.Bind(columnName, columnValue);
                        }

                        Val predicateResult = EvalExpr(filter.Predicate, rowScope, envP, envS);

                        if (predicateResult.AsBool())
                        {
                            filteredTable.AddRecord(record);
                        }
                    }

                    return new TableVal(filteredTable);
                }

            case Sum sum:
                {
                    Val tableValue = EvalExpr(sum.TableExpr, envV, envP, envS);
                    Table inputTable = tableValue.AsTable();

                    List<Column> resultSchema = envS.TryGet(sum.ResultSchemaId)!;

                    TableHeader tableHeader1 = new TableHeader(resultSchema[0].Id, resultSchema[0].Type);
                    TableHeader tableHeader2 = new TableHeader(resultSchema[1].Id, resultSchema[1].Type);

                    Table summedTable = new Table(inputTable.Identifier, resultSchema, new List<TableHeader> { tableHeader1, tableHeader2 }, new List<TableRecord>());

                    int groupByIndex = -1;
                    int sumIndex = -1;

                    // Find sum and groupBy column indexes
                    for (int valIndex = 0; valIndex < inputTable.Headers.Count; valIndex++)
                    {
                        if (inputTable.Headers[valIndex].Identifier == sum.GroupByColumn)
                        {
                            if (groupByIndex == -1)
                            {
                                groupByIndex = valIndex;
                            }
                        }

                        if (inputTable.Headers[valIndex].Identifier == sum.SumColumn)
                        {
                            if (sumIndex == -1)
                            {
                                sumIndex = valIndex;
                            }
                        }
                    }


                    // Sum expected column with groupBy
                    Dictionary<Val, Val> groupByDict = new Dictionary<Val, Val>();

                    for (int rowIndex = 0; rowIndex < inputTable.Records.Count; rowIndex++)
                    {
                        TableRecord tableRecord = inputTable.Records[rowIndex];

                        Val groupByIdentifier = tableRecord.Values[groupByIndex];
                        Val currentRowValue = tableRecord.Values[sumIndex];

                        Val? existingSum;

                        if (groupByDict.ContainsKey(groupByIdentifier))
                        {
                             existingSum = groupByDict[groupByIdentifier];
                        } else
                        {
                            existingSum = null;
                        }

                        if (existingSum == null)
                        {
                            groupByDict[groupByIdentifier] = currentRowValue;
                        }
                        else
                        {
                            if (existingSum is IntVal)
                            {
                                groupByDict[groupByIdentifier] = new IntVal(existingSum.AsInt() + currentRowValue.AsInt());
                            }
                            else
                            {
                                groupByDict[groupByIdentifier] = new FloatVal(existingSum.AsFloat() + currentRowValue.AsFloat());
                            }
                        }
                    }

                    // Add all records to the summed table
                    foreach (KeyValuePair<Val, Val> pair in groupByDict)
                    {
                        if (summedTable.Headers[0].Identifier == sum.GroupByColumn)
                        {
                            summedTable.AddRecord(new TableRecord(new List<Val> { pair.Key, pair.Value }));
                        }
                        else
                        {
                            summedTable.AddRecord(new TableRecord(new List<Val> { pair.Value, pair.Key }));
                        }
                    }

                    return new TableVal(summedTable);
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