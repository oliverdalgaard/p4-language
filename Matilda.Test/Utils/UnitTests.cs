using Matilda;

namespace MatildaTests.UnitTests;

public class UnitTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get
        {
            var testProjectPath = Path.GetDirectoryName(typeof(UnitTestHelper).Assembly.Location)!;
            return Path.GetFullPath(Path.Combine(
                testProjectPath,
                "../../../../Matilda.Test/TestMatildaScripts/ParserTestsScripts"));
        }
    }
}