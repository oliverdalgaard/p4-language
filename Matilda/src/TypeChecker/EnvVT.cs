namespace Matilda;

public class EnvVT
{
    private readonly EnvVT? parentScope;
    private Dictionary<string, Type?> bindings;

    public EnvVT(EnvVT? parentScope = null, Dictionary<string, Type?>? bindings = null)
    {
        this.parentScope = parentScope;
        this.bindings = bindings ?? new Dictionary<string, Type?>();
    }

    public EnvVT NewScope()
    {
        return new EnvVT(this);
    }

    public void Bind(string variable, Type? value)
    {
        if (IsLocal(variable))
        {
            throw new Exception($"The identifer {variable} has already been bound in the local scope.");
        }

        bindings[variable] = value;
    }

    public void Set(string variable, Type? value)
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

    public Type? TryGet(string variable)
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