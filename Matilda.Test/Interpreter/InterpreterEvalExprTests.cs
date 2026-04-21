using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matilda;
using System;

namespace MatildaTests;

[TestClass]
public class InterpreterEvalExprTests
{
    [TestMethod]
    public void EvalExprPrecedenceCheck()
  {
      // Arange
      Expr expr = new BinaryOp(BinaryOperators.ADD, new IntV(1,1),new BinaryOp(BinaryOperators.MUL, new IntV(2,1), new IntV(3,1),2),1);

      var envV = new EnvV();
      var envP = new EnvP();

      // Act
      var reuslt = Interpreter.EvalExpr(expr, envV, envP);

      // Assert
      Assert.IsInstanceOfType(reuslt, typeof(IntVal));
      Assert.AreEqual(7, reuslt.AsInt());
  }
}
