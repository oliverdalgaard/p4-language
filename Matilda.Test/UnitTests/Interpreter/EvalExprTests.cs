using Matilda;

namespace MatildaTests.UnitTests.InterpreterTests.EvalExprTests;

[TestClass]
public class UnknownExprTests
{
    [TestMethod]
    public void EvalExprUnknown()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Expr expr = null;

        // Assert
        try
        {
            Interpreter.EvalExpr(expr, envV, envP, envS);
            Assert.Fail();

        }
        catch (Exception exception)
        {
            Assert.AreEqual("Not a valid expression", exception.Message);
        }
    }

    [TestMethod]
    public void EvalBinaryOpUnknown()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Expr expr = new BinaryOp((BinaryOperators)999, new IntV(1, 1), new IntV(1, 1), -1);

        // Assert
        try
        {
            Interpreter.EvalExpr(expr, envV, envP, envS);
            Assert.Fail();

        }
        catch (Exception exception)
        {
            Assert.AreEqual("Not a valid binaryOp expression", exception.Message);
        }
    }

    [TestMethod]
    public void EvalUnaryOpUnknown()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Expr expr = new UnaryOp((UnaryOperators)999, new BoolV(false, -1), -1);

        // Assert
        try
        {
            Interpreter.EvalExpr(expr, envV, envP, envS);
            Assert.Fail();

        }
        catch (Exception exception)
        {
            Assert.AreEqual("Not a valid unaryOp expression", exception.Message);
        }
    }
}

[TestClass]
public class InterpreterEvalExprTests

{
    // Test 1: 1 + (2 * 3) = 9

    [TestMethod]
    public void EvalExprPrecedenceCheck1()
    {
        // Arange
        Expr left = new IntV(1, -1);

        Expr multiplyLeft = new IntV(2, -1);
        Expr multiplyRight = new IntV(3, -1);
        Expr right = new BinaryOp(BinaryOperators.MUL, multiplyLeft, multiplyRight, -1);

        Expr expr = new BinaryOp(BinaryOperators.ADD, left, right, -1);

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType(result, typeof(IntVal));
        Assert.AreEqual(7, result.AsInt());
        Assert.AreNotEqual(9, result.AsInt());
    }


    // Test 2: 1 + (2 * 3) + 4 = 11

    [TestMethod]
    public void EvalExprPrecedenceCheck2()
    {
        // Arrange
        Expr mul = new BinaryOp(
            BinaryOperators.MUL,
            new IntV(2, -1),
            new IntV(3, -1),
            -1
        );

        Expr leftAdd = new BinaryOp(
            BinaryOperators.ADD,
            new IntV(1, -1),
            mul,
            -1
        );

        Expr expr = new BinaryOp(
                BinaryOperators.ADD,
                leftAdd,
                new IntV(4, -1),
                -1
            );

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<IntVal>(result);
        Assert.AreEqual(11, result.AsInt());
    }


    // Test 3: (2 * 3) + (4 * 5) = 26
    [TestMethod]
    public void EvalExprPrecedenceCheck3()
    {
        // Arrange
        Expr left = new BinaryOp(
            BinaryOperators.MUL,
            new IntV(2, -1),
            new IntV(3, -1),
            -1
        );

        Expr right = new BinaryOp(
            BinaryOperators.MUL,
            new IntV(4, -1),
            new IntV(5, -1),
            -1
        );

        Expr expr = new BinaryOp(
            BinaryOperators.ADD,
            left,
            right,
            -1
        );

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<IntVal>(result);
        Assert.AreEqual(26, result.AsInt());
    }


    // Test 4: 8 + (6 / 2) = 11
    [TestMethod]
    public void EvalExprPrecedenceCheck4()
    {
        // Arrange
        Expr div = new BinaryOp(
            BinaryOperators.DIV,
            new IntV(6, -1),
            new IntV(2, -1),
            -1
        );

        Expr expr = new BinaryOp(
            BinaryOperators.ADD,
            new IntV(8, -1),
            div,
            -1
        );

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<FloatVal>(result);
        Assert.AreEqual(11, result.AsFloat());
    }

    // Test 5: 5 + (0 * 99) = 5

    [TestMethod]
    public void EvalExprPrecedenceCheck7()
    {
        // Arrange
        Expr mul = new BinaryOp(
            BinaryOperators.MUL,
            new IntV(0, -1),
            new IntV(99, -1),
            -1
        );

        Expr expr = new BinaryOp(
            BinaryOperators.ADD,
            new IntV(5, -1),
            mul,
            -1
        );

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<IntVal>(result);
        Assert.AreEqual(5, result.AsInt());
    }

    // Test 6: !(false || true)
    [TestMethod]
    public void EvalExprPrecedenceCheck8()
    {
        // Arrange
        Expr expr = new UnaryOp(
            UnaryOperators.NOT,
            new BinaryOp(BinaryOperators.OR, new BoolV(false, -1), new BoolV(true, -1), -1),
            -1
        );

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        var result = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<BoolVal>(result);
        Assert.AreEqual(false, result.AsBool());
    }

    [TestMethod]
    public void FunctionRefParamaterMisCountTest()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        // Act
        envP.Bind(new FunctionDeclaration(IntT.Instance, "TestFunc", new List<Parameter>(), Skip.Instance, -1));


        // Assert
        try
        {
            Interpreter.EvalExpr(new FunctionRef("TestFunc", new List<Expr> { new IntV(1, -1) }, -1), envV, envP, envS);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Number of arguments do not match the amount of parameters.", exception.Message);
        }
    }

    [TestMethod]
    public void FilterExprTest()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        List<Column> schema = new List<Column> { new Column("TestCol1", StringT.Instance), new Column("TestCol2", IntT.Instance) };

        List<TableHeader> headers = new List<TableHeader> { new TableHeader("TestCol1", StringT.Instance), new TableHeader("TestCol2", IntT.Instance) };
        List<TableRecord> unfilteredRecords = new List<TableRecord> {
            new TableRecord(new List<Val> {new StringVal("Test1"), new IntVal(1)}),
            new TableRecord(new List<Val> {new StringVal("Test2"), new IntVal(3)}),
            new TableRecord(new List<Val> {new StringVal("Test3"), new IntVal(5)}),
        };

        TableVal filterTable = new TableVal(new Table("TestTable", schema, headers, unfilteredRecords));

        Expr predicate = new BinaryOp(BinaryOperators.LT, new IntV(4, -1), new Ref("TestCol2", -1), -1);

        Filter filterExpr = new Filter(new Ref("TestTable", -1), predicate, -1);

        // Act
        envV.Bind("TestTable", filterTable);
        Val result = Interpreter.EvalExpr(filterExpr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<TableVal>(result);

        Assert.HasCount(1, result.AsTable().Records);
        Assert.HasCount(2, result.AsTable().Headers);

        Assert.AreEqual("TestCol1", result.AsTable().Headers[0].Identifier);
        Assert.AreEqual(StringT.Instance, result.AsTable().Headers[0].Type);

        Assert.AreEqual("TestCol2", result.AsTable().Headers[1].Identifier);
        Assert.AreEqual(IntT.Instance, result.AsTable().Headers[1].Type);

        Assert.HasCount(2, result.AsTable().Records[0].Values);

        Assert.AreEqual("Test3", result.AsTable().Records[0].Values[0].ToString());
        Assert.AreEqual(5, result.AsTable().Records[0].Values[1].AsInt());
    }


    [TestMethod]
    public void SumExprTest()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        List<Column> initialSchema = new List<Column> { new Column("ID", IntT.Instance),
                                                        new Column("TransactionOwner", StringT.Instance),
                                                        new Column("Amount", IntT.Instance) };

        TableVal initialTable = new TableVal(new Table("initialTable", initialSchema,
                                        new List<TableHeader> { new TableHeader("ID", StringT.Instance), new TableHeader("TransactionOwner", StringT.Instance), new TableHeader("Amount", StringT.Instance) },
                                        new List<TableRecord> { new TableRecord(new List<Val> { new IntVal(1), new StringVal("Peter"), new IntVal(30) }),
                                                                new TableRecord(new List<Val> { new IntVal(2), new StringVal("Lotte"), new IntVal(40) }),
                                                                new TableRecord(new List<Val> { new IntVal(1), new StringVal("Peter"), new IntVal(50) }),
                                                                new TableRecord(new List<Val> { new IntVal(2), new StringVal("Lotte"), new IntVal(60) }) }));


        List<Column> resultSchema = new List<Column> { new Column("ID", IntT.Instance),
                                                       new Column("Amount", IntT.Instance)};

        TableVal resultTable = new TableVal(new Table("resulTable", resultSchema,
                                       new List<TableHeader> { new TableHeader("ID", StringT.Instance), new TableHeader("Amount", StringT.Instance) },
                                       new List<TableRecord> { new TableRecord(new List<Val> { new IntVal(1), new IntVal(80) }), new TableRecord(new List<Val> { new IntVal(2), new IntVal(100) }) }));


        Sum sumExpr = new Sum(new Ref("initialTable", -1), "ID", "Amount", "resultSchema", -1);


        // Act
        envS.Bind("initialSchema", initialSchema);
        envS.Bind("resultSchema", resultSchema);
        envV.Bind("initialTable", initialTable);

        Val result = Interpreter.EvalExpr(sumExpr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<TableVal>(result);

        Assert.HasCount(2, result.AsTable().Records);
        Assert.HasCount(2, result.AsTable().Headers);

        Assert.AreEqual("ID", result.AsTable().Headers[0].Identifier);
        Assert.AreEqual(IntT.Instance, result.AsTable().Headers[0].Type);

        Assert.AreEqual("Amount", result.AsTable().Headers[1].Identifier);
        Assert.AreEqual(IntT.Instance, result.AsTable().Headers[1].Type);

        Assert.HasCount(2, result.AsTable().Records[0].Values);
        Assert.HasCount(2, result.AsTable().Records[1].Values);

        Assert.AreEqual(1, result.AsTable().Records[0].Values[0].AsInt());
        Assert.AreEqual(80, result.AsTable().Records[0].Values[1].AsInt());

        Assert.AreEqual(2, result.AsTable().Records[1].Values[0].AsInt());
        Assert.AreEqual(100, result.AsTable().Records[1].Values[1].AsInt());
    }

    [TestMethod]
    public void SumExprTest2()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        List<Column> initialSchema = new List<Column> { new Column("ID", FloatT.Instance),
                                                        new Column("TransactionOwner", StringT.Instance),
                                                        new Column("Amount", FloatT.Instance) };

        TableVal initialTable = new TableVal(new Table("initialTable", initialSchema,
                                        new List<TableHeader> { new TableHeader("ID", StringT.Instance), new TableHeader("TransactionOwner", StringT.Instance), new TableHeader("Amount", StringT.Instance) },
                                        new List<TableRecord> { new TableRecord(new List<Val> { new FloatVal(1), new StringVal("Peter"), new FloatVal(30) }),
                                                                new TableRecord(new List<Val> { new FloatVal(2), new StringVal("Lotte"), new FloatVal(40) }),
                                                                new TableRecord(new List<Val> { new FloatVal(1), new StringVal("Peter"), new FloatVal(50) }),
                                                                new TableRecord(new List<Val> { new FloatVal(2), new StringVal("Lotte"), new FloatVal(60) }) }));


        List<Column> resultSchema = new List<Column> { new Column("Amount", FloatT.Instance),
                                                       new Column("ID", FloatT.Instance)};

        TableVal resultTable = new TableVal(new Table("resulTable", resultSchema,
                                       new List<TableHeader> { new TableHeader("ID", StringT.Instance), new TableHeader("Amount", StringT.Instance) },
                                       new List<TableRecord> { new TableRecord(new List<Val> { new FloatVal(1), new FloatVal(80) }), new TableRecord(new List<Val> { new FloatVal(2), new FloatVal(100) }) }));


        Sum sumExpr = new Sum(new Ref("initialTable", -1), "ID", "Amount", "resultSchema", -1);


        // Act
        envS.Bind("initialSchema", initialSchema);
        envS.Bind("resultSchema", resultSchema);
        envV.Bind("initialTable", initialTable);

        Val result = Interpreter.EvalExpr(sumExpr, envV, envP, envS);

        // Assert
        Assert.IsInstanceOfType<TableVal>(result);

        Assert.HasCount(2, result.AsTable().Records);
        Assert.HasCount(2, result.AsTable().Headers);

        Assert.AreEqual("Amount", result.AsTable().Headers[0].Identifier);
        Assert.AreEqual(FloatT.Instance, result.AsTable().Headers[0].Type);

        Assert.AreEqual("ID", result.AsTable().Headers[1].Identifier);
        Assert.AreEqual(FloatT.Instance, result.AsTable().Headers[1].Type);

        Assert.HasCount(2, result.AsTable().Records[0].Values);
        Assert.HasCount(2, result.AsTable().Records[1].Values);

        Assert.AreEqual(80, result.AsTable().Records[0].Values[0].AsFloat());
        Assert.AreEqual(1, result.AsTable().Records[0].Values[1].AsFloat());

        Assert.AreEqual(100, result.AsTable().Records[1].Values[0].AsFloat());
        Assert.AreEqual(2, result.AsTable().Records[1].Values[1].AsFloat());
    }

    [TestMethod]
    [DataRow(BinaryOperators.AND, true, false, false)]
    [DataRow(BinaryOperators.AND, false, true, false)]
    [DataRow(BinaryOperators.AND, false, false, false)]
    [DataRow(BinaryOperators.AND, true, true, true)]
    [DataRow(BinaryOperators.OR, true, false, true)]
    [DataRow(BinaryOperators.OR, false, true, true)]
    [DataRow(BinaryOperators.OR, false, false, false)]
    [DataRow(BinaryOperators.OR, true, true, true)]
    public void BinaryOpLogicTest(BinaryOperators binaryOperator, bool testValLeft, bool testValRight, bool resultVal)
    {
        BoolV testValLeftV = new BoolV(testValLeft, -1);
        BoolV testValRightV = new BoolV(testValRight, -1);

        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Expr expr = new BinaryOp(binaryOperator, testValLeftV, testValRightV, -1);

        // Act
        Val returnVal = Interpreter.EvalExpr(expr, envV, envP, envS);

        // Assert
        Assert.AreEqual(resultVal, returnVal.AsBool());
    }
}