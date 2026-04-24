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
	public const int maxT = 41;

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
		switch (la.kind) {
		case 24: case 25: case 26: case 27: {
			Declaration(out stmt);
			break;
		}
		case 1: {
			Assignment(out stmt);
			break;
		}
		case 16: {
			FunctionDeclaration(out stmt);
			break;
		}
		case 5: {
			Print(out stmt);
			break;
		}
		case 20: {
			If(out stmt);
			break;
		}
		case 23: {
			While(out stmt);
			break;
		}
		case 19: {
			Return(out stmt);
			break;
		}
		case 9: {
			SchemaDeclaration(out stmt);
			break;
		}
		case 13: {
			TableDeclaration(out stmt);
			break;
		}
		default: SynErr(42); break;
		}
	}

	void Declaration(out Stmt stmt) {
		Type(out Type type);
		Expect(1);
		string var = t.val; 
		Expect(7);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Declaration(type, var, expr, t.line); 
		Expect(6);
	}

	void Assignment(out Stmt stmt) {
		Expect(1);
		string var = t.val;
		Expect(7);
		int lineNumber = t.line; 
		Expr(out Expr expr);
		stmt = new Assign(var, expr, lineNumber); 
		Expect(6);
	}

	void FunctionDeclaration(out Stmt stmt) {
		Expect(16);
		Type(out Type type);
		Expect(1);
		string var = t.val; int lineNumber = t.line; List<Parameter> parameters = new List<Parameter>(); List<Stmt> bodyStmts = new List<Stmt>(); Stmt funcBody = Skip.Instance; 
		Expect(17);
		if (StartOf(2)) {
			Parameter(out Parameter param);
			parameters.Add(param); 
			while (la.kind == 11) {
				Get();
				Parameter(out param);
				parameters.Add(param); 
			}
		}
		Expect(18);
		Expect(10);
		Stmt(out funcBody);
		bodyStmts.Add(funcBody); 
		while (StartOf(1)) {
			Stmt(out funcBody);
			bodyStmts.Add(funcBody); 
		}
		Expect(12);
		stmt = new FunctionDeclaration(type, var, parameters, bodyStmts, lineNumber); 
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
		Expect(20);
		lineNumber = t.line; 
		Expect(17);
		Expr(out Expr condition);
		Expect(18);
		Expect(10);
		Stmts(out Stmt thenStmt);
		Expect(12);
		while (la.kind == 21) {
			Get();
			Expect(17);
			Expr(out Expr elseIfCondition);
			Expect(18);
			Expect(10);
			Stmts(out Stmt elseIfStmt);
			Expect(12);
			elseIfStmts.Add(new If(elseIfCondition, elseIfStmt, null, Skip.Instance, lineNumber)); 
		}
		if (la.kind == 22) {
			Get();
			Expect(10);
			Stmts(out elseStmt);
			Expect(12);
		}
		stmt = new If(condition, thenStmt, elseIfStmts, elseStmt, lineNumber); 
	}

	void While(out Stmt stmt) {
		Expect(23);
		int lineNumber = t.line; 
		Expect(17);
		Expr(out Expr condition);
		Expect(18);
		Expect(10);
		Stmts(out Stmt body);
		Expect(12);
		stmt = new While(condition, body, lineNumber); 
	}

	void Return(out Stmt stmt) {
		Expect(19);
		Expr(out Expr expr);
		stmt = new Return(expr, t.line); 
		Expect(6);
	}

	void SchemaDeclaration(out Stmt stmt) {
		List<Column> cols = new List<Column>(); int lineNumber = t.line; 
		Expect(9);
		Expect(1);
		string ident = t.val; 
		Expect(7);
		Expect(10);
		Column(out Column col);
		cols.Add(col); 
		while (la.kind == 11) {
			Get();
			Column(out col);
			cols.Add(col); 
		}
		Expect(12);
		stmt = new SchemaDeclaration(ident, cols, lineNumber); 
	}

	void TableDeclaration(out Stmt stmt) {
		Expect(13);
		Expect(14);
		Expect(1);
		string schemaId = t.val; 
		Expect(15);
		Expect(1);
		string var = t.val; int lineNumber = t.line; 
		Expect(7);
		Expr(out Expr expr);
		Expect(6);
		stmt = new TableDeclaration(var, schemaId, expr, lineNumber); 
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

	void Type(out Type type) {
		type = null; 
		if (la.kind == 24) {
			Get();
			type = IntT.Instance; 
		} else if (la.kind == 25) {
			Get();
			type = FloatT.Instance; 
		} else if (la.kind == 26) {
			Get();
			type = BoolT.Instance; 
		} else if (la.kind == 27) {
			Get();
			type = StringT.Instance; 
		} else SynErr(43);
	}

	void Column(out Column column) {
		Expect(1);
		string id = t.val; 
		Expect(8);
		Type(out Type type);
		column = new Column(id, type); 
	}

	void Parameter(out Parameter param) {
		Type(out Type type);
		Expect(1);
		param = new Parameter(type, t.val, t.line); 
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
		while (la.kind == 14) {
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
			if (la.kind == 17) {
				Get();
				List<Expr> arguments = new List<Expr>(); 
				if (StartOf(3)) {
					Expr(out Expr argument);
					arguments.Add(argument); 
					while (la.kind == 11) {
						Get();
						Expr(out argument);
						arguments.Add(argument); 
					}
				}
				Expect(18);
				expr = new FunctionRef(name, arguments, lineNumber); 
			} else if (StartOf(4)) {
				expr = new Ref(name, lineNumber); 
			} else SynErr(44);
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
			expr = new StringV(t.val, t.line); 
			break;
		}
		case 17: {
			Get();
			Expr(out expr);
			Expect(18);
			break;
		}
		case 39: {
			Get();
			lineNumber = t.line; 
			Expect(17);
			Expect(4);
			expr = new Read(t.val, lineNumber); 
			Expect(18);
			break;
		}
		case 40: {
			Get();
			lineNumber = t.line; 
			Expect(17);
			Expr(out Expr tableExpr);
			Expect(11);
			Expr(out Expr predicate);
			Expect(18);
			expr = new FilterExpr(tableExpr, predicate, lineNumber); 
			break;
		}
		default: SynErr(45); break;
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_x,_x, _x,_T,_x,_x, _x,_T,_x,_x, _x,_T,_x,_x, _T,_x,_x,_T, _T,_x,_x,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _x,_x,_T,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x}

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
			case 8: s = "\":\" expected"; break;
			case 9: s = "\"schema\" expected"; break;
			case 10: s = "\"{\" expected"; break;
			case 11: s = "\",\" expected"; break;
			case 12: s = "\"}\" expected"; break;
			case 13: s = "\"table\" expected"; break;
			case 14: s = "\"<\" expected"; break;
			case 15: s = "\">\" expected"; break;
			case 16: s = "\"function\" expected"; break;
			case 17: s = "\"(\" expected"; break;
			case 18: s = "\")\" expected"; break;
			case 19: s = "\"return\" expected"; break;
			case 20: s = "\"if\" expected"; break;
			case 21: s = "\"elseif\" expected"; break;
			case 22: s = "\"else\" expected"; break;
			case 23: s = "\"while\" expected"; break;
			case 24: s = "\"int\" expected"; break;
			case 25: s = "\"float\" expected"; break;
			case 26: s = "\"bool\" expected"; break;
			case 27: s = "\"string\" expected"; break;
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
			case 39: s = "\"read\" expected"; break;
			case 40: s = "\"FILTER\" expected"; break;
			case 41: s = "??? expected"; break;
			case 42: s = "invalid Stmt"; break;
			case 43: s = "invalid Type"; break;
			case 44: s = "invalid Term"; break;
			case 45: s = "invalid Term"; break;

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