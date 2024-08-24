using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public class DropCard : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private string Position(Card.card_position pos)                 // Retorna cadena string con la posición de la carta
    {
        switch (pos)
        {
            case Card.card_position.M:
                return "Melee";
            case Card.card_position.R:
                return "Range";
            case Card.card_position.S:
                return "Siege";
            case Card.card_position.MR:
                return "Melee-Range";
            case Card.card_position.MS:
                return "Melee-Siege";
            case Card.card_position.MRS:
                return "Mel-Ran-Sie";
            case Card.card_position.RS:
                return "Range-Siege";
            default:
                return "";
        }
    }
    private string KindCard(Card.kind_card kind)                    // Retorna cadena string con el tipo de carta
    {
        switch (kind)
        {
            case Card.kind_card.golden:
                return "Golden";
            case Card.kind_card.silver:
                return "Silver";
            case Card.kind_card.climate:
                return "Climate";
            case Card.kind_card.clear:
                return "Clear";
            case Card.kind_card.bait:
                return "Bait";
            case Card.kind_card.increase:
                return "Increase";
            case Card.kind_card.leader:
                return "Leader";
        }
        return "";
    }
    public void OnPointerEnter(PointerEventData eventData)          // Se ejecuta cuando el puntero entra en el área del objeto
    {
        // Enviar la información de la carta el Panel de Datos
        if (this != null && !this.GetComponent<CardDisplay>().backImage.enabled)
        {
            GameObject.Find("Panel_Card").transform.GetChild(3).GetComponent<Text>().text = this.name;                                                // Nombre
            GameObject.Find("Panel_Card").transform.GetChild(5).GetComponent<Text>().text = this.GetComponent<CardDisplay>().card.faction;            // Facción
            GameObject.Find("Panel_Card").transform.GetChild(7).GetComponent<Text>().text = this.GetComponent<CardDisplay>().power.text;          // Poder
            GameObject.Find("Panel_Card").transform.GetChild(9).GetComponent<Text>().text = KindCard(this.GetComponent<CardDisplay>().type_Card);     // Tipo de carta
            GameObject.Find("Panel_Card").transform.GetChild(11).GetComponent<Text>().text = Position(this.GetComponent<CardDisplay>().cardPosition); // Posición
            GameObject.Find("Panel_Card").transform.GetChild(13).GetComponent<Text>().text = this.GetComponent<CardDisplay>().card.description;       // Descripción
            GameObject.Find("Panel_Card").transform.GetChild(0).GetComponent<Image>().sprite = this.GetComponent<CardDisplay>().artWork.sprite;
            GameObject.Find("Panel_Card").transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = this.GetComponent<CardDisplay>().portrait.sprite;
            
            if (this.GetComponent<CardDisplay>().type_Card == Card.kind_card.climate)                                                                                                                         
            {
                GameObject.Find("Panel_Card").transform.GetChild(6).GetComponent<Text>().text = "Harm:";
                GameObject.Find("Panel_Card").transform.GetChild(7).GetComponent<Text>().text += $"  R:{this.GetComponent<CardDisplay>().card.affectedRow+1}";
            }                     // Si es clima imprime "Harm"/"Damage"
            else if (this.GetComponent<CardDisplay>().type_Card == Card.kind_card.increase)                  // Si es aumento imprime "Bonus"
                GameObject.Find("Panel_Card").transform.GetChild(6).GetComponent<Text>().text = "Bonus:";
            else GameObject.Find("Panel_Card").transform.GetChild(6).GetComponent<Text>().text = "Power:";   // De lo contrario "Power"
        }
    }
    public void OnPointerExit(PointerEventData eventData)           // Se ejecuta cuando el puntero sale del área del objeto
    {
        if (this != null)                                           // Limpiar(vaciar datos) el Panel de Datos
        {
             GameObject.Find("Panel_Card").transform.GetChild(3).GetComponent<Text>().text = "";        // Nombre
             GameObject.Find("Panel_Card").transform.GetChild(6).GetComponent<Text>().text = "Power:";  // Poder(Text)
             GameObject.Find("Panel_Card").transform.GetChild(7).GetComponent<Text>().text = "";        // Poder
             GameObject.Find("Panel_Card").transform.GetChild(5).GetComponent<Text>().text = "";        // Facción
             GameObject.Find("Panel_Card").transform.GetChild(9).GetComponent<Text>().text = "";        // Tipo de carta
             GameObject.Find("Panel_Card").transform.GetChild(11).GetComponent<Text>().text = "";       // Posición
             GameObject.Find("Panel_Card").transform.GetChild(13).GetComponent<Text>().text = "";       // Descripción
             GameObject.Find("Panel_Card").transform.GetChild(0).GetComponent<Image>().sprite = this.GetComponent<CardDisplay>().artWork.sprite;
             GameObject.Find("Panel_Card").transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Panel");
        }
    }
    public void OnDrop(PointerEventData eventData)                  // Se ejecuta cuando un objeto es soltado sobre el objeto asociado a este script
    {
        if (Effects.baitEffect) 
        {
            Drag item = eventData.pointerDrag.GetComponent<Drag>();                         // Almacena el Componente Darg de la carta
            GameObject panel = this.gameObject.transform.parent.gameObject;                 // Almacena el panel padre del objeto de este componente
            CardDisplay cardDrag = eventData.pointerDrag.GetComponent<CardDisplay>();

            // Si el objeto tiene un componente Drag, tiene poder 0, cabe en la fila y pertenece al panel establece el padre del objeto arrastrado al objeto actual
            if (item != null && !cardDrag.card.isUnity && panel.GetComponent<Panels>().itemsCounter <= panel.GetComponent<Panels>().maxItems && Drop.CardPosition(panel.GetComponent<Drop>(), eventData.pointerDrag))
            {   
                //Añade la carta nueva
                item.parent = panel.transform;                                           
                GameManager.currentPlayer.hand.GetComponent<Panels>().cards.Remove(eventData.pointerDrag); 
                panel.GetComponent<Panels>().cards.Add(eventData.pointerDrag);           

                //Devuelve la carta a la mano
                Card thisCard = this.gameObject.GetComponent<CardDisplay>().card;
                Destroy(this.gameObject);
                GameManager.currentPlayer.TakeCard(thisCard);

                GameManager.currentPlayer.oneMove = true;                                   // Indica que el jugador ha realizado un movimiento
                cardDrag.card.ActiveClip();                                                 // Activa el AudioClip de la carta 

                if (eventData.pointerDrag.GetComponent<CardDisplay>().card.effect != null)
                    Drop.ActiveEffect(cardDrag);                                            // Activa el efecto de la carta
                Effects.baitEffect = false;
            }
        }
    }
}
