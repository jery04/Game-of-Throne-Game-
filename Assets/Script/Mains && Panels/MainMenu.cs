using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public static DataBase data;
    public GameObject musicManager;               // Objeto que almacena la musica de fondo y el sonido de los botones
    public Button buttonMute;                     // Botón para silenciar la música de fondo 
    public Sprite playMusicIcon;                  // Imagen de boton Play         
    public Sprite stopMusicIcon;                  // Imagen de boton Stop  
    bool sound = true;                            // Estado(Active) del sonido de fondo

    public void ActiveSound()                     // Modifica el estado(Active) del sonido de fondo
    {
        if (sound)                                // Si está sonando lo pausa                                
        {
            buttonMute.GetComponent<Image>().sprite = playMusicIcon;
            musicManager.GetComponent<AudioSource>().Stop();
            sound = false;
        }
        else                                       // Si está pausado lo reproduce                                       
        {
            buttonMute.GetComponent<Image>().sprite = stopMusicIcon;
            musicManager.GetComponent<AudioSource>().Play();
            sound = true;
        }

    }
    public void ActionEvent(string nameMethod)    // Llama a la próxima escena
    {
        Invoke(nameMethod, 0.2f);
    }
    private void GoPlay()                         // Llama a la próxima escena
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void CreateCard()
    {
        SceneManager.LoadScene("CreateCard");
    }                   // LLama a ala escena "Crear Carta"
    private void Out()                            // Cierra la aplicación
    {
        Application.Quit();
    }
    void Start()                                  // Instancia la base de datos de las Cartas
    {
        data = new DataBase();
    }
}                                                 
