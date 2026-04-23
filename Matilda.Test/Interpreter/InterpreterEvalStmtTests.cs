using Matilda;
namespace MatildaTests;

[TestClass]
public class InterpreterEvalStmtTests
{
    // Stmt skipped

    [TestMethod]
    public void EvalStmtSkipped()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        var stmt = new Skip();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            Interpreter.EvalStmt(stmt, envV, envP);

            // Assert
            Assert.AreEqual("Skipped" + Environment.NewLine, sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // Stmt comp execute both statements wen no return
    /* [TestMethod]
    public void EvalStmtCompExecutesBothStatementsWhenNoReturn()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();

        Stmt stmt = new Comp(
            new Declaration(new StringT(), "x", -1, ""),
            new Assign("x", new IntV(5, -1), -1)
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(5, envV.TryGet("x")!.AsInt());
    } */

    // Stmt eval comp does dot execute second statement when return exists
    [TestMethod]
    public void EvalStmtCompDoesNotExecuteSecondStatementWhenReturnExists()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new Comp(
            new Return(new IntV(5, -1), -1),
            new Assign("x", new IntV(999, -1), -1)
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(0, envV.TryGet("x")!.AsInt());
        Assert.AreEqual(5, envV.TryGet("return")!.AsInt());
    }

    //  Stmt print
    [TestMethod]
    public void EvalStmtPrintPrintsExpressionValue()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        Stmt stmt = new Print(new IntV(42, -1), -1);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            Interpreter.EvalStmt(stmt, envV, envP);

            // Assert
            Assert.AreEqual("42" + Environment.NewLine, sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // Stmt declaration without expression binds null => int x;

    /*     [TestMethod]
        public void EvalStmtDeclarationWithoutExpressionBindsNull()
        {
            // Arrange
            var envV = new EnvV();
            var envP = new EnvP();
            Stmt stmt = new Declaration(new StringT(), "x", new StringV("Hej", -1), -1);

            // Act
            Interpreter.EvalStmt(stmt, envV, envP);

            // Assert
            Assert.Equals(envV.TryGet("x"));
        } */

    // Stmt declaration with expression binds evaluated value => int x = 5;

    [TestMethod]
    public void EvalStmtDeclarationWithExpressionBindsEvaluatedValue()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        Stmt stmt = new Declaration(new IntT(), "x", new IntV(5, -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.IsNotNull(envV.TryGet("x"));
        Assert.AreEqual(5, envV.TryGet("x")!.AsInt());
    }

    // Stmt assign updates existing variable => First x is bound to 1, then updated to 67

    [TestMethod]
    public void EvalStmtAssignUpdatesExistingVariable()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        envV.Bind("x", new IntVal(1));

        Stmt stmt = new Assign("x", new IntV(67, -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(67, envV.TryGet("x")!.AsInt());
    }

    // Stmt function declaration binds function in procedure environment

    [TestMethod]
    public void EvalStmtFunctionDeclarationBindsFunctionInProcedureEnvironment()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();

        var function = new FunctionDeclaration(
            new IntT(),
            "foo",
            new List<Parameter>(),
            new List<Stmt> { new Return(new IntV(1, -1), -1) },
            -1
        );

        // Act
        Interpreter.EvalStmt(function, envV, envP);

        // Assert
        Assert.IsNotNull(envP.TryGet("foo"));
    }

    // Stmt return binds return value in variable environment
    [TestMethod]
    public void EvalStmt_Return_BindsReturnValue()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        Stmt stmt = new Return(new IntV(67, -1), -1);

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.IsNotNull(envV.TryGet("return"));
        Assert.AreEqual(67, envV.TryGet("return")!.AsInt());
    }

    // Stmt if then branch runs when condition true

    [TestMethod]
    public void EvalStmtIfThenBranchRunsWhenConditionTrue()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new If(
            new BoolV(true, -1),
            new Assign("x", new IntV(1, -1), -1),
            new List<If>(),
            new Assign("x", new IntV(2, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(1, envV.TryGet("x")!.AsInt());
    }

    // Stmt if else branch runs when condition false

    [TestMethod]
    public void EvalStmtIfElseBranchRunsWhenConditionFalse()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
        envV.Bind("x", new IntVal(0));

        Stmt stmt = new If(
            new BoolV(false, -1),
            new Assign("x", new IntV(1, -1), -1),
            new List<If>(),
            new Assign("x", new IntV(2, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(2, envV.TryGet("x")!.AsInt());
    }

    // Stmt if propagates return to outer scope => Muligvis ændres på baggrund af side effekter i at den ændre globale variabler

    [TestMethod]
    public void EvalStmtIfPropagatesReturnToOuterScope()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();

        Stmt stmt = new If(
            new BoolV(true, -1),
            new Return(new IntV(67, -1), -1),
            new List<If>(),
            new Skip(),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.IsNotNull(envV.TryGet("return"));
        Assert.AreEqual(67, envV.TryGet("return")!.AsInt());
    }

    // Stmt while repeats until condition false

    [TestMethod]
    public void EvalStmtWhileRepeatsUntilConditionFalse()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();
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
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(3, envV.TryGet("x")!.AsInt());
    }

    // Stmt while propagates return to outer scope => Muligvis ændres på baggrund af side effekter i at den ændre globale variabler

    [TestMethod]
    public void EvalStmt_While_StopsAndPropagatesReturn()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();

        Stmt stmt = new While(
            new BoolV(true, -1),
            new Return(new IntV(10, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.IsNotNull(envV.TryGet("return"));
        Assert.AreEqual(10, envV.TryGet("return")!.AsInt());
    }
}