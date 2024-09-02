using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card: ScriptableObject
{
    // Propiedades (Campo) 
    public new string name;                                                             // Nombre de la carta
    public string faction;                                                              // Facción de la carta
    public int power;                                                                   // Poder(unidad), daño(clima) o incremento(aumento)
    public string description;                                                          // Descripción de la carta
    public Sprite artWork;                                                              // Imagen principal
    public Sprite portrait;                                                             // Imagen del marco
    public enum card_position { M, R, S, MR, MS, RS, MRS, I, C, L};                     // Posiciones en que se puede ubicar
    public enum kind_card { golden, silver, climate, clear, bait, increase, leader };   // Tipos de carta
    public kind_card type;                                                              // Tipo de carta
    public card_position position;                                                      // Tipo de posición
    public bool isUnity;                                                                // Es carta unidad?
    public bool isHeroe;                                                                // Es carta héroe?
    public delegate void EffectDelegate(params object[] item);
    public EffectDelegate effect;                                                       // Delegado que almacena el efecto(Método)
    private AudioClip clip;                                                             // Audio de las cartas al colocarse
    public int affectedRow;                                                             // Fila que afectan las cartas climas

    // Constructores (Sobrecargado) 
    public Card() { }
    public Card(string name, string faction, int power, bool IsUnity, bool IsHeroe, Sprite artWork, Sprite portrait, kind_card typeCard, card_position cardPosition, string description, EffectDelegate effect, AudioClip clip = null)
    {
        this.name = name;
        this.faction = faction;
        this.power = power;
        this.isUnity = IsUnity;
        this.isHeroe = IsHeroe;
        this.artWork = artWork;
        this.portrait = portrait;
        this.type = typeCard;
        this.position = cardPosition;
        this.description = description;
        this.effect = effect;
        this.clip = clip;
    }
    public Card(string name, string faction, int power, int affectedRow, bool IsUnity, bool IsHeroe, Sprite artWork, Sprite portrait, kind_card typeCard, card_position cardPosition, string description, EffectDelegate effect, AudioClip clip = null)
    {
        this.name = name;
        this.faction = faction;
        this.power = power;
        this.affectedRow = affectedRow;
        this.isUnity = IsUnity;
        this.isHeroe = IsHeroe;
        this.artWork = artWork;
        this.portrait = portrait;
        this.type = typeCard;
        this.position = cardPosition;
        this.description = description;
        this.effect = effect;
        this.clip = clip;
    }
    public Card(string name, string faction, int power, kind_card typeCard, card_position cardPosition)
    {
        this.name = name;
        this.faction = faction;
        this.power = power;
        this.type = typeCard;
        this.position = cardPosition;
    }

    // Métodos
    public void ActiveClip()                                                            // Activa el AudioClip de la carta (En caso de que contenga)
    {
        if(clip != null)
        {
            AudioSource audioEffect = GameObject.Find("MusicCards").GetComponent<AudioSource>();
            audioEffect.clip = this.clip;
            audioEffect.Play();
        }
    }
}          // Cartas
public class CardClimate: Card
{
    public int harm;
    public int rowAfected;
    public CardClimate() { }
}   // Cartas Climas
public class CardIncrease: Card
{
    public int bonus;
    public CardIncrease() { }

}  // Cartas Aumento
public class CardCompiler: Card                                                        // Cartas creadas por el Compilador
{
    // Property
    public new OnActivation effect { get; private set; }                               // Effectos otorgados a la carta
    private IScope scope { get; set; }                                                 // Alcance de variables

    // Builder 
    public CardCompiler(string name, string faction, int power, kind_card type, card_position position, OnActivation effects, IScope scope)
        : base(name, faction, power, type, position)
    {
        this.effect = effects;
        this.scope = scope;
        this.isHeroe = IsHeroe(type);
        this.isUnity = IsUnity(type);
        artWork = ArtWork();
        portrait = Portrait(type);
        this.description = Description_Maker(scope);
        
    }

    // Methods
    public void Active_Effect()
    {
        effect.Evaluate();
    }                                                     // Activar los efectos correspondientes 
    public Sprite ArtWork()
    {
        bool[] select = new bool[32];
        string[] path = new string[32] 
        { "cc1", "cc2", "cc3", "cc4", "cc5", "cc6","cc7", "cc8", "cc9","cc10", "cc11", "cc12", "cc13", "cc14", "cc15","cc16", "cc17", "cc18","cc19", "cc20", "cc21", "cc22", "cc23", "cc24", "cc25", "cc26", "cc27","cc28", "cc29", "cc30","cc31", "cc32" };

        System.Random random = new System.Random();
        int rand_num;

        do
        {
            rand_num = random.Next(0, 32);
        } while (select[rand_num]);
        select[rand_num] = true;

        return Resources.Load<Sprite>(path[rand_num]);
    }                                                         // Selecciona una Imagen para la carta
    private string Description_Maker(IScope scope)
    {
        string description = $"{this.name} it's a card made in a compiler. ";
        List<string> effect_names = GetEffectNames(effect, scope);

        if (effect_names.Count == 0)
            description += "It doesn't contain effects.";
        else
        {
            description += $@"Contains {effect_names.Count} effect(s) called";
            for(int i = 0; i < effect_names.Count; i++)
            {
                if(i != 0)
                {
                    if (i == effect_names.Count - 1)
                        description += $@" y";
                    else
                        description += $@",";
                }

                description += @$" ""{effect_names[i]}""";
            }
            description += ".";
        }

        return description;
    }                                  // Crea una descripción para la carta                                                                                                                    
    private List<string> GetEffectNames(OnActivation effects, IScope scope)
    {
        List<string> effect_list = new List<string>(); 

        if(effects is not null)
        {
            foreach (OnActivationBody body in effects.Body) 
            {
                if(body is not null)
                {
                    effect_list.Add(Convert.ToString(body.EffectActivation.Name.Evaluate(scope)));

                    if(body.PosAction is not null)
                        foreach (PosAction pos_action in body.PosAction)
                            effect_list.Add(Convert.ToString(pos_action.Name.Evaluate(scope)));
                }
            }
        }

        return effect_list;
    }         // Retorna los nombres de los efectos asignados a la carta
    private static Sprite Portrait(kind_card typeCard)
    {
        if (typeCard == Card.kind_card.golden)
            return Resources.Load<Sprite>("golden");

        else if (typeCard == Card.kind_card.silver)
            return Resources.Load<Sprite>("silver");

        return Resources.Load<Sprite>("emerald");
    }                              // Designa el marco(Sprite) de la carta
    private static bool IsHeroe(kind_card typeCard)
    {
        if (typeCard == Card.kind_card.golden)
            return true;

        return false;
    }                                 // Identifica si es Héroe
    private static bool IsUnity(kind_card typeCard)
    {
        if (typeCard == Card.kind_card.golden || typeCard == Card.kind_card.silver)
            return true;

        return false;
    }                                 // Identifica si es Unidad
}  // Cartas de Compilador
