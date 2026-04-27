using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matilda;
using System.Collections.Generic;

namespace MatildaTests;

[TestClass]
public class CompTests
{
    // Comp statements tests to see if they return correct statements.
    [TestMethod]
    public void CompStoresStatements()
    {
        // Arrange
        Stmt stmt1 = new Assign("x", new IntV(5, -1), -1);
        Stmt stmt2 = new Print(new Ref("x", -1), -1);

        // Act
        Comp comp = new Comp(stmt1, stmt2);
        
        // Assert
        Assert.AreSame(stmt1, comp.Stmt1);
        Assert.AreSame(stmt2, comp.Stmt2);
    }
    
    // Test to see if Comp returns correct line numbers for statements.
    [TestMethod]
    public void CompLineNumberReturnsFirstStatementLineNumber()
    {
        // Arrange
        Stmt stmt1 = new Assign("x", new IntV(5, -1), -1);
        Stmt stmt2 = new Print(new Ref("x", -1), -2);

        Comp comp = new Comp(stmt1, stmt2);

        Assert.AreEqual(-1, comp.Stmt1!.LineNumber);
        Assert.AreEqual(-2, comp.Stmt2!.LineNumber);
    }

    [TestMethod]
    public void CompLineNumberThrowsWhenStmt1IsNull()
    {
        Comp comp = new Comp(null, new Print(new IntV(1, -1), -1));

        try
        {
            int line = comp.LineNumber;
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (Exception)
        {
            // Expected
        }
    }
}

[TestClass]
public class ParameterTests
    {
    [TestMethod]
    public void ParameterStoresProperties()
    {
        // Arrange
        IntT expectedType = new IntT();
        string expectedIdentifier = "x";
        int expectedLineNumber = -1;

        // Act
        Parameter parameter = new Parameter(expectedType, expectedIdentifier, expectedLineNumber);

        // Assert
        Assert.AreSame(expectedType, parameter.Type);
        Assert.AreEqual(expectedIdentifier, parameter.Identifier);
        Assert.AreEqual(expectedLineNumber, parameter.LineNumber);
    }
}

[TestClass]
public class DeclarationTests
{
    [TestMethod]
    public void DeclarationStoresProperties()
    {
        // Arrange
        IntT expectedType = new IntT();
        string expectedIdentifier = "Test";
        Expr expectedExpression = new IntV(10, -1);
        int expectedLineNumber = -1;

        // Act
        Declaration declaration = new Declaration(expectedType, expectedIdentifier, expectedExpression, expectedLineNumber);

        // Assert
        Assert.AreSame(expectedType, declaration.Type);
        Assert.AreEqual(expectedIdentifier, declaration.Identifier);
        Assert.AreSame(expectedExpression, declaration.Expression);
        Assert.AreEqual(expectedLineNumber, declaration.LineNumber);
    }
}

[TestClass]
public class AssignTests
{
    [TestMethod]
    public void AssignStoresProperties()
    {
        // Arrange
        string expectedIdentifier = "x";
        Expr expectedValue = new IntV(5,-1);
        int expectedLineNumber = -1;

        // Act
        Assign assign = new Assign(expectedIdentifier, expectedValue, expectedLineNumber);

        // Assert
        Assert.AreEqual(expectedIdentifier, assign.Identifier);
        Assert.AreSame(expectedValue, assign.Value);
        Assert.AreEqual(expectedLineNumber, assign.LineNumber);
    }
}

[TestClass]
public class FunctionDeclarationTests
{
    [TestMethod]
    public void FunctionDeclarationStoresProperties()
    {
        // Arrange
        IntT expectedType = new IntT();
        string expectedIdentifier = "x";
        var expectedParameters = new List<Parameter>
        {
            new Parameter(new IntT(), "x", -1)
        };
        var expectedBody = new List<Stmt>
        {
            new Return(new Ref("x", -1), -2)
        };

        // Act
        FunctionDeclaration function = new FunctionDeclaration(
            expectedType,
            expectedIdentifier,
            expectedParameters,
            expectedBody,
            -1
        );

        // Assert
        Assert.AreEqual(expectedType, function.Type);
        Assert.AreEqual(expectedIdentifier, function.Identifier);
        Assert.AreSame(expectedParameters, function.Parameters);
        Assert.AreSame(expectedBody, function.Body);
        Assert.AreEqual(-1, function.LineNumber);
    }
}

[TestClass]
public class ReturnTests
{
    [TestMethod]
    public void ReturnStoresProperties()
    {
        // Arrange
        Expr expectedValue = new IntV(5,-1);
        int expectedLineNumber = -1;

        // Act
        Return returnStmt = new Return(expectedValue, expectedLineNumber);

        // Assert
        Assert.AreSame(expectedValue, returnStmt.Value);
        Assert.AreEqual(expectedLineNumber, returnStmt.LineNumber);
    }
}

[TestClass]
public class PrintTests
{
    [TestMethod]
    public void PrintStoresProperties()
    {
        // Arrange
        Expr expectedValue = new IntV(5,-1);
        int expectedLineNumber = -1;

        // Act
        Print print = new Print(expectedValue, expectedLineNumber);

        // Assert
        Assert.AreSame(expectedValue, print.Value);
        Assert.AreEqual(expectedLineNumber, print.LineNumber);
    }
}

[TestClass]
public class IfTests
{
    [TestMethod]
    public void IfStoresProperties()
    {
        // Arrange
        Expr condition = new BoolV(true, -1);
        Stmt thenBody = new Print(new StringV("then", -1), -1);
        var elseIfs = new List<If>();
        Stmt elseBody = new Print(new StringV("else", -1), -1);

        // Act
        If ifStmt = new If(condition, thenBody, elseIfs, elseBody, -1);

        // Assert
        Assert.AreSame(condition, ifStmt.Condition);
        Assert.AreSame(thenBody, ifStmt.ThenBody);
        Assert.AreSame(elseIfs, ifStmt.ElseIfStmts);
        Assert.AreSame(elseBody, ifStmt.ElseBody);
        Assert.AreEqual(-1, ifStmt.LineNumber);
    }
}

[TestClass]
public class WhileTests
{
    [TestMethod]
    public void WhileStoresProperties()
    {
        // Arrange
        Expr condition = new BoolV(true, -1);
        Stmt body = new Print(new StringV("loop", -1), -1);

        // Act
        While whileStmt = new While(condition, body, -1);

        // Assert
        Assert.AreSame(condition, whileStmt.Condition);
        Assert.AreSame(body, whileStmt.Body);
        Assert.AreEqual(-1, whileStmt.LineNumber);
    }
}
