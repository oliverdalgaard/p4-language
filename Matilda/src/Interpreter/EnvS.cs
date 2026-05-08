namespace Matilda;

public class EnvS
{
    private readonly EnvS? parentScope;
    public Dictionary<string, List<Column>?> Bindings { get; }

    public EnvS(EnvS? parentScope = null, Dictionary<string, List<Column>?>? bindings = null)
    {
        this.parentScope = parentScope;
        Bindings = bindings ?? new Dictionary<string, List<Column>?>();
    }

    public EnvS NewScope()
    {
        return new EnvS(this);
    }

    public void Bind(string variable, List<Column>? value)
    {
        if (IsLocal(variable))
        {
            throw new Exception($"The identifer {variable} has already been bound in the local scope.");
        }

        Bindings[variable] = value;
    }

    public void Set(string variable, List<Column>? value)
    {
        if (Bindings.ContainsKey(variable))
        {
            Bindings[variable] = value;
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

    public List<Column>? TryGet(string variable)
    {
        if (IsLocal(variable))
        {
            return Bindings[variable];
        }
        else
        {
            return parentScope?.TryGet(variable);
        }
    }

    private bool IsLocal(string variable)
    {
        return Bindings.ContainsKey(variable);
    }

}