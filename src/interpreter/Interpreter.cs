namespace Matilda
{

    public static class Interpreter
    {
        public static void evalStmt(Stmt stmt)
        {
            switch (stmt)
            {
                case Skip:
                    Console.WriteLine("Skipped");
                    break;

                case Print print:
                    Val value = evalExpr(print.Value);
                    Console.WriteLine(value.ToString());
                    break;



                default:
                    throw new Exception("Not valid statement");

            }
        }

        public static Val evalExpr(Expr expr)
        {
            return new IntVal(2);
        }
    }

}