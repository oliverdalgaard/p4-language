using Matilda;

namespace MatildaTests.UnitTests.TypeCheckerTests.ExprTTests;

[TestClass]
public class TestInvalidExpression : RunTypeChecker
{
    [TestMethod]
    public void InvalidExpressionTest()
    {
        // Arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        null, -1);

        // Assert
        try
        {
            TypeChecker checker = Run(new Program(stmt));
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Invalid expression", exception.Message);
        }
    }
}

[TestClass]
public class UnaryOpTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void TestInvalidUnaryOp()
    {
        // Arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new UnaryOp((UnaryOperators)999,
            new BoolV(true, 1), -1), -1);

        // Assert
        try
        {
            TypeChecker checker = Run(new Program(stmt));
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Unknown unary operator", exception.Message);
        }
    }

    [TestMethod]
    public void NeqTestTypecheck()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void NeqTestTypecheckFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new UnaryOp(UnaryOperators.NOT,
            new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Operator '!' expected a operand of type 'bool', but got 'Matilda.IntT'."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

[TestClass]
public class BinaryOpTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void TestInvalidBinaryOp()
    {
        // Arrange
        Stmt stmt = new LocalDeclaration(
        BoolT.Instance,
        "x",
        new BinaryOp((BinaryOperators)999,
            new IntV(1, 1), new IntV(1, 1), -1), -1);

        // Assert
        try
        {
            TypeChecker checker = Run(new Program(stmt));
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Invalid binary operation", exception.Message);
        }
    }

    //add,sub,mul,div,lt,eq,neq,and,or
    [TestMethod]
    [DataRow(BinaryOperators.ADD)]
    [DataRow(BinaryOperators.SUB)]
    [DataRow(BinaryOperators.MUL)]
    public void BinaryOpTypeCheckerIntReturnTest(BinaryOperators binaryOperator)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        IntT.Instance,
        "x",
        new BinaryOp(binaryOperator, new IntV(5, 1), new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD)]
    [DataRow(BinaryOperators.SUB)]
    [DataRow(BinaryOperators.MUL)]
    [DataRow(BinaryOperators.DIV)]
    public void BinaryOpTypeCheckerFloatReturnTest(BinaryOperators binaryOperator)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(
        FloatT.Instance,
        "x",
        new BinaryOp(binaryOperator, new FloatV(5, 1), new FloatV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD, "+")]
    [DataRow(BinaryOperators.SUB, "-")]
    [DataRow(BinaryOperators.MUL, "*")]
    [DataRow(BinaryOperators.DIV, "/")]
    public void BinaryOpTypeCheckerFailsLeftOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BinaryOp(binaryOperator,
                        new BoolV(true, 1),
                        new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'int' or 'float', but got 'Matilda.BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.ADD, "+")]
    [DataRow(BinaryOperators.SUB, "-")]
    [DataRow(BinaryOperators.MUL, "*")]
    [DataRow(BinaryOperators.DIV, "/")]
    public void BinaryOpTypeCheckerFailsRightOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(5, 1),
                        new BoolV(true, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right operand of type 'int' or 'float', but got 'Matilda.BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void LogicOpTypeCheckerFailsOperandLT()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(BinaryOperators.LT,
                        new StringV("test", 1),
                        new StringV("test", 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '<' expected a left operand of type 'int' or 'float', but got 'Matilda.StringT'.",
            $"Line 1: Operator '<' expected a right operand of type 'int' or 'float', but got 'Matilda.StringT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new StringV("test", 1),
                        new StringV("test", 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'bool','int' or 'float', but got 'Matilda.StringT'.",
            $"Line 1: Operator '{symbol}' expected a right operand of type 'bool','int' or 'float', but got 'Matilda.StringT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand2(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(1, 1),
                        new BoolV(false, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right and left operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.EQ, "==")]
    [DataRow(BinaryOperators.NEQ, "!=")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand3(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new BoolV(false, 1),
                        new IntV(1, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a right and left operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    [DataRow(BinaryOperators.AND, "&&")]
    [DataRow(BinaryOperators.OR, "||")]
    public void LogicOpTypeCheckerFailsLeftAndRightOperand4(BinaryOperators binaryOperator, string symbol)
    {
        //arrange
        Stmt stmt = new LocalDeclaration(BoolT.Instance, "x", new BinaryOp(binaryOperator,
                        new IntV(1, 1),
                        new IntV(1, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            $"Line 1: Operator '{symbol}' expected a left operand of type 'bool', but got 'Matilda.IntT'.",
            $"Line 1: Operator '{symbol}' expected a right operand of type 'bool', but got 'Matilda.IntT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

//ref, functionRef
[TestClass]
public class RefTestsTypeChecker : RunTypeChecker
{
    [TestMethod]
    public void RefTestTypeCheckerDeclaredFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "f", new Ref("x", -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Variable x is not declared.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void RefTestTypeCheckerDeclared()
    {
        //arrange
        Stmt stmt = new Comp(
          new LocalDeclaration(IntT.Instance, "x", new IntV(5, 1), -1),
          new LocalDeclaration(IntT.Instance, "y", new Ref("x", -1), -1)
      );
        //act
        var checker = Run(new Program(stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
}
[TestClass]
public class FunctionRefTestsTypeChecker : RunTypeChecker
{
    //function not declared
    [TestMethod]
    public void FunctionRefTestTypeCheckerFails()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(IntT.Instance, "x", new FunctionRef("func1", new List<Expr>(), -1), -1);

        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Function func1 is not declared.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    //function with wrong param
    [TestMethod]
    public void FunctionRefTestTypeCheckerWithWrongParam()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
            new Return(new IntV(5, 1), -1)
        ),
        -1);
        Stmt stmt = new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr> { new IntV(5, 1) }, -1), -1);

        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        var expected = new List<string>
    {
        "Line -1: Function func1 argument count mismatch.",
        "Line -1: Declaration type does not match the type of the expression."
    };
        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void FunctionRefTestTypeChecker()
    {
        //arrange
        TopLevelDeclaration topLevelDeclaration = new FunctionDeclaration(IntT.Instance, "func1",
        new List<Parameter>(),
        new Comp
        (
            new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
            new Return(new IntV(5, 1), -1)
        ),
        -1);

        Stmt stmt = new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr>(), -1), -1);

        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void FunctionRefTestWrongSchemaParameter()
    {
        //arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration("testSchema1", new List<Column>(), -1),
            new SchemaDeclaration("testSchema2", new List<Column> {new Column("test", IntT.Instance)}, -1),
            new FunctionDeclaration(IntT.Instance, "func1",
                new List<Parameter> { new Parameter(new TableT("testSchema2"), "table", -1) },
                new Comp
                    (
                        new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
                        new Return(new IntV(5, 1), -1)
                    ),
                -1)
            };

        Stmt stmt = new Comp(
            new TableDeclaration(new TableT("testSchema1"), "testTable", "testFilePath", -1),
            new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr> { new Ref("testTable", -1) }, -1), -1)
            );

        //act
        var checker = Run(new Program(topLevelDeclarations, stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Function 'func1' expect parameter 1 to have table with schema 'testSchema1' but got 'testSchema2'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }

    [TestMethod]
    public void FunctionRefTestWrongParameterType()
    {
        //arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new FunctionDeclaration(IntT.Instance, "func1",
                new List<Parameter> { new Parameter(IntT.Instance, "int", -1) },
                new Comp
                    (
                        new LocalDeclaration(IntT.Instance, "x", new IntV(2, 1), -1),
                        new Return(new IntV(5, 1), -1)
                    ),
                -1)
            };

        Stmt stmt = new LocalDeclaration(IntT.Instance, "y", new FunctionRef("func1", new List<Expr> { new FloatV(1, -1) }, -1), -1);

        //act
        var checker = Run(new Program(topLevelDeclarations, stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Function 'func1' expect parameter 1 to have type 'Matilda.IntT' but got 'Matilda.FloatT'.",
        };

        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

//filter expr test
[TestClass]
public class FilterTestsTypechecker : RunTypeChecker
{
    [TestMethod]
    public void FilterTestType()
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

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Filter(new Ref("tab1", -1), new BoolV(true, 1), -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }
    // check not tableT and not bool for agr 2
    [TestMethod]
    public void FilterTestTypeFailsArg1()
    {
        //arrange
        Stmt stmt = new LocalDeclaration(new TableT("test"), "x", new Filter(new IntV(2, 1), new IntV(5, 1), -1), -1);
        //act
        var checker = Run(new Program(stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Argument 1 must be of type 'TableT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
    [TestMethod]
    public void FilterTestTypeFailsArg2()
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

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "../../../TypeChecker/TestMatildaScriptTypeChecker/TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Filter(new Ref("tab1", -1), new IntV(5, 1), -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        var expected = new List<string>
        {
            "Line -1: Argument 2 must be of type 'BoolT'.",
            "Line -1: Declaration type does not match the type of the expression."
        };
        CollectionAssert.AreEqual(expected, checker.errors);
    }
}

[TestClass]
public class SumTestsTypechecker : RunTypeChecker
{
    [TestMethod]
    public void SumTestType()
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

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new Ref("tab1", -1), "name", "Id", "schema1", -1), -1)
            );
        //act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));
        //assert
        Assert.IsFalse(checker.HasErrors());
    }

    [TestMethod]
    public void SumTestType2()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            );

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new IntV(1, -1), "name", "Id", "schema1", -1), -1)
            );

        // Act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(2, checker.errors);
        Assert.AreEqual("Line -1: Argument 1 must be of type 'TableT'.", checker.errors[0]);
        Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
    }

    [TestMethod]
    public void SumTestWrongSchemaId()
    {
        // Arrange
        TopLevelDeclaration topLevelDeclaration = new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            );

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new Ref("tab1", -1), "name", "Id", "schema2", -1), -1)
            );

        // Act
        var checker = Run(new Program(new List<TopLevelDeclaration> { topLevelDeclaration }, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(2, checker.errors);
        Assert.AreEqual("Line -1: Result schema 'schema2' has not been defined.", checker.errors[0]);
        Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
    }

    [TestMethod]
    public void SumTestWrongSchemaColumnAmount()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
         new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            ),
         new SchemaDeclaration(
                "schema2",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance),
            new Column("name2", StringT.Instance)
                },
                -1
            )
        };

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new Ref("tab1", -1), "name", "Id", "schema2", -1), -1)
            );

        // Act
        var checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(2, checker.errors);
        Assert.AreEqual("Line -1: Result schema 'schema2' may only contain two columns but has 3 columns.", checker.errors[0]);
        Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
    }

    [TestMethod]
    [DataRow("Id", "test")]
    [DataRow("test", "name")]
    public void SumTestWrongSchemaColumnIdentifiers(string leftId, string rightId)
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
         new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            ),
         new SchemaDeclaration(
                "schema2",
                new List<Column>
                {
            new Column(leftId, IntT.Instance),
            new Column(rightId, StringT.Instance),
                },
                -1
            )
        };

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new Ref("tab1", -1), "name", "Id", "schema2", -1), -1)
            );

        // Act
        var checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(2, checker.errors);
        Assert.AreEqual("Line -1: The column 'test' does not exist in schema 'schema1'.", checker.errors[0]);
        Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
    }

    [TestMethod]
    public void SumTestWrongSumColumnType()
    {
        // Arrange
        List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
         new SchemaDeclaration(
                "schema1",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance)
                },
                -1
            ),
         new SchemaDeclaration(
                "schema2",
                new List<Column>
                {
            new Column("Id", IntT.Instance),
            new Column("name", StringT.Instance),
                },
                -1
            )
        };

        Stmt stmt = new Comp(new TableDeclaration(
            new TableT("schema1"),
            "tab1",
            "TableDeclaration.csv",
            -1),
            new LocalDeclaration(new TableT("schema1"), "x", new Sum(new Ref("tab1", -1), "Id", "name", "schema2", -1), -1)
            );

        // Act
        var checker = Run(new Program(topLevelDeclarations, stmt));

        // Assert
        Assert.IsTrue(checker.HasErrors());
        Assert.HasCount(2, checker.errors);
        Assert.AreEqual("Line -1: The column 'name' must be of type 'IntT' or 'FloatT', but got 'Matilda.StringT'.", checker.errors[0]);
        Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
    }

    [TestClass]
    public class JoinTestsTypechecker : RunTypeChecker
    {
        [TestMethod]
        public void JoinTestType()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(new TableDeclaration(
                new TableT("schema1"),
                "tab1",
                "TableDeclaration.csv",
                -1),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsFalse(checker.HasErrors());
        }

        [TestMethod]
        public void JoinTestType2()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(new LocalDeclaration(
                IntT.Instance,
                "tab1",
                new IntV(1, -1),
                -1),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Argument 1 must be of type 'TableT'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestType3()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(new TableDeclaration(
                    new TableT("schema2"),
                    "tab1",
                    "TableDeclaration2.csv",
                -1),
                new Comp(
                    new LocalDeclaration(
                    IntT.Instance,
                    "tab2",
                    new IntV(1, -1),
                    -1
                    ),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Argument 2 must be of type 'TableT'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestWrongSchemaId()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                        new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Result schema 'schema3' has not been defined.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestWrongSchemaColumnAmount()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                    new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1
                ),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Result schema 'schema3' may only contain 3 columns but has 2 columns.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestWrongJoinOnReferenceColumn()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                    new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1
                ),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Join schema 'schema1' must contain 'customer_id'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestWrongJoinFromReferenceColumn()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                    new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1
                ),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Join schema 'schema2' must contain 'id'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestResultSchemaContainsFromReferenceColumn()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("schema1_id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                        new Column("name", IntT.Instance),
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                    new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1
                ),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Result schema 'schema3' may not contain column with id 'id'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }

        [TestMethod]
        public void JoinTestResultSchemaContainsFromReferenceColumn2()
        {
            // Arrange
            List<TopLevelDeclaration> topLevelDeclarations = new List<TopLevelDeclaration> {
            new SchemaDeclaration(
                    "schema1",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("customer_id", IntT.Instance),
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema2",
                    new List<Column>
                    {
                        new Column("customer_id", IntT.Instance),
                        new Column("name", StringT.Instance)
                    },
                    -1
                ),
            new SchemaDeclaration(
                    "schema3",
                    new List<Column>
                    {
                        new Column("id", IntT.Instance),
                        new Column("c_id", IntT.Instance),
                        new Column("number", IntT.Instance),
                    },
                    -1
                )
            };

            Stmt stmt = new Comp(
                new TableDeclaration(
                    new TableT("schema1"),
                        "tab1",
                        "TableDeclaration2.csv",
                    -1
                ),
                new Comp(
                    new TableDeclaration(
                        new TableT("schema2"),
                        "tab2",
                        "TableDeclaration2.csv",
                    -1),
                    new LocalDeclaration(new TableT("schema3"), "x", new Join(new Ref("tab1", -1), new Ref("tab2", -1), "customer_id", "customer_id", "schema3", -1), -1)
                )
                );

            // Act
            var checker = Run(new Program(topLevelDeclarations, stmt));

            // Assert
            Assert.IsTrue(checker.HasErrors());
            Assert.HasCount(2, checker.errors);
            Assert.AreEqual("Line -1: Result schema 'schema3' may not contain a column 'c_id' that does not exist in schema 'schema1' or 'schema2'.", checker.errors[0]);
            Assert.AreEqual("Line -1: Declaration type does not match the type of the expression.", checker.errors[1]);
        }
    }
}