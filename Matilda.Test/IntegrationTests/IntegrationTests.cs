namespace Matilda;

[TestClass]
public class IntegrationTests
{
    // Helper function to locate the integration test script directory
    private static readonly string ScriptFolder =
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../TestMatildaScripts/IntegrationTestsScripts"));

    private Program ParseFile(string fileName)
    {
        string path = Path.Combine(ScriptFolder, fileName);

        Assert.IsTrue(File.Exists(path), $"Test script file was not found: {path}");

        Scanner scanner = new Scanner(path);
        Parser parser = new Parser(scanner);

        parser.Parse();

        Assert.IsFalse(parser.hasErrors(), "Parser had errors.");

        return parser.mainNode;
    }
    private void AssertNoTypeErros(Program program)
    {
        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        TypeChecker typeChecker = new TypeChecker(program, envVT, envPT, envST);

        Assert.IsFalse(typeChecker.HasErrors(), $"Type checker has erros: {string.Join("\n", typeChecker.errors)}");
    }

    private string RunProgram(Program program)
    {
        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);

        TextWriter originalOutput = Console.Out;

        StringWriter output = new StringWriter();

        try
        {
            Console.SetOut(output);  
            Interpreter.EvalStmt(program.Stmt, envV, envP, envS);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        return output.ToString().Trim();
    }

    [TestMethod]
    public void FullFunctionPrintsExpectedResult()
    {
        // Arrange
        Program program = ParseFile("FunctionCallTest.matilda");

        // Act
        AssertNoTypeErros(program);
        string output = RunProgram(program);

        // Assert
        Assert.AreEqual("171", output);
    }
}