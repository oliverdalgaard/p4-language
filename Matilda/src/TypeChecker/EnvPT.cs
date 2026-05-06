namespace Matilda;

public class EnvPT
{
    private readonly EnvPT? parentScope;
    public Dictionary<string, FunctionType> Bindings { get; }

    public EnvPT(EnvPT? parentScope = null, Dictionary<string, FunctionType>? bindings = null)
    {
        this.parentScope = parentScope;
        Bindings = bindings ?? new Dictionary<string, FunctionType>();
    }

    public void Bind(string identifier, FunctionType funcType)
    {
        if (IsLocal(identifier))
        {
            throw new Exception($"The identifer {identifier} has already been bound in the local scope.");
        }

        Bindings[identifier] = funcType;
    }

    public FunctionType? TryGet(string function)
    {
        if (IsLocal(function))
        {
            return Bindings[function];
        }
        else
        {
            return parentScope?.TryGet(function);
        }
    }

    private bool IsLocal(string function)
    {
        return Bindings.ContainsKey(function);
    }

}
