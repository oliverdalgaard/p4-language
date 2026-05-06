using Matilda;

namespace MatildaTests.IntegrationTests;

public class IntegrationTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get => Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../TestMatildaScripts/IntegrationTestsScripts"));
    }
}