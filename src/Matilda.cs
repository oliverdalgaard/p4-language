using System;

namespace Matilda
{

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

					Console.WriteLine(program);
				}


			}
			else
				Console.WriteLine("-- No source file specified");
		}

	}

}
