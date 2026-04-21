namespace Matilda;

public class EnvP
{
    private readonly EnvP? parentScope;
    private Dictionary<string, FunctionDeclaration> bindings;

    public EnvP(EnvP? parentScope = null, Dictionary<string, FunctionDeclaration>? bindings = null)
    {
        this.parentScope = parentScope;
        this.bindings = bindings ?? new Dictionary<string, FunctionDeclaration>();
    }

    public void Bind(FunctionDeclaration func)
    {
        if (IsLocal(func.Identifier))
        {
            throw new Exception($"The identifer {func.Identifier} has already been bound in the local scope.");
        }

        bindings[func.Identifier] = func;
    }

    public FunctionDeclaration? TryGet(string function)
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
