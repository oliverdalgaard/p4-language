
using System;

namespace Matilda {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int maxT = 14;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public CodeGenerator gen;

  Dictionary<string, Var> symbols = new Dictionary<string, Var>();



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Matilda() {
		while (StartOf(1)) {
			if (la.kind == 3 || la.kind == 4) {
				Declaration();
			} else if (la.kind == 6 || la.kind == 12) {
				Stmt(out int stmt);
			} else {
				Expr(out int result);
			}
		}
		Expect(0);
	}

	void Declaration() {
		VarType(out Type type);
		Expect(1);
		string name = t.val;
		
		if (symbols.ContainsKey(name)) throw new Exception("Variable  "+name+" already declared");
		
		Var var = new Var(name, type);
		symbols[name] = var; 
		
		Expect(5);
		Expr(out int result);
		symbols[name].setValue(result); 
	}

	void Stmt(out int stmt) {
		stmt = 0; 
		if (la.kind == 6) {
			Get();
			Expect(7);
			Expr(out int cond);
			Expect(8);
			Expect(9);
			Expr(out int thenBranch);
			if (cond != 0) stmt = thenBranch; 
			Expect(10);
			if (la.kind == 11) {
				Get();
				Expect(9);
				Expr(out int elseBranch);
				Expect(10);
				if (cond == 0) stmt = elseBranch; 
			}
		} else if (la.kind == 12) {
			Get();
			Expr(out int result);
			Console.WriteLine(result); 
		} else SynErr(15);
	}

	void Expr(out int result) {
		Term(out result);
		while (la.kind == 13) {
			Get();
			Term(out int t);
			result += t; 
		}
	}

	void VarType(out Type type) {
		type = Type.INT; 
		if (la.kind == 3) {
			Get();
			type = Type.INT; 
		} else if (la.kind == 4) {
			Get();
			type = Type.BOOL; 
		} else SynErr(16);
	}

	void Term(out int result) {
		result = 0; 
		if (la.kind == 2) {
			Get();
			result = int.Parse(t.val); 
		} else if (la.kind == 1) {
			Get();
			string name = t.val;
			if (!symbols.ContainsKey(name)) throw new Exception("Variable "+name+" is not defined");
			
			result = symbols[name].getValue();
			     
		} else SynErr(17);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Matilda();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_T,_T, _T,_x,_T,_x, _x,_x,_x,_x, _T,_x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "NUMBER expected"; break;
			case 3: s = "\"int\" expected"; break;
			case 4: s = "\"bool\" expected"; break;
			case 5: s = "\"=\" expected"; break;
			case 6: s = "\"if\" expected"; break;
			case 7: s = "\"(\" expected"; break;
			case 8: s = "\")\" expected"; break;
			case 9: s = "\"{\" expected"; break;
			case 10: s = "\"}\" expected"; break;
			case 11: s = "\"else\" expected"; break;
			case 12: s = "\"print\" expected"; break;
			case 13: s = "\"+\" expected"; break;
			case 14: s = "??? expected"; break;
			case 15: s = "invalid Stmt"; break;
			case 16: s = "invalid VarType"; break;
			case 17: s = "invalid Term"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}