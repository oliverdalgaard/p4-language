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

    [TestMethod]
    public void SetJoinPropertiesTest()
    {
        // Arrange
        Expr joinTableExpr1 = new Ref("TestTable", -1);
        Expr joinTableExpr2 = new Ref("TestTable2", -1);
        string keyCol1 = "id";
        string keyCol2 = "id";
        string resultSchemaId = "resultSchema";

        // Act
        Join joinExpr = new Join(joinTableExpr1, joinTableExpr2, keyCol1, keyCol2, resultSchemaId, -1);

        // Assert
        Assert.AreEqual(joinTableExpr1, joinExpr.JoinOnTableExpr);
        Assert.AreEqual(joinTableExpr2, joinExpr.JoinFromTableExpr);
        Assert.AreEqual(keyCol1, joinExpr.KeyColumn1);
        Assert.AreEqual(keyCol2, joinExpr.KeyColumn2);
        Assert.AreEqual(resultSchemaId, joinExpr.ResultSchemaId);
        Assert.AreEqual(-1, joinExpr.LineNumber);
    }
}