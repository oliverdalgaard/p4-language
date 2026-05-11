namespace Matilda;

class Matilda
{

	public static void Main(string[] arg)
	{
		if (arg.Length > 0)
		{
			Scanner scanner = new Scanner(arg[0]);
			Parser parser = new Parser(scanner);
			parser.Parse();

			if (parser.hasErrors())
			{
				Console.WriteLine("Errors during syntactic analysis!");
			}
			else
			{
				Program program = parser.mainNode;

				TypeChecker typeChecker = new TypeChecker(program, new EnvVT(), new EnvPT(), new EnvST());

				if (typeChecker.HasErrors())
				{
					typeChecker.errors.ForEach(Console.WriteLine);

					Console.WriteLine("Errors during static analysis!");
				}
				else
				{
					Console.WriteLine("Program starting!");

					EnvV envV = new EnvV();
					EnvP envP = new EnvP();
					EnvS envS = new EnvS();

					Interpreter.EvalTopLevelDeclarations(program.TopLevelDeclarations, envP, envS);
					Interpreter.EvalStmt(program.Stmt, envV, envP, envS);

					foreach (KeyValuePair<string, Val?> keyValuePair in envV.Bindings)
					{
						if (keyValuePair.Value != null)
						{
							Console.WriteLine($"{keyValuePair.Key}:\n{keyValuePair.Value}\n");
						}
					}

					Console.WriteLine("Program stopped!");
				}
			}


		}
		else
			Console.WriteLine("-- No source file specified");
	}

}
