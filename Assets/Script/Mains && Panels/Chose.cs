using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Chose : MonoBehaviour
{
    public Button[] button;                                                      // Arrays de botones(Mazos a elegir)                                                         
    public DataBase data;                                                        // Variable que gestiona los mazos
    public InputField input1, input2;                                            // Entradas de los nombres de los jugadores
    public static List<Card> deck1, deck2;                                       // Almacenarán las cartas de los mazos selecionados
    public static string name1, name2, faction1, faction2;                       // Almacena los nombres de los jugadores
    public GameObject warn_Panel;

    public void ActionEvent(string nameMethod)                                   // Llama a la próxima escena
    {
        if (deck1 != null && deck2 != null)                                      // Verifica que los mazos estén seleccionados
        {
            Invoke(nameMethod, 0.2f);
        }
    }
    public void ButtonGoBack() => Invoke("GoBack", 0.2f);                        // Volver al Menú Principal
    private void GoBack() => SceneManager.LoadScene(0);                          // Cambia de escena (menú principal)
    private void GoPlay()                                                        // Llama a la próxima escena
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ButtonStark(bool key)                                            // Inicializa un mazo de cartas Stark
    {
        if (key && input1.text != "Name" && input1.text != "")
        {
            name1 = input1.text;
            faction1 = "Stark";
            deck1 = data.deckStark;
            button[1].GetComponent<Button>().interactable = false;
            button[2].GetComponent<Button>().interactable = false;
            button[3].GetComponent<Button>().interactable = false;
            button[6].GetComponent<Button>().interactable = false;
        }
        else if (input2.text != "Name" && input2.text != "")
        {
            name2 = input2.text;
            faction2 = "Stark";
            deck2 = data.deckStark;
            button[4].GetComponent<Button>().interactable = false;
            button[5].GetComponent<Button>().interactable = false;
            button[0].GetComponent<Button>().interactable = false;
            button[7].GetComponent<Button>().interactable = false;
        }
    }
    public void ButtonTargaryen(bool key)                                        // Inicializa un mazo de cartas Targaryen
    {
        if (key && input1.text != "Name" && input1.text != "")
        {
            name1 = input1.text;
            faction1 = "Targaryen";
            deck1 = data.deckTargaryen;
            button[0].GetComponent<Button>().interactable = false;
            button[2].GetComponent<Button>().interactable = false;
            button[4].GetComponent<Button>().interactable = false;
            button[6].GetComponent<Button>().interactable = false;
        }
        else if (input2.text != "Name" && input2.text != "")
        {
            name2 = input2.text;
            faction2 = "Targaryen";
            deck2 = data.deckTargaryen;
            button[3].GetComponent<Button>().interactable = false;
            button[5].GetComponent<Button>().interactable = false;
            button[1].GetComponent<Button>().interactable = false;
            button[7].GetComponent<Button>().interactable = false;
        }

    }
    public void ButtonDead(bool key)                                             // Inicializa un mazo de cartas Dead
    {
        if (key && input1.text != "Name" && input1.text != "")
        {
            name1 = input1.text;
            faction1 = "Dead";
            deck1 = data.deckDead;
            button[0].GetComponent<Button>().interactable = false;
            button[1].GetComponent<Button>().interactable = false;
            button[5].GetComponent<Button>().interactable = false;
            button[6].GetComponent<Button>().interactable = false;
        }
        else if (input2.text != "Name" && input2.text != "")
        {
            name2 = input2.text;
            faction2 = "Dead";
            deck2 = data.deckDead;
            button[3].GetComponent<Button>().interactable = false;
            button[4].GetComponent<Button>().interactable = false;
            button[2].GetComponent<Button>().interactable = false;
            button[7].GetComponent<Button>().interactable = false;
        }
    }
    public void ButtonCompiler(bool key)                                         // Inicializa un mazo de cartas Compiler
    {
        if (DataBase.deckCompiler.Count >= 10)
        {
            if (key && input1.text != "Name" && input1.text != "")
            {
                name1 = input1.text;
                deck1 = DataBase.deckCompiler; FindLeader(deck1);
                faction1 = deck1[1].faction;
                button[0].GetComponent<Button>().interactable = false;
                button[1].GetComponent<Button>().interactable = false;
                button[2].GetComponent<Button>().interactable = false;
                button[7].GetComponent<Button>().interactable = false;
            }
            else if (input2.text != "Name" && input2.text != "")
            {
                name2 = input2.text;
                deck2 = DataBase.deckCompiler; FindLeader(deck2);
                faction2 = deck2[1].faction;
                button[3].GetComponent<Button>().interactable = false;
                button[4].GetComponent<Button>().interactable = false;
                button[5].GetComponent<Button>().interactable = false;
                button[6].GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            StartCoroutine(WaitAndPrintMessage());

            IEnumerator WaitAndPrintMessage()
            {
                warn_Panel.SetActive(true);
                yield return new WaitForSeconds(3);     // Espera 3 segundos (mostrando el aviso)
                warn_Panel.SetActive(false);
            }
        }
    }
    private void FindLeader(List<Card> deck)
    {
        for (int i = 0; i < deck.Count; i++)
            if (deck[i].type == Card.kind_card.leader)
                Swap(0, i, deck);
    }                                  // Ajuste final (Encuentran la carta líder)
    private void Swap(int i, int j, List<Card> deck)
    {
        Card temp = deck[i];
        deck[i] = deck[j];
        deck[j] = temp;
    }                          // Ubica el líder en la posición 0

    void Start()                                                                 // Instancia la base de datos de las Cartas
    {
        data = MainMenu.data;
    }
    void Update()
    {

    }
}
