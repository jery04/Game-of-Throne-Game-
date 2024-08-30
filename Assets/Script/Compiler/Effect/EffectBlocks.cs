using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.GraphicsBuffer;
using System.Linq;


// Effect Block
#region  
#nullable enable
public class EffectBlock: ISemantic
{
    // Property
    public Variable? Name { get; set; }         // Almacena el nombre del efecto
    public Params? Params { get; set; }         // Almacena los parámetros del bloque "Effect"
    public Action? Action { get; set; }         // Estructura del bloque "Action"

    // Methods
    public void Evaluate(List<GameObject> target, Dictionary<string, object>? parameters)
    {
        Debug.Log("Effect");
        Action?.Evaluate(target, parameters);
    }   // Evalúa el efecto
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Name is not null && !Name.CheckSemantic(scope))
            check = false;

        if (Params is not null)
            Params.AddParams(Convert.ToString(Name?.Evaluate(scope)), scope);
        else
            Utils.AddEffect(Convert.ToString(Name?.Evaluate(scope)));

        if (Action is not null && !Action.CheckSemantic(scope))
            check = false;

        return check;
    }                                                 // Analiza el semántico del bloque "Effect"
    public string GetName()
    {
        return Convert.ToString(Name?.Evaluate(new Scope()));
    }                                                                 // Retorna el nombre del efecto
}            // Estructura de un bloque "Effect"
public class Params
{
    // Property
    public Dictionary<Token, Utils.ReturnType?> Parameters { get; set; }    // Listado de los parámetros y sus retornos

    // Builder
    public Params()
    {
        this.Parameters = new Dictionary<Token, Utils.ReturnType?>();
    }

    // Methods
    public void AddParams(string? effect, IScope scope)
    {
        Utils.AddEffect(effect);

        if (effect is not null)
        {
            foreach (Token item in Parameters.Keys)
            {
                Utils.effects[effect].Add(item.Value, Parameters[item]);
                scope.Define(new Variable(item, new Parameters(Parameters[item])));
            }
        }
    }   // Agrega los parámetros al Scope correpsondiente
}                 // Almacena los parámetros del efecto
public class Action
{
    // Property
    public List<Token?>? Parameters { get; set; }           // Almacena el nombre de los dos parámetros recibidos
    public List<Instructions?>? Instruction { get; set; }   // Almacena las instrucciones a realizar dentro del bloque
    private IScope? Scope { get; set; }                     // Almacena el alcance correspondiente al bloque (Scope)

    // Builder
    public Action()
    {
        this.Parameters = new List<Token?>();
        this.Instruction = new List<Instructions?>();
    }

    // Methods
    public void Evaluate(List<GameObject> target, Dictionary<string, object>? parameters)
    {
        Debug.Log("Action");
        Visitor visitor = new Visitor(this.Scope);

        if (parameters is not null)
            visitor.Defined = parameters;
        if (Parameters is not null)
        {
            string? firstParam = Parameters[0]?.Value; 
            if(firstParam is not null)
                visitor.Defined.Add(firstParam, target);
        }
        Debug.Log(visitor.GetValue("amount"));
        //visitor.AddInstance();

        if (Instruction is not null)
            foreach (Instructions? item in Instruction)
                item?.Evaluate(visitor);
    }  // Evalúa el bloque "PosAction"
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        IScope child = scope.CreateChild(); this.Scope = child; //test
        child.Define(new Variable(Parameters?[0], new Target()));
        child.Define(new Variable(Parameters?[1], new Context()));

        if (Instruction is not null)
            foreach (Instructions? item in Instruction)
                if (item is not null && !item.CheckSemantic(child))
                    check = false;

        return check;
    }                                                // Analiza la semántica del bloque "PosAction"
}                 // Estructura de un bloque "Action"
public abstract class Instructions
{
    // Abstract Class
    public abstract bool CheckSemantic(IScope scope);
    public abstract void Evaluate(IVisitor visitor);
}  // Representa un grupo de instrucciones
public class BucleWhile: Instructions
{
    // Property
    public Statement? Condition { get; set; }               // Almacena el condicional del While
    public List<Instructions?>? Instruction { get; set; }   // Listado de instrucciones a realizar dentro del While
    private IScope? Scope { get; set; }                     // Almacena el alcance correspondiente al bloque (Scope)

    //Methods
    public override void Evaluate(IVisitor visitor)
    {
        Debug.Log("While");
        IVisitor child;

        while (Convert.ToBoolean(Condition?.Evaluate(Scope, visitor)))
        {
            Debug.Log("Siuuuuu");
            child = visitor.CreateChild(Scope); child.AddInstance();

            if (Instruction is not null)
                foreach (Instructions? item in Instruction)
                    item?.Evaluate(child);

            visitor.AddIncrease();
        }
    }     // Evalúa las instrucciones del bloque While
    public override bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Condition is not null)
        {
            if (Condition.CheckSemantic(scope))
            {
                if (!(Instruction is null) && Condition.GetType(scope) == Utils.ReturnType.Bool)
                {
                    IScope child = scope.CreateChild(); this.Scope = child;

                    foreach (Instructions? item in Instruction)
                        if (item is not null && !item.CheckSemantic(child))
                            check = false;
                }
                else if (Instruction is not null)
                {
                    Token? warn = Condition.Location();
                    Utils.errors.Add($@"El condicional del while no retorna un booleano Line: {warn?.Line} Column: {warn?.Column}");
                    return false;
                }
            }
            else
                return false;
        }
        return check;
    }    // Analiza la semántica del bloque y sus instrucciones
}             // Estructura de un bloque "While"
public class BucleFor: Instructions
{
    // Property
    public Token? Iterator { get; set; }                  // Almacena el iterador del For
    public Token? List { get; set; }                      // Almacena la lista a iterar
    public List<Instructions?>? Instruction { get; set; } // Listado de instrucciones a realizar dentro del While
    private IScope? Scope { get; set; }                   // Almacena el alcance correspondiente al bloque (Scope)

    // Methods
    public override void Evaluate(IVisitor visitor)
    {
        Debug.Log("For");
        IVisitor child;
        IEnumerable<GameObject>? list = new List<GameObject>();

        if (List is not null)
        { 
            object? get_list = visitor.GetValue(List.Value); 
            list = get_list is not null ? (List<GameObject>)get_list : default;
        }

        foreach (GameObject card in list.AsEnumerable())
        {
            child = visitor.CreateChild(Scope); child.AddInstance();
            visitor.Define(Iterator?.Value, card);

            if (Instruction is not null)
                foreach (Instructions? item in Instruction)
                    item?.Evaluate(child);

            visitor.AddIncrease();
        }
    }   // Evalúa las instrucciones del bloque For
    public override bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (!(List is null) && scope.IsDefined(List.Value))
        {
            if (scope.GetType(List.Value, scope) != Utils.ReturnType.List)
            {
                Utils.errors.Add(@$"El for debería iterar sobre una lista Line: {List?.Line} Column: {List?.Column}");
                check = false;
            }
        }
        else
        {
            Utils.errors.Add(@$"La variable {List?.Value} no existe en el contexto actual Line: {List?.Line} Column: {List?.Column}");
            check = false;
        }

        if (Instruction is not null)
        {
            IScope child = scope.CreateChild(); this.Scope = child;
            child.Define(new Variable(Iterator, new CardKey()));

            foreach (Instructions? item in Instruction)
                if (item is not null && !item.CheckSemantic(child))
                    check = false;
        }
        return check;
    }  // Analiza la semántica del bloque y sus instrucciones
}               // Estructura de un bloque "For"
public class Variable: Instructions, ISemantic
{
    // Properties
    public Token? Name { get; set; }                    // Almacena el nombre de la variable
    public GeneralStatement? Value { get; set; }        // Almacena el valor de la variable

    // Builder
    public Variable() { }
    public Variable(Token? token, GeneralStatement? statement)
    {
        Name = token;
        Value = statement;
    }

    // Methods
    public override void Evaluate(IVisitor visitor)
    {
        Debug.Log("Variable");
        visitor.Define(Name?.Value, Value?.Evaluate(visitor.Scope, visitor));
    }   // Agrega la variable con su valor al patrón "Visitor"
    public virtual object? Evaluate(IScope scope)
    {
        return Value?.Evaluate(scope);
    }     // Llama al evaluador del "GeneralStatement"
    public Utils.ReturnType? GetType(IScope scope)
    {
        return Value?.GetType(scope);
    }    // Retorna el tipo de la variable (Bool, String, Number)
    public override bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (CardFieldIdentify())
        {
            if (Name?.Type == Token.TokenType.Power)
            {
                if (Value is not null && Value.CheckSemantic(scope))
                {
                    if (Utils.ReturnType.Number != Value?.GetType(scope))
                    {
                        Utils.errors.Add($@"El campo ""Power"" no acepta valores de tipo ""{Value?.GetType(scope)}"" Line: {Name.Line} Column: {Name.Column}");
                        return false;
                    }
                }
            }
            else if (Value is not null && Value.CheckSemantic(scope))
            {
                if (Utils.ReturnType.String != Value?.GetType(scope))
                {
                    Utils.errors.Add($@"El campo ""{Name?.Type}"" no acepta valores de tipo ""{Value?.GetType(scope)}"" Line: {Name?.Line} Column: {Name?.Column}");
                    return false;
                }
            }
            else return false;
        }
        else
        {
            if (Name?.Type == Token.TokenType.Source)
            {
                if (Value is not null && Value.CheckSemantic(scope))
                {
                    if (Utils.ReturnType.String != Value?.GetType(scope))
                    {
                        Utils.errors.Add($@"El campo ""Source"" no acepta valores de tipo ""{Value?.GetType(scope)}"" Line: {Name.Line} Column: {Name.Column}");
                        return false;
                    }
                }
            }
            else if (Name?.Type == Token.TokenType.Single)
            {
                if (Value is not null && Value.CheckSemantic(scope))
                {
                    if (Utils.ReturnType.Bool != Value?.GetType(scope))
                    {
                        Utils.errors.Add($@"El campo ""Single"" no acepta valores de tipo ""{Value?.GetType(scope)}"" Line: {Name.Line} Column: {Name.Column}");
                        return false;
                    }
                }
            }
            else
            {
                if (!(Value is null) && !Value.CheckSemantic(scope))
                    check = false;

                if (Value is not null)
                    scope.Define(this);
            }
        }
        return check;
    }  // Analiza la semántica de la variable
    public Token? Location()
    {
        return Name;
    }                          // Retorna el Token "Name"
    private bool CardFieldIdentify()
    {
        if (Name != null)
            return (Utils.cardField.Contains(Name.Type));
        else
            return false;
    }                  // Identifica si el campo pertence al bloque "Card" (palabra reservada)
    public string? ReturnName()
    {
        return Name?.Value;
    }                       // Retorna el nombre de la variable
}               // Almacena una variable
public class Array: Variable
{
    // Property
    public new List<Atom?>? Value { get; set; }           // Listado de valores almacenados en el array

    //Builder
    public Array()
    {
        Value = new List<Atom?>();
    }

    // Methods
    public override object? Evaluate(IScope scope)
    {
        bool[] position = new bool[3];
        if (Value is not null)
        {
            foreach (Atom? atom in Value)
            {
                string? pos = Convert.ToString(atom?.Evaluate(scope));
                if (pos == "Melee")
                    position[0] = true;

                else if (pos == "Ranged")
                    position[1] = true;

                else if (pos == "Siege")
                    position[2] = true;

                else if (pos == "Climate")
                    return Card.card_position.C;

                else if (pos == "Increase")
                    return Card.card_position.I;

                else if (pos == "Leader")
                    return Card.card_position.L;

                else Utils.errors.Add(@$"La posición ""{pos}"" no existe. Line: {Name?.Line} Column: {Name?.Column} ");
            }
        }

        if (position[0] && position[1] && position[2])
            return Card.card_position.MRS;

        else if (position[0] && position[1])
            return Card.card_position.MR;

        else if (position[0] && position[2])
            return Card.card_position.MS;

        else if (position[1] && position[2])
            return Card.card_position.RS;

        else if (position[0])
            return Card.card_position.M;

        else if (position[1])
            return Card.card_position.R;

        else if (position[2])
            return Card.card_position.S;

        return null;
    }     // Retorna el "Type" del campo "Range" (valores string)
    public new Utils.ReturnType? GetType()
    {
        return Utils.ReturnType.String;
    }             // Retorna el tipo que conforma el Array
    public override bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Value is not null)
        {
            int i = 1;
            foreach (Atom? item in Value)
            {
                if ((item != null) && (item.CheckSemantic(scope)))
                {
                    if (item.GetType(scope) != Utils.ReturnType.String)
                    {
                        Utils.errors.Add($@"El elemento {i} del array {Name?.Type} no es de tipo ""String"" ");
                        check = false;
                    }
                }
                else check = false;
                i++;
            }
        }
        return check;
    }   // Analiza la semnántica del Array  
}                  // Almacena los elementos de un array
public class IfElse: Instructions
{
    // Properties
    public Statement Condition { get; set; }                    // Condicional
    public List<Instructions?>? Instruction_If { get; set; }      // Instrucciones del bloque "If"
    public List<Instructions?>? Instruction_Else { get; set; }    // Instrucciones del bloque "Else"  
    public IScope Scope { get; set; }                           // Alcance asociado al bloque (Scope)

    // Builder
    public IfElse()
    {
        this.Instruction_If = new List<Instructions?>();
        this.Instruction_Else = new List<Instructions?>();
    }

    // Methods
    public override void Evaluate(IVisitor visitor)
    {
        IVisitor child; child = visitor.CreateChild(Scope); 
        child.AddInstance();

        if(Instruction_If is not null)
        {
            if (Convert.ToBoolean(Condition.Evaluate(Scope, child)))
            {
                foreach (Instructions? item in Instruction_If)
                    if (item is not null)
                        item.Evaluate(child);
            }

            else if (Instruction_Else is not null && Instruction_Else.Count > 0)
                foreach (Instructions? item in Instruction_Else)
                    if (item is not null)
                        item.Evaluate(child);
        }
    }
    public override bool CheckSemantic(IScope scope)
    {
        IScope child = scope.CreateChild(); this.Scope = child;
        bool check = true;

        if (Condition is not null)
            if (!Condition.CheckSemantic(scope))
                check = false;
        
        if(Instruction_If is not null)
            foreach (Instructions? item in Instruction_If)
                if (item is not null && !item.CheckSemantic(child))
                    check = false;

        if (Instruction_Else is not null && Instruction_Else.Count > 0)
            foreach (Instructions? item in Instruction_Else)
                if (item is not null && !item.CheckSemantic(child))
                    check = false;

        return check;
    }

}                 // Estructura del bloque "If-Else"
#endregion
