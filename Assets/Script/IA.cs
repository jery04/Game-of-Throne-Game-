using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class IA : MonoBehaviour
{   
    // Property 
    public Player Player {  get; set; }                     // Jugador con la IA activada

    // Builders
    public IA() { }
    public IA(Player player)
    {
        this.Player = player;
    }

    // Methods
    public void Play()
    {
        GameObject card = CardSelect(Player.hand);

        if (card is not null)
        {
            // Active Audio Clip
            ActiveClip();

            // Active Card
            ActiveCard(card);

            // Active Effect
            EffectActive(card);

            // Destroy Hand's Card
            GameObject.Destroy(card);

            GameManager.instance.ButtonSkipTurn();
        }
        else
            GameManager.instance.ButtonSkipRound();
    }                                   // Jugar por cada turno
    private void ActiveCard(GameObject card)
    {
        card.GetComponent<CardDisplay>().backImage.enabled = false;
        Player.TakeCard(card, GetPosition(card));
    }             // Activar la carta en el campo
    private void EffectActive(GameObject card)
    {
        CardDisplay displayCard = card.GetComponent<CardDisplay>();

        if (displayCard.card is CardCompiler compiler_card) // Si es de tipo CardCompiler
            compiler_card.Active_Effect();

        else if (displayCard.card.effect != null)            // Si es de tipo Card
            Drop.ActiveEffect(displayCard);
    }           // Activar el efecto correspondiente
    private void ActiveClip()
    {
        // Clip IA's Path
        string[] clipPath = { "01IA", "02IA", "03IA", "04IA", "05IA", "06IA", "07IA", "08IA" };

        AudioSource audioEffect = GameObject.Find("MusicCards").GetComponent<AudioSource>();
        audioEffect.clip = Resources.Load<AudioClip>($"Audios/{clipPath[UnityEngine.Random.Range(0, 9)]}");
        audioEffect.Play();
    }                            // Reproducir el audio del jugador artificial
    private  GameObject CardSelect(GameObject hand)
    {
        GameObject card = null;
        List<GameObject> hand_current = hand.GetComponent<Panels>().cards;

        if (hand_current.Count > 0)
        {
            // Carta héroe con mayor poder
            foreach (GameObject item in hand_current)
            {
                CardDisplay card_selected = item.GetComponent<CardDisplay>();
                if (card_selected.card.isHeroe && card_selected.Power() > 5)
                {
                    if (card == null)
                        card = item;

                    else if (card_selected.Power() > card.GetComponent<CardDisplay>().Power())
                        card = item;
                }
            }

            // Carta de Aumento
            if (card is null)
            {
                foreach (GameObject item in hand_current) 
                {
                    CardDisplay card_selected = item.GetComponent<CardDisplay>();
                    if (card_selected.card.type == Card.kind_card.increase && card_selected.Power() > 1) 
                    {
                        if (card == null)
                            card = item;

                        else if (card_selected.Power() > card.GetComponent<CardDisplay>().Power())
                            card = item;
                    }
                }
            }

            // Carta Clima
            if (card is null)
            {
                int damage = 0;
                foreach (GameObject item in hand_current)
                {
                    CardDisplay card_selected = item.GetComponent<CardDisplay>();
                    if (card_selected.card.type == Card.kind_card.climate && AmountOtherRow(item) >= 2)
                    {
                        if (card == null)
                            card = item;

                        else if (card_selected.Power() < damage)
                            card = item;
                    }
                }

                int AmountOtherRow(GameObject climate)
                {
                    GameObject[] otherField = GameManager.instance.PlayerNotCurrent().field;
                    Panels panel = otherField[climate.GetComponent<CardDisplay>().card.affectedRow].GetComponent<Panels>();
                    return panel.CounterSilver();
                }
            }

            // Carta Unidad
            if (card is null)
            {
                foreach (GameObject item in hand_current)
                {
                    CardDisplay card_selected = item.GetComponent<CardDisplay>();
                    if (card_selected.card.isUnity && card_selected.Power() > 2)
                    {
                        if (card == null)
                            card = item;

                        else if (card_selected.Power() > card.GetComponent<CardDisplay>().Power())
                            card = item;
                    }
                }
            }
        }

        return card;
    }      // Seleccionar la mejor jugada (carta)
    private GameObject GetPosition(GameObject card)
    {
        // Clima
        if (Player.climate.GetComponent<Panels>().PutCard() && card.GetComponent<CardDisplay>().type_Card == Card.kind_card.climate)
            return Player.climate;

        // Field
        foreach (GameObject place in Player.field)
            if (Drop.CardPosition(place.GetComponent<Drop>(), card) && place.GetComponent<Panels>().PutCard())
                return place;

        // Increase
        for(int i = 0; i < 3; i++)
        {
            GameObject panel = Player.increase[i];
            int amount_field = Player.field[i].GetComponent<Panels>().itemsCounter;

            if (Drop.CardPosition(panel.GetComponent<Drop>(), card) && panel.GetComponent<Panels>().PutCard() && amount_field > 0)
                return panel;
        }

        return null;
    }      // Posición dónde se jugará la carta
    public void MoveCtrl(Player player1, Player player2)
    {
        if (player1.iaActive)
            { player1.iaActive = false; Player = null; }
        else
        {
            if (!player1.myTurn)
                player1.iaActive = !player1.iaActive;
            else
            {
                player1.myTurn = false; player2.myTurn = true;
                GameManager.currentPlayer = player2;
            }
            this.Player = player1;
        }
    } // Ajustar la IA al jugador seleccionado (Lógica)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
