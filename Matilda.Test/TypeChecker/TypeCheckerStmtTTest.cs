using Matilda;
using Mono.Cecil.Cil;

namespace MatildaTests;

public abstract class RunTypeChecker
{
    public TypeChecker Run(Stmt stmt)
    {
        var envVT = new EnvVT();
        var envPT = new EnvPT();
        var envST = new EnvST();

        return new TypeChecker(stmt, envVT, envPT, envST);
    }
}
[TestClass]
public class CompTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void CompcheckBothStatementsFails()
    {
        // arrange 
        Stmt stmt = new Comp(
            new Assign("x", new IntV(5, 1), -1), // error
            new Assign("y", new IntV(10, 2), -1) // error
        );
        // act 
        var checker = Run(stmt);
        // assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
        "Line -1: Variable y is not declared."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void CompcheckBothStatements()
    {
        // arrange
        Stmt stmt = new Comp(new Declaration(IntT.Instance, "x", new IntV(5, 1), -1), new Assign("x", new IntV(10, 2), -1));
        // act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}

[TestClass]
public class IfTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void IfCheckCondition()
    {
        //arrange
        Stmt stmt = new If(new BoolV(true, -1), new Declaration(IntT.Instance, "x", new IntV(5, 1), -1), null, null, -1);
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
    [TestMethod]
    public void IfCheckBody()
    {
        //arrange
        Stmt stmt = new If(new BoolV(true, -1),
            new Assign("x", new IntV(5, 1), -1),    //error
            null, null, -1);
        //act
        var checker = Run(stmt);
        // assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

[TestClass]
public class WhileTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void WhileCheckCondition()
    {
        //arrange
        Stmt stmt = new While(new BoolV(true, -1), new Declaration(IntT.Instance, "x", new IntV(5, 1), -1), -1);
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
    [TestMethod]
    public void WhileCheckBody()
    {
        //arrange
        Stmt stmt = new While(new BoolV(true, -1),
            new Assign("x", new IntV(5, 1), -1), -1);    //error
        //act
        var checker = Run(stmt);
        // assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

[TestClass]
public class AssignTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void AssignCheckNotDeclared()
    {
        //arrange
        Stmt stmt = new Assign("x", new IntV(5, 1), -1);
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void AssignCheckWrongType()
    {
        //arrange
        Stmt stmt = new Comp(new Declaration(IntT.Instance, "x", new IntV(5, 1), -1),
        new Assign("x", new BoolV(true, 2), -1));
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
    {
        "Line -1: Cannot assign 'Matilda.BoolT' to variable 'x' of type 'Matilda.IntT'.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void AssignCheck()
    {
        //arrange
        Stmt stmt = new Comp(new Declaration(IntT.Instance, "x", new IntV(5, 1), -1),
        new Assign("x", new IntV(10, 2), -1));
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}

[TestClass]
public class DeclarationTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void DeclarationCheckWrongType()
    {
        //arrange
        Stmt stmt = new Declaration(IntT.Instance, "x", new BoolV(true, 1), -1);
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
    {
        "Line -1: Declaration type does not match the type of the expression.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void DeclarationCheck()
    {
        //arrange
        Stmt stmt = new Declaration(IntT.Instance, "x", new IntV(5, 1), -1);
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}

[TestClass]
public class FunctionTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void FunctionDeclarationCheckbodyAndReturn()
    {
        //arrange
        Stmt stmt = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new List<Stmt>
        {
            new Declaration(IntT.Instance, "x", new BoolV(true, 1), -1),
            new Return(new BoolV(true, 1), -1)
        },
        -1);
        //act
        var checker = Run(stmt);
        //assert
        var expected = new List<string>
    {
        "Line -1: Declaration type does not match the type of the expression.",
        "Line -1: Return type 'Matilda.BoolT' does not match function return type 'Matilda.IntT'.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }


    [TestMethod]
    public void FunctionDeclarationCheck()
    {
        //arrange
        Stmt stmt = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new List<Stmt>
        {
            new Declaration(IntT.Instance, "x", new IntV(5, 1), -1),
            new Return(new IntV(5, 1), -1)
        },
        -1);
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}
// return
[TestClass]
public class ReturnTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void ReturnCheckOutSideFunc()
    {
        //arrange
        Stmt stmt = new Return(new IntV(5, 1), -1);
        //act
        var checker = Run(stmt);
        // assert
        var expected = new List<string>
    {
        "Line -1: Return outside of a function is not allowed.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void ReturnCheck()
    {
        //arrange
        Stmt stmt = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new List<Stmt>
        {
            new Declaration(IntT.Instance, "x", new IntV(5, 1), -1),
            new Return(new IntV(5, 1), -1)
        },
        -1);
        //act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}