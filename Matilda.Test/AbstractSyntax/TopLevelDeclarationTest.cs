using Matilda;

namespace MatildaTests;

[TestClass]
public class SchemaDeclarationTests
{
    [TestMethod]
    public void SchemaDeclarationStoresProperties()
    {
        // Arrange
        string expectedIdentifier = "Test";
        List<Column> expectedColumns = new List<Column>
        {
            new Column("x", new IntT())
        };
        int expectedLineNumber = -1;

        // Act
        SchemaDeclaration schema = new SchemaDeclaration(
            expectedIdentifier, 
            expectedColumns, 
            expectedLineNumber
        );

        // Assert
        Assert.AreEqual(expectedIdentifier, schema.Identifier);
        Assert.AreSame(expectedColumns, schema.Columns);
        Assert.AreEqual(expectedLineNumber, schema.LineNumber);

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
        string expectedIdentifier = "Test";
        List<Parameter> expectedParameters = new List<Parameter>
        {
            new Parameter(new IntT(), "x", -1)
        };
        Stmt expectedBody = new Return(new Ref("x", -1), -1);
        int expectedLineNumber = -1;

        // Act
        FunctionDeclaration function = new FunctionDeclaration(
            expectedType,
            expectedIdentifier,
            expectedParameters,
            expectedBody,
            expectedLineNumber
        );

        // Assert
        Assert.AreSame(expectedType, function.Type);
        Assert.AreEqual(expectedIdentifier, function.Identifier);
        Assert.AreSame(expectedParameters, function.Parameters);
        Assert.AreSame(expectedBody, function.Body);
        Assert.AreEqual(expectedLineNumber, function.LineNumber);
    }
}