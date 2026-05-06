using Matilda;

namespace MatildaTests;

[TestClass]
public class InterpreterEvalStmtTests
{

    [TestMethod]
    public void EvalStmtParameterTest()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Stmt stmt = new Parameter(IntT.Instance, "TestId", -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.IsNull(envV.TryGet("testId"));
    }

    [TestMethod]
    public void EvalStmtTableDeclarationTest()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        envS.Bind("testSchemaId", new List<Column> { new Column("hej", IntT.Instance), new Column("dig", IntT.Instance) });

        Stmt stmt = new TableDeclaration(new TableT("testSchemaId"), "TestId", "../../../MatildaCSVFiles/TableDeclarationTest.csv", -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        Val tableVal = envV.TryGet("TestId");
        Table table = tableVal.AsTable();
        List<TableHeader> headers = table.Headers;
        List<TableRecord> records = table.Records;

        // Assert
        Assert.AreEqual("hej", headers[0].Identifier);
        Assert.AreEqual(IntT.Instance, headers[0].Type);
        Assert.AreEqual("dig", headers[1].Identifier);
        Assert.AreEqual(IntT.Instance, headers[1].Type);

        Assert.AreEqual(1, records[0].Values[0].AsInt());
        Assert.AreEqual(2, records[0].Values[1].AsInt());
        Assert.AreEqual(3, records[1].Values[0].AsInt());
        Assert.AreEqual(4, records[1].Values[1].AsInt());
    }

    // Stmt comp executes both statements when no return exists

    [TestMethod]
    public void EvalStmtCompExecutesBothStatementsWhenNoReturn()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Stmt stmt = new Comp(
            new LocalDeclaration(new StringT(), "x", new IntV(1, -1), -1),
            new Assign("x", new IntV(5, -1), -1)
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(5, envV.TryGet("x")!.AsInt());
    }

    // Stmt eval comp does dot execute second statement when return exists

    [TestMethod]
    public void EvalStmtCompDoesNotExecuteSecondStatementWhenReturnExists()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new Comp(
            new Return(new IntV(5, -1), -1),
            new Assign("x", new IntV(999, -1), -1)
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(5, envV.FunctionReturnValue!.AsInt());
    }

    // Stmt declaration with expression binds evaluated value => string x = "Test";

    [TestMethod]
    public void EvalStmtDeclarationWithExpressionBindsEvaluatedValue()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        Stmt stmt = new LocalDeclaration(new StringT(), "x", new StringV("Test", -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual("Test", envV.TryGet("x")!.ToString());

    }

    // Stmt assign updates existing variable => First x is bound to 1, then updated to 67

    [TestMethod]
    public void EvalStmtAssignUpdatesExistingVariable()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        envV.Bind("x", new IntVal(1));

        Stmt stmt = new Assign("x", new IntV(67, -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(67, envV.TryGet("x")!.AsInt());
    }

    // Stmt return binds return value in variable environment

    [TestMethod]
    public void EvalStmt_Return_BindsReturnValue()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        Stmt stmt = new Return(new IntV(67, -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.IsNotNull(envV.FunctionReturnValue);
        Assert.AreEqual(67, envV.FunctionReturnValue!.AsInt());
    }

    // Stmt if then branch runs when condition true

    [TestMethod]
    public void EvalStmtIfThenBranchRunsWhenConditionTrue()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new If(
            new BoolV(true, -1),
            new Assign("x", new IntV(1, -1), -1),
            new Assign("x", new IntV(2, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(1, envV.TryGet("x")!.AsInt());
    }

    // Stmt if else branch runs when condition false

    [TestMethod]
    public void EvalStmtIfElseBranchRunsWhenConditionFalse()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new If(
            new BoolV(false, -1),
            new Assign("x", new IntV(1, -1), -1),
            new Assign("x", new IntV(2, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(2, envV.TryGet("x")!.AsInt());
    }

    // Stmt while repeats until condition false

    [TestMethod]
    public void EvalStmtWhileRepeatsUntilConditionFalse()
    {
        // Arrange
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();
        envV.Bind("x", new IntVal(0));

        Stmt body = new Assign(
            "x",
            new BinaryOp(
                BinaryOperators.ADD,
                new Ref("x", -1),
                new IntV(1, -1),
                -1
            ),
            -1
        );

        Stmt stmt = new While(
            new BinaryOp(
                BinaryOperators.LT,
                new Ref("x", -1),
                new IntV(3, -1),
                -1
            ),
            body,
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP, envS);

        // Assert
        Assert.AreEqual(3, envV.TryGet("x")!.AsInt());
    }

    // Stmt while propagates return to outer scope => Muligvis ændres på baggrund af side effekter i at den ændre globale variabler

    // [TestMethod]
    // public void EvalStmtWhileStopsAndPropagatesReturn()
    // {
    //     // Arrange
    //     EnvV envV = new EnvV();
    //     EnvP envP = new EnvP();
    //     EnvS envS = new EnvS();

    //     Stmt stmt = new While(
    //         new BoolV(true, -1),
    //         new Return(new IntV(10, -1), -1),
    //         -1
    //     );

    //     // Act
    //     Interpreter.EvalStmt(stmt, envV, envP, envS);

    //     // Assert
    //     Assert.IsNotNull(envV.TryGet("return"));
    //     Assert.AreEqual(10, envV.TryGet("return")!.AsInt());
    // }
}