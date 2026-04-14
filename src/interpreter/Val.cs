namespace Matilda
{
    public abstract class Val
    {
        public virtual int AsInt()
        {
            throw new Exception("Value is not an int");
        }

        public virtual bool AsBool()
        {
            throw new Exception("Value is not a bool");
        }
    }

    public class IntVal : Val
    {
        public int N { get; }

        public IntVal(int n)
        {
            N = n;
        }

        public override int AsInt()
        {
            return N;
        }

        public override string ToString()
        {
            return N.ToString();
        }
    }

    public class BoolVal : Val
    {
        public bool B { get; }

        public BoolVal(bool b)
        {
            B = b;
        }

        public override bool AsBool()
        {
            return B;
        }

        public override string ToString()
        {
            return B.ToString();
        }
    }
}