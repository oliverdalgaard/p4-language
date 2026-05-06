using Matilda;

namespace MatildaTests.AcceptanceTests;

public class AcceptanceTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get => Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../TestMatildaScripts/AcceptanceTestsScripts"));
    }
}