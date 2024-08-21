using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LineNumberDisplay : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text lineNumbersText;
    public Scrollbar scrollbar;
    public Scrollbar otherScrollbar; // Aquí asigna el Scrollbar del otro texto que también se puede desplazar
    public static string[] code;

    private void Start()
    {
        lineNumbersText.text = "test";
        // Suscribirse al evento de cambio de texto del campo de entrada
        inputField.onValueChanged.AddListener(UpdateLineNumbers);

        // Suscribirse al evento de desplazamiento del Scrollbar del campo de entrada
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);

        // Actualizar los números de línea inicialmente
        UpdateLineNumbers(inputField.text);
    }
    private void UpdateLineNumbers(string newText)
    {
        string[] lines = newText.Split('\n');

        // Construir el texto de los números de línea
        string lineNumbers = "";
        for (int i = 1; i <= lines.Length; i++)
        {
            lineNumbers += i.ToString() + "\n";
        }
        // Actualizar el texto que muestra los números de línea
        lineNumbersText.text = lineNumbers;
         code = lines;
    }
    private void OnScrollbarValueChanged(float value)
    {
        // Si tienes otro scrollbar, ajusta su valor también
        if (otherScrollbar != null)
        {
            otherScrollbar.value = value;
        }

        // Ajustar la posición vertical del texto de los números de línea
        //lineNumbersText.rectTransform.anchoredPosition = new Vector2(0, value * (inputField.textComponent.preferredHeight - inputField.textViewport.rect.height));
    }
    public void ActionEvent(string nameMethod)    // Llama a la próxima escena
    {
        Invoke(nameMethod, 0.2f);
    }
    private void GoMain()                         // Llama a la próxima escena
    {
        SceneManager.LoadScene(0);
    }
}

