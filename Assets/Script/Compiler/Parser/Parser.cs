using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class Parser : IParsing                   // Analizador sintáctico
{
    // Properties
    private Token[] Tokens { get; }              // Código en array de Tokens (Léxico)
    private Token? CurrentToken { get; set; }    // Token actual durante el recorrido
    private int Index { get; set; }              // Índice actual del recorrido                                                                                            

    // Builder
    public Parser(Token[] tokens)
    {
        this.Tokens = tokens;
        this.CurrentToken = null;
        this.Index = -1;
    }

    // Methods
    #region
    private bool ThereIsNext(int i = 1)
    {
        if (Index + i < Tokens.Length)
            return true;

        return false;
    }  // Retorna true si hay próximo
    private Token LookAhead(int i = 1)
    {
        if (ThereIsNext(i))
            return Tokens[Index + i];

        return Tokens[Index-1];
    }   // Retorna el (i)próximo elemento (Sin avanzar)
    private bool LookAhead(bool chose, params Token.TokenType[] nextTokens)
    {
        if (ThereIsNext())
        {
            foreach (Token.TokenType item in nextTokens)
            {
                if (item == LookAhead()?.Type)
                {
                    if (chose)
                    {
                        this.CurrentToken = Tokens[++Index];
                        return true;
                    }
                    else return true;
                }
            }
        }
        return false;
    }   // Si chose es <true> y alguno de estos elementos coincide con el próximo, avanza y retorna true. Else retorna <true>
    private bool LookBeyond(params Token.TokenType[] nextTokens)
    {
        for (int i = 0; i < nextTokens.Length; i++)
            if (ThereIsNext(i + 1) && nextTokens[i] != LookAhead(i + 1)?.Type)
                return false;

        return true;
    }       // Retorna <true> si los siguientes Tokens corresponden con la secuencia pasada por parámetro
    private void Match()
    {
        if (ThereIsNext())
            CurrentToken = Tokens[++Index];
    }                // Avanza sin importar el siguiente Token
    private void Match(params Token.TokenType?[] nextTokens)
    {
        foreach (Token.TokenType? item in nextTokens)
        {
            if (item == LookAhead()?.Type)
                this.CurrentToken = Tokens[++Index];
            else
            {
                Utils.errors.Add($"Error: No se esperaba un \"{LookAhead()?.Value}\" Line: {LookAhead()?.Line}, Column: {LookAhead()?.Column}");
                if(ThereIsNext())
                    this.CurrentToken = Tokens[++Index];
            }
        }
    }           // Avanza en el orden de parámetros de entrada
    private Token? MatchReturn(params Token.TokenType[] nextTokens)
    {
        if (nextTokens.Length != 0)
        {
            foreach (Token.TokenType item in nextTokens)
            {
                if (item == LookAhead()?.Type)
                {
                    this.CurrentToken = Tokens[++Index];
                    return CurrentToken;
                }
            }
            Utils.errors.Add($"Error: No se esperaba un \"{LookAhead()?.Value}\" Line: {LookAhead()?.Line}, Column: {LookAhead()?.Column}");
            if (ThereIsNext())
                this.CurrentToken = Tokens[++Index];
        }
        else
        {
            this.CurrentToken = Tokens[++Index];
            return CurrentToken;
        }

        return null;
    }   // Retorna uno de los coincidentes
    #endregion

    // Program (Beginning)
    public ProgramCompiler Parse()
    {
        ProgramCompiler ast = new ProgramCompiler();

        // Effects
        if(LookAhead(false, Token.TokenType.Effect))
            while (LookAhead(false, Token.TokenType.Effect))
                ast.Effect?.Add(EffectBuilder());

        // Cards
        if (LookAhead(false, Token.TokenType.Card))
            while (LookAhead(false, Token.TokenType.Card))
                ast.Card?.Add(CardBuilder());
        else
            Match(Token.TokenType.Card);

        return ast;
    }

    // Effect Block
    #region
    private EffectBlock EffectBuilder()
    {
        EffectBlock effect = new EffectBlock();             // Almacena el tipo EffectBlock a retornar

        Match(Token.TokenType.Effect, Token.TokenType.OpenKey);

        effect.Name = FieldBuilder(Token.TokenType.Name);   // Construye el nombre

        if (LookAhead()?.Type == Token.TokenType.Params)    // Construye el bloque de parámetros
            effect.Params = ParamsBuilder();

        effect.Action = ActionBuilder();                    // Construye el bloque "Action"

        Match(Token.TokenType.ClosedKey);

        return effect;
    }       // Bloque de efectos
    private Params ParamsBuilder()
    {
        Params param = new Params();

        Match(Token.TokenType.Params, Token.TokenType.Colon, Token.TokenType.OpenKey);

        while (LookAhead(false, Token.TokenType.UnKnown))
        {
            Token? name = MatchReturn(Token.TokenType.UnKnown);
            Match(Token.TokenType.Colon);
            Token.TokenType? value = MatchReturn(Token.TokenType.Number, Token.TokenType.String, Token.TokenType.Bool)?.Type;

            if (name is not null && value is not null)
            {
                switch (value)
                {
                    case Token.TokenType.Number:        // Parámetro de tipo "Number"
                        param.Parameters.Add(name, Utils.ReturnType.Number);
                        break;

                    case Token.TokenType.String:        // Parámetro de tipo "String"
                        param.Parameters.Add(name, Utils.ReturnType.String);
                        break;

                    case Token.TokenType.Bool:          // Parámetro de tipo "Bool"
                        param.Parameters.Add(name, Utils.ReturnType.Bool);
                        break;
                }
            }
            Match(Token.TokenType.Comma);
        }   // Construye los parámetros posibles
        Match(Token.TokenType.ClosedKey, Token.TokenType.Comma);

        return param;
    }            // Definición de parámetros
    private Action ActionBuilder()
    {
        Action action = new Action();

        Match(Token.TokenType.Action, Token.TokenType.Colon, Token.TokenType.OpenParan);

        if (LookAhead(false, Token.TokenType.UnKnown))
            LookAhead().Type = Token.TokenType.Targets;

        action.Parameters?.Add(MatchReturn(Token.TokenType.Targets));   // Almacena el primer parámetro

        Match(Token.TokenType.Comma);

        if (LookAhead(false, Token.TokenType.UnKnown))
            LookAhead().Type = Token.TokenType.Context;                    

        action.Parameters?.Add(MatchReturn(Token.TokenType.Context));  // Almacena el segundo parámetro 

        Match(Token.TokenType.ClosedParan, Token.TokenType.Arrow, Token.TokenType.OpenKey);
        action.Instruction = InstructionBuilder();                     // Construye el bloque las instrucciones del bloque "Action"
        Match(Token.TokenType.ClosedKey);

        return action;
    }            // Acción del efecto (efecto en sí)
    private BucleWhile WhileBuilder()
    {
        BucleWhile bucleWhile = new BucleWhile();// Almacena el bloque While a retornar

        Match(Token.TokenType.While, Token.TokenType.OpenParan);
        bucleWhile.Condition = StatementBuilder();              // Almacena el condicional del bloque While
        Match(Token.TokenType.ClosedParan);

        if (LookAhead(false, Token.TokenType.OpenKey))          // Si es un bloque de instrucciones 
        {
            Match(Token.TokenType.OpenKey);
            bucleWhile.Instruction = InstructionBuilder();      // Construir bloque While
            Match(Token.TokenType.ClosedKey);
            Match(Token.TokenType.SemiColon);
        }
        else
            bucleWhile.Instruction = InstructionBuilder(false); // Si es una sola instrucción

        return bucleWhile;
    }         // Instruciones del Bloque "While"
    private BucleFor ForBuilder()
    {
        BucleFor bucleFor = new BucleFor();      // Almacena el bloque For a retornar

        Match(Token.TokenType.For);
        bucleFor.Iterator = MatchReturn(Token.TokenType.UnKnown);   // Almacena el Iterador
        Match(Token.TokenType.In);
        bucleFor.List = MatchReturn(Token.TokenType.UnKnown);       // Almacena la lista a iterar

        if (LookAhead(false, Token.TokenType.OpenKey))              // Si es un bloque de instrucciones
        {   
            Match(Token.TokenType.OpenKey);
            bucleFor.Instruction = InstructionBuilder();            // Construye el bloque For
            Match(Token.TokenType.ClosedKey);
            Match(Token.TokenType.SemiColon);
        }
        else
            bucleFor.Instruction = InstructionBuilder(false);       // Si es una sola instrucción

        return bucleFor;
    }             // Instruciones del Bloque "For"
    private List<Instructions?>? InstructionBuilder(bool OnTime = true)
    {
        List<Instructions?> body = new List<Instructions?>();   // Almacena un listado de las instrucciones a retornar

        do
        {
            if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Assignment))    
                body.Add(VariableBuilder());                                    // Instrucciones de variables

            else if(LookBeyond(Token.TokenType.If))
                body.Add(IfElseBuilder());

            else if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Dot, Token.TokenType.UnKnown))
            { body.Add(MoleculeBuilder()); Match(Token.TokenType.SemiColon); }  // Instrucciones de Moléculas

            else if (LookAhead(false, Token.TokenType.For))
                body.Add(ForBuilder());                                         // Instrucciones de bloques For

            else if (LookAhead(false, Token.TokenType.While))
                body.Add(WhileBuilder());                                       // Instrucciones de bloques While

            else
                OnTime = false;                                                 // Una sola instrucción

        } while (OnTime);

        return body;
    }   // Instrucciones (Hojas del bloque efecto)
    private Variable VariableBuilder()
    {
        Variable variable = new Variable();      // Almacena variable a retornar   

        variable.Name = MatchReturn(Token.TokenType.UnKnown);   // Almacena el nombre
        Match(Token.TokenType.Assignment);
        variable.Value = StatementBuilder();                    // Almacena el valor
        Match(Token.TokenType.SemiColon);

        return variable;
    }        // Construye variables
    private IfElse IfElseBuilder()
    {
        IfElse if_else = new IfElse();           // Almacena el bloque "If_Else" a retornar

        Match(Token.TokenType.If, Token.TokenType.OpenParan);
        if_else.Condition = StatementBuilder();  // Construye el Condicional del bloque
        Match(Token.TokenType.ClosedParan, Token.TokenType.OpenKey);

        if_else.Instruction_If = InstructionBuilder();  // Instrucciones
        Match(Token.TokenType.ClosedKey);
        
        if(LookAhead(true, Token.TokenType.Else))       // Estructura "Else"
        {
            if(LookAhead(false, Token.TokenType.If))
                if_else.ElseIf = IfElseBuilder();
            else
            {
                Match(Token.TokenType.OpenKey);
                if_else.Instruction_Else = InstructionBuilder();
                Match(Token.TokenType.ClosedKey);
            }
        }

        return if_else;
    }            // Construye bloques "If_Else"
    #endregion

    // Card Block
    #region
    private CardBlock CardBuilder()
    {
        CardBlock card = new CardBlock();                // Almacena el bloque Card a retornar
        List<string> fieldCard = new List<string>()
            {
                "Name", "Type", "Faction", "Power", "Range"
            };  // Posibles parámetros

        Match(Token.TokenType.Card, Token.TokenType.OpenKey);
        while (fieldCard.Count != 0)
        {
            if (fieldCard.Contains(LookAhead().Value))
            {
                switch (LookAhead().Value)
                {
                    case ("Name"):
                        LookAhead().Type = Token.TokenType.Name;
                        card.Name = FieldBuilder(Token.TokenType.Name);
                        fieldCard.Remove("Name");
                        break;
                    case ("Type"):
                        LookAhead().Type = Token.TokenType.Type;
                        card.Type = FieldBuilder(Token.TokenType.Type);
                        fieldCard.Remove("Type");
                        break;
                    case ("Faction"):
                        LookAhead().Type = Token.TokenType.Faction;
                        card.Faction = FieldBuilder(Token.TokenType.Faction);
                        fieldCard.Remove("Faction");
                        break;
                    case ("Power"):
                        LookAhead().Type = Token.TokenType.Power;
                        card.Power = FieldBuilder(Token.TokenType.Power);
                        fieldCard.Remove("Power");
                        break;
                    case ("Range"):
                        LookAhead().Type = Token.TokenType.Range;
                        card.Range = RangeBuilder();
                        fieldCard.Remove("Range");
                        break;
                }
            }
            else
            {
                Utils.errors.Add($"Error: No se esperaba un \"{LookAhead()?.Value}\" Line: {LookAhead()?.Line}, Column: {LookAhead()?.Column}");
                break;
            }
        }                 // Construye los campos sin importar el orden

        card.OnActivation = OnActivationBuilder();      // Llama al constructor de la sintaxis del bloque "OnActivation"

        Match(Token.TokenType.ClosedKey);

        return card;
    }       // Bloque de cartas  
    private Variable RangeBuilder()
    {
        Array range = new Array();           // Almacena el array a retornar

        range.Name = MatchReturn(Token.TokenType.Range); // Almacena el nombre del array
        Match(Token.TokenType.Colon, Token.TokenType.OpenBracket);

        do
        {
            range.Value?.Add(AtomBuilder());             // Elementos dentro del array
        } while (LookAhead(true, Token.TokenType.Comma));

        Match(Token.TokenType.ClosedBracket, Token.TokenType.Comma);

        return range;
    }       // Campo "Range" (Array)
    private Variable FieldBuilder(Token.TokenType tokenField)
    {
        Variable field = new Variable();     // Almacena el campo a retornar

        if (Utils.card.Contains(LookAhead().Value))
            LookAhead().Type = Utils.cardField[Utils.card.IndexOf(LookAhead().Value)];

        field.Name = MatchReturn(tokenField);   // Almacena el nombre del campo
        Match(Token.TokenType.Colon);

        Debug.Log(LookAhead().Value +" "+LookAhead(2).Type);
        field.Value = StatementBuilder();       // Almacena el valor
        Match(Token.TokenType.Comma);

        return field;
    }  // Construye los distintos campos
    private List<Variable?>? ParamsActivationBuilder() 
    {
        List<Variable?>? parameters = new List<Variable?>();      // Listado de parámetros a retornar

        while (LookAhead(false, Token.TokenType.UnKnown))
        {
            Variable variable = new Variable();                   // Variable

            variable.Name = MatchReturn(Token.TokenType.UnKnown); // Nombre de la variable
            Match(Token.TokenType.Colon);
            variable.Value = StatementBuilder();                  // Almacena el valor de la variable  

            parameters.Add(variable);                             // Agrega la variable al listado
            Match(Token.TokenType.Comma);
        }
        return parameters;
    }    // Construye los parámetros que pasar al efecto
    private OnActivation OnActivationBuilder()
    {
        OnActivation onActivation = new OnActivation();       // Almacena el bloque "OnActivation" a retornar

        Match(Token.TokenType.OnActivation, Token.TokenType.Colon, Token.TokenType.OpenBracket);

        if (LookAhead(false, Token.TokenType.OpenKey))        // Si viene un bloque...
        {
            do
            {
                onActivation.Body?.Add(OnActivationBodyBuilder());  // Almacena cada bloque de Activación en el listado

            } while (LookAhead(true, Token.TokenType.Comma)); // Si viene otro bloque...
        }

        Match(Token.TokenType.ClosedBracket);

        return onActivation;
    }             // Construye el Bloque "OnActivation"
    private OnActivationBody OnActivationBodyBuilder()
    {
        OnActivationBody activationBody = new OnActivationBody();   // Almacena el bloque de activación a retornar

        Match(Token.TokenType.OpenKey);

        activationBody.EffectActivation = EffectActivationBuilder(); // Llama al constructor de "EffectActivation" 
        activationBody.Selector = SelectorBuilder();                 // LLama al constructor de "Selector"

        while (LookAhead(false, Token.TokenType.PostAction))
            activationBody.PosAction?.Add(PosActionBuilder());       // Llama al constructor del bloque "PosAction"

        Match(Token.TokenType.ClosedKey);

        return activationBody;
    }     // Construye cada activación
    private PosAction PosActionBuilder()
    {
        PosAction posAction = new PosAction();

        Match(Token.TokenType.PostAction, Token.TokenType.Colon, Token.TokenType.OpenKey);

        if (LookAhead().Value == "Type")
            LookAhead().Type = Token.TokenType.Type;

        posAction.Name = FieldBuilder(Token.TokenType.Type);

        if (LookAhead(false, Token.TokenType.Selector))
            posAction.Selector = SelectorBuilder();

        Match(Token.TokenType.ClosedKey);

        return posAction;
    }                   // Construye la sintaxis del bloque "PosAction"
    private EffectActivation EffectActivationBuilder()
    {
        EffectActivation effect = new EffectActivation();     // Almacena el bloque "EffecActivation" a retornar

        Match(Token.TokenType.EffectActivation, Token.TokenType.Colon);

        if (LookAhead(false, Token.TokenType.OpenKey))      // Si es un bloque...
        {
            Match(Token.TokenType.OpenKey);

            Debug.Log(LookAhead().Value);

            if (LookAhead().Value == "Name")
                LookAhead().Type = Token.TokenType.Name;

            Debug.Log(LookAhead().Value);
            effect.Name = FieldBuilder(Token.TokenType.Name);  // Almacena el nombre del efecto a activar

            if (LookAhead(false, Token.TokenType.UnKnown))
                effect.Parameters = ParamsActivationBuilder(); // Almacena los parámetros a enviar

            Match(Token.TokenType.ClosedKey);
        }
        else                                                // Si no es un bloque...
        {
            effect.Name = new Variable();                   // Almacena el nombre del efecto a activar
            effect.Name.Value = StatementBuilder();
            Match(Token.TokenType.Comma);
        }

        return effect;
    }     // Almacena el nombre del efecto y los parámetros a pasar
    private Selector SelectorBuilder()
    {
        Selector selector = new Selector();                   // Almacena el bloque "Selector" a retornar  

        Match(Token.TokenType.Selector, Token.TokenType.Colon, Token.TokenType.OpenKey);
        selector.Source = FieldBuilder(Token.TokenType.Source); // Construye el campo "Source"
        selector.Single = FieldBuilder(Token.TokenType.Single); // Construye el campo "Single"
        selector.Predicate = PredicateBuilder(Token.TokenType.Predicate); // Construye el predicado
        Match(Token.TokenType.ClosedKey);

        return selector;
    }                     // Construye la sintaxis del bloque "Selector"
    private Predicate PredicateBuilder(Token.TokenType? token = null)
    {
        Predicate predicate = new Predicate();                // Almacena el predicado a retornar 

        if(token is null)
            Match(Token.TokenType.OpenParan);
        else
            Match(token, Token.TokenType.Colon, Token.TokenType.OpenParan);

        predicate.Card = MatchReturn(Token.TokenType.UnKnown);// Almacena el nombre de la variable_carta
        Match(Token.TokenType.ClosedParan, Token.TokenType.Arrow);
        predicate.Condition = StatementBuilder();             // Construye el condicional (predicado en sí)

        return predicate;
    }  // Construye la sintaxis de un predicado
    #endregion

    // Binary's Expressions   
    #region
    private Expressions ExpressionBuilder()
    {
        Expressions expression = new Expressions();
        expression.Term = TermsBuilder();      // Llama al constructor de un término  

        if (LookAhead(false, Token.TokenType.Plus, Token.TokenType.Minus)) // + o -
        {
            expression.Opeartor = MatchReturn(Token.TokenType.Plus, Token.TokenType.Minus);
            expression.Expression = ExpressionBuilder();
        }
        return expression;
    }    // Construye la sintaxis de una expresión
    private Terms TermsBuilder()
    {
        Terms term = new Terms();
        term.Factor = FactorBuilder(); // Llama al constructor de un factor 

        if (LookAhead(false, Token.TokenType.Times, Token.TokenType.Divide, Token.TokenType.Pow)) // % o x o ^
        {
            term.Opeartor = MatchReturn(Token.TokenType.Times, Token.TokenType.Divide, Token.TokenType.Pow);
            term.Term = TermsBuilder(); // Llama al constructor de un término
        }
        return term;
    }               // Construye la sintaxis de un término
    private Factor FactorBuilder()
    {
        Factor factor = new Factor();

        if (LookAhead(false, Token.TokenType.OpenParan))            // Si viene un paréntesis...
        {
            Match();
            factor.Expression = ExpressionBuilder();                // Llama al constructor de una expresión
            Match(Token.TokenType.ClosedParan);
        }
        else                                                        // Si no viene un paréntesis...
        {
            if (LookAhead(false, Token.TokenType.Digit))
                factor.Leaf = MatchReturn(Token.TokenType.Digit);   // Almacena los dígitos

            else if(LookAhead(false, Token.TokenType.UnKnown))
            {
                factor.Leaf = MatchReturn(Token.TokenType.UnKnown); // Almacena las variables
                if (LookAhead(true, Token.TokenType.PlusPlus))
                    factor.Increase = true;
            }
        }

        return factor;
    }             // Construye la sintaxis de un factor
    #endregion

    // Statement's Tree
    #region
    private Statement StatementBuilder()
    {
        Statement boolean = new Statement();                // Almacena el "Statement" a retornar
        if (LookAhead()?.Type == Token.TokenType.OpenParan) // Si viene paréntesis...
        {
            Match(Token.TokenType.OpenParan);
            boolean.NodeLeft = SubStatementBuilder();       // Construye el nodo izquierdo
            Match(Token.TokenType.ClosedParan);
        }
        else                                                // Si no viene paréntesis...
            boolean.NodeLeft = SubStatementBuilder();       // Construye solo el nodo izquierdo

        if (LookAhead(false, Token.TokenType.AND, Token.TokenType.OR))
        {
            boolean.LogOperator = MatchReturn();            // && o || 
            boolean.NodeRight = StatementBuilder();         // Construye el nodo derecho
        }

        return boolean;
    }          // Construye un declaración
    private SubStatement SubStatementBuilder()
    {
        SubStatement subBoolean = new SubStatement();       // Almacena el "SubStatement" a retornar

        if (LookAhead(false, Token.TokenType.OpenParan))    // Si viene paréntesis...
        {
            Match(Token.TokenType.OpenParan);
            subBoolean.NodeLeft = MoleculeBuilder();        // Construye el nodo izquierdo
            Match(Token.TokenType.ClosedParan);
        }
        else
            subBoolean.NodeLeft = MoleculeBuilder();        // Construye solo el nodo izquierdo

        if (LookAhead(false, Token.TokenType.AND, Token.TokenType.OR))
        {
            subBoolean.LogOperator = MatchReturn();         // && o || 
            subBoolean.NodeRight = StatementBuilder();      // Construye el nodo derecho
        }

        return subBoolean;
    }    // Construye un sub_declaración
    private Molecule MoleculeBuilder()
    {
        Molecule molecule = new Molecule(); // Almacena la "Molécula" a retornar 

        molecule.NodeLeft = AtomBuilder();  // Construye el nodo izquierdo

        if (LookAhead(false, Utils.symbols.ToArray()))
        {
            molecule.ArtOpeartor = MatchReturn();   // <, <=, ==, >, >=
            molecule.NodeRight = AtomBuilder();     // Construye el nodo derecho
        }

        return molecule;
    }            // Construye una comparación
    private Atom? AtomBuilder()
    {
        if (LookAhead(false, Token.TokenType.False, Token.TokenType.True))
            return Atom0Builder();                  // Constrye un Boolean

        else if (LookAhead(false, Token.TokenType.Quote))
            return Atom3Builder();                  // Constrye un String

        else if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Dot))
            return Atom2Builder();                  // Constrye un Llamados

        else return Atom1Builder();                 // Constrye una Expresión numéricas
    }                   // Construye un "Átomo" (A0_A1_A2_A3)
    private Atom0 Atom0Builder()
    {
        Atom0 atom0 = new Atom0();
        atom0.Boolean = MatchReturn(Token.TokenType.False, Token.TokenType.True);
        return atom0;
    }                  // Construye un "Átomo0" (Boolean)
    private Atom1 Atom1Builder()
    {
        Atom1 atom1 = new Atom1();                  // Almacena la expresión numérica retornar
        atom1.Expression = ExpressionBuilder();

        return atom1;
    }                  // Construye un "Átomo1" (expresiones numéricas)
    private Atom2 Atom2Builder()
    {
        Atom2 atom2 = new Atom2();                   // Almacena el llamado a retornar

        do
        {
            atom2.Call?.Add(MatchReturn(Token.TokenType.UnKnown));

        } while (LookAhead(true, Token.TokenType.Dot));  // Mientras allá propiedades o métodos
            
        if (LookAhead(false, Token.TokenType.OpenParan)) // parámetro a un método
        {
            Match(Token.TokenType.OpenParan);

            if (LookBeyond(Token.TokenType.UnKnown))
                atom2.Nested = Atom2Builder();          // Recursivo...

            else if(LookBeyond(Token.TokenType.OpenParan, Token.TokenType.UnKnown, Token.TokenType.ClosedParan))
                atom2.Predicate = PredicateBuilder();

            Match(Token.TokenType.ClosedParan);
        }

        return atom2;
    }                  // Construye un "Átomo2" (Llamadas a propiedades)
    private Atom3 Atom3Builder()
    {
        Atom3 atom3 = new Atom3();                   // Almacena el tipo string a retornar
        do
        {
            Match(Token.TokenType.Quote);
            do
            {
                atom3.String?.Add(MatchReturn(Token.TokenType.UnKnown)); // Almacena cada palabra

            } while (LookAhead(false, Token.TokenType.UnKnown));

            Match(Token.TokenType.Quote);

            if (LookAhead(false, Token.TokenType.ATAT, Token.TokenType.AT)) // Concatenar palabras 
                atom3.String?.Add(MatchReturn());
            else
                break;
        } while (true);

        return atom3;
    }                  // Construye un "Átomo3" (String)
    #endregion
}   