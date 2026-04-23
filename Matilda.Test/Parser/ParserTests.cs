using Matilda;
namespace MatildaTests;

[TestClass]
public class ParserTests
{
    // Helper method for all the test methods
    private Stmt Parse(string source)
    {
        var scanner = new Scanner(source);
        var parser = new Parser(scanner);

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
        Stmt ast = ParseFile("PrintASTTest.matilda");

        Assert.IsInstanceOfType<Print>(ast);

        var print = (Print)ast;
        Assert.IsInstanceOfType<IntV>(print.Value);

        var value = (IntV)print.Value;
        Assert.AreEqual(5, value.Value);
    }


    [TestMethod]
    public void ParseIfElseifElse()
    {
        Stmt ast = ParseFile("IfElseThenASTTest.matilda");

        var comp = (Comp)ast;

        var declarationStatement = (Declaration)comp.Stmt1!;
        var ifStatement = (If)comp.Stmt2!;

        Assert.IsInstanceOfType<Comp>(ast);
        Assert.IsInstanceOfType<Declaration>(declarationStatement);
        Assert.IsInstanceOfType<If>(ifStatement);
    }

    [TestMethod]
    public void ParseAddMulPrecedenceCorrect()
    {
        Stmt ast = ParseFile("PrecedenceASTTest1.matilda");
        // print 1 + 2 * 3 + 1;

        var print = (Print)ast;
        var addRight = (BinaryOp)print.Value;               // (1 + (2 * 3)) + 1
        var addLeft = (BinaryOp)addRight.ExprLeft;          // 1 + (2 * 3)
        var mul = (BinaryOp)addLeft.ExprRight;              // 2 * 3

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
        Stmt ast = ParseFile("PrecedenceASTTest2.matilda");
        // print 1 - 2 / 3 / 2 - 1;

        var print = (Print)ast;
        var subRight = (BinaryOp)print.Value;               // (1 - ((2 / 3) / 2)) - 1
        var subLeft = (BinaryOp)subRight.ExprLeft;          // 1 - ((2 / 3) / 2)
        var divRight = (BinaryOp)subLeft.ExprRight;         // (2 / 3) / 2
        var divLeft = (BinaryOp)divRight.ExprLeft;          // 2 / 3

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
        Stmt ast = ParseFile("PrecedenceASTTest3.matilda");
        // print 5 - 4 - 3 - 2;

        var print = (Print)ast;
        var subRight = (BinaryOp)print.Value;            // ((5 - 4) - 3) - 2
        var subMid = (BinaryOp)subRight.ExprLeft;        // (5 - 4) - 3
        var subLeft = (BinaryOp)subMid.ExprLeft;         // 5 - 4

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

}