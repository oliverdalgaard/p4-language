namespace Matilda;

public class FunctionType
{
    public List<Type> Parameters { get; }
    public Type ReturnType { get; }

    public FunctionType(FunctionDeclaration f)
    {
        Parameters = new List<Type>();
        ReturnType = f.Type;

        foreach (Parameter param in f.Parameters)
        {
            Parameters.Add(param.Type);
        }
    }
}