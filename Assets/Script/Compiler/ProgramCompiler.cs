using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Program (Beginning)
#nullable enable
public class ProgramCompiler : ISemantic
{
    // Property
    public List<EffectBlock>? Effect { get; set; }
    public List<CardBlock>? Card { get; set; }

    // Builder
    public ProgramCompiler()
    {
        this.Effect = new List<EffectBlock>();
        this.Card = new List<CardBlock>();
    }

    // Methods
    public void Evaluate(IScope scope)
    {
        if (Card != null)
            foreach (CardBlock card in Card)
                DataBase.deckCompiler.Add(card.Evaluate(scope));
    }
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Effect != null)
            foreach (EffectBlock effect in Effect)
                if (!effect.CheckSemantic(scope))
                    check = false;

        if (Card != null)
            foreach (CardBlock card in Card)
                if (!card.CheckSemantic(scope))
                    check = false;

        return check;
    }
}
