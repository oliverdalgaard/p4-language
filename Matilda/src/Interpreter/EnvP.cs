namespace Matilda;

public class EnvP
{
    private readonly EnvP? parentScope;
    public Dictionary<string, FunctionDeclaration> Bindings { get; }

    public EnvP(EnvP? parentScope = null, Dictionary<string, FunctionDeclaration>? bindings = null)
    {
        this.parentScope = parentScope;
        Bindings = bindings ?? new Dictionary<string, FunctionDeclaration>();
    }

    public void Bind(FunctionDeclaration func)
    {
        if (IsLocal(func.Identifier))
        {
            throw new Exception($"The identifer {func.Identifier} has already been bound in the local scope.");
        }

        Bindings[func.Identifier] = func;
    }

    public FunctionDeclaration? TryGet(string function)
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
