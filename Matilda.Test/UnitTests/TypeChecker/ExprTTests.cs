using Matilda;

namespace MatildaTests.UnitTests.TypeCheckerTests.ExprTTests;


[TestClass]
public class UnaryOpTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void NeqTestTypecheck()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void NeqTestTypecheckFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Operator '!' expected a operand of type 'bool', but got 'Matilda.IntT'."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

[TestClass]
public class BinaryOpTestsTypeChecker : RunTypeChecker
{
    //add,sub,mul,div,lt,eq,neq,and,or
    [TestMethod]
    [DataRow(BinaryOperators.ADD)]
    [DataRow(BinaryOperators.SUB)]
    [DataRow(BinaryOperators.MUL)]
    public void BinaryOpTypeCheckerIntReturnTest(BinaryOperators binaryOperator)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        IntT.Instance,
        "x",
        new BinaryOp(binaryOperator, new IntV(5, 1), new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD)]
    [DataRow(BinaryOperators.SUB)]
    [DataRow(BinaryOperators.MUL)]
    [DataRow(BinaryOperators.DIV)]
    public void BinaryOpTypeCheckerFloatReturnTest(BinaryOperators binaryOperator)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        FloatT.Instance,
        "x",
        new BinaryOp(binaryOperator, new FloatV(5, 1), new FloatV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD, "+")]
    [DataRow(BinaryOperators.SUB, "-")]
    [DataRow(BinaryOperators.MUL, "*")]
    [DataRow(BinaryOperators.DIV, "/")]
    public void BinaryOpTypeCheckerFailsLeftOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BinaryOp(binaryOperator,
                        new BoolV(true, 1),
                        new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'int' or 'float', but got 'Matilda.BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD, "+")]
    [DataRow(BinaryOperators.SUB, "-")]
    [DataRow(BinaryOperators.MUL, "*")]
    [DataRow(BinaryOperators.DIV, "/")]
    public void BinaryOpTypeCheckerFailsRightOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(5, 1),
                        new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right operand of type 'int' or 'float', but got 'Matilda.BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void LogicOpTypeCheckerFailsOperandLT()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(BinaryOperators.LT,
                        new StringV("test", 1),
                        new StringV("test", 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '<' expected a left operand of type 'int' or 'float', but got 'Matilda.StringT'.",
            $"Line 1: Operator '<' expected a right operand of type 'int' or 'float', but got 'Matilda.StringT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new StringV("test", 1),
                        new StringV("test", 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'bool','int' or 'float', but got 'Matilda.StringT'.",
            $"Line 1: Operator '{symbol}' expected a right operand of type 'bool','int' or 'float', but got 'Matilda.StringT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand2(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(1, 1),
                        new BoolV(false, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right and left operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand3(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new BoolV(false, 1),
                        new IntV(1, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right and left operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.AND, "&&")]
    [DataRow(BinaryOperators.OR, "||")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand4(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(1, 1),
                        new IntV(1, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'bool', but got 'Matilda.IntT'.",
            $"Line 1: Operator '{symbol}' expected a right operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

//ref, functionRef
[TestClass]
public class RefTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void RefTestTypeCheckerDeclaredFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "f", new Ref("x", -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void RefTestTypeCheckerDeclared()
    {
        //arrange
        Stmt stmt = new Comp(
          new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1),
          new LocalDeclaration(IntT.Instance, "y", new Ref("x", -1), -1)
      );
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
}
[TestClass]
public class FunctionRefTestsTypeChecker : RunTypeChecker
{
    //function not declared
    [TestMethod]
    public void FunctionRefTestTypeCheckerFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new FunctionRef("func1", new List<Expr>(), -1), -1);

        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Function func1 is not declared.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    //function with wrong param
    [TestMethod]
    public void FunctionRefTestTypeCheckerWithWrongParam()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
            new Return(new IntV(5, 1), -1)
        ),
        -1);
        Stmt stmt = new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr> { new IntV(5, 1) }, -1), -1);

        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Function func1 argument count mismatch.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void FunctionRefTestTypeChecker()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
            new Return(new IntV(5, 1), -1)
        ),
        -1);

        Stmt stmt = new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr>(), -1), -1);

        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
}

//filter expr test
[TestClass]
public class FilterTestsTypechecker : RunTypeChecker
{
    [TestMethod]
    public void FilterTestType()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            );

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Filter(new Ref("tab1", -1), new BoolV(true, 1), -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
    // check not tableT and not bool for agr 2
    [TestMethod]
    public void FilterTestTypeFailsArg1()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(new TableT("test"), "x", new Filter(new IntV(2, 1), new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Argument 1 must be of type 'TableT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void FilterTestTypeFailsArg2()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            );

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Filter(new Ref("tab1", -1), new IntV(5, 1), -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Argument 2 must be of type 'BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}
