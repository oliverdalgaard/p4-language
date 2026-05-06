using Matilda;
using VerifyMSTest;

namespace MatildaTests.AcceptanceTests;

[UsesVerify]
[TestClass]
public partial class FunctionRequirementsTests : AcceptanceTestHelper
{
    [TestMethod]
    public Task FunctionDeclarationTest()
    {
        Program ast = ParseFile("FunctionDeclarationTest.matilda");

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(ast.TopLevelDeclarations, envP, envS);

        return Verify(envP);
    }
}
