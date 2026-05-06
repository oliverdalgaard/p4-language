using Matilda;

namespace MatildaTests.AcceptanceTests.FunctionsRequirementTests;

[UsesVerify]
[TestClass]
public class FunctionRequiements : VerifyBase
{
    [TestMethod]
    public Task FunctionDeclarationTest() => Verify("The content");
}
