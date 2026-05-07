using Matilda;

namespace MatildaTests;

public abstract class TestHelper
{
    public abstract string ScriptFolder { get; }


    public Program ParseFile(string fileName)
    {
        string path = Path.Combine(ScriptFolder, fileName);

        Assert.IsTrue(File.Exists(path), $"Test script file was not found: {path}");

        Scanner scanner = new Scanner(path);
        Parser parser = new Parser(scanner);

        parser.Parse();

        Assert.IsFalse(parser.hasErrors(), "Parser had errors.");

        return parser.mainNode;
    }
    public static void AssertNoTypeErrors(Program program)
    {
        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        TypeChecker typeChecker = new TypeChecker(program, envVT, envPT, envST);

        Assert.IsFalse(typeChecker.HasErrors(), $"Type checker has erros: {string.Join("\n", typeChecker.errors)}");
    }
}