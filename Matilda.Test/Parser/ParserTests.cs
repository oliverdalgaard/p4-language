using Matilda;
namespace MatildaTests;

[TestClass]
public class ParserTests
{
    // Helper method for all the test methods
    private Stmt Parse(string soruce)
    {
        var scanner = new Scanner(soruce);
        var parser = new Parser(scanner);

        parser.Parse();

        // If the parser fails the test must fail too.    
        Assert.IsFalse(parser.hasErrors());

        return parser.mainNode;
    }

    [TestMethod]
    public void ParsePrintLiteralReturnsPrintNode()
    {
        Stmt ast = Parse("print 5;");

        Assert.IsInstanceOfType<Print>(ast);
        
        // Safe cast of ast to Print stmt
        var print = (Print)ast;
        Assert.IsInstanceOfType<IntV>(print.Value);

        var value = (IntV)print.Value;
        Assert.AreEqual(5, value.Value);
    }
}