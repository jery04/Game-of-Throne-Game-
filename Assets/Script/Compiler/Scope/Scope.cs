using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public List<string> IncreaseVariables {  get; set; }  
    public IScope? Scope { get; set; }

    // Builder
    public Visitor()
    {
        this.IncreaseVariables = new List<string>();
        this.Defined = new Dictionary<string, object>();
        this.Parent = null;
    }
    public Visitor(IScope? scope)
    {
        this.IncreaseVariables = new List<string>();
        this.Scope = scope;
        this.Defined = new Dictionary<string, object>();
        this.Parent = null;
    }

    // Methods
    public void AddInstance()
    {
        if(Scope is not null)
        {
            Dictionary<string, GeneralStatement?> instance = Scope.Defined;

            foreach (string variable in instance.Keys)
                Define(variable, instance[variable]?.Evaluate(Scope));
        }
    }
    public void AddIncrease()
    {
        foreach(string variable in IncreaseVariables)
        {
            int value = Convert.ToInt32(GetValue(variable));
            Define(variable, value+1);
        }
    }
    public object? GetValue(string? search)
    {
        if ((search != null) && this.IsDefined(search))
        {  
            if (this.Defined.ContainsKey(search))
                return this.Defined[search];

            else if (this.Parent != null)
                return this.Parent.GetValue(search);
        }

        return null;
    }
    public void Define(Variable variable)
    {
        string? name = variable.Name?.Value;
        GeneralStatement? value = variable.Value;

        if (!(name is null) && !(value is null))
        {
            if (!Defined.ContainsKey(name))
                Defined.Add(name, value);
            else
                Defined[name] = value;
        }
    }
    public void Define(string? name, object? value)
    {
        if (!(name is null) && !(value is null))
        {
            if(!Defined.ContainsKey(name))
                Defined.Add(name, value);
            else
                Defined[name] = value;
        }          
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
    public IVisitor CreateChild(IScope? scope)
    {
        Visitor child = new Visitor(scope);
        child.Parent = this;

        return child;
    }
    public IVisitor CreateChild()
    {
        Visitor child = new Visitor();
        child.Parent = this;

        return child;
    }
}
