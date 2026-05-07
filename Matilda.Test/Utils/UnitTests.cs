using Matilda;

namespace MatildaTests.UnitTests;

public class UnitTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get => Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "../../../TestMatildaScripts/ParserTestsScripts"));
    }
}