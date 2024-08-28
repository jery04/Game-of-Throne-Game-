using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Card;
using static Unity.VisualScripting.Antlr3.Runtime.Tree.TreeWizard;
using static Unity.VisualScripting.Member;

// Statement's Class
#region
#nullable enable
public abstract class GeneralStatement
{
    public abstract Utils.ReturnType? GetType(IScope scope);
    public abstract bool CheckSemantic(IScope scope);
    public abstract object? Evaluate(IScope? scope, IVisitor? visitor = null);
}
public class Target: GeneralStatement
{
    // Property
    public List<GameObject>? target {  get; set; }

    // Builder
    public Target(List<GameObject>? target = null)
    {
        this.target = target;
    }

    // Methods
    public override object? Evaluate(IScope? scope = null, IVisitor? visitor = null)
    {
        return target;
    }
    public override Utils.ReturnType? GetType(IScope scope)
    {
        return Utils.ReturnType.List;
    }
    public override bool CheckSemantic(IScope scope) { return true; }
}
public class Context: GeneralStatement
{
    // Methods-List
    public static List<GameObject> Board()
    {
        List<GameObject> board = new List<GameObject>();
        GameObject[] player1_field = GameManager.instance.player1.field;
        GameObject[] player2_field = GameManager.instance.player2.field;

        for (int i = 0; i < 3; i++)
        {
            board.AddRange(player1_field[i].GetComponent<Panels>().cards);
            board.AddRange(player2_field[i].GetComponent<Panels>().cards);
        }
        return board;
    }
    public static List<GameObject> HandOfPlayer(string? playerName)
    {
        GameManager game = GameManager.instance;
        return game.GetPlayer(playerName).hand.GetComponent<Panels>().cards;
    }
    public static List<GameObject> FieldOfPlayer(string? playerName)
    {
        List<GameObject> field = new List<GameObject>();    
        GameObject[] player_field = GameManager.instance.GetPlayer(playerName).field;

        for (int i = 0; i < 3; i++)
            field.AddRange(player_field[i].GetComponent<Panels>().cards);

        return field;
    }
    public static List<GameObject> DeckOfPlayer(string? playerName)
    {
        return GameManager.instance.GetPlayer(playerName).deck;
    }

    // Methods-Ctrls
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        return null;
    }
    public override Utils.ReturnType? GetType(IScope scope)
    {
        return Utils.ReturnType.Context;
    }
    public override bool CheckSemantic(IScope scope) { return true; }
}
public class Parameters: GeneralStatement
{
    //Property
    public Utils.ReturnType? Type { get; set; }

    // Builder
    public Parameters(Utils.ReturnType? type)
    {
        this.Type = type;
    }

    // Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        return null;
    }
    public override Utils.ReturnType? GetType(IScope scope)
    {
        return Type;
    }
    public override bool CheckSemantic(IScope scope) { return true; }
}
public class CardKey: GeneralStatement
{
    // Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        return null;
    }
    public override Utils.ReturnType? GetType(IScope scope)
    {
        return Utils.ReturnType.Card;
    }
    public override bool CheckSemantic(IScope scope) { return true; }
}
public class Statement: GeneralStatement
{
    // Property 
    public Token? LogOperator { get; set; }
    public SubStatement? NodeLeft { get; set; }
    public Statement? NodeRight { get; set; }

    // Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (!(NodeLeft is null) && !(NodeRight is null) && !(LogOperator is null))
        {
            bool boolean1 = Convert.ToBoolean(NodeLeft.Evaluate(scope, visitor)); 
            bool boolean2 = Convert.ToBoolean(NodeRight.Evaluate(scope, visitor));

            if (LogOperator.Type == Token.TokenType.AND)
                return boolean1 && boolean2;
            else
                return boolean1 || boolean2;
        }
        else
            return NodeLeft?.Evaluate(scope, visitor);
    }
    public override Utils.ReturnType? GetType(IScope scope)
    {
        if (LogOperator != null)
            return Utils.ReturnType.Bool;

        else if (NodeLeft != null)
            return NodeLeft.GetType(scope);

        return null;
    }
    public override bool CheckSemantic(IScope scope)
    {
        if (NodeLeft != null && NodeRight != null)
        {
            if (!NodeLeft.CheckSemantic(scope) || !NodeRight.CheckSemantic(scope))
                return false;
        }
        else if (NodeLeft != null)
        {
            if (!NodeLeft.CheckSemantic(scope))
                return false;
        }
        return true;
    }
    public Token? Location()
    {
        return NodeLeft?.Location();
    }
}
public class SubStatement
{
    // Property
    public Token? LogOperator { get; set; }
    public Molecule? NodeLeft { get; set; }
    public Statement? NodeRight { get; set; }

    // Methods
    public object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (!(NodeLeft is null) && !(NodeRight is null) && !(LogOperator is null))
        {
            bool boolean1 = Convert.ToBoolean(NodeLeft.Evaluate(scope, visitor));
            bool boolean2 = Convert.ToBoolean(NodeRight.Evaluate(scope, visitor));

            if (LogOperator.Type == Token.TokenType.AND)
                return boolean1 && boolean2;
            else
                return boolean1 || boolean2;
        }
        else
            return NodeLeft?.Evaluate(scope, visitor);
    }
    public Utils.ReturnType? GetType(IScope scope)
    {
        if (LogOperator != null)
        {
            return Utils.ReturnType.Bool;
        }
        else if (NodeLeft != null)
        {
            return NodeLeft.GetType(scope);
        }
        return null;
    }
    public bool CheckSemantic(IScope scope)
    {
        if (NodeLeft != null && NodeRight != null)
        {
            if (!NodeLeft.CheckSemantic(scope) || !NodeRight.CheckSemantic(scope))
                return false;
        }
        else if (NodeLeft != null)
        {
            if (!NodeLeft.CheckSemantic(scope))
                return false;
        }
        return true;
    }
    public Token? Location()
    {
        return NodeLeft?.Location();
    }
}
public class Molecule: Instructions
{
    // Property
    public Token? ArtOpeartor { get; set; }
    public Atom? NodeLeft { get; set; }
    public Atom? NodeRight { get; set; }

    // Methods
    public override void Evaluate(IVisitor visitor)
    {
        Debug.Log("Molecule");

        Evaluate(visitor.Scope, visitor);
    }
    public object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (!(NodeLeft is null) && !(NodeRight is null) && !(ArtOpeartor is null))
        {
            if (ArtOpeartor.Type != Token.TokenType.Assignment && ArtOpeartor.Type != Token.TokenType.Increase && ArtOpeartor.Type != Token.TokenType.Decrease)
                return Utils.LogOperator(NodeLeft.Evaluate(scope, visitor), NodeRight.Evaluate(scope, visitor), ArtOpeartor, NodeLeft.GetType(scope));

            else
            {
                Debug.Log("Heyyyyyyyyyyyyyyyyyyy");
                Debug.Log(NodeLeft.Evaluate(scope, visitor) 
                    +" "+NodeRight.Evaluate(scope, visitor));
                if(visitor is not null)
                {
                    visitor.Assig = new Assig(NodeRight.Evaluate(scope, visitor), ArtOpeartor);
                    NodeLeft.Evaluate(scope, visitor);
                }

                return null;
            }
        }
        else
            return NodeLeft?.Evaluate(scope, visitor);
    }
    public Utils.ReturnType? GetType(IScope scope)
    {
        if (ArtOpeartor != null)
        {
            if (ArtOpeartor.Type != Token.TokenType.Increase && ArtOpeartor.Type != Token.TokenType.Decrease && ArtOpeartor.Type != Token.TokenType.Assignment)
                return Utils.ReturnType.Bool;
            else
                return Utils.ReturnType.Void;
        }
        else if (NodeLeft != null)
            return NodeLeft.GetType(scope);

        return null;
    }
    public override bool CheckSemantic(IScope scope)
    {
        bool check = true;
        if (NodeLeft != null && NodeRight != null)
        {
            if (!NodeLeft.CheckSemantic(scope) || !NodeRight.CheckSemantic(scope))
                check = false;

            if (NodeLeft?.GetType(scope) == NodeRight?.GetType(scope))
            {
                Utils.ReturnType? type = NodeLeft?.GetType(scope);

                if (ArtOpeartor?.Type != Token.TokenType.Equal && ArtOpeartor?.Type != Token.TokenType.Assignment
                    && (type == Utils.ReturnType.String || type == Utils.ReturnType.Bool))
                {
                    Utils.errors.Add(@$"No se puede establecer una relación entre los tipos ""{type}"" mediante el operador ""{ArtOpeartor?.Value}"" Line: {ArtOpeartor?.Line} Column: {ArtOpeartor?.Column}");
                    check = false;
                }
            }
            else
            {
                Utils.errors.Add(@$"No se puede establecer una relación entre los tipos ""{NodeLeft?.GetType(scope)}"" y ""{NodeRight?.GetType(scope)}"" Line: {ArtOpeartor?.Line} Column: {ArtOpeartor?.Column}");
                check = false;
            }
        }
        else if (NodeLeft != null)
            if (!NodeLeft.CheckSemantic(scope))
                check = false;

        return check;
    }
    public Token? Location()
    {
        return NodeLeft?.Location();
    }
}
public abstract class Atom
{
    // Abstract class
    public abstract Utils.ReturnType? GetType(IScope? scope);
    public abstract bool CheckSemantic(IScope scope);
    public abstract object? Evaluate(IScope? scope, IVisitor? visitor = null);
    public abstract Token? Location();
}
public class Atom0: Atom
{
    // Property
    public Token? Boolean { get; set; }

    //Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        if (Boolean is not null)
        {
            if (Boolean.Type == Token.TokenType.True)
                return true;
            else if (Boolean.Type == Token.TokenType.False)
                return false;
        }
        return null;
    }
    public override Utils.ReturnType? GetType(IScope? scope)
    {
        return Utils.ReturnType.Bool;
    }
    public override bool CheckSemantic(IScope scope)
    {
        return true;
    }
    public override Token? Location()
    {
        return Boolean;
    }
}
public class Atom1: Atom
{
    // Property
    public Expressions? Expression { get; set; }

    //Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        return Expression?.Evaluate(scope , visitor);
    }
    public override Utils.ReturnType? GetType(IScope? scope)
    {
        return Expression?.GetType(scope);
    }
    public override bool CheckSemantic(IScope scope)
    {
        if (!(Expression is null) && !Expression.CheckSemantic(scope))
            return false;

        return true;
    }
    public override Token? Location()
    {
        return Expression?.Location();
    }
}
public class Atom2: Atom
{
    //Property
    public List<Token?>? Call { get; set; }
    public Atom2? Nested { get; set; }
    public Predicate? Predicate { get; set; }

    // Builder
    public Atom2()
    {
        this.Call = new List<Token?>();
        this.Predicate = null;
    }

    //Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        Debug.Log("Atom2");
        if (Call is not null)
        {
            if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Card)
            {
                GameObject? card = (GameObject?)visitor?.GetValue(Call[0]?.Value);
                if (Call[1] is not null)
                    return CardProperty(Call[1]?.Value, card, visitor?.GetAssig());

                else return card;
            }
            else if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Context)
            {
                Debug.Log("Context");
                if (Call[1] is not null)
                {
                    if (Call[2] is not null)
                    {
                        List<GameObject>? list = (List<GameObject>?)GetList(Call[1]?.Value);
                        return Methods(Call[2]?.Value, list, visitor);
                    }
                    else if (Call[1]?.Value == "TriggerPlayer")
                        return GameManager.currentPlayer.playerName;
                    else
                        return GetList(Call[1]?.Value, Convert.ToString(Nested?.Evaluate(scope, visitor)));
                }
                else new Context();
            }
            else if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.List)
            {
                Debug.Log("List");
                List<GameObject>? list = (List<GameObject>?)visitor?.GetValue(Call[0]?.Value);
                if (Call[1] is not null)
                    return Methods(Call[1]?.Value, list, visitor); 

                else return list;
            }
            else
                return visitor?.GetValue(Call[0]?.Value);
        }
        return null;
    }
    public override Utils.ReturnType? GetType(IScope? scope)
    {
        NullFill();
        if (Call is not null)
        {
            if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Card)
            {
                if (Call[1] is not null)
                {
                    if (Call[1]?.Value == "Power")
                        return Utils.ReturnType.Number;
                    else
                        return Utils.ReturnType.String;
                }
                else return Utils.ReturnType.Card;
            }
            else if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Context)
            {
                if (Call[1] is not null)
                {
                    if (Call[2] is not null)
                    {
                        if (Call[2]?.Value == "Find")
                            return Utils.ReturnType.List;

                        else if (Call[2]?.Value == "Pop")
                            return Utils.ReturnType.Card;

                        else
                            return Utils.ReturnType.Void;
                    }
                    else
                    {
                        if (Call[1]?.Value == "TriggerPlayer")
                            return Utils.ReturnType.String;
                        else
                            return Utils.ReturnType.List;
                    }
                }
                else return Utils.ReturnType.Context;
            }
            else if (scope?.GetType(Call[0]?.Value, scope) == Utils.ReturnType.List)
            {
                if (Call[1] is not null)
                {
                    if (Call[1]?.Value == "Find")
                        return Utils.ReturnType.List;

                    else if (Call[1]?.Value == "Pop")
                        return Utils.ReturnType.Card;

                    else
                        return Utils.ReturnType.Void;
                }
                else return Utils.ReturnType.List;
            }
            else
                return scope?.GetType(Call[0]?.Value, scope);
        }
        return Utils.ReturnType.Void;
    }
    public override bool CheckSemantic(IScope scope)
    {
        NullFill();
        if (Call is not null)
        {
            if (scope.IsDefined(Call[0]?.Value))
            {
                if (scope.GetType(Call[0]?.Value, scope) == Utils.ReturnType.List)
                {

                    if (!(Call[1] is null) && !Utils.listMethods.Contains(Call[1]?.Value))
                    {
                        Utils.errors.Add(@$"""{Call[0]?.Value}"" no tiene una definición para ""{Call[1]?.Value}"" Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                        return false;
                    }
                    else if (Call[1] is not null)
                    {
                        if(Call[1]?.Value == "Find")
                        {
                            if(Predicate is not null)
                            {
                                if (!Predicate.CheckSemantic(scope))
                                    return false;
                            }
                            else
                            {
                                Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe un predicado como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                return false;
                            }
                        }
                        else if (Call[1]?.Value == "Remove" || Call[1]?.Value == "Push" || Call[1]?.Value == "SendBottom" || Call[1]?.Value == "Add")
                        {
                            if (Nested is not null)
                            {
                                if (Nested.CheckSemantic(scope))
                                {
                                    if (Nested.GetType(scope) != Utils.ReturnType.Card)
                                    {
                                        Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe una carta como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                        return false;
                                    }
                                }
                                else return false;
                            }
                            else
                            {
                                Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe una carta como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                return false;
                            }
                        }
                        else if (Nested is not null)
                        {
                            Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" no recibe parámetros en su definición Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                            return false;
                        }
                    }
                }
                else if (scope.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Context)
                {
                    if (Call[1] is not null)
                    {
                        if (!Utils.context.Contains(Call[1]?.Value))
                        {
                            Utils.errors.Add(@$"""{Call[0]?.Value}"" no tiene una definición para "" {Call[1]?.Value}"" Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                            return false;
                        }
                        else
                        {
                            if (Call[1]?.Value == "Hand" || Call[1]?.Value == "Deck" || Call[1]?.Value == "Field" || Call[1]?.Value == "Graveyard" || Call[1]?.Value == "Board")
                            {
                                if (Call[2] is not null)
                                {
                                    if (!Utils.listMethods.Contains(Call[2]?.Value))
                                    {
                                        Utils.errors.Add(@$"""{Call[1]?.Value}"" no tiene una definición para ""{Call[2]?.Value}"" Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                                        return false;
                                    }
                                    else
                                    {
                                        if (Call[2]?.Value == "Find")
                                        {
                                            if (Predicate is not null)
                                            {
                                                if (!Predicate.CheckSemantic(scope))
                                                    return false;
                                            }
                                            else
                                            { 
                                                Utils.errors.Add(@$"El método ""{Call[2]?.Value}"" recibe un predicado como parámetro Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                                                return false;
                                            }
                                        }
                                        if (Call[2]?.Value == "Remove" || Call[2]?.Value == "Push" || Call[2]?.Value == "SendBottom" || Call[2]?.Value == "Add")
                                        {
                                            if (Nested is not null)
                                            {
                                                if (Nested.CheckSemantic(scope))
                                                {
                                                    if (Nested.GetType(scope) != Utils.ReturnType.Card)
                                                    {
                                                        Utils.errors.Add(@$"El método ""{Call[2]?.Value}"" recibe una carta como parámetro Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                                                        return false;
                                                    }
                                                }
                                                else return false;
                                            }
                                            else
                                            {
                                                Utils.errors.Add(@$"El método ""{Call[2]?.Value}"" recibe una carta como parámetro Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                                                return false;
                                            }
                                        }
                                        else if (Nested is not null)
                                        {
                                            Utils.errors.Add(@$"El método ""{Call[2]?.Value}"" no recibe parámetros en su definición Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe una carta como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                    return false;
                                }
                            }
                            else if (Call[1]?.Value != "TriggerPlayer")
                            {
                                if (Nested is not null)
                                {
                                    if (Nested.CheckSemantic(scope))
                                    {
                                        if (Nested.GetType(scope) != Utils.ReturnType.String)
                                        {
                                            Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe un ""String"" como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                            return false;
                                        }
                                    }
                                    else return false;
                                }
                                else
                                {
                                    Utils.errors.Add(@$"El método ""{Call[1]?.Value}"" recibe un ""String"" como parámetro Line: {Call[1]?.Line} Column: {Call[1]?.Column} ");
                                    return false;
                                }
                            }
                        }
                    }
                }
                else if (scope.GetType(Call[0]?.Value, scope) == Utils.ReturnType.Card)
                {
                    if (Call[1] is not null)
                    {
                        if (!Utils.card.Contains(Call[1]?.Value))
                        {
                            Utils.errors.Add(@$"""{Call[1]?.Value}"" no tiene una definición para ""{Call[2]?.Value}"" Line: {Call[2]?.Line} Column: {Call[2]?.Column} ");
                            return false;
                        }
                    }
                }
            }
            else
            {
                Utils.errors.Add(@$"La variable ""{Call[0]?.Value}"" no existe en el contexto actual Line: {Call[0]?.Line} Column: {Call[0]?.Column} ");
                return false;
            }
        }

        return true;
    }
    private object? Methods(string? method, List<GameObject>? list, IVisitor? visitor)
    {
        switch (method)
        {
            case "Pop":
                if(list?.Count > 0)
                {
                    GameObject? clone = GameObject.Instantiate(list?[list.Count - 1]);
                    list?.RemoveAt(list.Count - 1);
                    return clone;
                }
            return null;

            case "SendBottom":
                GameObject? card1 = (GameObject?)Nested?.Evaluate(null, visitor);

                if(card1 is not null)
                    list?.Insert(0, card1);
            break;

            case "Add":
            case "Push":
                GameObject? card2 = (GameObject?)Nested?.Evaluate(visitor?.Scope, visitor);
               
                if (card2 is not null)
                    list?.Add(card2);
            break;

            case "Remove":
                GameObject? card3 = (GameObject?)Nested?.Evaluate(null, visitor);
                string? card_remove = card3?.GetComponent<CardDisplay>().name;

                if(card3 is not null && list is not null)
                    for(int i = 0; i < list.Count; i++)
                        if (list[i].GetComponent<CardDisplay>().name == card_remove)
                        { list[i] = null; GameObject.Destroy(card3); }
                break;

            case "Shuffle":
                if (list is not null)
                {
                    int dimen = list.Count;
                    for (int i = 0; i < dimen / 2; i++)
                    {
                        // Index
                        int random = UnityEngine.Random.Range(i+1, dimen);
                        GameObject swap = list[i]; 
                        list[i] = list[random]; list[random] = swap;
                    }
                }
            break;

            case "Find":
                Debug.Log("Findddd");

                List<GameObject> predicate = new List<GameObject>();

                if(!(list is null) && !(Predicate is null))
                    foreach (GameObject card in list)
                        if ((bool)Predicate.Evaluate(card))
                            predicate.Add(card);
            return predicate;
        }
        return null;
    }
    private List<GameObject>? GetList(string? list, string? player = null)
    {
        string current = GameManager.currentPlayer.playerName;
        switch (list)
        {
            case "Board":
                return Context.Board();

            case "Hand":
                return Context.HandOfPlayer(current);

            case "HandOfPlayer":
                return Context.HandOfPlayer(player);

            case "Field":
                return Context.HandOfPlayer(current);

            case "FieldOfPlayer":
                return Context.HandOfPlayer(player);

            case "Deck":
                return Context.DeckOfPlayer(current);

            case "DeckOfPlayer":
                return Context.DeckOfPlayer(player);

            case "Graveyard":
                return Context.DeckOfPlayer(current);

            case "GraveyardOfPlayer":
                return Context.DeckOfPlayer(player);
        }
        return null;
    }
    private object CardProperty(string? property, GameObject? card, Assig? assig)
    {
        if(card is not null)
        {
            switch (property)
            {
                case "Name":
                    if (assig is null)
                        return card.GetComponent<CardDisplay>().name;
                    else
                        card.GetComponent<CardDisplay>().name = Convert.ToString(assig.Value);
                    break;

                case "Faction":
                    if (assig is null)
                        return card.GetComponent<CardDisplay>().faction;
                    else
                        card.GetComponent<CardDisplay>().faction = Convert.ToString(assig.Value);
                break;

                case "Power":
                    if (assig is null)
                        return card.GetComponent<CardDisplay>().Power();
                    else
                    {
                        if (assig.Token.Type == Token.TokenType.Assignment)
                            card.GetComponent<CardDisplay>().NewPower(Convert.ToInt32(assig.Value));

                        else if(assig.Token.Type == Token.TokenType.Increase)
                            card.GetComponent<CardDisplay>().PowerDelta(Convert.ToInt32(assig.Value));
                        else
                            card.GetComponent<CardDisplay>().PowerDelta(-1*Convert.ToInt32(assig.Value));
                    }
                break;

                case "Type":
                    if (assig is null)
                        return Transform_Enum_String(card.GetComponent<CardDisplay>().type_Card);
                    else
                        card.GetComponent<CardDisplay>().type_Card = Transform_String_Enum((string?)assig.Value);
                break;

                case "Range":
                    throw new NotImplementedException();

                case "Owner":
                    if (assig is null)
                        return card.GetComponent<CardDisplay>().owner;
                    else
                        card.GetComponent<CardDisplay>().owner = Convert.ToString(assig.Value);
                    break;
            }
        }
        return "";
    }
    private Card.kind_card Transform_String_Enum(string? type)
    {
        switch (type)
        {
            case "oro":
            case "golden":
                return kind_card.golden;

            case "plata":
            case "silver":
                return kind_card.silver;

            case "clima":
            case "climate":
                return kind_card.climate;

            case "despeje":
            case "clear":
                return kind_card.clear;

            case "señuelo":
            case "bait":
                return kind_card.bait;

            case "aumento":
            case "increase":
                return kind_card.increase;
        }
        return kind_card.leader;
    }
    private string Transform_Enum_String(Card.kind_card type)
    {
        switch (type)
        {
            case Card.kind_card.golden:
                return "oro";

            case Card.kind_card.silver:
                return "plata";

            case  Card.kind_card.climate:
                return "clima";

            case Card.kind_card.increase:
                return "aumento";

            case Card.kind_card.clear:
                return "despeje";

            case Card.kind_card.bait:
                return "señuelo";
        }
        return "líder";
    } 
    public override Token? Location()
    {
        return Call?[0];
    }
    private void NullFill()
    {
        if (Call is not null && Call.Count < 7)
            for (int i = 0; i < 6; i++)
                Call.Add(null);
    }

}
public class Atom3: Atom
{
    //Property
    public List<Token?>? String { get; set; }

    //Builder
    public Atom3()
    {
        this.String = new List<Token?>();
    }

    // Methods
    public override object? Evaluate(IScope? scope, IVisitor? visitor = null)
    {
        string result = "";
        if (String is not null)
        {
            for(int i = 0; i < String.Count; i++)
            {
                if (String[i]?.Type == Token.TokenType.ATAT)
                    result += " " + String[++i]?.Value;

                else if (String[i]?.Type == Token.TokenType.AT)
                    result += String[++i]?.Value;

                else if (i == 0)
                    result += String[i]?.Value;

                else
                    result += " " + String[i]?.Value;
            }
            return result;
        }
        return null;
    }
    public override Utils.ReturnType? GetType(IScope? scope)
    {
        return Utils.ReturnType.String;
    }
    public override bool CheckSemantic(IScope scope)
    {
        return true;
    }
    public override Token? Location()
    {
        return String?[0];
    }
}
#endregion
