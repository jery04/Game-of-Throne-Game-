using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public static class Utils
{
    // Árbol de sintaxis abstracta
    public static ProgramCompiler program = new ProgramCompiler();

    // Listados de palabras claves
    public enum ReturnType { Bool, String, Number, Context, List, Card, Owner, Void }
    public static List<Token.TokenType> fieldCard = new List<Token.TokenType>()
    {
        Token.TokenType.Name, Token.TokenType.Type, Token.TokenType.Faction,
        Token.TokenType.Power, Token.TokenType.Range
    };
    public static List<string?> card = new List<string?>
    {
        "Name", "Type", "Faction", "Power", "Range", "Owner"
    };
    public static List<string?> context = new List<string?>
    {
        "DeckOfPlayer", "Deck", "FieldOfPlayer", "Field", "Board",
        "GraveyardOfPlayer", "Graveyard", "HandOfPlayer", "Hand", "TriggerPlayer"
    };
    public static List<string?> listMethods = new List<string?>
    {
        "Push", "Find", "SendBottom", "Pop", "Remove", "Shuffle", "Add"
    };
    public static List<Token.TokenType> symbols = new List<Token.TokenType>
    {
        Token.TokenType.Equal, Token.TokenType.LessThan,
        Token.TokenType.LessThanEqual, Token.TokenType.GreaterThan,
        Token.TokenType.GreaterThanEqual, Token.TokenType.Increase,
        Token.TokenType.Assignment, Token.TokenType.Decrease
    };

    // Errores encontrados
    public static List<string> errors = new List<string>();    
    public static bool NotError
    {
        get
        {
            return (errors.Count == 0);
        }
        set { }
    }

    // Evaluador de operaciones
    public static double Operation(double a, double b, Token? operation)
    {
        switch (operation?.Type)
        {
            case Token.TokenType.Plus:
                return a + b;

            case Token.TokenType.Minus:
                return a - b;

            case Token.TokenType.Divide:
                if (b != 0)
                    return a / b;
                else
                    Utils.errors.Add($"No se puede dividir por cero Line: {operation.Line} Column: {operation.Column}");
                break;

            case Token.TokenType.Times:
                return a * b;

            case Token.TokenType.Pow:
                return Math.Pow(a, b);
        }
        return -1;
    }
    public static bool? LogOperator(object? object1, object? object2, Token operation, Utils.ReturnType? type)
    {
        if (!(object1 is null || object2 is null || operation is null))
        {
            switch (operation.Type)
            {
                case Token.TokenType.Equal:
                    if (type == Utils.ReturnType.Number)
                    {
                        double num1 = Convert.ToDouble(object1);
                        double num2 = Convert.ToDouble(object2);
                        return num1 == num2;
                    }
                    else if (type == Utils.ReturnType.String)
                    {
                        string? cadena1 = Convert.ToString(object1);
                        string? cadena2 = Convert.ToString(object2);
                        return cadena1 == cadena2;
                    }
                    else
                    {
                        bool boolean1 = Convert.ToBoolean(object1);
                        bool boolean2 = Convert.ToBoolean(object1);
                        return boolean1 == boolean2;
                    }

                case Token.TokenType.LessThan:
                    if (type == Utils.ReturnType.Number)
                    {
                        double num1 = Convert.ToDouble(object1);
                        double num2 = Convert.ToDouble(object2);
                        return num1 < num2;
                    }
                    return false;

                case Token.TokenType.GreaterThan:
                    if (type == Utils.ReturnType.Number)
                    {
                        double num1 = Convert.ToDouble(object1);
                        double num2 = Convert.ToDouble(object2);
                        return num1 > num2;
                    }
                    return false;

                case Token.TokenType.LessThanEqual:
                    if (type == Utils.ReturnType.Number)
                    {
                        double num1 = Convert.ToDouble(object1);
                        double num2 = Convert.ToDouble(object2);
                        return num1 <= num2;
                    }
                    return false;

                case Token.TokenType.GreaterThanEqual:
                    if (type == Utils.ReturnType.Number)
                    {
                        double num1 = Convert.ToDouble(object1);
                        double num2 = Convert.ToDouble(object2);
                        return num1 >= num2;
                    }
                    return false;
            }
        }
        return null;
    }

    // Métodos para recorrer efectos
    public static Dictionary<string, Dictionary<string, Utils.ReturnType?>> effects = new Dictionary<string, Dictionary<string, ReturnType?>>();
    public static bool ContainsEffect(string? effect)
    {
        if (effect is not null)
            if (!effects.ContainsKey(effect))
                return false;

        return true;
    }
    public static bool ContainsParameter(string? effect, string? parameter)
    {
        if (!(effect is null) && !(parameter is null) && !effects[effect].ContainsKey(parameter))
            return false;

        return true;
    }
    public static Utils.ReturnType? ReturnTypeParams(string? effect, string? parameter)
    {
        if (!(effect is null) && !(parameter is null))
            return effects[effect][parameter];

        return null;
    }
    public static bool CheckAmountParams(string? effect, int? parameters)
    {
        if (effect is not null)
        {
            if (parameters != null && effects[effect].Count != parameters)
                return false;

            else if (parameters == null && effects[effect].Count != 0)
                return false;
        }

        return true;
    }
    public static void AddEffect(string? effect)
    {
        if (effect is not null)
            effects.Add(effect, new Dictionary<string, ReturnType?>());
    }
    public static void Reset()
    {
        errors.Clear();
        effects.Clear();
    }
}
