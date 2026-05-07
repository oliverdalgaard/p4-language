using Matilda;

namespace MatildaTests.UnitTests.TypeCheckerTests;

public abstract class RunTypeChecker
{
    public TypeChecker Run(Program program)
    {
        var envVT = new EnvVT();
        var envPT = new EnvPT();
        var envST = new EnvST();

        return new TypeChecker(program, envVT, envPT, envST);
    }
}