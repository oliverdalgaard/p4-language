using Matilda;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
namespace MatildaTests;

[TestClass]
public class ParserTests
{
    // Helper method for all the test methods
    private Stmt Parse(string source)
    {
        Scanner scanner = new Scanner(source);
        Parser parser = new Parser(scanner);

        parser.Parse();

        Assert.IsFalse(parser.hasErrors());

        return parser.mainNode;
    }

    // Helper function locate the correct directory
    private static readonly string ScriptFolder =
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "../../../Parser/TestMatildaScripts"));

    // Helper method to parse a file from the TestMatildaScripts directory
    private Stmt ParseFile(string fileName)
    {
        string path = Path.Combine(ScriptFolder, fileName);
        return Parse(path);
    }

    //

    [TestMethod]
    public void ParsePrintLiteralReturnsPrintNode()
    {
        // Arrange & act
        Stmt ast = ParseFile("PrintASTTest.matilda");

        // Assert
        Assert.IsInstanceOfType<Print>(ast);

        Print print = (Print)ast;
        Assert.IsInstanceOfType<IntV>(print.Value);

        var value = (IntV)print.Value; // Er dette rigtigt sat ind?
        Assert.AreEqual(5, value.Value);
    }


    [TestMethod]
    public void ParseDeclarationProgram()
    {
        // Arrange + Act
        Stmt ast = ParseFile("DeclarationASTTest.matilda");

        // Arange
        Assert.IsInstanceOfType(ast, typeof(Declaration));
        Declaration declaration = (Declaration)ast;

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
        Stmt ast = ParseFile("AssignASTTest.matilda");

        // Assert
        Assert.IsInstanceOfType<Comp>(ast);
        Comp comp = (Comp)ast;

        // First stmt (declaration)
        Assert.IsInstanceOfType<Declaration>(comp.Stmt1);
        Declaration declaration = (Declaration)comp.Stmt1;

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
    public void ParseWhileProgram()
    {
        // Arrange + Act
        Stmt ast = ParseFile("WhileASTTest.matilda");

        // Assert
        Assert.IsInstanceOfType<Comp>(ast);
        Comp comp = (Comp)ast;

        // First stmt (declaration)
        Assert.IsInstanceOfType<Declaration>(comp.Stmt1);
        Declaration declaration = (Declaration)comp.Stmt1;

        Assert.AreEqual("number", declaration.Identifier);
        Assert.IsInstanceOfType<IntV>(declaration.Expression);

        // Second stmt (while)
        Assert.IsInstanceOfType<While>(comp.Stmt2);
        While whileStmt = (While)comp.Stmt2;

        // Condition
        Assert.IsInstanceOfType<BinaryOp>(whileStmt.Condition);

        BinaryOp condition = (BinaryOp)whileStmt.Condition;
        Assert.AreEqual(BinaryOperators.LT, condition.Op);

        // Body
        Assert.IsInstanceOfType<Comp>(whileStmt.Body);
    }


    [TestMethod]
    public void ParseIfElseifElseProgram()
    {
        // Arrange & act
        Stmt ast = ParseFile("IfElseThenASTTest.matilda");

        Comp comp = (Comp)ast;
        Declaration declarationStatement = (Declaration)comp.Stmt1!;
        If ifStatement = (If)comp.Stmt2!;

        // Assert
        Assert.IsInstanceOfType<Comp>(ast);
        Assert.IsInstanceOfType<Declaration>(declarationStatement);
        Assert.IsInstanceOfType<If>(ifStatement);
        Assert.IsInstanceOfType<Print>(ifStatement.ThenBody);

        // Check stms inside ifelse and else body
        Assert.IsInstanceOfType<Print>(ifStatement.ElseIfStmts![0].ThenBody);
        Assert.IsInstanceOfType<Print>(ifStatement.ElseBody);
    }

    [TestMethod]
    public void ParseAddMulPrecedenceCorrect()
    {
        // print 1 + 2 * 3 + 1;
        // Arrange
        Stmt ast = ParseFile("PrecedenceASTTest1.matilda");


        // Act
        Print print = (Print)ast;
        BinaryOp addRight = (BinaryOp)print.Value;               // (1 + (2 * 3)) + 1
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
        Stmt ast = ParseFile("PrecedenceASTTest2.matilda");

        // Act
        Print print = (Print)ast;
        BinaryOp subRight = (BinaryOp)print.Value;               // (1 - ((2 / 3) / 2)) - 1
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
        Stmt ast = ParseFile("PrecedenceASTTest3.matilda");

        // Act
        Print print = (Print)ast;
        BinaryOp subRight = (BinaryOp)print.Value;            // ((5 - 4) - 3) - 2
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
        Scanner scanner = new Scanner("../../../Parser/TestMatildaScripts/InvalidSyntaxHasErrors.matilda");
        Parser parser = new Parser(scanner);

        // Act
        parser.Parse();

        // Assert
        Assert.IsTrue(parser.hasErrors());
    }
}