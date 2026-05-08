using Matilda;

namespace MatildaTests.UnitTests.TypeCheckerTests.StmtTTests;

[TestClass]
public class TestInvalidStatement : RunTypeChecker
{
    [TestMethod]
    public void InvalidStatementTest()
    {
        // Arrange
        Stmt stmt = null;

        // Assert
        try
        {
            TypeChecker checker = Run(new Program(stmt));
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Invalid statement", exception.Message);
        }
    }
}

[TestClass]
public class CompTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void CompcheckBothStatementsFails()
    {
        // arrange 
        Stmt stmt = new Comp(
            new Assign("x", new IntV(5, 1), -1), // error
            new Assign("y", new IntV(10, 2), -1) // error
        );
        // act 
        TypeChecker checker = Run(new Program(stmt));
        // assert
        List<string> expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
        "Line -1: Variable y is not declared."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void CompcheckBothStatements()
    {
        // arrange
        Stmt stmt = new Comp(new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1), new Assign("x", new IntV(10, 2), -1));
        // act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}

[TestClass]
public class IfTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void IfCheckCondition()
    {
        //arrange
        Stmt stmt = new If(new BoolV(true, -1), new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1), Skip.Instance, -1);
        //act
        var checker = Run(new Program(stmt));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void IfCheckBody()
    {
        //arrange
        Stmt stmt = new If(new BoolV(true, -1),
            new Assign("x", new IntV(5, 1), -1),    //error
            Skip.Instance, -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        List<string> expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void IfConditionTest()
    {
        // Arrange

        Stmt stmt = new If(new StringV("Hej", -1),
            Skip.Instance,
            Skip.Instance, -1);

        // Act
        TypeChecker checker = Run(new Program(stmt));

        // Assert
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: If statement requires a condition with type 'bool', but got 'Matilda.StringT'.", checker.errors[0]);
    }

    [TestMethod]
    public void IfConditionNullTest()
    {
        // Arrange

        Stmt stmt = new If(null,
            Skip.Instance,
            Skip.Instance, -1);

        // Act
        TypeChecker checker = Run(new Program(stmt));

        // Assert
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: If statement requires a condition.", checker.errors[0]);
    }
}

[TestClass]
public class AssignTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void AssignCheckNullValue()
    {
        //arrange
        Stmt stmt = new Assign(null, new IntV(5, 1), -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Invalid assignment",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void AssignCheckNullValue2()
    {
        //arrange
        Stmt stmt = new Assign("x", null, -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Invalid assignment",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void AssignCheckNotDeclared()
    {
        //arrange
        Stmt stmt = new Assign("x", new IntV(5, 1), -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void AssignCheckWrongType()
    {
        //arrange
        Stmt stmt = new Comp(new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1),
        new Assign("x", new BoolV(true, 2), -1));
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Cannot assign 'Matilda.BoolT' to variable 'x' of type 'Matilda.IntT'.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void AssignCheck()
    {
        //arrange
        Stmt stmt = new Comp(new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1),
        new Assign("x", new IntV(10, 2), -1));
        //act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void AssignTableTestWrongSchema()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { new SchemaDeclaration("test1", new List<Column>(), -1), new SchemaDeclaration("test2", new List<Column> { new Column("test", IntT.Instance) }, -1) };
        Stmt stmt = new Comp(new TableDeclaration(new TableT("test1"), "x", "testFilePath", -1), new Comp(new TableDeclaration(new TableT("test2"), "testTable", "testFilePath", -1), new Assign("x", new Ref("testTable", -1), -1)));

        // Act
        TypeChecker checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Cannot assign table with schema 'test2' to table 'x' with schema 'test1'.", checker.errors[0]);
    }

    [TestMethod]
    public void AssignTableTestSuccess()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { new SchemaDeclaration("test1", new List<Column>(), -1) };
        Stmt stmt = new Comp(new TableDeclaration(new TableT("test1"), "x", "testFilePath", -1), new Comp(new TableDeclaration(new TableT("test1"), "testTable", "testFilePath", -1), new Assign("x", new Ref("testTable", -1), -1)));

        // Act
        TypeChecker checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsFalse(checker.HasErrors());
    }
}

[TestClass]
public class LocalDeclarationTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void LocalDeclarationCheckNullValue()
    {
        // Arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, null, new IntV(1, -1), -1);

        // Act
        TypeChecker checker = Run(new Program(stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Invalid declaration.", checker.errors[0]);
    }

    [TestMethod]
    public void LocalDeclarationCheckNullValue2()
    {
        // Arrange
        Stmt stmt = new LocalDeclaration(null, "x", new IntV(1, -1), -1);

        // Act
        TypeChecker checker = Run(new Program(stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Invalid declaration.", checker.errors[0]);
    }

    [TestMethod]
    public void LocalDeclarationAlreadyDeclaredTest()
    {
        // Arrange
        Stmt stmt = new Comp(new LocalDeclaration(IntT.Instance, "x", new IntV(1, -1), -1), new LocalDeclaration(IntT.Instance, "x", new IntV(2, -1), -1));

        // Act
        TypeChecker checker = Run(new Program(stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Variable 'x' is already declared.", checker.errors[0]);
    }

    [TestMethod]
    public void LocalDeclarationSchemaDoesNotMatch()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { new SchemaDeclaration("test1", new List<Column>(), -1), new SchemaDeclaration("test2", new List<Column> { new Column("Test", IntT.Instance) }, -1) };
        Stmt stmt = new Comp(new TableDeclaration(new TableT("test1"), "testTable", "testFilePath", -1), new LocalDeclaration(new TableT("test2"), "x", new Ref("testTable", -1), -1));

        // Act
        TypeChecker checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Declaration schema does not match the schema of the expression.", checker.errors[0]);
    }

    [TestMethod]
    public void LocalDeclarationCheckWrongType()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BoolV(true, 1), -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Declaration type does not match the type of the expression.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void LocalDeclarationCheck()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }
}

// return
[TestClass]
public class ReturnTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void ReturnCheckOutSideFunc()
    {
        //arrange
        Stmt stmt = new Return(new IntV(5, 1), -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        List<string> expected = new List<string>
    {
        "Line -1: Return outside of a function is not allowed.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void ReturnNeedsAValue()
    {
        //arrange
        Stmt stmt = new Return(null, -1);
        //act
        TypeChecker checker = Run(new Program(stmt));
        // assert
        List<string> expected = new List<string>
    {
        "Line -1: 'return' needs a value.",
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void ReturnCheckInsideFunction()
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
        TypeChecker checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void ReturnDoesNotReturnCorrectSchemaType()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration("test1", new List<Column>(), -1),
            new SchemaDeclaration("test2", new List<Column> {new Column("test", IntT.Instance)}, -1),

            new FunctionDeclaration(new TableT("test1"), "func1",
                new List<Parameter>(),
                new Comp
                    (
                        new TableDeclaration(new TableT("test2"), "testTable", "testFilePath", -1),
                        new Return(new Ref("testTable", -1), -1)
                    ),
                -1)
                };

        // Act
        TypeChecker checker = Run(new Program(topLevelDeclarations));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Return type schema 'test2' does not match function return type schema 'test1'.", checker.errors[0]);
    }
}

//table declaration test
[TestClass]
public class TableDeclarationtestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void TableDeclarationCheckNullValue()
    {
        //arrange
        Stmt stmt = new TableDeclaration(
            IntT.Instance,
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1
        );
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Invalid table declaration."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void TableDeclarationCheckNullValue2()
    {
        //arrange
        Stmt stmt = new TableDeclaration(
            null,
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1
        );
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Invalid table declaration."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void TableDeclarationCheckNullValue3()
    {
        //arrange
        Stmt stmt = new TableDeclaration(
            new TableT("schema1"),
            null,
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1
        );
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Invalid table declaration."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void TableDeclarationTestWrongType()
    {
        //arrange
        Stmt stmt = new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1
        );
        //act
        TypeChecker checker = Run(new Program(stmt));
        //assert
        List<string> expected = new List<string>
    {
        "Line -1: Schema with identifier 'schema1' is not declared."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void TableDeclarationcheck()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            );

        Stmt stmt = new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1
        );
        //act
        TypeChecker checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        // assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void TableDeclarationDuplicateFails()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> { new SchemaDeclaration("test1", new List<Column>(), -1) };
        Stmt stmt = new Comp(new TableDeclaration(new TableT("test1"), "testTable", "testFilePath", -1), new TableDeclaration(new TableT("test1"), "testTable", "testFilePath", -1));

        // Act
        TypeChecker checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(1, checker.errors);
        Assert.AreEqual("Line -1: Table 'testTable' is already declared.", checker.errors[0]);
    }
}