using Matilda;

namespace MatildaTests.UnitTests.LibTests.TypeCheckerTests.CompareSchemaTests;

[TestClass]
public class CompareSchemaTests
{
    [TestMethod]
    public void CompareCheckNullTest()
    {
        // Arrange
        List<Column>? l1Null = null;
        List<Column>? l2Null = null;
        List<Column>? l1 = new List<Column>();
        List<Column>? l2 = new List<Column>();

        // Act
        bool result1 = CompareSchema.Compare(l1Null, l2Null);
        bool result2 = CompareSchema.Compare(l1Null, l2);
        bool result3 = CompareSchema.Compare(l1, l2Null);

        // Assert
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
        Assert.IsFalse(result3);
    }

    [TestMethod]
    public void CompareCheckDifferentCountTest()
    {
        // Arrange
        List<Column>? l1 = new List<Column> { new Column("test", IntT.Instance), new Column("test2", IntT.Instance) };
        List<Column>? l2 = new List<Column> { new Column("test", IntT.Instance) };

        // Act
        bool result1 = CompareSchema.Compare(l1, l2);

        // Assert
        Assert.IsFalse(result1);
    }

    [TestMethod]
    public void CompareCheckDifferentSchemasTest()
    {
        // Arrange
        List<Column>? l1False = new List<Column> { new Column("test", IntT.Instance) };
        List<Column>? l2False = new List<Column> { new Column("test2", IntT.Instance) };
        List<Column>? l1False2 = new List<Column> { new Column("test", IntT.Instance) };
        List<Column>? l2False2 = new List<Column> { new Column("test", FloatT.Instance) };
        List<Column>? l1True = new List<Column> { new Column("test", IntT.Instance) };
        List<Column>? l2True = new List<Column> { new Column("test", IntT.Instance) };

        // Act
        bool result1 = CompareSchema.Compare(l1False, l2False);
        bool result2 = CompareSchema.Compare(l1False2, l2False2);
        bool result3 = CompareSchema.Compare(l1True, l2True);

        // Assert
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
        Assert.IsTrue(result3);
    }

    [TestMethod]
    public void ContainsDuplicateTest()
    {
        // Arrange
        List<Column>? l1Duplicate = new List<Column> { new Column("test1", IntT.Instance), new Column("test1", IntT.Instance) };
        List<Column>? l1DuplicateDifferentType = new List<Column> { new Column("test1", IntT.Instance), new Column("test1", FloatT.Instance) };
        List<Column>? l1NoDuplicate = new List<Column> { new Column("test1", IntT.Instance), new Column("test2", IntT.Instance) };

        // Act
        bool result1 = CompareSchema.ContainsDuplicate(l1Duplicate);
        bool result2 = CompareSchema.ContainsDuplicate(l1DuplicateDifferentType);
        bool result3 = CompareSchema.ContainsDuplicate(l1NoDuplicate);

        // Assert
        Assert.IsTrue(result1);
        Assert.IsTrue(result2);
        Assert.IsFalse(result3);
    }
}