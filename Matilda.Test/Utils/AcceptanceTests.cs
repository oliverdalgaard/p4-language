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

    public TypeChecker RunTypeChecker(Program program, EnvVT envVT, EnvPT envPT, EnvST envST)
    {
        return new TypeChecker(program, envVT, envPT, envST);
    }
}