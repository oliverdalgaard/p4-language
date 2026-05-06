using Matilda;

namespace MatildaTests;

[TestClass]
public class TableTests
{
    [TestMethod]
    public void TableInitTest1()
    {
        // Arrange
        string identifier = "testId";
        List<Column> schema = new List<Column>();
        List<string[]> file = new List<string[]>();

        // Act
        Table table = new Table(identifier, schema, file);

        // Assert
        Assert.AreEqual(identifier, table.Identifier);
        Assert.AreEqual(schema, table.Schema);
        Assert.AreEqual(file, table.File);
        Assert.IsInstanceOfType<List<TableHeader>>(table.Headers);
        Assert.IsInstanceOfType<List<TableRecord>>(table.Records);
    }

    [TestMethod]
    public void TableInitTest2()
    {
        // Arrange
        string identifier = "testId";
        List<Column> schema = new List<Column>();
        List<TableHeader> tableHeaders = new List<TableHeader>();
        List<TableRecord> tableRecords = new List<TableRecord>();

        // Act
        Table table = new Table(identifier, schema, tableHeaders, tableRecords);

        // Assert
        Assert.AreEqual(identifier, table.Identifier);
        Assert.AreEqual(schema, table.Schema);
        Assert.AreEqual(tableHeaders, table.Headers);
        Assert.AreEqual(tableRecords, table.Records);
    }

    [TestMethod]
    public void TableAddRecordTest()
    {
        // Arrange
        string identifier = "testId";
        List<Column> schema = new List<Column>();
        List<TableHeader> tableHeaders = new List<TableHeader>();
        List<TableRecord> tableRecords = new List<TableRecord>();

        List<Val> recordValList = new List<Val> { new IntVal(2) };

        // Act
        Table table = new Table(identifier, schema, tableHeaders, tableRecords);
        table.addRecord(recordValList);

        // Assert
        Assert.AreEqual(identifier, table.Identifier);
        Assert.AreEqual(schema, table.Schema);
        Assert.AreEqual(tableHeaders, table.Headers);
        Assert.AreEqual(recordValList, table.Records[0].Values);
    }

    [TestMethod]
    public void TableAddRecordTest2()
    {
        // Arrange
        string identifier = "testId";
        List<Column> schema = new List<Column>();
        List<TableHeader> tableHeaders = new List<TableHeader>();
        List<TableRecord> tableRecords = new List<TableRecord>();

        List<Val> recordValList = new List<Val> { new IntVal(2) };
        TableRecord newRecord = new TableRecord(recordValList);

        // Act
        Table table = new Table(identifier, schema, tableHeaders, tableRecords);
        table.addRecord(newRecord);

        // Assert
        Assert.AreEqual(identifier, table.Identifier);
        Assert.AreEqual(schema, table.Schema);
        Assert.AreEqual(tableHeaders, table.Headers);
        Assert.AreEqual(recordValList, table.Records[0].Values);
    }

    [TestMethod]
    public void ParseTypesSchemaCountNotMatchTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", IntT.Instance) }, new List<string[]> { new string[] { "123", "321" } });

        // Assert
        try
        {
            table.ParseTypes();
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("File does not match schema", exception.Message);
        }
    }

    [TestMethod]
    public void ParseTypesSchemaHeadersNotMatchTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", IntT.Instance) }, new List<string[]> { new string[] { "test2" } });

        // Assert
        try
        {
            table.ParseTypes();
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Headers in file does not match schema", exception.Message);
        }
    }

    [TestMethod]
    public void ParseTypesFailureUnknownTypeTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", RowValT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "1" } });

        // Assert
        try
        {
            table.ParseTypes();
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Unknown type", exception.Message);
        }
    }

    [TestMethod]
    public void ParseTypesRowDoesNotMatchTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", IntT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "1", "2" } });

        // Assert
        try
        {
            table.ParseTypes();
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Row does not match schema", exception.Message);
        }
    }

    [TestMethod]
    public void ParseTypesSuccessIntTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", IntT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "1" } });

        // Act
        table.ParseTypes();

        // Assert
        Assert.AreEqual("test", table.Headers[0].Identifier);
        Assert.AreSame(IntT.Instance, table.Headers[0].Type);
        Assert.AreEqual(1, table.Records[0].Values[0].AsInt());
    }

    [TestMethod]
    public void ParseTypesSuccessFloatTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", FloatT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "1" } });

        // Act
        table.ParseTypes();

        // Assert
        Assert.AreEqual("test", table.Headers[0].Identifier);
        Assert.AreSame(FloatT.Instance, table.Headers[0].Type);
        Assert.AreEqual(1, table.Records[0].Values[0].AsFloat());
    }

    [TestMethod]
    public void ParseTypesSuccessBoolTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", BoolT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "True" } });

        // Act
        table.ParseTypes();

        // Assert
        Assert.AreEqual("test", table.Headers[0].Identifier);
        Assert.AreSame(BoolT.Instance, table.Headers[0].Type);
        Assert.IsTrue(table.Records[0].Values[0].AsBool());
    }

    [TestMethod]
    public void ParseTypesSuccessStringTest()
    {
        // Arrange
        Table table = new Table("testId", new List<Column> { new Column("test", StringT.Instance) }, new List<string[]> { new string[] { "test" }, new string[] { "Hej" } });

        // Act
        table.ParseTypes();

        // Assert
        Assert.AreEqual("test", table.Headers[0].Identifier);
        Assert.AreSame(StringT.Instance, table.Headers[0].Type);
        Assert.AreEqual("Hej", table.Records[0].Values[0].ToString());
    }
}