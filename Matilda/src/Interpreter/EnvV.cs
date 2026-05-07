using System.Reflection.Metadata;

namespace Matilda;

public class EnvV
{
    private readonly EnvV? parentScope;
    public Dictionary<string, Val?> Bindings { get; }

    public Val? FunctionReturnValue { get; set; }
    public bool IsFunctionScope { get; }

    public EnvV(EnvV? parentScope = null, Dictionary<string, Val?>? bindings = null, bool isFunctionScope = false)
    {
        this.parentScope = parentScope;
        Bindings = bindings ?? new Dictionary<string, Val?>();

        IsFunctionScope = isFunctionScope;
    }

    public EnvV NewScope(bool isFunctionScope = false)
    {
        return new EnvV(this, null, isFunctionScope);
    }

    public void Bind(string variable, Val? value)
    {
        if (IsLocal(variable))
        {
            throw new Exception($"The identifer {variable} has already been bound in the local scope.");
        }

        Bindings[variable] = value;
    }

    public void Set(string variable, Val? value)
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

    public Val? TryGet(string variable)
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