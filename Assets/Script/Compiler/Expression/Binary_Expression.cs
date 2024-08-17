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
    public double Evaluate(IScope? scope)
    {
        double num1 = 0, num2 = 0;

        if (Term is not null)
            num1 = Term.Evaluate(scope);

        if (Expression is not null)
            num2 = Expression.Evaluate(scope);

        else return num1;

        return Utils.Operation(num1, num2, Opeartor);
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
    public double Evaluate(IScope? scope)
    {
        double num1 = 0, num2 = 0;

        if (Factor is not null)
            num1 = Factor.Evaluate(scope);

        if (Term is not null)
            num2 = Term.Evaluate(scope);

        else return num1;

        return Utils.Operation(num1, num2, Opeartor);
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

    // Methods
    public double Evaluate(IScope? scope)
    {
        if (Expression is null)
        {
            if (Leaf?.Type == Token.TokenType.Digit)
                return Convert.ToInt32(Leaf.Value);

            else if (Leaf?.Type == Token.TokenType.UnKnown)
                return Convert.ToDouble(scope?.Defined[Leaf.Value]?.Evaluate(scope));
        }
        else
            return Expression.Evaluate(scope);

        return -1;
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
                if (!scope.IsDefined(Leaf.Value))
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