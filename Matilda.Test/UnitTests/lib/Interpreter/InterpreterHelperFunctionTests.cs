using Matilda;

namespace MatildaTests.UnitTests.LibTests.InterpreterTests.InterpreterHelperFunctionTests;

[TestClass]
public class IsEqualTests
{
    [TestMethod]
    public void IntAndIntEqualTrueTest()
    {
        // Arrange

        IntVal val = new IntVal(10);
        IntVal val2 = new IntVal(10);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, val.AsInt());
        Assert.AreEqual(10, val2.AsInt());
    }

    [TestMethod]
    public void IntAndIntEqualFalseTest()
    {
        // Arrange

        IntVal val = new IntVal(10);
        IntVal val2 = new IntVal(12);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, val.AsInt());
        Assert.AreEqual(12, val2.AsInt());
    }

    [TestMethod]
    public void FloatAndFloatEqualTrueTest()
    {
        // Arrange

        FloatVal val = new FloatVal(10);
        FloatVal val2 = new FloatVal(10);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, val.AsFloat());
        Assert.AreEqual(10, val2.AsFloat());
    }

    [TestMethod]
    public void FloatAndFloatEqualFalseTest()
    {
        // Arrange

        FloatVal val = new FloatVal(10);
        FloatVal val2 = new FloatVal(12);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, val.AsFloat());
        Assert.AreEqual(12, val2.AsFloat());
    }

    [TestMethod]
    public void FloatAndIntEqualTrueTest()
    {
        // Arrange
        FloatVal val = new FloatVal(10);
        IntVal val2 = new IntVal(10);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, val.AsFloat());
        Assert.AreEqual(10, val2.AsInt());
    }

    [TestMethod]
    public void FloatAndIntEqualFalseTest()
    {
        // Arrange

        FloatVal val = new FloatVal(10);
        IntVal val2 = new IntVal(12);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, val.AsFloat());
        Assert.AreEqual(12, val2.AsInt());
    }

    [TestMethod]
    public void IntAndFloatEqualTrueTest()
    {
        // Arrange

        IntVal val = new IntVal(10);
        FloatVal val2 = new FloatVal(10);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, val.AsInt());
        Assert.AreEqual(10, val2.AsFloat());
    }

    [TestMethod]
    public void IntAndFloatEqualFalseTest()
    {
        // Arrange

        IntVal val = new IntVal(10);
        FloatVal val2 = new FloatVal(12);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, val.AsInt());
        Assert.AreEqual(12, val2.AsFloat());
    }

    [TestMethod]
    public void BoolAndBoolEqualTrueTest()
    {
        // Arrange
        BoolVal val = new BoolVal(true);
        BoolVal val2 = new BoolVal(true);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(val.AsBool());
        Assert.IsTrue(val2.AsBool());
    }

    [TestMethod]
    public void BoolAndBoolEqualFalseTest()
    {
        // Arrange
        BoolVal val = new BoolVal(true);
        BoolVal val2 = new BoolVal(false);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(val.AsBool());
        Assert.IsFalse(val2.AsBool());
    }

    [TestMethod]
    public void IntAndBoolEqualFalseTest()
    {
        // Arrange
        IntVal val = new IntVal(10);
        BoolVal val2 = new BoolVal(true);

        // Act
        bool result = InterpreterHelperFunction.IsEqual(val, val2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, val.AsInt());
        Assert.IsTrue(val2.AsBool());
    }

    [TestMethod]
    public void ValAndValLessThanFalseTest()
    {
        // Arrange
        IntVal intVal = new IntVal(10);
        FloatVal floatVal = new FloatVal(10);

        // Act
        bool result1 = InterpreterHelperFunction.HelperFunctionLT(intVal, floatVal);
        bool result2 = InterpreterHelperFunction.HelperFunctionLT(floatVal, intVal);
        bool result3 = InterpreterHelperFunction.HelperFunctionLT(floatVal, floatVal);
        bool result4 = InterpreterHelperFunction.HelperFunctionLT(intVal, intVal);

        // Assert
        Assert.AreEqual(10, intVal.AsInt());
        Assert.AreEqual(10, floatVal.AsFloat());
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
        Assert.IsFalse(result3);
        Assert.IsFalse(result4);
    }

    [TestMethod]
    public void ValAndValLessThanTrueTest()
    {
        // Arrange
        IntVal intVal = new IntVal(9);
        FloatVal floatVal = new FloatVal(10);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(9);

        // Act
        bool result1 = InterpreterHelperFunction.HelperFunctionLT(intVal, floatVal);
        bool result2 = InterpreterHelperFunction.HelperFunctionLT(floatVal2, intVal2);
        bool result3 = InterpreterHelperFunction.HelperFunctionLT(floatVal2, floatVal);
        bool result4 = InterpreterHelperFunction.HelperFunctionLT(intVal, intVal2);

        // Assert
        Assert.AreEqual(9, intVal.AsInt());
        Assert.AreEqual(10, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(9, floatVal2.AsFloat());
        Assert.IsTrue(result1);
        Assert.IsTrue(result2);
        Assert.IsTrue(result3);
        Assert.IsTrue(result4);
    }

    [TestMethod]
    public void BoolAndBoolLessThanThrowTest()
    {
        // Arrange
        Val val1 = new BoolVal(true);
        Val val2 = new BoolVal(false);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionLT(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Type error: '<' supports only numeric types (int/float)", exception.Message);
        }
    }

    [TestMethod]
    public void ValAndValAddTrueTest()
    {
        // Arrange
        IntVal intVal = new IntVal(9);
        FloatVal floatVal = new FloatVal(10);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(9);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionADD(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionADD(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionADD(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionADD(intVal, intVal2);

        // Assert
        Assert.AreEqual(9, intVal.AsInt());
        Assert.AreEqual(10, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(9, floatVal2.AsFloat());
        Assert.AreEqual(19, result1.AsFloat());
        Assert.AreEqual(19, result2.AsFloat());
        Assert.AreEqual(19, result3.AsFloat());
        Assert.AreEqual(19, result4.AsInt());
    }

    [TestMethod]
    public void ValAndValAddTrueTest2()
    {
        // Arrange
        IntVal intVal = new IntVal(5);
        FloatVal floatVal = new FloatVal(5);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(10);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionADD(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionADD(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionADD(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionADD(intVal, intVal2);

        // Assert
        Assert.AreEqual(5, intVal.AsInt());
        Assert.AreEqual(5, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(10, floatVal2.AsFloat());
        Assert.AreEqual(10, result1.AsFloat());
        Assert.AreEqual(20, result2.AsFloat());
        Assert.AreEqual(15, result3.AsFloat());
        Assert.AreEqual(15, result4.AsInt());
    }

    [TestMethod]
    public void BoolAndBoolAddThrowTest()
    {
        // Arrange
        Val val1 = new BoolVal(true);
        Val val2 = new BoolVal(false);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionADD(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Type error: '+' supports only numeric types (int/float)", exception.Message);
        }
    }

    [TestMethod]
    public void ValAndValSubTrueTest()
    {
        // Arrange
        IntVal intVal = new IntVal(9);
        FloatVal floatVal = new FloatVal(10);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(9);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionSUB(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionSUB(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionSUB(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionSUB(intVal, intVal2);

        // Assert
        Assert.AreEqual(9, intVal.AsInt());
        Assert.AreEqual(10, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(9, floatVal2.AsFloat());
        Assert.AreEqual(-1, result1.AsFloat());
        Assert.AreEqual(-1, result2.AsFloat());
        Assert.AreEqual(-1, result3.AsFloat());
        Assert.AreEqual(-1, result4.AsInt());
    }

    [TestMethod]
    public void ValAndValSubTrueTest2()
    {
        // Arrange
        IntVal intVal = new IntVal(5);
        FloatVal floatVal = new FloatVal(5);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(10);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionSUB(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionSUB(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionSUB(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionSUB(intVal, intVal2);

        // Assert
        Assert.AreEqual(5, intVal.AsInt());
        Assert.AreEqual(5, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(10, floatVal2.AsFloat());
        Assert.AreEqual(0, result1.AsFloat());
        Assert.AreEqual(0, result2.AsFloat());
        Assert.AreEqual(5, result3.AsFloat());
        Assert.AreEqual(-5, result4.AsInt());
    }

    [TestMethod]
    public void BoolAndBoolSubThrowTest()
    {
        // Arrange
        Val val1 = new BoolVal(true);
        Val val2 = new BoolVal(false);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionSUB(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Type error: '-' supports only numeric types (int/float)", exception.Message);
        }
    }

    [TestMethod]
    public void ValAndValMulTrueTest()
    {
        // Arrange
        IntVal intVal = new IntVal(9);
        FloatVal floatVal = new FloatVal(10);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(9);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionMUL(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionMUL(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionMUL(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionMUL(intVal, intVal2);

        // Assert
        Assert.AreEqual(9, intVal.AsInt());
        Assert.AreEqual(10, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(9, floatVal2.AsFloat());
        Assert.AreEqual(90, result1.AsFloat());
        Assert.AreEqual(90, result2.AsFloat());
        Assert.AreEqual(90, result3.AsFloat());
        Assert.AreEqual(90, result4.AsInt());
    }

    [TestMethod]
    public void ValAndValMulTrueTest2()
    {
        // Arrange
        IntVal intVal = new IntVal(5);
        FloatVal floatVal = new FloatVal(5);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(10);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionMUL(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionMUL(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionMUL(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionMUL(intVal, intVal2);

        // Assert
        Assert.AreEqual(5, intVal.AsInt());
        Assert.AreEqual(5, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(10, floatVal2.AsFloat());
        Assert.AreEqual(25, result1.AsFloat());
        Assert.AreEqual(100, result2.AsFloat());
        Assert.AreEqual(50, result3.AsFloat());
        Assert.AreEqual(50, result4.AsInt());
    }

    [TestMethod]
    public void BoolAndBoolMulThrowTest()
    {
        // Arrange
        Val val1 = new BoolVal(true);
        Val val2 = new BoolVal(false);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionMUL(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Type error: '*' supports only numeric types (int/float)", exception.Message);
        }
    }

    [TestMethod]
    public void ValAndValDivTrueTest()
    {
        // Arrange
        IntVal intVal = new IntVal(2);
        FloatVal floatVal = new FloatVal(1);
        IntVal intVal2 = new IntVal(1);
        FloatVal floatVal2 = new FloatVal(2);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionDIV(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionDIV(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionDIV(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionDIV(intVal, intVal2);

        // Assert
        Assert.AreEqual(2, intVal.AsInt());
        Assert.AreEqual(1, floatVal.AsFloat());
        Assert.AreEqual(1, intVal2.AsInt());
        Assert.AreEqual(2, floatVal2.AsFloat());
        Assert.AreEqual(2, result1.AsFloat());
        Assert.AreEqual(2, result2.AsFloat());
        Assert.AreEqual(2, result3.AsFloat());
        Assert.AreEqual(2, result4.AsFloat());
    }

    [TestMethod]
    public void ValAndValDivTrueTest2()
    {
        // Arrange
        IntVal intVal = new IntVal(5);
        FloatVal floatVal = new FloatVal(5);
        IntVal intVal2 = new IntVal(10);
        FloatVal floatVal2 = new FloatVal(10);

        // Act
        Val result1 = InterpreterHelperFunction.HelperFunctionDIV(intVal, floatVal);
        Val result2 = InterpreterHelperFunction.HelperFunctionDIV(floatVal2, intVal2);
        Val result3 = InterpreterHelperFunction.HelperFunctionDIV(floatVal2, floatVal);
        Val result4 = InterpreterHelperFunction.HelperFunctionDIV(intVal, intVal2);

        // Assert
        Assert.AreEqual(5, intVal.AsInt());
        Assert.AreEqual(5, floatVal.AsFloat());
        Assert.AreEqual(10, intVal2.AsInt());
        Assert.AreEqual(10, floatVal2.AsFloat());
        Assert.AreEqual(1, result1.AsFloat());
        Assert.AreEqual(1, result2.AsFloat());
        Assert.AreEqual(2, result3.AsFloat());
        Assert.AreEqual(0.5, result4.AsFloat());
    }

    [TestMethod]
    public void BoolAndBoolDivThrowTest()
    {
        // Arrange
        Val val1 = new BoolVal(true);
        Val val2 = new BoolVal(false);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionDIV(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Type error: '/' supports only numeric types (int/float)", exception.Message);
        }
    }

    [TestMethod]
    public void DivisionByZero()
    {
        // Arrange
        Val val1 = new IntVal(1);
        Val val2 = new IntVal(0);

        // Assert
        try
        {
            InterpreterHelperFunction.HelperFunctionDIV(val1, val2);
            Assert.Fail();
        }
        catch (Exception exception)
        {
            Assert.AreEqual("Division by zero not allowed.", exception.Message);
        }
    }
}