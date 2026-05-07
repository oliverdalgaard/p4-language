using Matilda;

namespace MatildaTests.UnitTests.ParserTests.ParserTests;

[TestClass]
public class ParserTests : UnitTestHelper
{
    [TestMethod]
    public void ParseDeclarationProgram()
    {
        // Arrange + Act
        Program ast = ParseFile("DeclarationASTTest.matilda");

        // Arange
        Assert.IsInstanceOfType<LocalDeclaration>(ast.Stmt);
        LocalDeclaration declaration = (LocalDeclaration)ast.Stmt;

        // Check identifier name
        Assert.AreEqual("number", declaration.Identifier);

        // Check type
        Assert.IsInstanceOfType(declaration.Type, typeof(IntT));

        // Value assigned to declaration
        Assert.IsNotNull(declaration.Expression);
        Assert.IsInstanceOfType(declaration.Expression, typeof(IntV));

        var value = (IntV)declaration.Expression;
        Assert.AreEqual(5, value.Value);
    }


    [TestMethod]
    public void ParseAssignProgram()
    {
        // Arrange & act
        Program ast = ParseFile("AssignASTTest.matilda");

        // Assert
        Assert.IsInstanceOfType<Comp>(ast.Stmt);
        Comp comp = (Comp)ast.Stmt;

        // First stmt (declaration)
        Assert.IsInstanceOfType<LocalDeclaration>(comp.Stmt1);
        LocalDeclaration declaration = (LocalDeclaration)comp.Stmt1;

        Assert.AreEqual("name", declaration.Identifier);

        Assert.IsInstanceOfType<StringV>(declaration.Expression);
        var originalValue = (StringV)declaration.Expression;
        Assert.AreEqual("Peter", originalValue.Value);

        // Second stmt (assign)
        Assert.IsInstanceOfType<Assign>(comp.Stmt2);
        Assign assign = (Assign)comp.Stmt2;

        Assert.AreEqual("name", assign.Identifier);

        Assert.IsInstanceOfType<StringV>(assign.Value);
        var reassignedValue = (StringV)assign.Value;
        Assert.AreEqual("Niels", reassignedValue.Value);
    }

    [TestMethod]
    public void ParseIfElseifElseProgram()
    {
        // Arrange & act
        Program ast = ParseFile("IfElseThenASTTest.matilda");

        Comp comp = (Comp)ast.Stmt;
        LocalDeclaration declarationStatement = (LocalDeclaration)comp.Stmt1!;
        Comp comp2 = (Comp)comp.Stmt2!;
        LocalDeclaration declarationStatement2 = (LocalDeclaration)comp2.Stmt1!;
        If ifStatement = (If)comp2.Stmt2!;

        // Assert
        Assert.IsInstanceOfType<Comp>(comp);
        Assert.IsInstanceOfType<Comp>(comp2);
        Assert.IsInstanceOfType<LocalDeclaration>(declarationStatement);
        Assert.IsInstanceOfType<LocalDeclaration>(declarationStatement2);
        Assert.IsInstanceOfType<If>(ifStatement);
        Assert.IsInstanceOfType<Assign>(ifStatement.ThenBody);

        // Check stms inside else body
        Assert.IsInstanceOfType<Assign>(ifStatement.ElseBody);
    }

    [TestMethod]
    public void ParseAddMulPrecedenceCorrect()
    {
        // print 1 + 2 * 3 + 1;
        // Arrange
        Program ast = ParseFile("PrecedenceASTTest1.matilda");


        // Act
        LocalDeclaration declaration = (LocalDeclaration)ast.Stmt;
        BinaryOp addRight = (BinaryOp)declaration.Expression;          // (1 + (2 * 3)) + 1
        BinaryOp addLeft = (BinaryOp)addRight.ExprLeft;          // 1 + (2 * 3)
        BinaryOp mul = (BinaryOp)addLeft.ExprRight;              // 2 * 3

        // Assert
        // Add right first ...
        Assert.AreEqual(BinaryOperators.ADD, addRight.Op);
        Assert.IsInstanceOfType(addRight.ExprLeft, typeof(BinaryOp));
        Assert.IsInstanceOfType(addRight.ExprRight, typeof(IntV));

        Assert.AreEqual(BinaryOperators.ADD, addLeft.Op);
        Assert.IsInstanceOfType(addLeft.ExprLeft, typeof(IntV));
        Assert.IsInstanceOfType(addLeft.ExprRight, typeof(BinaryOp));

        Assert.AreEqual(BinaryOperators.MUL, mul.Op);
        Assert.IsInstanceOfType(mul.ExprLeft, typeof(IntV));
        Assert.IsInstanceOfType(mul.ExprRight, typeof(IntV));
    }

    [TestMethod]
    public void ParseSubDivPrecedenceCorrect()
    {
        // print 1 - 2 / 3 / 2 - 1;
        // Arrange
        Program ast = ParseFile("PrecedenceASTTest2.matilda");

        // Act
        LocalDeclaration declaration = (LocalDeclaration)ast.Stmt;
        BinaryOp subRight = (BinaryOp)declaration.Expression;               // (1 - ((2 / 3) / 2)) - 1
        BinaryOp subLeft = (BinaryOp)subRight.ExprLeft;          // 1 - ((2 / 3) / 2)
        BinaryOp divRight = (BinaryOp)subLeft.ExprRight;         // (2 / 3) / 2
        BinaryOp divLeft = (BinaryOp)divRight.ExprLeft;          // 2 / 3

        // Assert
        Assert.AreEqual(BinaryOperators.SUB, subRight.Op);
        Assert.IsInstanceOfType(subRight.ExprLeft, typeof(BinaryOp));
        Assert.IsInstanceOfType(subRight.ExprRight, typeof(IntV));

        Assert.AreEqual(BinaryOperators.SUB, subLeft.Op);
        Assert.IsInstanceOfType(subLeft.ExprLeft, typeof(IntV));
        Assert.IsInstanceOfType(subLeft.ExprRight, typeof(BinaryOp));

        Assert.AreEqual(BinaryOperators.DIV, divRight.Op);
        Assert.IsInstanceOfType(divRight.ExprLeft, typeof(BinaryOp));
        Assert.IsInstanceOfType(divRight.ExprRight, typeof(IntV));

        Assert.AreEqual(BinaryOperators.DIV, divLeft.Op);
        Assert.IsInstanceOfType(divLeft.ExprLeft, typeof(IntV));
        Assert.IsInstanceOfType(divLeft.ExprRight, typeof(IntV));
    }

    [TestMethod]
    public void ParseSubASTCorrect()
    {
        // print 5 - 4 - 3 - 2;
        // Arrange
        Program ast = ParseFile("PrecedenceASTTest3.matilda");

        // Act
        LocalDeclaration declaration = (LocalDeclaration)ast.Stmt;
        BinaryOp subRight = (BinaryOp)declaration.Expression;            // ((5 - 4) - 3) - 2
        BinaryOp subMid = (BinaryOp)subRight.ExprLeft;        // (5 - 4) - 3
        BinaryOp subLeft = (BinaryOp)subMid.ExprLeft;         // 5 - 4

        // Assert
        Assert.AreEqual(BinaryOperators.SUB, subRight.Op);
        Assert.IsInstanceOfType(subRight.ExprLeft, typeof(BinaryOp));
        Assert.IsInstanceOfType(subRight.ExprRight, typeof(IntV));

        Assert.AreEqual(BinaryOperators.SUB, subMid.Op);
        Assert.IsInstanceOfType(subMid.ExprLeft, typeof(BinaryOp));
        Assert.IsInstanceOfType(subMid.ExprRight, typeof(IntV));

        Assert.AreEqual(BinaryOperators.SUB, subLeft.Op);
        Assert.IsInstanceOfType(subLeft.ExprLeft, typeof(IntV));
        Assert.IsInstanceOfType(subLeft.ExprRight, typeof(IntV));
    }


    // Parser must give an error on invalid syntax error
    [TestMethod]
    public void ParseInvalidSyntaxHasErrors()
    {
        // Arrange
        string path = Path.Combine(ScriptFolder, "InvalidSyntaxHasErrors.matilda");

        Scanner scanner = new Scanner(path);
        Parser parser = new Parser(scanner);

        // Act
        parser.Parse();

        // Assert
        Assert.IsTrue(parser.hasErrors());
    }
}