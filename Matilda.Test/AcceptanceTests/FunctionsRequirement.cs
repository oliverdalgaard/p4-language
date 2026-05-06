using Matilda;

namespace MatildaTests.AcceptanceTests.FunctionsRequirementTests;

[TestClass]
public class FunctionRequiements : VerifyBase
{
    [TestMethod]
    public Task FunctionDeclarationTest() => Verify("The content");
}
