using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matilda;

namespace MatildaTests;

[TestClass]
public class InterpreterEvalExprTests

    // TEST FOR PRECENCE //
{
    [TestMethod]
    public void EvalExprPrecedenceCheck1()
  {
      // Arange
      Expr expr = new BinaryOp(BinaryOperators.ADD, new IntV(1,-1),new BinaryOp(BinaryOperators.MUL, new IntV(2,-1), new IntV(3,-1),2),-1);

      var envV = new EnvV();
      var envP = new EnvP();

      // Act
      var result = Interpreter.EvalExpr(expr, envV, envP);

      // Assert
      Assert.IsInstanceOfType(result, typeof(IntVal));
      Assert.AreEqual(7, result.AsInt());
      Assert.AreNotEqual(9, result.AsInt());
  }  
}
