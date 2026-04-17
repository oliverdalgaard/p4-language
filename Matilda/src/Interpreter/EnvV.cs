namespace Matilda;

public class EnvV
{
    private readonly EnvV? parentScope;
    private Dictionary<string, Val?> bindings;

    public EnvV(EnvV? parentScope = null, Dictionary<string, Val?>? bindings = null)
    {
        this.parentScope = parentScope;
        this.bindings = bindings ?? new Dictionary<string, Val?>();
    }

    public EnvV NewScope()
    {
        return new EnvV(this);
    }

    public void Bind(string variable, Val value)
    {
        if (IsLocal(variable))
        {
            throw new Exception($"The identifer {variable} has already been bound in the local scope.");
        }

        bindings[variable] = value;
    }

    public void Set(string variable, Val? value)
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

    public Val? TryGet(string variable)
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