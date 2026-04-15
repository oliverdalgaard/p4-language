using System.ComponentModel;

namespace Matilda
{
    public abstract class Type
    {

    }

    public sealed class IntT : Type
    {
        public static readonly IntT Instance = new IntT();

        private IntT() { }
    }

    public sealed class FloatT : Type
    {
        public static readonly FloatT Instance = new FloatT();

        private FloatT() { }
    }

    public sealed class BoolT : Type
    {
        public static readonly BoolT Instance = new BoolT();

        private BoolT() { }
    }

    public sealed class StringT : Type
    {
        public static readonly StringT Instance = new StringT();

        private StringT() { }
    }
}