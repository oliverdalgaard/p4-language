namespace Matilda;

public class EnvPT
{
    private readonly EnvPT? parentScope;
    private Dictionary<string, FunctionType> bindings;

    public EnvPT(EnvPT? parentScope = null, Dictionary<string, FunctionType>? bindings = null)
    {
        this.parentScope = parentScope;
        this.bindings = bindings ?? new Dictionary<string, FunctionType>();
    }

    public void Bind(string identifier, FunctionType funcType)
    {
        if (IsLocal(identifier))
        {
            throw new Exception($"The identifer {identifier} has already been bound in the local scope.");
        }

        bindings[identifier] = funcType;
    }

    public FunctionType? TryGet(string function)
    {
        if (IsLocal(function))
        {
            return bindings[function];
        }
        else
        {
            return parentScope?.TryGet(function);
        }
    }

    private bool IsLocal(string function)
    {
        return bindings.ContainsKey(function);
    }

}
