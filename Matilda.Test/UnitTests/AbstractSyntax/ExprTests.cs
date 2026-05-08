using Matilda;

namespace MatildaTests.UnitTests.AbstractSyntaxTests.ExprTests;

[TestClass]
public class UnaryOpTests
{
    [TestMethod]
    public void SetUnaryOpProperties()
    {
        // Arrange
        Expr expr = new BoolV(true, -1);

        // Act
        UnaryOp unaryOp = new UnaryOp(UnaryOperators.NOT, expr, -1);

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
        Expr exprLeft = new IntV(5, -1);
        Expr exprRight = new IntV(10, -1);

        // Act
        BinaryOp binaryOp = new(BinaryOperators.ADD, exprLeft, exprRight, -1);

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
        Ref result = new(expectedName, expectedLineNumber);

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

[TestClass]
public class FilterTests
{
    [TestMethod]
    public void SetFilterPropertiesTest()
    {
        // Arrangge
        Expr filterTableExpr = new Ref("TestTable", -1);
        Expr predicateExpr = new BoolV(true, -1);

        // Act
        Filter filter = new Filter(filterTableExpr, predicateExpr, -1);

        // Assert
        Assert.AreEqual(filterTableExpr, filter.TableExpr);
        Assert.AreEqual(predicateExpr, filter.Predicate);
        Assert.AreEqual(-1, filter.LineNumber);
    }
}

[TestClass]
public class SumTests
{
    [TestMethod]
    public void SetSumPropertiesTest()
    {
        // Arrange
        Expr sumTableExpr = new Ref("TestTable", -1);
        string groupByColumn = "ID";
        string sumColumn = "Amount";
        string resultSchemaId = "transactionsSchema";

        // Act
        Sum sumExpr = new Sum(sumTableExpr, groupByColumn, sumColumn, resultSchemaId, -1);

        // Assert
        Assert.AreEqual(sumTableExpr, sumExpr.TableExpr);
        Assert.AreEqual(groupByColumn, sumExpr.GroupByColumn);
        Assert.AreEqual(sumColumn, sumExpr.SumColumn);
        Assert.AreEqual(resultSchemaId, sumExpr.ResultSchemaId);
        Assert.AreEqual(-1, sumExpr.LineNumber);
    }
}