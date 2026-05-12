using Matilda;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class ProblemStatementTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task ProblemStatementTest()
    {
        // Arrange
        Program ast = ParseFile("ProblemStatementTest.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        // Act
        TypeChecker typeChecker = RunTypeChecker(ast, envVT, envPT, envST);

        if (!typeChecker.HasErrors())
        {
            Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);
            Interpreter.EvalStmt(ast.Stmt, envV, envP, envS);
        }

        // Assert
        return Verify(new { envV, envP, envS, envVT, envPT, envST, typeChecker, ast.TopLevelDeclarations, ast.Stmt });
    }

}