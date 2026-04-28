using System.Windows.Markup;
using Matilda;

namespace MatildaTests;


[TestClass]
public class UnaryOpTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void NeqTestTypecheck()
    {
        //arrange
        Stmt stmt = new Declaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(stmt);
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
    [TestMethod]
    public void NeqTestTypecheckFails()
    {
        //arrange
        Stmt stmt = new Declaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new IntV(5, 1), -1), -1);
        //act
        var checker = Run(stmt);
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
        Stmt stmt = new Declaration(
        IntT.Instance,
        "x",
        new BinaryOp(BinaryOperators.ADD, new IntV(5, 1), new IntV(3, 1), -1), -1);
        //act
        var checker = Run(stmt);
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void AddTestTypeCheckerFails()
    {
        //arrange
        Stmt stmt = new Print(new BinaryOp(BinaryOperators.ADD,
                        new IntV(5, 1),
                        new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
        {
            "Line 1: Operator '+' expected a right operand of type 'int' or 'float', but got 'Matilda.BoolT'."
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
        Stmt stmt = new Print(new Ref("x", -1), -1);
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void RefTestTypeCheckerDeclared()
    {
        //arrange
        Stmt stmt = new Comp(
          new Declaration(IntT.Instance, "x", new IntV(5, 1), -1),
          new Print(new Ref("x", -1), -1)
      );
        //act
        var checker = Run(stmt);
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
}
[TestClass]
public class FunctionRefTestsTypeChecker : RunTypeChecker
{

}