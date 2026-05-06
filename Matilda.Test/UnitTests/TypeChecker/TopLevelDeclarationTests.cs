using Matilda;

namespace MatildaTests.UnitTests.TypeCheckerTests.TopLevelDeclarationTests;

[TestClass]
public class FunctionTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void FunctionDeclarationCheckbodyAndReturn()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new BoolV(true, 1), -1),
            new Return(new BoolV(true, 1), -1)
        ),
        -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        //assert
        var expected = new List<string>
    {
        "Line -1: Declaration type does not match the type of the expression.",
        "Line -1: Return type 'Matilda.BoolT' does not match function return type 'Matilda.IntT'.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }


    [TestMethod]
    public void FunctionDeclarationCheck()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1),
            new Return(new IntV(5, 1), -1)
        ),
        -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void FunctionDeclarationInvalidTest()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(null, "func1",
        new List<Parameter>(),
        Skip.Instance,
        -1);
        TopLevelDeclaration topLevelDeclaration2 = new FunctionDeclaration(IntT.Instance, null,
        new List<Parameter>(),
        Skip.Instance,
        -1);
        TopLevelDeclaration topLevelDeclaration3 = new FunctionDeclaration(null, null,
        new List<Parameter>(),
        Skip.Instance,
        -1);

        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { topLevelDeclaration, topLevelDeclaration2, topLevelDeclaration3 };

        // Act
        var checker = Run(new Program(topLevelDeclarations));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(3, checker.errors);
        Assert.AreEqual("Line -1: Invalid declaration.", checker.errors[0]);
        Assert.AreEqual("Line -1: Invalid declaration.", checker.errors[1]);
        Assert.AreEqual("Line -1: Invalid declaration.", checker.errors[2]);
    }

    [TestMethod]
    public void FunctionDeclarationFunctionAlreadyDeclaredTest()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Return(new IntV(1, -1), -1),
        -1);
        TopLevelDeclaration topLevelDeclaration2 = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Return(new IntV(1, -1), -1),
        -1);

        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { topLevelDeclaration, topLevelDeclaration2 };

        // Act
        var checker = Run(new Program(topLevelDeclarations));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Function 'func1' already declared.", checker.errors[0]);
    }

    [TestMethod]
    public void FunctionDeclarationDuplicateParametersTest()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter> { new Parameter(IntT.Instance, "Test", -1), new Parameter(IntT.Instance, "Test", -1) },
        new Return(new IntV(1, -1), -1),
        -1);

        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { topLevelDeclaration };

        // Act
        var checker = Run(new Program(topLevelDeclarations));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Duplicate parameter 'Test'.", checker.errors[0]);
    }

    [TestMethod]
    public void FunctionDeclarationHasNoReturnTest()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        Skip.Instance,
        -1);

        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { topLevelDeclaration };

        // Act
        var checker = Run(new Program(topLevelDeclarations));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Not all paths return a value in function 'func1'.", checker.errors[0]);
    }
}

[TestClass]
public class SchemaTestsTypechecker : RunTypeChecker
{
    [TestMethod]
    public void SchemaDeclarationCheck()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration("schema1", new List<Column> { new Column("1", IntT.Instance) }, -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void SchemaDeclarationContainsDuplicatesTest()
    {
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration("schema1", new List<Column> { new Column("1", IntT.Instance), new Column("1", IntT.Instance) }, -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        // assert
        Assert.IsTrue(checker.HasErrors());
        Assert.AreEqual("Line -1: Schema 'schema1' may not contain duplicate identifiers.", checker.errors[0]);
    }

    [TestMethod]
    public void SchemaAlreadyDeclaredTest()
    {
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration("schema1", new List<Column> { new Column("1", IntT.Instance), new Column("2", IntT.Instance) }, -1);
        TopLevelDeclaration topLevelDeclaration2 = new SchemaDeclaration("schema1", new List<Column> { new Column("1", IntT.Instance), new Column("2", IntT.Instance) }, -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration, topLevelDeclaration2 }));
        // assert
        Assert.IsTrue(checker.HasErrors());
        Assert.AreEqual("Line -1: Schema 'schema1' is already declared.", checker.errors[0]);
    }

    [TestMethod]
    public void SchemaDeclarationWrongTypeTest()
    {
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration("schema1", new List<Column> { new Column("1", RowValT.Instance) }, -1);
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        // assert
        Assert.IsTrue(checker.HasErrors());
        Assert.AreEqual("Line -1: Schema declaration requires type of either 'int', 'string', 'bool', or 'float' but got 'Matilda.RowValT'", checker.errors[0]);
    }

}