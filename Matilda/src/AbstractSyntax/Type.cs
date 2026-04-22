namespace Matilda;

public abstract class Type
{

}

public sealed class IntT : Type
{
    public static readonly IntT Instance = new IntT();

    public IntT() { }
}

public sealed class FloatT : Type
{
    public static readonly FloatT Instance = new FloatT();

    public FloatT() { }
}

public sealed class BoolT : Type
{
    public static readonly BoolT Instance = new BoolT();

    public BoolT() { }
}

public sealed class StringT : Type
{
    public static readonly StringT Instance = new StringT();

    public StringT() { }
}