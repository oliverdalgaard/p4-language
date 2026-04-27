using Matilda;

namespace MatildaTests;

[TestClass]
public class typeCheckerTests
{
    private TypeChecker Run(Stmt stmt)
    {
        var envVT = new EnvVT();
        var envPT = new EnvPT();
        var envST = new EnvST();

        return new TypeChecker(stmt, envVT, envPT, envST);
    }
    [TestMethod]
    public void CompcheckBothStatements()
    {
        // arrange 
        Stmt stmt = new Comp(
            new Assign("x", new IntV(5, 1), 1), // error
            new Assign("y", new IntV(10, 2), 2) // error
        );
        // act 
        var checker = Run(stmt);
        // assert
        var expected = new List<string>
    {
        "Line 1: Variable x is not declared.",
        "Line 2: Variable y is not declared."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    /*[TestMethod]
    public void CompcheckBothStatements2()
    {
        // arrange
        Stmt stmt = new Comp(new Declaration(new IntT(), "x", new IntV(5, 1), 1),
        new Assign("x", new IntV(10, 2), 2));
        // act
        var checker = Run(stmt);
        // assert
        Assert.IsFalse(checker.HasErrors());
    }*/
}
