namespace Matilda;

public class EnvST
{
    private readonly EnvST? parentScope;
    private Dictionary<string, List<Type>?> bindings;

    public EnvST(EnvST? parentScope = null, Dictionary<string, List<Type>?>? bindings = null)
    {
        this.parentScope = parentScope;
        this.bindings = bindings ?? new Dictionary<string, List<Type>?>();
    }

    public EnvST NewScope()
    {
        return new EnvST(this);
    }

    public void Bind(string variable, List<Type>? value)
    {
        if (IsLocal(variable))
        {
            throw new Exception($"The identifer {variable} has already been bound in the local scope.");
        }

        bindings[variable] = value;
    }

    public void Set(string variable, List<Type>? value)
    {
        if (bindings.ContainsKey(variable))
        {
            bindings[variable] = value;
        }
        else if (parentScope != null)
        {
            parentScope.Set(variable, value);
        }
        else
        {
            throw new Exception($"Failed to overwrite un-bound identifer {variable}.");
        }
    }

    public List<Type>? TryGet(string variable)
    {
        if (IsLocal(variable))
        {
            return bindings[variable];
        }
        else
        {
            return parentScope?.TryGet(variable);
        }
    }

    private bool IsLocal(string variable)
    {
        return bindings.ContainsKey(variable);
    }

}