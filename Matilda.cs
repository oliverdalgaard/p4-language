using System;

namespace Matilda {

class Matilda {

	public static void Main (string[] arg) {
		if (arg.Length > 0) {
			Scanner scanner = new Scanner(arg[0]);
			Parser parser = new Parser(scanner);
			parser.gen = new CodeGenerator();
			parser.Parse();
		} else
			Console.WriteLine("-- No source file specified");
	}
	
}

}
