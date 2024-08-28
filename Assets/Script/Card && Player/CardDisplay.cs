using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static Card;

public class CardDisplay : MonoBehaviour  
{  
    public Card card;                               // Almacena la carta    
    public Text power;                              // El poder (int)
    public Image artWork;                           // La imagen de la carta
    public Image portrait;                          // La imagen del marco
    public Image backImage;                         // La parte posterior de la imagen
    public kind_card type_Card;                     // El tipo de carta
    public card_position cardPosition;              // El tipo de carta

    // PropertyCompiler
    public new string name;
    public string faction;
    public string owner;

    public int Power() => int.Parse(power.text);                                                     // Retorna su poder
    public void NewPower(int delta) => power.text = delta.ToString();                                // Coloca un nuevo poder
    public void PowerDelta(int delta) => power.text = (int.Parse(power.text) + delta).ToString();    // Variar su poder (Aumentar-Disminuir)
    void Start()                                    // Inicializa propiedades
    {
        if(card != null)
        {
            this.name = card.name;
            this.faction = card.faction;
            this.type_Card = card.type;
            this.cardPosition = card.position;
            this.power.text = card.power.ToString();
            this.artWork.sprite = card.artWork;
            this.portrait.sprite = card.portrait;
            this.power.enabled = false;
        }
    }
    void Update()                                   // Actualiza propiedades
    {
        if (card != null)                           // Actualiza el poder
            power.text = power.text;    
    }
}
