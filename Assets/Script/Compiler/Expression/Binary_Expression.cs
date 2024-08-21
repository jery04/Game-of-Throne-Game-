using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

// Binary's Class
#region
#nullable enable
public class Expressions
{
    // Property
    public Terms? Term { get; set; }
    public Expressions? Expression { get; set; }
    public Token? Opeartor { get; set; }

    // Methods
    public object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if(!(Expression is null) && !(Term is null))
        {
            double num1 = 0, num2 = 0;
            num1 = Convert.ToDouble(Term.Evaluate(scope, visitor));
            num2 = Convert.ToDouble(Expression.Evaluate(scope, visitor));

            return Utils.ArtOperation(num1, num2, Opeartor);
        }
        return Term?.Evaluate(scope, visitor);
    }
    public Utils.ReturnType? GetType(IScope? scope)
    {
        if (Term is not null)
            return Term.GetType(scope);

        return null;
    }
    public bool CheckSemantic(IScope scope)
    {
        if (Term != null && Expression != null)
        {
            if (!Term.CheckSemantic(scope) || !Expression.CheckSemantic(scope))
                return false;

            if (Term.GetType(scope) != Expression.GetType(scope))
                return false;
        }
        else if (Term != null)
        {
            if (!Term.CheckSemantic(scope))
                return false;
        }
        return true;
    }
    public Token? Location()
    {
        return Term?.Location();
    }
}
public class Terms
{
    // Property
    public Factor? Factor { get; set; }
    public Terms? Term { get; set; }
    public Token? Opeartor { get; set; }

    // Methods
    public object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (!(Term is null) && !(Factor is null))
        {
            double num1 = 0, num2 = 0;
            num1 = Convert.ToDouble(Factor.Evaluate(scope, visitor));
            num2 = Convert.ToDouble(Term.Evaluate(scope, visitor));

            return Utils.ArtOperation(num1, num2, Opeartor);
        }

        return Factor?.Evaluate(scope, visitor);
    }
    public Utils.ReturnType? GetType(IScope? scope)
    {
        if (Factor is not null)
            return Factor.GetType(scope);

        return null;
    }
    public bool CheckSemantic(IScope scope)
    {
        if (Factor is not null && Term is not null)
        {
            if (!Factor.CheckSemantic(scope) || !Term.CheckSemantic(scope))
                return false;

            if (Factor.GetType(scope) != Term.GetType(scope))
                return false;
        }
        else if (Factor is not null)
        {
            if (!Factor.CheckSemantic(scope))
                return false;
        }
        return true;
    }
    public Token? Location()
    {
        return Factor?.Location();
    }
}
public class Factor
{
    // Property
    public Token? Leaf { get; set; }
    public Expressions? Expression { get; set; }
    public bool Increase {  get; set; }

    // Methods
    public object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (Expression is null)
        {
            if (Leaf?.Type == Token.TokenType.Digit)
                return Convert.ToDouble(Leaf.Value);

            else if (Leaf?.Type == Token.TokenType.UnKnown)
            {
                if(visitor is not null)
                {
                    if (Increase && !visitor.IncreaseVariables.Contains(Leaf.Value))
                        visitor.IncreaseVariables.Add(Leaf.Value);

                    return visitor.GetValue(Leaf?.Value);
                }
                else
                    return Convert.ToDouble(scope?.Defined[Leaf.Value]?.Evaluate(scope));
            }
        }
       
        return Expression?.Evaluate(scope, visitor);
    }
    public Utils.ReturnType? GetType(IScope? scope)
    {
        if (Expression is not null)
        {
            return Expression.GetType(scope);
        }
        else if (Leaf?.Type == Token.TokenType.UnKnown)
        {
            if (!(scope is null) && scope.IsDefined(Leaf.Value))
                return scope.GetType(Leaf.Value, scope);

            else
                return null;
        }
        return Utils.ReturnType.Number;
    }
    public bool CheckSemantic(IScope scope)
    {
        if (Expression is not null)
        {
            if (!Expression.CheckSemantic(scope))
                return false;
        }
        else
        {
            if (Leaf?.Type == Token.TokenType.UnKnown)
            {
                if (scope.IsDefined(Leaf.Value))
                {
                    if (Increase)
                    {
                        if(scope.GetType(Leaf.Value, scope) != Utils.ReturnType.Number)
                        {
                            Utils.errors.Add(@$"No se puede asignar el operador ""++"" al tipo ""{scope.GetType(Leaf.Value, scope)}"" Line: {Leaf.Line} Column: {Leaf.Column} ");
                            return false;
                        }
                    }
                }
                else
                {
                    Utils.errors.Add(@$"La variable ""{Leaf.Value}"" no existe en el contexto actual Line: {Leaf.Line} Column: {Leaf.Column}");
                    return false;
                }
            }
            else if (Leaf?.Type == Token.TokenType.Digit)
                return true;
        }
        return true;
    }
    public Token? Location()
    {
        if (Leaf is not null)
            return Leaf;
        else
            return Expression?.Location();
    }
}
#endregion