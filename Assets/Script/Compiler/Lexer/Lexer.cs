using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

#nullable enable
public class Lexer
{
    // Field
    private static Dictionary<string, Token.TokenType> keywords = new Dictionary<string, Token.TokenType>()
        {
            // Bucles
            {"while", Token.TokenType.While},
            {"for", Token.TokenType.For},

            // KeyWords
            {"Number", Token.TokenType.Number},
            {"String", Token.TokenType.String},
            {"Bool", Token.TokenType.Bool},
            {"Action", Token.TokenType.Action},
            {"card", Token.TokenType.Card},
            {"effect", Token.TokenType.Effect},
            {"Effect", Token.TokenType.EffectActivation},
            {" in ", Token.TokenType.In},
            {"OnActivation", Token.TokenType.OnActivation},
            {"Params", Token.TokenType.Params},
            {"Predicate", Token.TokenType.Predicate},
            {"Selector", Token.TokenType.Selector},
            {"Single", Token.TokenType.Single},
            {"Source", Token.TokenType.Source},
            {"PostAction", Token.TokenType.PostAction},


            // Símbolos
            {"\\(", Token.TokenType.OpenParan},
            {"\\)", Token.TokenType.ClosedParan},
            {"{", Token.TokenType.OpenKey},
            {"}", Token.TokenType.ClosedKey},
            {"\\[", Token.TokenType.OpenBracket},
            {"\\]", Token.TokenType.ClosedBracket},
            {"\"", Token.TokenType.Quote},
            {"<=", Token.TokenType.LessThanEqual},
            {">=", Token.TokenType.GreaterThanEqual},
            {"<", Token.TokenType.LessThan},
            {">", Token.TokenType.GreaterThan},
            {"\\+=", Token.TokenType.Increase},
            {"-=", Token.TokenType.Decrease},
            {"==", Token.TokenType.Equal},
            {"=>", Token.TokenType.Arrow},
            {"=", Token.TokenType.Assignment},
            {"\\:", Token.TokenType.Colon},
            {"\\.", Token.TokenType.Dot},
            {",", Token.TokenType.Comma},
            {";", Token.TokenType.SemiColon},

            //Operadores Aritméticos
            {"\\+\\+", Token.TokenType.PlusPlus},
            {"\\+", Token.TokenType.Plus},
            {"-",Token.TokenType.Minus },
            {"\\*", Token.TokenType.Times},
            {"/",Token.TokenType.Divide },
            {"\\^",Token.TokenType.Pow },

            // Operadores Lógicos
            {"&&", Token.TokenType.AND},
            {"\\|\\|", Token.TokenType.OR},

            // Booleano
            {"true", Token.TokenType.True},
            {"false", Token.TokenType.False},

            // Concatenación
            {"@@", Token.TokenType.ATAT},
            {"@", Token.TokenType.AT},

            // Keys Arbitrarias
            {"[a-zA-Z_]\\w*", Token.TokenType.UnKnown},
            {"\\d+", Token.TokenType.Digit},

        };  // Lenguaje de palabras reservadas y claves
    private string[] code;                                     // Líneas de códigos
    private List<Token> tokensList;                            // Lista de tokens

    // Poperties
    public Token[] TokensList
    {
        get
        {
            if (tokensList.Count == 0)
                return GetLexer();
            else
                return tokensList.ToArray();
        }
        private set { }
    }                               // Retorna un array de los tokens
    public string[]? Code { get; private set; }                // Retorna un array de las líneas de códigos     

    // Builder 
    public Lexer(string[] code)
    {
        this.code = code;
        tokensList = new List<Token>();
    }                             // Instancia un objeto con el código especificado (parámetro)

    // Methods
    public Token[] GetLexer()
    {
        string input = string.Join('\n', code);
        string pattern = $"{string.Join("|", keywords.Keys)}";
        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            if (keywords.TryGetValue(match.Value, out var tokenType))
                tokensList.Add(new Token(tokenType, match.Value, Token.SearchLine(input, match.Index), Token.SearchColumn(input, match.Index)));

            else if (match.Value.All(char.IsDigit) && Regex.IsMatch(match.Value, "\\d+"))
                tokensList.Add(new Token(Token.TokenType.Digit, match.Value, Token.SearchLine(input, match.Index), Token.SearchColumn(input, match.Index)));

            else
            {
                foreach (string item in keywords.Keys)
                {
                    if (Regex.IsMatch(match.Value, item))
                    {
                        tokensList.Add(new Token(keywords[item], match.Value, Token.SearchLine(input, match.Index), Token.SearchColumn(input, match.Index)));
                        break;
                    }
                }
            }
        }
        return tokensList.ToArray();
    }                               // Transforma los caracteres (palabras) a tokens
}
