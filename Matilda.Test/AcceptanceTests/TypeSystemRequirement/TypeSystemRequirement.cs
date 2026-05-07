using Matilda;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class TypeSystemRequirementsTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task PrimitiveTypeDeclarationsTest()
    {
        // Arrange
        Program ast = ParseFile("PrimitiveTypeDeclarationsTest.matilda");

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
    public Task FunctionTypeTest()
    {
        // Arrange
        Program ast = ParseFile("FunctionTypeTest.matilda");

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
    public Task TypecastingTest()
    {
        // Arrange
        Program ast = ParseFile("TypecastingTest.matilda");

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
    public Task SchemaTypeDeclarationTest()
    {
        // Arrange
        Program ast = ParseFile("SchemaTypeDeclarationTest.matilda");

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
    public Task BooleanExpressionsTest()
    {
        // Arrange
        Program ast = ParseFile("BooleanExpressionsTest.matilda");

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
    public Task SchemaFunctionTypeTest()
    {
        // Arrange
        Program ast = ParseFile("SchemaFunctionTypeTest.matilda");

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