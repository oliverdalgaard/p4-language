
using System;

namespace Matilda {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int maxT = 13;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Stmt mainNode = null;

  public bool hasErrors() {
    return errors.count > 0;
  }

  private Stmt toComp(List<Stmt> stmts) {
    if (!stmts.Any()) {
      return Skip.Instance;
    }

    Stmt result = stmts.Last();

    return result;
  }



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
		Stmts(out mainNode);
	}

	void Stmts(out Stmt stmt) {
		List<Stmt> list = new List<Stmt>(); 
		while (StartOf(1)) {
			Stmt(out Stmt temp);
			list.Add(temp); 
		}
		stmt = toComp(list); 
	}

	void Stmt(out Stmt stmt) {
		stmt = Skip.Instance; 
		if (StartOf(2)) {
			Declaration(out stmt);
		} else if (la.kind == 5) {
			Print(out stmt);
		} else SynErr(14);
	}

	void Declaration(out Stmt stmt) {
		Type(out Type type);
		Expect(1);
		string var = t.val; stmt = new Declaration(type, var, t.line); 
		Expect(3);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Comp(stmt, new Assign(var, expr, lineNumber)); 
		Expect(4);
	}

	void Print(out Stmt stmt) {
		Expect(5);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Print(expr, lineNumber); 
		Expect(4);
	}

	void Type(out Type type) {
		type = null; 
		if (la.kind == 6) {
			Get();
			type = IntT.Instance; 
		} else if (la.kind == 7) {
			Get();
			type = FloatT.Instance; 
		} else if (la.kind == 8) {
			Get();
			type = BoolT.Instance; 
		} else if (la.kind == 9) {
			Get();
			type = StringT.Instance; 
		} else SynErr(15);
	}

	void Expr(out Expr expr) {
		Term(out expr);
		while (la.kind == 10) {
			Get();
			int lineNumber = t.line; 
			Term(out Expr expr2);
			expr = new BinaryOp(BinaryOperators.ADD, expr, expr2, lineNumber); 
		}
	}

	void Term(out Expr expr) {
		expr = null; 
		if (la.kind == 1) {
			Get();
			expr = new Ref(t.val, t.line); 
		} else if (la.kind == 2) {
			Get();
			expr = new IntV(Int32.Parse(t.val), t.line); 
		} else if (la.kind == 11) {
			Get();
			expr = new BoolV(true, t.line); 
		} else if (la.kind == 12) {
			Get();
			expr = new BoolV(false, t.line); 
		} else SynErr(16);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Matilda();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x}

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
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\";\" expected"; break;
			case 5: s = "\"print\" expected"; break;
			case 6: s = "\"int\" expected"; break;
			case 7: s = "\"float\" expected"; break;
			case 8: s = "\"bool\" expected"; break;
			case 9: s = "\"string\" expected"; break;
			case 10: s = "\"+\" expected"; break;
			case 11: s = "\"true\" expected"; break;
			case 12: s = "\"false\" expected"; break;
			case 13: s = "??? expected"; break;
			case 14: s = "invalid Stmt"; break;
			case 15: s = "invalid Type"; break;
			case 16: s = "invalid Term"; break;

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