using System;
using System.Globalization;



using System;

namespace Matilda {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int _FLOAT = 3;
	public const int _STRING = 4;
	public const int maxT = 31;

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

  private Stmt ToComp(List<Stmt> stmts) {
    if (!stmts.Any()) {
      return Skip.Instance;
    }

    Stmt result = stmts.Last();
    int index = stmts.Count - 2;

    while (index >= 0) {
      Stmt current = stmts[index];

      if (current is Comp comp) {
        result = new Comp(comp.Stmt1, new Comp(comp.Stmt2, result));
      } else {
        result = new Comp(current, result);
      }
      index--;
    }

    return result;
  }

  private Expr applyUnaries(List<char> unariesReversed, Expr baseExpr, int lineNumber) {
    Expr result = baseExpr;
    int index = 0;
    while (index < unariesReversed.Count) {
      char ch = unariesReversed[index];
      switch (ch) {
        case '!':
          result = new UnaryOp(UnaryOperators.NOT, result, lineNumber);
          break;
        default:
          throw new Exception("Unknown unary operator: " + ch);
      }
      index += 1;
    }
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
		stmt = ToComp(list); 
	}

	void Stmt(out Stmt stmt) {
		stmt = Skip.Instance; 
		if (StartOf(2)) {
			Declaration(out stmt);
		} else if (la.kind == 5) {
			Print(out stmt);
		} else if (la.kind == 8) {
			If(out stmt);
		} else SynErr(32);
	}

	void Declaration(out Stmt stmt) {
		Type(out Type type);
		Expect(1);
		string var = t.val; stmt = new Declaration(type, var, t.line); 
		Expect(7);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Comp(stmt, new Assign(var, expr, lineNumber)); 
		Expect(6);
	}

	void Print(out Stmt stmt) {
		Expect(5);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Print(expr, lineNumber); 
		Expect(6);
	}

	void If(out Stmt stmt) {
		Stmt elseStmt = Skip.Instance; List<If> elseIfStmts = new List<If>(); int lineNumber = -1; 
		Expect(8);
		lineNumber = t.line; 
		Expect(9);
		Expr(out Expr condition);
		Expect(10);
		Expect(11);
		Stmt(out Stmt thenStmt);
		Expect(12);
		while (la.kind == 13) {
			Get();
			Expect(9);
			Expr(out Expr elseIfCondition);
			Expect(10);
			Expect(11);
			Stmt(out Stmt elseIfStmt);
			Expect(12);
			elseIfStmts.Add(new If(elseIfCondition, elseIfStmt, null, Skip.Instance, lineNumber)); 
		}
		if (la.kind == 14) {
			Get();
			Expect(11);
			Stmt(out elseStmt);
			Expect(12);
		}
		stmt = new If(condition, thenStmt, elseIfStmts, elseStmt, lineNumber); 
	}

	void Expr(out Expr expr) {
		BinaryOperators op = BinaryOperators.OR; int lineNumber = -1; 
		EqExpr(out expr);
		while (la.kind == 19 || la.kind == 20) {
			if (la.kind == 19) {
				Get();
				op = BinaryOperators.OR; lineNumber = t.line; 
			} else {
				Get();
				op = BinaryOperators.AND; lineNumber = t.line; 
			}
			EqExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void Type(out Type type) {
		type = null; 
		if (la.kind == 15) {
			Get();
			type = IntT.Instance; 
		} else if (la.kind == 16) {
			Get();
			type = FloatT.Instance; 
		} else if (la.kind == 17) {
			Get();
			type = BoolT.Instance; 
		} else if (la.kind == 18) {
			Get();
			type = StringT.Instance; 
		} else SynErr(33);
	}

	void EqExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.EQ; int lineNumber = -1; 
		RelExpr(out expr);
		while (la.kind == 21 || la.kind == 22) {
			if (la.kind == 21) {
				Get();
				op = BinaryOperators.EQ; lineNumber = t.line; 
			} else {
				Get();
				op = BinaryOperators.NEQ; lineNumber = t.line; 
			}
			RelExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void RelExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.LT; int lineNumber = -1; 
		PlusExpr(out expr);
		while (la.kind == 23) {
			Get();
			lineNumber = t.line; 
			PlusExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void PlusExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.ADD; int lineNumber = -1; 
		MulExpr(out expr);
		while (la.kind == 24 || la.kind == 25) {
			if (la.kind == 24) {
				Get();
				op = BinaryOperators.ADD; lineNumber = t.line; 
			} else {
				Get();
				op = BinaryOperators.SUB; lineNumber = t.line; 
			}
			MulExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void MulExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.MUL; int lineNumber = -1; 
		UnaryExpr(out expr);
		while (la.kind == 26 || la.kind == 27) {
			if (la.kind == 26) {
				Get();
				op = BinaryOperators.MUL; lineNumber = t.line; 
			} else {
				Get();
				op = BinaryOperators.DIV; lineNumber = t.line; 
			}
			UnaryExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void UnaryExpr(out Expr expr) {
		List<char> unaries = new List<char>(); int lineNumber = -1; 
		while (la.kind == 28) {
			Get();
			unaries.Add('!'); lineNumber = t.line; 
		}
		Term(out Expr expr2);
		unaries.Reverse(); expr = applyUnaries(unaries, expr2, lineNumber); 
	}

	void Term(out Expr expr) {
		expr = null; 
		switch (la.kind) {
		case 1: {
			Get();
			expr = new Ref(t.val, t.line); 
			break;
		}
		case 2: {
			Get();
			expr = new IntV(Int32.Parse(t.val), t.line); 
			break;
		}
		case 3: {
			Get();
			expr = new FloatV(float.Parse(t.val, new CultureInfo("en", false)), t.line); 
			break;
		}
		case 29: {
			Get();
			expr = new BoolV(true, t.line); 
			break;
		}
		case 30: {
			Get();
			expr = new BoolV(false, t.line); 
			break;
		}
		case 4: {
			Get();
			expr = new StringV(t.val, t.line); 
			break;
		}
		case 9: {
			Get();
			Expr(out expr);
			Expect(10);
			break;
		}
		default: SynErr(34); break;
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Matilda();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_T,_x,_x, _T,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x}

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
			case 3: s = "FLOAT expected"; break;
			case 4: s = "STRING expected"; break;
			case 5: s = "\"print\" expected"; break;
			case 6: s = "\";\" expected"; break;
			case 7: s = "\"=\" expected"; break;
			case 8: s = "\"if\" expected"; break;
			case 9: s = "\"(\" expected"; break;
			case 10: s = "\")\" expected"; break;
			case 11: s = "\"{\" expected"; break;
			case 12: s = "\"}\" expected"; break;
			case 13: s = "\"elseif\" expected"; break;
			case 14: s = "\"else\" expected"; break;
			case 15: s = "\"int\" expected"; break;
			case 16: s = "\"float\" expected"; break;
			case 17: s = "\"bool\" expected"; break;
			case 18: s = "\"string\" expected"; break;
			case 19: s = "\"||\" expected"; break;
			case 20: s = "\"&&\" expected"; break;
			case 21: s = "\"==\" expected"; break;
			case 22: s = "\"!=\" expected"; break;
			case 23: s = "\"<\" expected"; break;
			case 24: s = "\"+\" expected"; break;
			case 25: s = "\"-\" expected"; break;
			case 26: s = "\"*\" expected"; break;
			case 27: s = "\"/\" expected"; break;
			case 28: s = "\"!\" expected"; break;
			case 29: s = "\"true\" expected"; break;
			case 30: s = "\"false\" expected"; break;
			case 31: s = "??? expected"; break;
			case 32: s = "invalid Stmt"; break;
			case 33: s = "invalid Type"; break;
			case 34: s = "invalid Term"; break;

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