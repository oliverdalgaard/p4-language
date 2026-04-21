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
        var expr = new Ref("x", 1);

        // Act
        var unaryOp = new UnaryOp(UnaryOperators.NOT, expr, 2);

        // Assert
        Assert.AreEqual(UnaryOperators.NOT, unaryOp.Op);
        Assert.AreSame(expr, unaryOp.Expr);
        Assert.AreEqual(2, unaryOp.LineNumber);
    }
}

[TestClass]
public class BinaryOpTests
{
    [TestMethod]
    public void SetBinaryOpProperties()
    {
        // Arrange
        var exprLeft = new IntV(5,7);
        var exprRight = new IntV(10,5);

        // Act
        var binaryOp = new BinaryOp(BinaryOperators.ADD, exprLeft, exprRight, 2);

        // Assert
        Assert.AreEqual(BinaryOperators.ADD, binaryOp.Op);
        Assert.AreSame(exprLeft, binaryOp.ExprLeft);
        Assert.AreSame(exprRight, binaryOp.ExprRight);
        Assert.AreEqual(2, binaryOp.LineNumber);
    }
}
