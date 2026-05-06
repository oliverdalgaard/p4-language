using Matilda;

namespace MatildaTests;

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
    private void AssertNoTypeErrors(Program program)
    {
        EnvVT envVT = new EnvVT();
        EnvPT envPT = new EnvPT();
        EnvST envST = new EnvST();

        TypeChecker typeChecker = new TypeChecker(program, envVT, envPT, envST);

        Assert.IsFalse(typeChecker.HasErrors(), $"Type checker has erros: {string.Join("\n", typeChecker.errors)}");
    }

    private static readonly object ConsoleLock = new object();

    private string RunProgram(Program program)
    {
        lock (ConsoleLock)
        {
            EnvV envV = new EnvV();
            EnvP envP = new EnvP();
            EnvS envS = new EnvS();

            TextWriter originalOutput = Console.Out;
            StringWriter output = new StringWriter();

            try
            {
                Console.SetOut(output);

                Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
                Interpreter.EvalStmt(program.Stmt, envV, envP, envS);
            }
            finally
            {
                Console.SetOut(originalOutput);
            }

            return output.ToString().Trim();
        }
    }

    [TestMethod]
    public void FunctionCallProgramBindsExpectedValueInEnvironment()
    {
        // Arrange
        Program program = ParseFile("FunctionCallTest.matilda");

        // Act
        AssertNoTypeErrors(program);

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(program.Stmt, envV, envP, envS);

        // Assert
        Val? result = envV.TryGet("number");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IntVal>(result);

        Assert.AreEqual(171, result.AsInt());
    }

    [TestMethod]
    public void WhileLoopPrintsExpectedSequence()
    {
        // Arrange
        Program program = ParseFile("WhileLoopTest.matilda");

        // Act
        AssertNoTypeErrors(program);

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(program.Stmt, envV, envP, envS);

        string expected = "3";

        // Assert

        Val? result = envV.TryGet("i");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IntVal>(result);

        Assert.AreEqual(expected, result.ToString());
    }

    [TestMethod]
    public void IfStatementPrintsCorrectBranch()
    {
        // Arrange
        Program program = ParseFile("IfStatementTest.matilda");

        // Act
        AssertNoTypeErrors(program);

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(program.Stmt, envV, envP, envS);

        string expected = "100";

        // Assert
        Val? result = envV.TryGet("x");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IntVal>(result);

        Assert.AreEqual(expected, result.ToString());
    }

    [TestMethod]
    public void PeopleSchemaFunctionChecksAge()
    {
        // Arrange
        Program program = ParseFile("PeopleSchemaFunctionTest.matilda");

        // Act
        AssertNoTypeErrors(program);

        EnvV envV = new EnvV();
        EnvP envP = new EnvP();
        EnvS envS = new EnvS();

        Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
        Interpreter.EvalStmt(program.Stmt, envV, envP, envS);

        string expected = "| name  | age   | \n| Alice | 22    | \n";

        // Assert
        Val? result = envV.TryGet("isAdultVar");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TableVal>(result);

        Assert.AreEqual(expected, result.ToString());
    }
}