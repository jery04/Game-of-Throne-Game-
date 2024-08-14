using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public static class Effects
{
    public static bool baitEffect;
    public static void Increase(params object[] item)       // Poner un aumento en una fila propia
    {
        if (item[0] is Player player)
        {
            for (int j = 0; j < 3; j++)
            {
                Panels panel = player.increase[j].GetComponent<Panels>();
                if (panel.cards.Count != 0)
                {
                    for (int i = 0; i < player.field[j].GetComponent<Panels>().cards.Count; i++)
                    {
                        CardDisplay thisCard = player.field[j].GetComponent<Panels>().cards[i].GetComponent<CardDisplay>();
                        if (thisCard.card.isUnity && !thisCard.card.isHeroe)
                            thisCard.PowerDelta(panel.cards[0].GetComponent<CardDisplay>().Power());
                    }
                }
            }
        }
    }   
    public static void Climate(params object[] item)        // Afectan una o varias filas simultaneamente para ambos jugadores
    {
        if (item[0] is Player player1 && item[1] is Player player2 && item[2] is int affectedRow && item[3] is int harm)
        {
            for (int i = 0; i < player1.field[affectedRow].GetComponent<Panels>().cards.Count; i++)
            {
                CardDisplay thisCard = player1.field[affectedRow].GetComponent<Panels>().cards[i].GetComponent<CardDisplay>();
                if (thisCard.card.isUnity && !thisCard.card.isHeroe)
                    thisCard.PowerDelta(harm);
            }

            for (int i = 0; i < player2.field[affectedRow].GetComponent<Panels>().cards.Count; i++)
            {
                CardDisplay thisCard = player2.field[affectedRow].GetComponent<Panels>().cards[i].GetComponent<CardDisplay>();
                if (thisCard.card.isUnity && !thisCard.card.isHeroe)
                    thisCard.PowerDelta(harm);
            }
        }
    }   
    public static void RemoveMax(params object[] item)      // Eliminar la carta con más poder del campo
    {
        if (item[0] is Player player)
        {
            GameObject cardMax = null;
            foreach (GameObject row in player.field)
            {
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                {
                    if (thisCard.GetComponent<CardDisplay>().card.isUnity && !thisCard.GetComponent<CardDisplay>().card.isHeroe)
                    {
                        if (cardMax == null)
                            cardMax = thisCard;

                        else if (thisCard.GetComponent<CardDisplay>().Power() > cardMax.GetComponent<CardDisplay>().Power())
                            cardMax = thisCard;
                    }
                }
            }
            if (cardMax != null)
            { player.cementeryCards.Add(GameObject.Instantiate(cardMax)); GameObject.Destroy(cardMax); }
        }
    }
    public static void RemoveMin(params object[] item)      // Eliminar la carta con menos poder del rival
    {
        if (item[0] is Player player)
        {
            GameObject cardMin = null; 
            foreach (GameObject row in player.field)
            {
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                {
                    if (thisCard.GetComponent<CardDisplay>().card.isUnity && !thisCard.GetComponent<CardDisplay>().card.isHeroe)
                    {
                        if (cardMin == null)
                            cardMin = thisCard;

                        else if (thisCard.GetComponent<CardDisplay>().Power() < cardMin.GetComponent<CardDisplay>().Power())
                            cardMin = thisCard;
                    }
                }
            }
            if (cardMin != null) 
            { player.cementeryCards.Add(GameObject.Instantiate(cardMin)); GameObject.Destroy(cardMin); }
        }
    }
    public static void DrawCard(params object[] item)       // Robar una carta
    {
        if (item[0] is Player player)
            player.TakeCard(1);
    } 
    public static void MultiplyPower(params object[] item)  // Multiplica por n su ataque, siendo n la cantidad de cartas iguales a ella en el campo
    {
        if (item[0] is Player player && item[1] is CardDisplay cardEffect)
        {
            int counter = 0;
            foreach (GameObject row in player.field)  
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                    if (cardEffect.name == thisCard.name)
                        counter++;
            if (counter != 0)
                cardEffect.NewPower((counter) * cardEffect.Power());
        }
    }
    public static void ClearRow(params object[] item)       // Limpia la fila del campo (propia o del rival) con menos unidades
    {
        if (item[0] is Player player)
        {
            Panels minRow = null;
            foreach (GameObject row in player.field)
            {
                Panels panelRow = row.GetComponent<Panels>();
                if (panelRow.CounterUnity() > 0)
                    if((minRow == null) || (panelRow.CounterUnity() < minRow.CounterUnity()))
                        minRow = panelRow;
            }
            if(minRow != null)
                minRow.RemoveAll(player.cementeryCards);
        }
    }
    public static void Average(params object[] item)        // Caclula el promedio de poder entre todas las cartas del campo y lo iguala al poder de todas las cartas del campo
    {
        if (item[0] is Player player)
        {
            int counterUnity = 0;
            int sigmaUnity = 0;
            foreach (GameObject row in player.field) 
            { 
                sigmaUnity += row.GetComponent<Panels>().PowerRow(); 
                counterUnity += row.GetComponent<Panels>().CounterUnity();
            }

            foreach (GameObject row in player.field)  
                for (int i = 0; i < row.GetComponent<Panels>().cards.Count; i++)
                    row.GetComponent<Panels>().cards[i].GetComponent<CardDisplay>().NewPower(sigmaUnity/counterUnity);
        }
    }
    public static void ReturnToHand(params object[] item)   // Efecto de Señuelo
    {
        if (item[0] is Player player && player.hand.GetComponent<Panels>().cards.Count <= 10)
        {
            baitEffect = true;
            player.oneMove = false;
        }
    }
    public static void ClimateOut(params object[] item)     // Despeja una carta Clima
    {
        if (item[0] is Player player1 && item[1] is Player player2) 
        {
            if (player1.climate.GetComponent<Panels>().cards.Count != 0)
            {
                GameObject cardSelect = player1.climate.GetComponent<Panels>().cards[0];
                int affectedRow = cardSelect.GetComponent<CardDisplay>().card.affectedRow;
                int delta = cardSelect.GetComponent<CardDisplay>().Power();
                Panels panel1 = player1.field[affectedRow].GetComponent<Panels>(); 
                Panels panel2 = player2.field[affectedRow].GetComponent<Panels>();
                for (int i = 0; i < panel1.cards.Count; i++)
                {
                    CardDisplay thisCard = panel1.cards[i].GetComponent<CardDisplay>();
                    if (thisCard.card.isUnity && !thisCard.card.isHeroe)
                        thisCard.PowerDelta((-1)*delta);
                }
                for (int i = 0; i < panel2.cards.Count; i++)
                {
                    CardDisplay thisCard = panel2.cards[i].GetComponent<CardDisplay>();
                    if (thisCard.card.isUnity && !thisCard.card.isHeroe)
                        thisCard.PowerDelta((-1) * delta);
                }
                Player owner = GameManager.instance.GetPlayer(cardSelect.GetComponent<CardDisplay>().card.faction);
                owner.cementeryCards.Add(GameObject.Instantiate(cardSelect)); GameObject.Destroy(cardSelect);
            }
        }
    }  
    public static void JonSnow(params object[] item)        // Efecto del líder(Jon Snow) Aumenta en 2 el poder de las cartas de unidad en todas las filas propias.
    {
        if (item[0] is Player player)
        {
            foreach(GameObject row in player.field)
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                    if(thisCard.GetComponent<CardDisplay>().card.isUnity && !thisCard.GetComponent<CardDisplay>().card.isHeroe)
                        thisCard.GetComponent<CardDisplay>().PowerDelta(2);
        }
    }
    public static void Daenerys(params object[] item)       // Efecto del líder(Daenerys)
    {
        if (item[0] is Player player1 && item[1] is Player player2)
        {
            foreach (GameObject row in player1.field)
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                    if (thisCard.GetComponent<CardDisplay>().card.isUnity && !thisCard.GetComponent<CardDisplay>().card.isHeroe)
                        thisCard.GetComponent<CardDisplay>().PowerDelta(-2);

            foreach (GameObject row in player2.field)
                foreach (GameObject thisCard in row.GetComponent<Panels>().cards)
                    if (thisCard.GetComponent<CardDisplay>().card.isUnity && !thisCard.GetComponent<CardDisplay>().card.isHeroe )
                        thisCard.GetComponent<CardDisplay>().PowerDelta(-2);
        }
    }
    public static void NightKing(params object[] item)      // Efecto del líder(ReyNoche) Retorna a la mano la carta unidad con más poder del cementerio pertenciente a su facción
    {
        if (item[0] is Player player)
        {
            Card maxPower = null; int index = 0;
            foreach (GameObject thisCard in player.cementeryCards)
            {
                Card card = thisCard.GetComponent<CardDisplay>().card; int i = 0;
                if ((card.isUnity) && (maxPower == null || card.power > maxPower.power))
                    { maxPower = card; index = i; }
                i+=1;
            }
            if (maxPower != null)
            {
                player.TakeCard(maxPower);
                player.cementeryCards.RemoveAt(index);
            }
        }
    }
}                                                           // Esta línea (219) va dedicada a mi familia, gracias por todo.
