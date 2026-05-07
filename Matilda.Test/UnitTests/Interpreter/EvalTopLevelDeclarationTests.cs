using Matilda;

namespace MatildaTests.UnitTests.InterpreterTests.EvalTopLevelDeclarationTests;


[TestClass]
public class InterpreterSchemaDeclarationTests
{
    // Stmt function declaration binds function in procedure environment

    [TestMethod]
    public void EvalStmtSchemaDeclarationBindsFunctionInProcedureEnvironment()
    {
        // Arrange
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        SchemaDeclaration schema = new SchemaDeclaration(
            "Test",
            new List<Column>(),
            -1
        );

        // Act
        Interpreter.EvalTopLevelDeclarations(new List<TopLevelDeclaration> { schema }, envP, envS);

        // Assert
        Assert.IsNotNull(envS.TryGet("Test"));
    }
}

[TestClass]
public class InterpreterFunctionDeclarationTests
{
    // Stmt function declaration binds function in procedure environment

    [TestMethod]
    public void EvalStmtFunctionDeclarationBindsFunctionInProcedureEnvironment()
    {
        // Arrange
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        FunctionDeclaration function = new FunctionDeclaration(
            new IntT(),
            "Test",
            new List<Parameter>(),
            new Return(new IntV(1, -1), -1),
            -1
        );

        // Act
        Interpreter.EvalTopLevelDeclarations(new List<TopLevelDeclaration> { function }, envP, envS);

        // Assert
        Assert.IsNotNull(envP.TryGet("Test"));
    }
}