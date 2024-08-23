using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable
public class Scope: IScope
{
    // Property 
    public IScope? Parent { get; set; }                                 // Padre de este objeto en el árbol de Scope
    public Dictionary<string, GeneralStatement?> Defined { get; set; }  // Variables almacenadas (nombre y valores)

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
    }  // Retorna el tipo de variable (String, Bool, Digit)
    public void Define(Variable variable)
    {
        if (!(variable.Name?.Value is null) && !Defined.ContainsKey(variable.Name.Value))
            Defined.Add(variable.Name.Value, variable.Value);
    }                           // Agrega la variable
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
    }                           // Verifica si la variable está almacenada
    public IScope CreateChild()
    {
        Scope child = new Scope();
        child.Parent = this;

        return child;
    }                                     // Retorna un nuevo hijo de este Scope
}          // Alcance de las variables (Proceso Semántico)    
public class Visitor: IVisitor
{
    // Property
    public IVisitor? Parent { get; set; }                   // Padre de este objeto en el árbol de Visitor
    public IScope? Scope { get; set; }                      // Scope asociado al alcance (Máscara)
    public Dictionary<string, object> Defined { get; set; } // Variables almacenadas (nombre y valores)
    public List<string> IncreaseVariables {  get; set; }    // Variables a incrementadas dentro del alcance
    public Assig? Assig { get; set; }                       // Retorna el objeto auxiliar

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
    }               // Verifica si la variable está almacenada
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
    }               // Agrega la variable
    public void Define(string? name, object? value)
    {
        if (!(name is null) && !(value is null))
        {
            if (!Defined.ContainsKey(name))
                Defined.Add(name, value);
            else
                Defined[name] = value;
        }
    }     // Agrega la variable (nombre y valor)
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
    }             // Devuelve el valor específico de un tipo (variable)
    public IVisitor CreateChild()
    {
        Visitor child = new Visitor();
        child.Parent = this;

        return child;
    }                       // Retorna un nuevo hijo de este Visitor
    public IVisitor CreateChild(IScope? scope)
    {
        Visitor child = new Visitor(scope);
        child.Parent = this;

        return child;
    }          // Retorna un hijo de este objeto y le asigna un Scope
    public void AddIncrease()
    {
        foreach (string variable in IncreaseVariables)
        {
            int value = Convert.ToInt32(GetValue(variable));
            Define(variable, value + 1);
        }
    }                           // Agrega incremento a las con "++" dentro del alcance
    public void AddInstance()
    {
        if(Scope is not null)
        {
            Dictionary<string, GeneralStatement?> instance = Scope.Defined;

            foreach (string variable in instance.Keys)
                Define(variable, instance[variable]?.Evaluate(Scope));
        }
    }                           // Agrega las variables del Scope
    public Assig? GetAssig()
    {
        Assig? result = Assig; Assig = null;
        return result;
    }                            // Objeto auxiliar (usado para asignar)
}        // Alcance de las variables (Proceso para Evaluar)   
public class Assig
{
    public object Value;
    public Token Token { get; set; }

    public Assig(object value, Token token)
    {
        this.Value = value;
        this.Token = token;
    }
}          // Objeto auxiliar 