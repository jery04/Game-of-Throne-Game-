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
    public Player Player {  get; set; }

    // Builder
    public IA() { }

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
    }
    private void ActiveCard(GameObject card)
    {
        card.GetComponent<CardDisplay>().backImage.enabled = false;
        Player.TakeCard(card, Player.field[0]);
    }
    private void EffectActive(GameObject card)
    {
        CardDisplay displayCard = card.GetComponent<CardDisplay>();

        if (displayCard.card is CardCompiler compiler_card) // Si es de tipo CardCompiler
            compiler_card.Active_Effect();

        else if (displayCard.card.effect != null)            // Si es de tipo Card
            Drop.ActiveEffect(displayCard);
    }
    private void ActiveClip()
    {
        // Clip IA's Path
        string[] clipPath = { "01IA", "02IA", "03IA", "04IA", "05IA", "06IA", "07IA", "08IA" };

        AudioSource audioEffect = GameObject.Find("MusicCards").GetComponent<AudioSource>();
        audioEffect.clip = Resources.Load<AudioClip>($"Audios/{clipPath[UnityEngine.Random.Range(0, 9)]}");
        audioEffect.Play();
    }
    private  GameObject CardSelect(GameObject hand)
    {
        GameObject card = null;
        List<GameObject> hand_current = hand.GetComponent<Panels>().cards;

        if (hand_current.Count > 0)
        {
            // Carta héroe con mayor poder
            foreach (GameObject item in hand_current)
            {
                if (item.GetComponent<CardDisplay>().card.isHeroe && item.GetComponent<CardDisplay>().Power() > 5)
                {
                    if (card == null)
                        card = item;

                    else if (item.GetComponent<CardDisplay>().Power() > card.GetComponent<CardDisplay>().Power())
                        card = item;
                }
            }

            // Carta de Aumento
            if (card is null)
            {
                foreach (GameObject item in hand_current)
                {
                    if (item.GetComponent<CardDisplay>().card.type == Card.kind_card.increase && item.GetComponent<CardDisplay>().Power() > 1)
                    {
                        if (card == null)
                            card = item;

                        else if (item.GetComponent<CardDisplay>().Power() > card.GetComponent<CardDisplay>().Power())
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
                    if (item.GetComponent<CardDisplay>().card.type == Card.kind_card.climate && AmountOtherRow(item) >= 2)
                    {
                        if (card == null)
                            card = item;

                        else if (item.GetComponent<CardDisplay>().Power() < damage)
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
                    if (item.GetComponent<CardDisplay>().card.isUnity && item.GetComponent<CardDisplay>().Power() > 2)
                    {
                        if (card == null)
                            card = item;

                        else if (item.GetComponent<CardDisplay>().Power() > card.GetComponent<CardDisplay>().Power())
                            card = item;
                    }
                }
            }
        }

        return card;
    }
    private GameObject GetPosition(GameObject card)
    {
        throw new NotImplementedException();
    }
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
