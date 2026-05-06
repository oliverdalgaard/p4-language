using Matilda;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class FunctionRequirementsTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task FunctionDeclarationTest()
    {
        Program ast = ParseFile("FunctionDeclarationTest.matilda");

        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);

        return Verify(new { envP, ast.TopLevelDeclarations });
    }

    [TestMethod]
    public Task FunctionReferenceTest()
    {
        Program ast = ParseFile("FunctionReferenceTest.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(ast.Stmt, envV, envP, envS);

        return Verify(new { envV, envP, ast.TopLevelDeclarations, ast.Stmt });
    }

    [TestMethod]
    public Task MultipleFunctionDeclarationsTest()
    {
        Program ast = ParseFile("MultipleFunctionDeclarations.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(ast.Stmt, envV, envP, envS);

        return Verify(new { envP, ast.TopLevelDeclarations });
    }
}
