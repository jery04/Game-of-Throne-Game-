using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Card;


// Card Block
#region
#nullable enable
public class CardBlock: ISemantic
{
    //Property 
    public Variable? Type { get; set; }     // Campo "Type" (tipo de carta)
    public Variable? Name { get; set; }     // Campo "Name" (nombre de la carta)
    public Variable? Faction { get; set; }  // Campo "Faction" (facción de la carta)
    public Variable? Power { get; set; }    // Campo "Power" (poder de la carta)
    public Variable? Range { get; set; }    // Campo "Range" (posición de la carta)
    public OnActivation? OnActivation { get; set; } // Bloque de activación y selector de los efecto

    // Methods
    public CardCompiler Evaluate(IScope scope)
    {
        CardCompiler card_compiler;

        string name = this.Name_Field(scope);
        string faction = this.Faction_Field(scope);
        int power = (int)this.Power_Field(scope);
        Card.kind_card type = this.Type_Field(scope);
        Card.card_position range = this.Range_Field(scope);

        card_compiler = new CardCompiler(name, faction, power, type, range, this.OnActivation, scope);

        return card_compiler;
    }      // Crea un instancia de la carta (evalúa sus propiedades)
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (!(Type is null) && !Type.CheckSemantic(scope))
            check = false;

        if (!(Name is null) && !Name.CheckSemantic(scope))
            check = false;

        if (!(Faction is null) && !Faction.CheckSemantic(scope))
            check = false;

        if (!(Power is null) && !Power.CheckSemantic(scope))
            check = false;

        if (!(Range is null) && !Range.CheckSemantic(scope))
            check = false;

        if (!(OnActivation is null) && !OnActivation.CheckSemantic(scope))
            check = false;

        return check;
    }         // Analiza la semántica del bloque "Card" 
    private Card.kind_card Type_Field(IScope scope)
    {
        switch (Convert.ToString(Type?.Evaluate(scope)))
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
    } // Retorna el tipo (enum) de carta dado el campo "Type"
    private string Name_Field(IScope scope)
    {
        return Convert.ToString(Name?.Evaluate(scope));
    }         // Retorna el nombre (string) de la carta dado el campo "Name"
    private string Faction_Field(IScope scope)
    {
        return Convert.ToString(Faction?.Evaluate(scope));
    }      // Retorna la facción de la carta dado el campo "Faction"
    private double Power_Field(IScope scope)
    {
        return Convert.ToDouble(Power?.Evaluate(scope));
    }        // Retorna el poder de la carta dado el campo "Power"
    private Card.card_position Range_Field(IScope scope)
    {   
        return (Card.card_position)Convert.ChangeType(Range?.Evaluate(scope), typeof(Card.card_position));
    }   // Retorna la posición (enum) de la carta dado el campo "Range"
}          // Estructura del bloque "Card"
public class OnActivation
{
    // Property
    public List<OnActivationBody?>? Body { get; set; }  // Listado de las estructura de cada activación

    // Builder 
    public OnActivation()
    {
        this.Body = new List<OnActivationBody?>();
    }

    // Methods
    public void Evaluate()
    {
        if (Body is not null)
            foreach (OnActivationBody? body in Body)
                if (body is not null)
                    body.Evaluate();
    }                    // Llama al evaluador de cada bloque de activación
    public bool CheckSemantic(IScope scope)
    {
        if (Body != null && Body.Count > 0)
            foreach (OnActivationBody? item in Body)
                if (!(item is null) && !item.CheckSemantic(scope))
                    return false;

        return true;
    }   // Llama al "CheckSemantic" de cada bloque de activación
}       // Estructura del bloque "OnActivation"
public class OnActivationBody
{
    // Property
    public EffectActivation? EffectActivation { get; set; } // Estructura del bloque que nombre el efecto a llamar
    public Selector? Selector { get; set; }                 // Estructura del bloque "Selector"
    public List<PosAction?>? PosAction { get; set; }        // Listado de estructuras del bloque "PosAction"

    // Builder
    public OnActivationBody()
    {
        this.PosAction = new List<PosAction?>();
    }

    // Methods
    public void Evaluate()
    {
        List<GameObject> target = new List<GameObject>(); 
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        string effectActive = "";

        if (!(Selector is null) && !(EffectActivation is null))
        {
            target = Selector.Evaluate();
            parameters = EffectActivation.Evaluate();
            effectActive = EffectActivation.GetName();

            if (Utils.program.Effect is not null)
                foreach (EffectBlock effects in Utils.program.Effect)
                    if (effects.GetName() == effectActive)
                        { Debug.Log(effectActive);  effects.Evaluate(target, parameters); break; }
        }

        if (PosAction is not null)
            foreach (PosAction? pos_action in PosAction)
                if (pos_action is not null)
                    pos_action.Evaluate(target);
    }                    // Evalua cada bloque de activación
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (!(EffectActivation is null) && !EffectActivation.CheckSemantic(scope))
            check = false;

        if (!(Selector is null) && !Selector.CheckSemantic(scope))
            check = false;

        if (PosAction is not null)
            foreach (PosAction? item in PosAction)
                if (!(item is null) && !item.CheckSemantic(scope))
                    check = false;

        return check;
    }   // Analiza el semántico de cada bloque de activación
}   // Estructura del bloque de cada activación
public class EffectActivation
{
    // Property
    public Variable? Name { get; set; }              // Almacena el nombre del efecto a invocar
    public List<Variable?>? Parameters { get; set; } // Listado de parámetros que pasar al efecto

    // Methods
    public Dictionary<string, object> Evaluate()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        if (Parameters is not null)
        {
            foreach (Variable? variable in Parameters)
            {
                if(variable is not null)
                {
                    string? name = variable.ReturnName()?.ToLower();
                    object? value = variable.Evaluate(new Scope());

                    if (!(name is null) && !(value is null))
                        parameters.Add(name, value);
                }
            }
        }
        return parameters;
    }  // Retorna un listado (nombre-valor) de los parámetros
    public bool CheckSemantic(IScope scope)
    {
        string? nameEffect = Convert.ToString(Name?.Evaluate(scope));
        if (Name is not null)
        {
            if (Name.CheckSemantic(scope))
            {
                if (Utils.ContainsEffect(nameEffect))
                {
                    if (Utils.CheckParamsAmount(nameEffect, Parameters?.Count))
                    {
                        for (int i = 0; i < Parameters?.Count; i++)
                        {
                            string? param = Convert.ToString(Parameters[i]?.ReturnName())?.ToLower();
                            if (Utils.ContainsParameter(nameEffect, param))
                            {
                                if (Utils.ReturnParamsType(nameEffect, param) != Parameters[i]?.GetType(scope))
                                {

                                    Utils.errors.Add(@$"El tipo de retorno de ""{param}"" no coincide con la definición del efecto ""{nameEffect}"" Line: {Parameters[i]?.Location()?.Line} Column: {Parameters[i]?.Location()?.Column} ");
                                    return false;
                                }
                            }
                            else
                            {
                                Utils.errors.Add($@"El parámetro ""{param}"" no está declarado en ""{nameEffect}"" Line: {Parameters[i]?.Location()?.Line} Column: {Parameters[i]?.Location()?.Column} ");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Utils.errors.Add(@$"La cantidad de parámetros no coincide con la definición del efecto ""{nameEffect}"" Line: {Name.Location()?.Line} Column: {Name.Location()?.Column} ");
                        return false;
                    }
                }
                else
                {
                    Utils.errors.Add(@$"El efecto ""{nameEffect}"" no ha sido declarado Line: {Name.Location()?.Line} Column: {Name.Location()?.Column} ");
                    return false;
                }
            }
            else return false;
        }
        return true;
    }       // Analiza la semántica de este bloque
    public string GetName()
    {
        return Convert.ToString(Name?.Evaluate(new Scope()));
    }                       // Retorna el nombre del efecto a invocar
}   // Estructura del bloque que nombre el efecto a llamar
public class Selector
{
    // Property
    public Variable? Source { get; set; }      // Campo "Source" (fuente de cartas)
    public Variable? Single { get; set; }      // Campo "Single" 
    public Predicate? Predicate { get; set; }  // Estructura del predicado

    // Methods
    public List<GameObject> Evaluate(List<GameObject>? parent = null)
    {
        List<GameObject> selector = new List<GameObject>();
        List<GameObject> source = GetSource(parent);
        bool single = Convert.ToBoolean(Single?.Evaluate(new Scope()));

        foreach (GameObject card in source)
        {
            bool predicate_bool = Convert.ToBoolean(Predicate?.Evaluate(card));
            if (single && predicate_bool)
                { selector.Add(card); break; }

            else if (predicate_bool)
                selector.Add(card);
        }
        return selector;
    }   // Retorna un listado de las cartas a aplicar el efecto
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Source is not null && Source.CheckSemantic(scope))
            check = CheckSource();
        else check = false;

        if (Single is not null && !Single.CheckSemantic(scope))
            check = false;

        if (Predicate is not null && !Predicate.CheckSemantic(scope))
            check = false;

        return check;
    }                             // Analiza la semántica del bloque "Selector"
    private List<GameObject> GetSource(List<GameObject>? parent = null)
    {
        List<GameObject> source = new List<GameObject>();
        string current = GameManager.currentPlayer.playerName;
        string notCurrent = GameManager.instance.PlayerNotCurrent().playerName;

        switch (Convert.ToString(Source?.Evaluate(new Scope())))
        {
            case "board":
                source = Context.Board();
                break;
            case "hand":
                source = Context.HandOfPlayer(current); 
                break;
            case "otherHand":
                source = Context.HandOfPlayer(notCurrent);
                break;
            case "deck":
                source = Context.DeckOfPlayer(current);
                break;
            case "otherDeck":
                source = Context.DeckOfPlayer(notCurrent);
                break;
            case "field":
                source = Context.FieldOfPlayer(current);
                break;
            case "otherField":
                source = Context.FieldOfPlayer(notCurrent);
                break;
            case "parent":
                if(parent is not null)
                    source = parent;
                break;
        }
        return source;
    } // Retorna las cartas de la fuente seleccionada
    private bool CheckSource()
    {
        bool check = false;
        string source = Convert.ToString(Source?.Evaluate(new Scope()));
        switch (source)
        {
            case "board":
                check = true;
                break;
            case "hand":
                check = true;
                break;
            case "otherHand":
                check = true;
                break;
            case "deck":
                check = true;
                break;
            case "otherDeck":
                check = true;
                break;
            case "field":
                check = true;
                break;
            case "otherField":
                check = true;
                break;
            case "parent":
                check = true;
                break;
            default:
                Utils.errors.Add(@$"La fuente ""{source}"" es desconocida Line: {Source?.Location()?.Line} Column: {Source?.Location()?.Column} ");
                break;
        }
        return check;
    }                                          // Verifica si la fuente ingresada es conocida
}           // Estructura del bloque "Selector"
public class PosAction
{
    // Property
    public Variable? Name { get; set; }     // Nombre del próximo efecto a evaluar
    public Selector? Selector { get; set; } // Bloque del "Selector" para el próximo efecto

    // Methods
    public void Evaluate(List<GameObject> parent_selector)
    {
        List<GameObject> target_selector = new List<GameObject>();
        string effectActive = "";

        if (!(Name is null))
        {
            if (Selector is null)
                target_selector = parent_selector;
            else
                target_selector = Selector.Evaluate(parent_selector);

            effectActive = Convert.ToString(Name.Evaluate(new Scope()));

            if (Utils.program.Effect is not null)
                foreach (EffectBlock effects in Utils.program.Effect)
                    if (effects.GetName() == effectActive)
                        effects.Evaluate(target_selector, null);
        }
    }  // Evalúa el bloque "PosAction" (Llama al evaluador del efecto seleccionado)
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Name is not null)
        {
            if (Name.CheckSemantic(scope))
            {
                string? nameEffect = Convert.ToString(Name?.Evaluate(scope));
                if (!Utils.ContainsEffect(nameEffect))
                {
                    Utils.errors.Add(@$"El efecto ""{nameEffect}"" no está declarado Line: {Name?.Name?.Line} Column: {Name?.Name?.Column}");
                    check = false;
                }
            }
            else
                check = false;
        }

        if (Selector is not null)
            if (!Selector.CheckSemantic(scope))
                check = false;

        return check;
    }                 // Analiza la semántica del bloque "PosAction"
}          // Estructura del bloque "PosAction"
public class Predicate
{
    // Property
    public Token? Card { get; set; }           // Nombre de la variable que se la aplica el predicado
    public Statement? Condition { get; set; }  // Predicado en sí (Condición)
    private IScope? Scope { get; set; }        // Alcance correspondiente a esta estructura (Scope)

    // Methods
    public bool Evaluate(GameObject card)
    {
        Visitor visitor = new Visitor(); 
        visitor.Define(Card?.Value, card);

        return Convert.ToBoolean(Condition?.Evaluate(Scope, visitor));
    }   // Evalúa el predicado dado la carta especificada
    public bool CheckSemantic(IScope scope)
    {
        IScope child = scope.CreateChild(); this.Scope = child;

        child.Define(new Variable(Card, new CardKey()));

        if (Condition is not null && !Condition.CheckSemantic(child))
            return false;

        return true;
    } // Analiza la semántica la estructura "Predicate" 
}          // Estructura del Predicado 
#endregion
