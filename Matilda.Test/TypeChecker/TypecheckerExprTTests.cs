using Matilda;

namespace MatildaTests;


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
    public void AddTestTypeChecker()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        IntT.Instance,
        "x",
        new BinaryOp(BinaryOperators.ADD, new IntV(5, 1), new IntV(3, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void AddTestTypeCheckerFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BinaryOp(BinaryOperators.ADD,
                        new IntV(5, 1),
                        new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            "Line 1: Operator '+' expected a right operand of type 'int' or 'float', but got 'Matilda.BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
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
public class FilterExprTestsTypechecker : RunTypeChecker
{
    [TestMethod]
    public void FilterExprTestType()
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
            new LocalDeclaration(new TableT("schema1"), "x", new FilterExpr(new Ref("tab1", -1), new BoolV(true, 1), -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
    // check not tableT and not bool for agr 2
    [TestMethod]
    public void FilterExprTestTypeFailsArg1()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(new TableT("test"), "x", new FilterExpr(new IntV(2, 1), new IntV(5, 1), -1), -1);
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
    public void FilterExprTestTypeFailsArg2()
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
            new LocalDeclaration(new TableT("schema1"), "x", new FilterExpr(new Ref("tab1", -1), new IntV(5, 1), -1), -1)
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
