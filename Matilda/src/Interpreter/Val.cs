namespace Matilda;

public abstract class Val
{
    public virtual int AsInt()
    {
        throw new Exception("Value is not an int");
    }

    public virtual float AsFloat()
    {
        throw new Exception("Value is not a float");
    }

    public virtual bool AsBool()
    {
        throw new Exception("Value is not a bool");
    }

    public virtual Table AsTable()
    {
        throw new Exception("Value is not a table");
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
public class FloatVal : Val
{
    public float F { get; }

    public FloatVal(float f)
    {
        F = f;
    }

    public override float AsFloat()
    {
        return F;
    }

    public override string ToString()
    {
        return F.ToString();
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

public class StringVal : Val
{
    public string S { get; }

    public StringVal(string s)
    {
        S = s;
    }

    public override string ToString()
    {
        return S;
    }
}

public class TableVal : Val
{
    public Table T { get; }

    public TableVal(Table t)
    {
        T = t;
    }

    public override Table AsTable()
    {
        return T;
    }

    public override string ToString()
    {
        string returnString = "| ";

        foreach (TableHeader thead in T.Headers)
        {
            returnString += thead.Identifier + " |";
        }

        returnString += "\n";

        foreach (TableRecord tRecord in T.Records)
        {
            returnString += "| ";
            foreach (Val value in tRecord.Values)
            {
                returnString += value.ToString() + " |";
            }
            returnString += "\n";
        }

        return returnString;
    }
}