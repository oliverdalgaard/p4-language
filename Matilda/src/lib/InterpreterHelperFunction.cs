namespace Matilda;

public class InterpreterHelperFunction
{
    public static bool IsEqual(Val a, Val b)
    {
        if (a is IntVal ai && b is IntVal bi)
        {
            return ai.AsInt() == bi.AsInt();
        }
        else if (a is FloatVal af && b is FloatVal bf)
        {
            return af.AsFloat() == bf.AsFloat();
        }
        else if (a is FloatVal af2 && b is IntVal bi2)
        {
            return af2.AsFloat() == bi2.AsInt();
        }
        else if (a is IntVal ai2 && b is FloatVal bf2)
        {
            return ai2.AsInt() == bf2.AsFloat();
        }
        else if (a is BoolVal ab && b is BoolVal bb)
        {
            return ab.AsBool() == bb.AsBool();
        }
        return false;
    }

    public static bool HelperFunctionLT(Val v1, Val v2)
    {
        if (v1 is IntVal ai && v2 is IntVal bi)
        {
            return ai.AsInt() < bi.AsInt();
        }
        else if (v1 is FloatVal af && v2 is FloatVal bf)
        {
            return af.AsFloat() < bf.AsFloat();
        }
        else if (v1 is FloatVal af2 && v2 is IntVal bi2)
        {
            return af2.AsFloat() < bi2.AsInt();
        }
        else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
        {
            return ai2.AsInt() < bf2.AsFloat();
        }
        throw new Exception("Type error: '<' supports only numeric types (int/float)");
    }

    public static Val HelperFunctionADD(Val v1, Val v2)
    {
        if (v1 is IntVal ai && v2 is IntVal bi)
        {
            return new IntVal(ai.AsInt() + bi.AsInt());
        }
        else if (v1 is FloatVal af && v2 is FloatVal bf)
        {
            return new FloatVal(af.AsFloat() + bf.AsFloat());
        }
        else if (v1 is FloatVal af2 && v2 is IntVal bi2)
        {
            return new FloatVal(af2.AsFloat() + bi2.AsInt());
        }
        else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
        {
            return new FloatVal(ai2.AsInt() + bf2.AsFloat());
        }
        throw new Exception("Type error: '+' supports only numeric types (int/float)");
    }

    public static Val HelperFunctionSUB(Val v1, Val v2)
    {
        if (v1 is IntVal ai && v2 is IntVal bi)
        {
            return new IntVal(ai.AsInt() - bi.AsInt());
        }
        else if (v1 is FloatVal af && v2 is FloatVal bf)
        {
            return new FloatVal(af.AsFloat() - bf.AsFloat());
        }
        else if (v1 is FloatVal af2 && v2 is IntVal bi2)
        {
            return new FloatVal(af2.AsFloat() - bi2.AsInt());
        }
        else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
        {
            return new FloatVal(ai2.AsInt() - bf2.AsFloat());
        }
        throw new Exception("Type error: '-' supports only numeric types (int/float)");
    }

    public static Val HelperFunctionMUL(Val v1, Val v2)
    {
        if (v1 is IntVal ai && v2 is IntVal bi)
        {
            return new IntVal(ai.AsInt() * bi.AsInt());
        }
        else if (v1 is FloatVal af && v2 is FloatVal bf)
        {
            return new FloatVal(af.AsFloat() * bf.AsFloat());
        }
        else if (v1 is FloatVal af2 && v2 is IntVal bi2)
        {
            return new FloatVal(af2.AsFloat() * bi2.AsInt());
        }
        else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
        {
            return new FloatVal(ai2.AsInt() * bf2.AsFloat());
        }
        throw new Exception("Type error: '*' supports only numeric types (int/float)");
    }

    public static Val HelperFunctionDIV(Val v1, Val v2)
    {
        if (v1 is IntVal ai && v2 is IntVal bi)
        {
            return new FloatVal(ai.AsInt() / bi.AsInt());
        }
        else if (v1 is FloatVal af && v2 is FloatVal bf)
        {
            return new FloatVal(af.AsFloat() / bf.AsFloat());
        }
        else if (v1 is FloatVal af2 && v2 is IntVal bi2)
        {
            return new FloatVal(af2.AsFloat() / bi2.AsInt());
        }
        else if (v1 is IntVal ai2 && v2 is FloatVal bf2)
        {
            return new FloatVal(ai2.AsInt() / bf2.AsFloat());
        }
        throw new Exception("Type error: '/' supports only numeric types (int/float)");
    }
}