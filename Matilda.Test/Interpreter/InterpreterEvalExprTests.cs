using Matilda;
namespace MatildaTests;

[TestClass]
public class InterpreterEvalExprTests

{
  // Test 1: 1 + (2 * 3) = 9

  [TestMethod]
  public void EvalExprPrecedenceCheck1()
  {
    // Arange
    Expr left = new IntV(1, -1);

    Expr multiplyLeft = new IntV(2, -1);
    Expr multiplyRight = new IntV(3, -1);
    Expr right = new BinaryOp(BinaryOperators.MUL, multiplyLeft, multiplyRight, -1);

    Expr expr = new BinaryOp(BinaryOperators.ADD, left, right, -1);

    EnvV envV = new EnvV();
    EnvP envP = new EnvP();
    EnvS envS = new EnvS();

    // Act
    var result = Interpreter.EvalExpr(expr, envV, envP, envS);

    // Assert
    Assert.IsInstanceOfType(result, typeof(IntVal));
    Assert.AreEqual(7, result.AsInt());
    Assert.AreNotEqual(9, result.AsInt());
  }


  // Test 2: 1 + (2 * 3) + 4 = 11

  [TestMethod]
  public void EvalExprPrecedenceCheck2()
  {
    // Arrange
    Expr mul = new BinaryOp(
        BinaryOperators.MUL,
        new IntV(2, -1),
        new IntV(3, -1),
        -1
    );

    Expr leftAdd = new BinaryOp(
        BinaryOperators.ADD,
        new IntV(1, -1),
        mul,
        -1
    );

    Expr expr = new BinaryOp(
            BinaryOperators.ADD,
            leftAdd,
            new IntV(4, -1),
            -1
        );

    EnvV envV = new EnvV();
    EnvP envP = new EnvP();
    EnvS envS = new EnvS();

    // Act
    var result = Interpreter.EvalExpr(expr, envV, envP, envS);

    // Assert
    Assert.IsInstanceOfType<IntVal>(result);
    Assert.AreEqual(11, result.AsInt());
  }


  // Test 3: (2 * 3) + (4 * 5) = 26
  [TestMethod]
  public void EvalExprPrecedenceCheck3()
  {
    // Arrange
    Expr left = new BinaryOp(
        BinaryOperators.MUL,
        new IntV(2, -1),
        new IntV(3, -1),
        -1
    );

    Expr right = new BinaryOp(
        BinaryOperators.MUL,
        new IntV(4, -1),
        new IntV(5, -1),
        -1
    );

    Expr expr = new BinaryOp(
        BinaryOperators.ADD,
        left,
        right,
        -1
    );

    EnvV envV = new EnvV();
    EnvP envP = new EnvP();
    EnvS envS = new EnvS();

    // Act
    var result = Interpreter.EvalExpr(expr, envV, envP, envS);

    // Assert
    Assert.IsInstanceOfType<IntVal>(result);
    Assert.AreEqual(26, result.AsInt());
  }


  // Test 4: 8 + (6 / 2) = 11
  [TestMethod]
  public void EvalExprPrecedenceCheck4()
  {
    // Arrange
    Expr div = new BinaryOp(
        BinaryOperators.DIV,
        new IntV(6, -1),
        new IntV(2, -1),
        -1
    );

    Expr expr = new BinaryOp(
        BinaryOperators.ADD,
        new IntV(8, -1),
        div,
        -1
    );

    EnvV envV = new EnvV();
    EnvP envP = new EnvP();
    EnvS envS = new EnvS();
    
    // Act
    var result = Interpreter.EvalExpr(expr,  envV, envP, envS);

    // Assert
    Assert.IsInstanceOfType<FloatVal>(result);
    Assert.AreEqual(11, result.AsFloat());
  }

  // Test 5: 5 + (0 * 99) = 5

  [TestMethod]
  public void EvalExprPrecedenceCheck7()
  {
    // Arrange
    Expr mul = new BinaryOp(
        BinaryOperators.MUL,
        new IntV(0, -1),
        new IntV(99, -1),
        -1
    );

    Expr expr = new BinaryOp(
        BinaryOperators.ADD,
        new IntV(5, -1),
        mul,
        -1
    );
    
    EnvV envV = new EnvV();
    EnvP envP = new EnvP();
    EnvS envS = new EnvS();

    // Act
    var result = Interpreter.EvalExpr(expr, envV, envP, envS);

    // Assert
    Assert.IsInstanceOfType<IntVal>(result);
    Assert.AreEqual(5, result.AsInt());
  }
}