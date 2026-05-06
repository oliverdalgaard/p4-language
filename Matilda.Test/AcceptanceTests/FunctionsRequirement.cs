using Matilda;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class FunctionRequirementsTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task FunctionDeclarationTest()
    {
        // Arrange
        Program ast = ParseFile("FunctionDeclarationTest.matilda");

        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        // Act
        RunTypeChecker(ast, envVT, envPT, envST);

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);

        // Assert
        return Verify(new { envP, ast.TopLevelDeclarations });
    }

    [TestMethod]
    public Task FunctionReferenceTest()
    {
        // Arrange
        Program ast = ParseFile("FunctionReferenceTest.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        // Act
        RunTypeChecker(ast, envVT, envPT, envST);

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(ast.Stmt, envV, envP, envS);

        // Assert
        return Verify(new { envV, envP, ast.TopLevelDeclarations, ast.Stmt });
    }

    [TestMethod]
    public Task MultipleFunctionDeclarationsTest()
    {
        // Arrange
        Program ast = ParseFile("MultipleFunctionDeclarations.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        // Act
        RunTypeChecker(ast, envVT, envPT, envST);

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(ast.Stmt, envV, envP, envS);

        // Assert
        return Verify(new { envP, ast.TopLevelDeclarations });
    }
}
