using Matilda;

namespace MatildaTests.IntegrationTests;

public class IntegrationTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get
        {
            var testProjectPath = Path.GetDirectoryName(typeof(IntegrationTestHelper).Assembly.Location)!;
            return Path.GetFullPath(Path.Combine(
                testProjectPath,
                "../../../../Matilda.Test/TestMatildaScripts/IntegrationTestsScripts"));
        }
    }
}