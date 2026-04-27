using System;

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
				Stmt program = parser.mainNode;

				/* TypeChecker typeChecker = new TypeChecker(program, new EnvVT(), new EnvPT(), new EnvST());

				if (typeChecker.HasErrors())
				{
					typeChecker.errors.ForEach(Console.WriteLine);

					Console.WriteLine("Errors during static analysis!");
				}
				else
				{ */
				Console.WriteLine("Program starting!");
				Interpreter.EvalStmt(program, new EnvV(), new EnvP(), new EnvS());
				Console.WriteLine("Program stopped!");
				/* } */
			}


		}
		else
			Console.WriteLine("-- No source file specified");
	}

}
