using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Program (Beginning)
#nullable enable
public class ProgramCompiler : ISemantic
{
    // Property
    public List<EffectBlock>? Effect { get; set; }      // Lista de Bloques de Efectos
    public List<CardBlock>? Card { get; set; }          // Lista de Bloques de Cartas

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
            foreach (CardBlock card in Card)           // Evalúa los bloques cartas (Crea las cartas y las agrega al mazo)
                DataBase.deckCompiler.Add(card.Evaluate(scope));
    }              // Evalúa todos los bloques de cartas y efectos
    public bool CheckSemantic(IScope scope)
    {
        bool check = true;

        if (Effect != null)                            // Análisis semántico de los bloques efectos
            foreach (EffectBlock effect in Effect)
                if (!effect.CheckSemantic(scope))
                    check = false;

        if (Card != null)                              // Análisis semántico de los bloques cartas
            foreach (CardBlock card in Card)
                if (!card.CheckSemantic(scope))
                    check = false;

        return check;
    }         // Chequea semántico todos los bloques de cartas y efectos
}
