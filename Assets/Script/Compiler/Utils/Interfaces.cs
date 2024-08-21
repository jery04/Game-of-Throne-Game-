using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public interface IParsing
{
    ProgramCompiler Parse();
}
public interface ISemantic
{
    public bool CheckSemantic(IScope scope);
}
public interface IScope 
{
    public IScope? Parent { get; set; }
    public Dictionary<string, GeneralStatement?> Defined { get; set; }
    public bool IsDefined(string? search);
    public void Define(Variable variable);
    public IScope CreateChild();
    public Utils.ReturnType? GetType(string? search, IScope scope);
}
public interface IVisitor
{
    public IVisitor? Parent { get; set; }
    public void AddIncrease();
    public IScope? Scope { get; set; }
    public List<string> IncreaseVariables { get; set; }
    public Dictionary<string, object> Defined { get; set; }
    public bool IsDefined(string search);
    public void Define(Variable variable);
    public void Define(string? name, object? value);
    public IVisitor CreateChild(IScope? scope);
    public IVisitor CreateChild();
    public void AddInstance();
    public object? GetValue(string? search);
}
