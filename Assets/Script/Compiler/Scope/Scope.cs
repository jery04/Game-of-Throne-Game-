using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class Scope : IScope
{
    // Property
    public IScope? Parent { get; set; }
    public Dictionary<string, GeneralStatement?> Defined { get; set; }

    // Builder
    public Scope()
    {
        this.Defined = new Dictionary<string, GeneralStatement?>();
        this.Parent = null;
    }

    // Methods
    public Utils.ReturnType? GetType(string? search, IScope scope)
    {
        if ((search != null) && scope.IsDefined(search))
        {
            if (scope.Defined.ContainsKey(search))
                return scope.Defined[search]?.GetType(scope);

            else if (scope.Parent != null)
                return scope.Parent.GetType(search, scope.Parent);
        }

        return null;
    }
    public void Define(Variable variable)
    {
        if (!(variable.Name?.Value is null) && !Defined.ContainsKey(variable.Name.Value))
            Defined.Add(variable.Name.Value, variable.Value);
    }
    public bool IsDefined(string? search)
    {
        if (search is not null)
        {
            if (Defined.ContainsKey(search))
                return true;

            else if (Parent != null)
                return Parent.IsDefined(search);
        }

        return false;
    }
    public IScope CreateChild()
    {
        Scope child = new Scope();
        child.Parent = this;

        return child;
    }
}
public class Visitor : IVisitor
{
    // Property
    public IVisitor? Parent { get; set; }
    public Dictionary<string, object> Defined { get; set; }

    // Builder
    public Visitor()
    {
        this.Defined = new Dictionary<string, object>();
        this.Parent = null;
    }

    // Methods
    public object? GetValue(string search, IVisitor scope)
    {
        if ((search != null) && scope.IsDefined(search))
        {
            if (scope.Defined.ContainsKey(search))
                return scope.Defined[search];

            else if (scope.Parent != null)
                return scope.Parent.GetValue(search, scope.Parent);
        }

        return null;
    }
    public void Define(Variable variable)
    {
        string? name = variable.Name?.Value;
        GeneralStatement? value = variable.Value;

        if (!(name is null) && !(value is null) && !Defined.ContainsKey(name))
            Defined.Add(name, value);
    }
    public bool IsDefined(string? search)
    {
        if (search is not null)
        {
            if (Defined.ContainsKey(search))
                return true;

            else if (Parent != null)
                return Parent.IsDefined(search);
        }

        return false;
    }
    public IVisitor CreateChild()
    {
        Visitor child = new Visitor();
        child.Parent = this;

        return child;
    }
}
