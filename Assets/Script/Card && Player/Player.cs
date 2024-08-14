using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    // Propiedades
    #region Property
    public string playerName;                              // Nombre del jugador 
    public string faction;                                 // Facción 
    public List<GameObject> deck = new List<GameObject>(); // Deck
    public int[] powerRound;                               // Puntos acumulados por rondas
    public int takeCardStartGame = 0;                      // Cantidad de cartas cambiadas antes de la batalla

    public bool myTurn;                                    // Dicta el turno del jugador
    public bool skipRound;                                 // Dicta si el jugador pasa la ronda
    public bool oneMove;                                   // Dicta si el jugador ya ha jugado una carta
    public bool iaActive;                                 // IA activada
    public Text counterDeck;                               // Cantidad de cartas en el mazo
    public Text counterCementery;                          // Cantidad de cartas en el cementerio

    // Paneles
    //public List<GameObject> cementery;
    public List<GameObject> cementeryCards;                // Cartas enviadas al cementerio
    public GameObject deckCards;
    public GameObject leader;                              // Carta líder
    public GameObject hand;                                // Cartas de la mano
    public GameObject[] field;                             // Cartas del campo(Melee-Range-Siege)
    public GameObject[] increase;                          // Cartas de aumento
    public GameObject climate;                             // Cartas clima
    public GameObject panelTakeCard;                       // Panel para robar carta antes de la batalla
    public GameObject infoTakeCard;                        // Boton-Info que indica poder robar cartas
    #endregion 

    // Métodos 
    public void IA_Play()
    {
        if (iaActive && myTurn)
        {
            GameObject card = SelectCardIA();
            if(card is not null)
            {
                StartCoroutine(Play());

                IEnumerator Play()
                {
                    yield return new WaitForSeconds(1);

                    TakeCard(card, field[0]);
                    cementeryCards.Add(Instantiate(card));
                    GameObject.Destroy(card);
                    
                    GameManager.instance.ButtonSkipTurn();

                    yield return new WaitForSeconds(1);
                }
            }
            else GameManager.instance.ButtonSkipRound();
        }
    }
    private GameObject SelectCardIA()
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
                    if (item.GetComponent<CardDisplay>().card.typeCard == Card.kind_card.increase && item.GetComponent<CardDisplay>().Power() > 1)
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
                    if (item.GetComponent<CardDisplay>().card.typeCard == Card.kind_card.climate && AmountOtherRow(item) >= 2)
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

            else
                card = hand_current[0];
        }

        return card;
    }
    public void Cementery()                               
    {
        climate.GetComponent<Panels>().RemoveAll(cementeryCards); // Envía las cartas de climate al cementerio

        foreach (GameObject item in field)                        // Envía las cartas de increase al cementerio
            item.GetComponent<Panels>().RemoveAll(cementeryCards);

        foreach (GameObject item in increase)                     // Envía las cartas Melee, Range y Siege al cementerio
            item.GetComponent<Panels>().RemoveAll(cementeryCards);
    }
    private void GeneralPower(int round)                   // Devuelve la puntuación del jugador al finalizar la ronda
    {
        int power = 0;
        foreach (GameObject item in field)
            power += item.GetComponent<Panels>().PowerRow();

        powerRound[round] = power;
    }
    private void BackImageAndDrag()                        // Modifica el estado(Active) del Script Drag e imágenes
    {
        if (!myTurn)                                                        // Si no está en juego...
        {
            foreach (GameObject item in hand.GetComponent<Panels>().cards)
            {
                item.GetComponent<CardDisplay>().backImage.enabled = true;  // Si no está jugando se activa el BackImage 
                item.GetComponent<Drag>().enabled = false;                  // Si no está jugando se desactiva el Script Drag

                if(item.GetComponent<CardDisplay>().card.isUnity)           //  Si no está jugando se desactiva el indicador de poder(carta unidad)
                    item.GetComponent<CardDisplay>().textPower.enabled = false;
            }
            foreach(GameObject item in field)                               // Desactiva el Script Drop de field
                item.GetComponent<Drop>().enabled = false;

            if(leader.GetComponent<Panels>().cards.Count != 0)
                leader.GetComponent<Panels>().cards[0].GetComponent<EventTrigger>().enabled = false;
        }
        else                                                                // De lo contrario, si está en juego...
        {
            if (!oneMove)                                                   // Si no ha hecho ningún movimiento
            {
                foreach (GameObject item in hand.GetComponent<Panels>().cards)
                {
                    item.GetComponent<CardDisplay>().backImage.enabled = false; // Si está jugando se desactiva el BackImage 
                    item.GetComponent<Drag>().enabled = true;                   // Si está jugando y no ha hecho ningún movimiento se activa el Script Drag
                    
                    if (item.GetComponent<CardDisplay>().card.isUnity)           //  Si está jugando se activa el indicador de poder(carta unidad) 
                        item.GetComponent<CardDisplay>().textPower.enabled = true;
                }
            }
            else
            {
                foreach (GameObject item in hand.GetComponent<Panels>().cards)
                {
                    item.GetComponent<CardDisplay>().backImage.enabled = false;   // Si está jugando se desactiva el BackImage 
                    item.GetComponent<Drag>().enabled = false;                    // Si está jugando y ya hizo un movimiento se desactiva el Script Drag
                }
            }
            foreach (GameObject item in field)                               // Activa el Script Drop de field
                item.GetComponent<Drop>().enabled = true;

            if(leader.GetComponent<Panels>().cards.Count != 0)
                leader.GetComponent<Panels>().cards[0].GetComponent<EventTrigger>().enabled = true;
        }
    }
    public void CreateDeckCard(List<Card> cards) 
    {
        GameObject prefarb = Resources.Load<GameObject>("Card");
        foreach (Card item in cards)
        {
            GameObject a = Instantiate(prefarb, deckCards.transform);
            a.GetComponent<EventTrigger>().enabled = false;
            a.GetComponent<CardDisplay>().card = item;
            a.name = item.name;
            deck.Add(a);
        }
    }
    private IEnumerator For(int max)                       // Cantidad de cartas que puede tomar del deck
    {
        for (int i = 0; i < max; i++)
        {
            int rand = UnityEngine.Random.Range(0, deck.Count-1);
            GameObject a = Instantiate(deck[rand], hand.transform);
            a.GetComponent<EventTrigger>().enabled = false;
            a.name = deck[rand].name;

            hand.GetComponent<Panels>().cards.Add(a);
            deck.RemoveAt(rand);

            yield return new WaitForSeconds(0.08f);
        }
    }
    public void TakeCard(int num = 0)                      // Tomar cartas del deck
    {
        int numChild = hand.GetComponent<Panels>().itemsCounter;

        if (numChild == 0)                                  // Tomar 10 iniciales  y el líder                                
        {
            TakeCard(deck[0], leader);                      // Instancia al líder
            deck.RemoveAt(0);
            StartCoroutine(For(10));                        // Instancia 10 cartas en la mano
        }                         

        else if((num != 0) && (numChild < 10))              // Toma num cartas
            StartCoroutine(For(1));

        else if (numChild < 10)                             // Tomar 2 cartas o menos
        {
            if (10 - numChild <= 2)
                StartCoroutine(For(10 - numChild));
            else
                StartCoroutine(For(2));
        }
    }
    public void TakeCard(GameObject card, GameObject panel)
    {
        GameObject newCard = Instantiate(card, panel.transform); 
        newCard.name = card.name;
        leader.GetComponent<Panels>().cards.Add(newCard);
    }   // Replica un GameObject determinada en un panel determinado
    public void TakeCard(Card card, GameObject panel = null) // Crea una carta determinada en un panel determinado
    {
        GameObject newCard;
        if (panel == null)
            newCard = Instantiate(Resources.Load<GameObject>("Card"), hand.transform);
        else
            newCard = Instantiate(Resources.Load<GameObject>("Card"), panel.transform);

        newCard.GetComponent<EventTrigger>().enabled = false;
        newCard.GetComponent<CardDisplay>().card = card;
        newCard.name = card.name;

        if(panel == null)
            hand.GetComponent<Panels>().cards.Add(newCard);
        else panel.GetComponent<Panels>().cards.Add(newCard);
    }
    public void ButtonInfoTakeCard()                       // Modifica la visibilidad del botón Info
    {
        if(GameManager.round == 0 && myTurn && takeCardStartGame < 2)
            infoTakeCard.SetActive(true);
        else 
            infoTakeCard.SetActive(false);
    }           
    public void Active(bool active)                        // Modifica el estado(Active) del componente EventTrigger
    {
        for (int i = 0; i < hand.GetComponent<Panels>().itemsCounter; i++)
        {
            hand.GetComponent<Panels>().cards[i].GetComponent<EventTrigger>().enabled = active;
        }
    }
    public void ButtonTrigger(bool active)                 // Botón(Yes) tomar cartas antes de la batalla
    {
        Active(active);
        panelTakeCard.SetActive(false);
        takeCardStartGame += 1;
    }
    public void ButtonNot()                                // Botón(No) tomar cartas antes de la batalla
    {
        takeCardStartGame = 2;
        panelTakeCard.SetActive(false);
    }
    public void ActivePanelTakeCard()                      // Muestra el panel DrawCard
    {
        panelTakeCard.SetActive(true);
    }

    void Start()
    {
        powerRound = new int[3] { 0, 0, 0 };              // Inicializa la puntuación de las ronndas en cero
    }
    public void Update()
    {
        ButtonInfoTakeCard();
        GeneralPower(GameManager.round);                  // Actualiza el poder
        counterDeck.text = deck.Count.ToString();         // Actualiza la cantidad de cartas en el mazo
        counterCementery.text = cementeryCards.Count.ToString(); // Actualiza las cantidad de cartas en el cementerio
        BackImageAndDrag();                               // Actualiza el Método
        if (oneMove) takeCardStartGame = 2;               // Si juega una carta se desactiva la opcion DrawCard al inicio
    }
}
