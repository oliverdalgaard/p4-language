using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matilda;
using System;

namespace MatildaTests;

[TestClass]
public class UnaryOpTests
{
    [TestMethod]
    // Start unit tests
    public void SetUnaryOpProperties()
    {
        // Arrange
        var expr = new BoolV(true, -1);

        // Act
        var unaryOp = new UnaryOp(UnaryOperators.NOT, expr, -1);

        // Assert
        Assert.AreEqual(UnaryOperators.NOT, unaryOp.Op);
        Assert.AreSame(expr, unaryOp.Expr);
        Assert.AreEqual(-1, unaryOp.LineNumber);
    }
}

[TestClass]
public class BinaryOpTests
{
    [TestMethod]
    public void SetBinaryOpProperties()
    {
        // Arrange
        var exprLeft = new IntV(5, -1);
        var exprRight = new IntV(10, -1);

        // Act
        var binaryOp = new BinaryOp(BinaryOperators.ADD, exprLeft, exprRight, -1);

        // Assert
        Assert.AreEqual(BinaryOperators.ADD, binaryOp.Op);
        Assert.AreSame(exprLeft, binaryOp.ExprLeft);
        Assert.AreSame(exprRight, binaryOp.ExprRight);
        Assert.AreEqual(-1, binaryOp.LineNumber);
    }
}

[TestClass]
public class RefTests
{
    [TestMethod]
    public void SetRefProperties()
    {
        // Arrange
        string expectedName = "x";
        int expectedLineNumber = -1;

        // Act
        var result = new Ref(expectedName, expectedLineNumber);

        // Assert
        Assert.AreEqual(expectedName, result.Name);
        Assert.AreEqual(expectedLineNumber, result.LineNumber);
    }
}

[TestClass]
public class FunctionTests
{
    [TestMethod]
    public void SetFunctionRefProperties()
    {
        // Arrange
        var arguments = new List<Expr>
        {
            new Ref("x",-1),
            new Ref("y",-1)
        };

        // Act
        var result = new FunctionRef("dummyFunction", arguments, -1);

        // Assert
        Assert.HasCount(2, result.Arguments);
        Assert.AreEqual("x", ((Ref)result.Arguments[0]).Name);
        Assert.AreEqual("y", ((Ref)result.Arguments[1]).Name);
    }
}