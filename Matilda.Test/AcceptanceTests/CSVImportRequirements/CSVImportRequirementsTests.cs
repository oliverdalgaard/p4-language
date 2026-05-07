using Matilda;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class CSVImportRequirementsTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task SimpleCSVImportTest()
    {
        // Arrange
        Program ast = ParseFile("SimpleCSVImportTest.matilda");

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

    [TestMethod]
    public Task CSVImportWithFilterTest()
    {
        // Arrange
        Program ast = ParseFile("CSVImportWithFilterTest.matilda");

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

    [TestMethod]
    public Task CSVImportPassedToFunctionTest()
    {
        // Arrange
        Program ast = ParseFile("CSVImportPassedToFunctionTest.matilda");

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