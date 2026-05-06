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
	public const int maxT = 40;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Program mainNode = null;

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
		List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration>(); 
		while (la.kind == 5 || la.kind == 11) {
			TopLevelDeclaration(out TopLevelDeclaration topLevelDeclaration);
			topLevelDeclarations.Add(topLevelDeclaration); 
		}
		Stmts(out Stmt stmt);
		mainNode = new Program(topLevelDeclarations, stmt); 
	}

	void TopLevelDeclaration(out TopLevelDeclaration topLevelDeclaration) {
		topLevelDeclaration = null; 
		if (la.kind == 5) {
			FunctionDeclaration(out topLevelDeclaration);
		} else if (la.kind == 11) {
			SchemaDeclaration(out topLevelDeclaration);
		} else SynErr(41);
	}

	void Stmts(out Stmt stmt) {
		List<Stmt> list = new List<Stmt>(); 
		while (StartOf(1)) {
			Stmt(out Stmt temp);
			list.Add(temp); 
		}
		stmt = ToComp(list); 
	}

	void FunctionDeclaration(out TopLevelDeclaration topLevelDeclaration) {
		Type type = null; 
		Expect(5);
		Type(out type);
		Expect(1);
		string var = t.val; int lineNumber = t.line; List<Parameter> parameters = new List<Parameter>(); Stmt funcBody = Skip.Instance; 
		Expect(6);
		if (StartOf(2)) {
			Parameter(out Parameter param);
			parameters.Add(param); 
			while (la.kind == 7) {
				Get();
				Parameter(out param);
				parameters.Add(param); 
			}
		}
		Expect(8);
		Expect(9);
		Stmts(out funcBody);
		Expect(10);
		topLevelDeclaration = new FunctionDeclaration(type, var, parameters, funcBody, lineNumber); 
	}

	void SchemaDeclaration(out TopLevelDeclaration topLevelDeclaration) {
		List<Column> cols = new List<Column>(); int lineNumber = t.line; 
		Expect(11);
		Expect(1);
		string ident = t.val; 
		Expect(12);
		Expect(9);
		Column(out Column col);
		cols.Add(col); 
		while (la.kind == 7) {
			Get();
			Column(out col);
			cols.Add(col); 
		}
		Expect(10);
		topLevelDeclaration = new SchemaDeclaration(ident, cols, lineNumber); 
	}

	void Parameter(out Parameter param) {
		Type type = null; 
		Type(out type);
		Expect(1);
		param = new Parameter(type, t.val, t.line); 
	}

	void Type(out Type type) {
		type = null; 
		if (la.kind == 21) {
			Get();
			type = IntT.Instance; 
		} else if (la.kind == 22) {
			Get();
			type = FloatT.Instance; 
		} else if (la.kind == 23) {
			Get();
			type = BoolT.Instance; 
		} else if (la.kind == 24) {
			Get();
			type = StringT.Instance; 
		} else if (la.kind == 25) {
			Get();
			Expect(26);
			Expect(1);
			type = new TableT(t.val); 
			Expect(27);
		} else SynErr(42);
	}

	void Column(out Column column) {
		Expect(1);
		string id = t.val; 
		Expect(16);
		Type(out Type type);
		column = new Column(id, type); 
	}

	void Stmt(out Stmt stmt) {
		stmt = Skip.Instance; 
		switch (la.kind) {
		case 21: case 22: case 23: case 24: case 25: {
			LocalDeclaration(out stmt);
			break;
		}
		case 1: {
			Assignment(out stmt);
			break;
		}
		case 13: {
			Print(out stmt);
			break;
		}
		case 18: {
			If(out stmt);
			break;
		}
		case 20: {
			While(out stmt);
			break;
		}
		case 17: {
			Return(out stmt);
			break;
		}
		default: SynErr(43); break;
		}
	}

	void LocalDeclaration(out Stmt stmt) {
		stmt = Skip.Instance; 
		Type(out Type type);
		Expect(1);
		string var = t.val; 
		Expect(12);
		int lineNumber = t.line; 
		if (StartOf(3)) {
			Expr(out Expr expr);
			stmt = new LocalDeclaration(type, var, expr, t.line); 
		} else if (la.kind == 15) {
			Get();
			Expect(6);
			Expect(4);
			string STR = t.val; 
			Expect(8);
			stmt = new TableDeclaration(type, var, STR.Substring(1, STR.Length - 2), lineNumber); 
		} else SynErr(44);
		Expect(14);
	}

	void Assignment(out Stmt stmt) {
		Expect(1);
		string var = t.val;
		Expect(12);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Assign(var, expr, lineNumber); 
		Expect(14);
	}

	void Print(out Stmt stmt) {
		Expect(13);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Print(expr, lineNumber); 
		Expect(14);
	}

	void If(out Stmt stmt) {
		Stmt elseStmt = Skip.Instance; int lineNumber = -1; 
		Expect(18);
		lineNumber = t.line; 
		Expect(6);
		Expr(out Expr condition);
		Expect(8);
		Expect(9);
		Stmts(out Stmt thenStmt);
		Expect(10);
		if (la.kind == 19) {
			Get();
			Expect(9);
			Stmts(out elseStmt);
			Expect(10);
		}
		stmt = new If(condition, thenStmt, elseStmt, lineNumber); 
	}

	void While(out Stmt stmt) {
		Expect(20);
		int lineNumber = t.line; 
		Expect(6);
		Expr(out Expr condition);
		Expect(8);
		Expect(9);
		Stmts(out Stmt body);
		Expect(10);
		stmt = new While(condition, body, lineNumber); 
	}

	void Return(out Stmt stmt) {
		Expect(17);
		Expr(out Expr expr);
		stmt = new Return(expr, t.line); 
		Expect(14);
	}

	void Expr(out Expr expr) {
		BinaryOperators op = BinaryOperators.OR; int lineNumber = -1; 
		EqExpr(out expr);
		while (la.kind == 28 || la.kind == 29) {
			if (la.kind == 28) {
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

	void EqExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.EQ; int lineNumber = -1; 
		RelExpr(out expr);
		while (la.kind == 30 || la.kind == 31) {
			if (la.kind == 30) {
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
		while (la.kind == 26) {
			Get();
			lineNumber = t.line; 
			PlusExpr(out Expr expr2);
			expr = new BinaryOp(op, expr, expr2, lineNumber); 
		}
	}

	void PlusExpr(out Expr expr) {
		BinaryOperators op = BinaryOperators.ADD; int lineNumber = -1; 
		MulExpr(out expr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
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
		while (la.kind == 34 || la.kind == 35) {
			if (la.kind == 34) {
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
		while (la.kind == 36) {
			Get();
			unaries.Add('!'); lineNumber = t.line; 
		}
		Term(out Expr expr2);
		unaries.Reverse(); expr = applyUnaries(unaries, expr2, lineNumber); 
	}

	void Term(out Expr expr) {
		expr = null; int lineNumber = -1; 
		switch (la.kind) {
		case 1: {
			Get();
			string name = t.val; lineNumber = t.line; 
			if (la.kind == 6) {
				Get();
				List<Expr> arguments = new List<Expr>(); 
				if (StartOf(3)) {
					Expr(out Expr argument);
					arguments.Add(argument); 
					while (la.kind == 7) {
						Get();
						Expr(out argument);
						arguments.Add(argument); 
					}
				}
				Expect(8);
				expr = new FunctionRef(name, arguments, lineNumber); 
			} else if (StartOf(4)) {
				expr = new Ref(name, lineNumber); 
			} else SynErr(45);
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
		case 37: {
			Get();
			expr = new BoolV(true, t.line); 
			break;
		}
		case 38: {
			Get();
			expr = new BoolV(false, t.line); 
			break;
		}
		case 4: {
			Get();
			expr = new StringV(t.val.Substring(1, t.val.Length - 2), t.line); 
			break;
		}
		case 6: {
			Get();
			Expr(out expr);
			Expect(8);
			break;
		}
		case 39: {
			Get();
			lineNumber = t.line; 
			Expect(6);
			Expr(out Expr tableExpr);
			Expect(7);
			Expr(out Expr predicate);
			Expect(8);
			expr = new FilterExpr(tableExpr, predicate, lineNumber); 
			break;
		}
		default: SynErr(46); break;
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_T,_T,_x, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x}

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
			case 5: s = "\"function\" expected"; break;
			case 6: s = "\"(\" expected"; break;
			case 7: s = "\",\" expected"; break;
			case 8: s = "\")\" expected"; break;
			case 9: s = "\"{\" expected"; break;
			case 10: s = "\"}\" expected"; break;
			case 11: s = "\"schema\" expected"; break;
			case 12: s = "\"=\" expected"; break;
			case 13: s = "\"print\" expected"; break;
			case 14: s = "\";\" expected"; break;
			case 15: s = "\"read\" expected"; break;
			case 16: s = "\":\" expected"; break;
			case 17: s = "\"return\" expected"; break;
			case 18: s = "\"if\" expected"; break;
			case 19: s = "\"else\" expected"; break;
			case 20: s = "\"while\" expected"; break;
			case 21: s = "\"int\" expected"; break;
			case 22: s = "\"float\" expected"; break;
			case 23: s = "\"bool\" expected"; break;
			case 24: s = "\"string\" expected"; break;
			case 25: s = "\"table\" expected"; break;
			case 26: s = "\"<\" expected"; break;
			case 27: s = "\">\" expected"; break;
			case 28: s = "\"||\" expected"; break;
			case 29: s = "\"&&\" expected"; break;
			case 30: s = "\"==\" expected"; break;
			case 31: s = "\"!=\" expected"; break;
			case 32: s = "\"+\" expected"; break;
			case 33: s = "\"-\" expected"; break;
			case 34: s = "\"*\" expected"; break;
			case 35: s = "\"/\" expected"; break;
			case 36: s = "\"!\" expected"; break;
			case 37: s = "\"true\" expected"; break;
			case 38: s = "\"false\" expected"; break;
			case 39: s = "\"FILTER\" expected"; break;
			case 40: s = "??? expected"; break;
			case 41: s = "invalid TopLevelDeclaration"; break;
			case 42: s = "invalid Type"; break;
			case 43: s = "invalid Stmt"; break;
			case 44: s = "invalid LocalDeclaration"; break;
			case 45: s = "invalid Term"; break;
			case 46: s = "invalid Term"; break;

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