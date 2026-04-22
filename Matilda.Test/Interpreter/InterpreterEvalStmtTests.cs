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
    [TestMethod]
    public void EvalStmtCompExecutesBothStatementsWhenNoReturn()
    {
        // Arrange
        var envV = new EnvV();
        var envP = new EnvP();

        Stmt stmt = new Comp(
            new Declaration(new StringT(), "x", -1, null),
            new Assign("x", new IntV(5,-1), -1)
        );  

        // Act
        Interpreter.EvalStmt(stmt, envV, envP);

        // Assert
        Assert.AreEqual(5, envV.TryGet("x")!.AsInt());
    }

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
}