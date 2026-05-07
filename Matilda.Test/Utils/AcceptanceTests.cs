using Matilda;

namespace MatildaTests.AcceptanceTests;

public class AcceptanceTestHelper : TestHelper
{
    public override string ScriptFolder
    {
        get
        {
            var testProjectPath = Path.GetDirectoryName(typeof(AcceptanceTestHelper).Assembly.Location)!;
            return Path.GetFullPath(Path.Combine(
                testProjectPath,
                "../../../../Matilda.Test/TestMatildaScripts/AcceptanceTestsScripts"));
        }
    }

    public TypeChecker RunTypeChecker(Program program, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        return new TypeChecker(program, envVT, envPT, envST);
    }
}