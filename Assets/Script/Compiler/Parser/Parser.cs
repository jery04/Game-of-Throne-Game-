using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class Parser : IParsing
{
    // Properties
    private Token[] Tokens { get; }              // Código en array de Tokens (Lexer)
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
    }   // Retorna true si hay próximo
    private Token LookAhead(int i = 1)
    {
        if (ThereIsNext(i))
            return Tokens[Index + i];

        throw new NotImplementedException();
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
    } // Retorna <true> si los siguientes Tokens corresponden con la secuencia pasada por parámetro
    private void Match()
    {
        if (ThereIsNext())
            CurrentToken = Tokens[++Index];
    }   // Avanza sin importar el siguiente Token
    private void Match(params Token.TokenType[] nextTokens)
    {
        foreach (Token.TokenType item in nextTokens)
        {
            if (item == LookAhead()?.Type)
                this.CurrentToken = Tokens[++Index];
            else
            {
                Utils.errors.Add($"Error: No se esperaba un \"{LookAhead()?.Value}\" Line: {LookAhead()?.Line}, Column: {LookAhead()?.Column}");
                this.CurrentToken = Tokens[++Index];
            }
        }
    }   // Avanza en el orden de parametros de entrada
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

        while (LookAhead(false, Token.TokenType.Effect))
            ast.Effect?.Add(EffectBuilder());

        while (LookAhead(false, Token.TokenType.Card))
            ast.Card?.Add(CardBuilder());

        return ast;
    }

    // Effect Block
    #region
    private EffectBlock EffectBuilder()
    {
        EffectBlock effect = new EffectBlock();

        Match(Token.TokenType.Effect, Token.TokenType.OpenKey);

        effect.Name = FieldBuilder(Token.TokenType.Name);

        if (LookAhead()?.Type == Token.TokenType.Params)
            effect.Params = ParamsBuilder();

        effect.Action = ActionBuilder();

        Match(Token.TokenType.ClosedKey);

        return effect;
    }
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
                    case Token.TokenType.Number:
                        param.Parameters.Add(name, Utils.ReturnType.Number);
                        break;

                    case Token.TokenType.String:
                        param.Parameters.Add(name, Utils.ReturnType.String);
                        break;

                    case Token.TokenType.Bool:
                        param.Parameters.Add(name, Utils.ReturnType.Bool);
                        break;
                }
            }
            Match(Token.TokenType.Comma);
        }
        Match(Token.TokenType.ClosedKey, Token.TokenType.Comma);

        return param;
    }
    private Action ActionBuilder()
    {
        Action action = new Action();

        Match(Token.TokenType.Action, Token.TokenType.Colon, Token.TokenType.OpenParan);

        if (LookAhead(false, Token.TokenType.UnKnown))
            LookAhead().Type = Token.TokenType.Targets;

        action.Parameters?.Add(MatchReturn(Token.TokenType.Targets));

        Match(Token.TokenType.Comma);

        if (LookAhead(false, Token.TokenType.UnKnown))
            LookAhead().Type = Token.TokenType.Context;

        action.Parameters?.Add(MatchReturn(Token.TokenType.Context));

        Match(Token.TokenType.ClosedParan, Token.TokenType.Arrow, Token.TokenType.OpenKey);
        action.Instruction = InstructionBuilder();
        Match(Token.TokenType.ClosedKey);

        return action;
    }
    private BucleWhile WhileBuilder()
    {
        BucleWhile bucleWhile = new BucleWhile();

        Match(Token.TokenType.While, Token.TokenType.OpenParan);
        bucleWhile.Condition = StatementBuilder();
        Match(Token.TokenType.ClosedParan);

        if (LookAhead(false, Token.TokenType.OpenKey))
        {
            Match(Token.TokenType.OpenKey);
            bucleWhile.Instruction = InstructionBuilder();
            Match(Token.TokenType.ClosedKey);
            Match(Token.TokenType.SemiColon);
        }
        else
            bucleWhile.Instruction = InstructionBuilder(false);

        return bucleWhile;
    }
    private BucleFor ForBuilder()
    {
        BucleFor bucleFor = new BucleFor();

        Match(Token.TokenType.For);
        bucleFor.Iterator = MatchReturn(Token.TokenType.UnKnown);
        Match(Token.TokenType.In);
        bucleFor.List = MatchReturn(Token.TokenType.UnKnown);

        if (LookAhead(false, Token.TokenType.OpenKey))
        {
            Match(Token.TokenType.OpenKey);
            bucleFor.Instruction = InstructionBuilder();
            Match(Token.TokenType.ClosedKey);
            Match(Token.TokenType.SemiColon);
        }
        else
            bucleFor.Instruction = InstructionBuilder(false);

        return bucleFor;
    }
    private List<Instructions?>? InstructionBuilder(bool OnTime = true)
    {
        List<Instructions?> body = new List<Instructions?>();

        do
        {
            if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Assignment))
                body.Add(VariableBuilder());

            else if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Dot, Token.TokenType.UnKnown))
            { body.Add(MoleculeBuilder()); Match(Token.TokenType.SemiColon); }

            else if (LookAhead(false, Token.TokenType.For))
                body.Add(ForBuilder());

            else if (LookAhead(false, Token.TokenType.While))
                body.Add(WhileBuilder());

            else
                OnTime = false;

        } while (OnTime);

        return body;
    }
    private Variable VariableBuilder()
    {
        Variable variable = new Variable();

        variable.Name = MatchReturn(Token.TokenType.UnKnown);
        Match(Token.TokenType.Assignment);
        variable.Value = StatementBuilder();
        Match(Token.TokenType.SemiColon);

        return variable;
    }
    #endregion

    // Card Block
    #region
    private CardBlock CardBuilder()
    {
        CardBlock card = new CardBlock();
        List<string> fieldCard = new List<string>()
            {
                "Name", "Type", "Faction", "Power", "Range"
            };

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
        }

        card.OnActivation = OnActivationBuilder();

        Match(Token.TokenType.ClosedKey);

        return card;
    }
    private Variable RangeBuilder()
    {
        Array range = new Array();

        range.Name = MatchReturn(Token.TokenType.Range);
        Match(Token.TokenType.Colon, Token.TokenType.OpenBracket);

        do
        {
            range.Value?.Add(AtomBuilder());
        } while (LookAhead(true, Token.TokenType.Comma));

        Match(Token.TokenType.ClosedBracket, Token.TokenType.Comma);

        return range;
    }
    private Variable FieldBuilder(Token.TokenType tokenField)
    {
        Variable field = new Variable();

        if (Utils.card.Contains(LookAhead().Value))
            LookAhead().Type = Utils.fieldCard[Utils.card.IndexOf(LookAhead().Value)];

        field.Name = MatchReturn(tokenField);
        Match(Token.TokenType.Colon);

        field.Value = StatementBuilder();
        Match(Token.TokenType.Comma);

        return field;
    }
    private List<Variable?>? ParamsActivationBuilder()
    {
        List<Variable?>? parameters = new List<Variable?>();

        while (LookAhead(false, Token.TokenType.UnKnown))
        {
            Variable variable = new Variable();

            variable.Name = MatchReturn(Token.TokenType.UnKnown);
            Match(Token.TokenType.Colon);
            variable.Value = StatementBuilder();

            parameters.Add(variable);
            Match(Token.TokenType.Comma);
        }
        return parameters;
    }
    private OnActivation OnActivationBuilder()
    {
        OnActivation onActivation = new OnActivation();

        Match(Token.TokenType.OnActivation, Token.TokenType.Colon, Token.TokenType.OpenBracket);

        if (LookAhead(false, Token.TokenType.OpenKey))
        {
            do
            {
                onActivation.Body?.Add(OnActivationBodyBuilder());

            } while (LookAhead(true, Token.TokenType.Comma));
        }

        Match(Token.TokenType.ClosedBracket);

        return onActivation;
    }
    private OnActivationBody OnActivationBodyBuilder()
    {
        OnActivationBody activationBody = new OnActivationBody();

        Match(Token.TokenType.OpenKey);

        activationBody.EffectActivation = EffectActivationBuilder();
        activationBody.Selector = SelectorBuilder();

        while (LookAhead(false, Token.TokenType.PostAction))
            activationBody.PosAction?.Add(PosActionBuilder());

        Match(Token.TokenType.ClosedKey);

        return activationBody;
    }
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
    }
    private EffectActivation EffectActivationBuilder()
    {
        EffectActivation effect = new EffectActivation();

        Match(Token.TokenType.EffectActivation, Token.TokenType.Colon);

        if (LookAhead(false, Token.TokenType.OpenKey))
        {
            Match(Token.TokenType.OpenKey);

            if (LookAhead().Value == "Name")
                LookAhead().Type = Token.TokenType.Name;

            effect.Name = FieldBuilder(Token.TokenType.Name);

            if (LookAhead(false, Token.TokenType.UnKnown))
                effect.Parameters = ParamsActivationBuilder();

            Match(Token.TokenType.ClosedKey);
        }
        else
        {
            effect.Name = new Variable();
            effect.Name.Value = StatementBuilder();
            Match(Token.TokenType.Comma);
        }

        return effect;
    }
    private Selector SelectorBuilder()
    {
        Selector selector = new Selector();

        Match(Token.TokenType.Selector, Token.TokenType.Colon, Token.TokenType.OpenKey);
        selector.Source = FieldBuilder(Token.TokenType.Source);
        selector.Single = FieldBuilder(Token.TokenType.Single);
        selector.Predicate = PredicateBuilder();
        Match(Token.TokenType.ClosedKey);

        return selector;
    }
    private Predicate PredicateBuilder()
    {
        Predicate predicate = new Predicate();
        Match(Token.TokenType.Predicate, Token.TokenType.Colon, Token.TokenType.OpenParan);
        predicate.Card = MatchReturn(Token.TokenType.UnKnown);
        Match(Token.TokenType.ClosedParan, Token.TokenType.Arrow);
        predicate.Condition = StatementBuilder();

        return predicate;
    }
    #endregion

    // Binary's Expressions   
    #region
    private Expressions ExpressionBuilder()
    {
        Expressions expression = new Expressions();
        expression.Term = TermsBuilder();

        if (LookAhead(false, Token.TokenType.Plus, Token.TokenType.Minus))
        {
            expression.Opeartor = MatchReturn(Token.TokenType.Plus, Token.TokenType.Minus);
            expression.Expression = ExpressionBuilder();
        }
        return expression;
    }
    private Terms TermsBuilder()
    {
        Terms term = new Terms();
        term.Factor = FactorBuilder();

        if (LookAhead(false, Token.TokenType.Times, Token.TokenType.Divide))
        {
            term.Opeartor = MatchReturn(Token.TokenType.Times, Token.TokenType.Divide);
            term.Term = TermsBuilder();
        }
        return term;
    }
    private Factor FactorBuilder()
    {
        Factor factor = new Factor();

        if (LookAhead(false, Token.TokenType.OpenParan))
        {
            Match();
            factor.Expression = ExpressionBuilder();
            Match(Token.TokenType.ClosedParan);
        }
        else
            factor.Leaf = MatchReturn(Token.TokenType.Digit, Token.TokenType.UnKnown);

        return factor;
    }
    #endregion

    // Statement's Tree
    #region
    private Statement StatementBuilder()
    {
        Statement boolean = new Statement();
        if (LookAhead()?.Type == Token.TokenType.OpenParan)
        {
            Match(Token.TokenType.OpenParan);
            boolean.NodeLeft = SubStatementBuilder();
            Match(Token.TokenType.ClosedParan);
        }
        else
            boolean.NodeLeft = SubStatementBuilder();

        if (LookAhead(false, Token.TokenType.AND, Token.TokenType.OR))
        {
            boolean.LogOperator = MatchReturn();
            boolean.NodeRight = StatementBuilder();
        }

        return boolean;
    }
    private SubStatement SubStatementBuilder()
    {
        SubStatement subBoolean = new SubStatement();

        if (LookAhead(false, Token.TokenType.OpenParan))
        {
            Match(Token.TokenType.OpenParan);
            subBoolean.NodeLeft = MoleculeBuilder();
            Match(Token.TokenType.ClosedParan);
        }
        else
            subBoolean.NodeLeft = MoleculeBuilder();

        if (LookAhead(false, Token.TokenType.AND, Token.TokenType.OR))
        {
            subBoolean.LogOperator = MatchReturn();
            subBoolean.NodeRight = StatementBuilder();
        }

        return subBoolean;
    }
    private Molecule MoleculeBuilder()
    {
        Molecule molecule = new Molecule();

        molecule.NodeLeft = AtomBuilder();

        if (LookAhead(false, Utils.symbols.ToArray()))
        {
            molecule.ArtOpeartor = MatchReturn();
            molecule.NodeRight = AtomBuilder();
        }

        return molecule;
    }
    private Atom? AtomBuilder()
    {
        if (LookAhead(false, Token.TokenType.False, Token.TokenType.True))
            return Atom0Builder();

        else if (LookAhead(false, Token.TokenType.Quote))
            return Atom3Builder();

        else if (LookBeyond(Token.TokenType.UnKnown, Token.TokenType.Dot))
            return Atom2Builder();

        else return Atom1Builder();
    }
    private Atom0 Atom0Builder()
    {
        Atom0 atom0 = new Atom0();
        atom0.Boolean = MatchReturn(Token.TokenType.False, Token.TokenType.True);
        return atom0;
    }
    private Atom1 Atom1Builder()
    {
        Atom1 atom1 = new Atom1();
        atom1.Expression = ExpressionBuilder();

        if (LookAhead(false, Token.TokenType.PlusPlus))
            atom1.OpIncrease = MatchReturn();

        return atom1;
    }
    private Atom2 Atom2Builder()
    {
        Atom2 atom2 = new Atom2();

        do
        {
            atom2.Call?.Add(MatchReturn(Token.TokenType.UnKnown));

        } while (LookAhead(true, Token.TokenType.Dot));

        if (LookAhead(false, Token.TokenType.OpenParan))
        {
            Match(Token.TokenType.OpenParan);

            if (LookBeyond(Token.TokenType.UnKnown))
                atom2.Nested = Atom2Builder();

            Match(Token.TokenType.ClosedParan);
        }

        return atom2;
    }
    private Atom3 Atom3Builder()
    {
        Atom3 atom3 = new Atom3();
        do
        {
            Match(Token.TokenType.Quote);
            do
            {
                atom3.String?.Add(MatchReturn(Token.TokenType.UnKnown));

            } while (LookAhead(false, Token.TokenType.UnKnown));

            Match(Token.TokenType.Quote);

            if (LookAhead(false, Token.TokenType.ATAT, Token.TokenType.AT))
                atom3.String?.Add(MatchReturn());
            else
                break;
        } while (true);

        return atom3;
    }
    #endregion
}
