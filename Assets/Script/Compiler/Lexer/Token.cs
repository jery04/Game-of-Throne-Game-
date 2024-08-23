using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token
{
    // Field
    public enum TokenType
    {
        // Bucles
        While,            // while
        For,              // for

        // Símbolos
        OpenParan,        // ( 
        ClosedParan,      // )
        OpenKey,          // {
        ClosedKey,        // } 
        OpenBracket,      // [
        ClosedBracket,    // ]
        Quote,            // "
        GreaterThan,      // >
        LessThan,         // <
        LessThanEqual,    // <=
        GreaterThanEqual, // =>
        Increase,         // +=
        Decrease,         // -=
        Assignment,       // =
        Equal,            // == 
        Colon,            // :
        Dot,              // .
        Comma,            // ,
        SemiColon,        // ;
        Arrow,            // =>


        // Operadores Aritméticos
        Plus,             // +
        Minus,            // -
        Times,            // *
        Divide,           // /
        Pow,              // ^
        PlusPlus,         // ++

        // Operadores Lógicos
        AND,              // &&
        OR,               // ||

        // Concatenación
        ATAT,             // @@
        AT,               // @

        // Tipos
        Targets,
        Context,
        Digit,           // 1354 
        UnKnown,          // qwevju

        //Boolean
        True,
        False,

        // KeyWords
        Call,
        Number,
        String,
        Bool,
        Add,
        Action,
        Board,
        Card,
        Deck,
        TriggerPlayer,
        DeckOfPlayer,
        Effect,
        EffectActivation,
        Field,
        FieldOfPlayer,
        Find,
        Faction,
        Graveyard,
        GraveyardOfPlayer,
        Hand,
        HandOfPlayer,
        In,
        Name,
        NumberKey,
        OnActivation,
        Owner,
        Params,
        Pop,
        PostAction,
        Power,
        Predicate,
        Push,
        Range,
        Remove,
        Selector,
        SendBottom,
        Shuffle,
        Single,
        Source,
        Type
    }

    // Properties
    public TokenType Type { get; set; }                         // Tipo en enum 
    public string Value { get; set; }                           // Valor correspondiente
    public int Line { get; set; }                               // Línea en la que se encuentra 
    public int Column { get; set; }                             // Caracter de la línea correspondiente 

    // Builder
    public Token(TokenType Type, string Value, int Line, int Column)
    {
        this.Type = Type;
        this.Value = Value;
        this.Line = Line;
        this.Column = Column;
    }

    // Methods
    public static int SearchLine(string input, int index)
    {
        int counter = 1;
        for (int i = 0; i <= index; i++)
        {
            if (index == i)
                return counter;
            else if (input[i] == '\n')
                counter++;
        }
        return counter;
    }     // Encuentra la línea del Token
    public static int SearchColumn(string input, int index)
    {
        for (int i = index; i >= 1; i--)
            if (input[i] == '\n')
                return (index - i) - 1;

        return index;
    }   // Encuentra la columna del Token
}
