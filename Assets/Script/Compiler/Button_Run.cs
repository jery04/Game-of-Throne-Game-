using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Run : MonoBehaviour
{
    // Properties
    public GameObject Error_Warns;                    // Text_Comunicado (Error o Listo)
    public GameObject InfErrors_Button;               // Botón para mostrar errores
    public GameObject Errors_Panel;                   // Panel de errores
    
    // Methods
    public void Run()
    {
        // Resetear 
        ResetEverything();
        Warn_Active(false);

        // Campo
        string[] code = LineNumberDisplay.code;       // Líneas de códigos
        IScope scope = new Scope();                   // Alcance  
        Lexer lexer = new Lexer(code);                // Léxico
        Parser parser = new Parser(lexer.GetLexer()); // Analizador sintáctico
        ProgramCompiler program;                      // Program_Clase_Madre

        // Lógica de inicio
        if (code.Length == 1 && code[0] == "")
        {
            Utils.errors.Add("No se ha escrito ninguna sentencia de código");
            Warn_Active(true);
        }
        else
        {
            program = parser.Parse();

            if (Utils.NotError)
            {
                if (program.CheckSemantic(scope))
                {
                    RunAndSave();
                    Utils.program = program;
                    MainMenu.data.CreateCardsCompiler(program, scope);
                }
                else
                    Warn_Active(true);
            }
            else
                Warn_Active(true);
        }
    }                             // Chequear y Evaluar  
    private void RunAndSave()
    {
        Error_Warns.GetComponent<Text>().text = "Run   And  Save";
        StartCoroutine(WaitAndPrintMessage());

        IEnumerator WaitAndPrintMessage()
        {
            Error_Warns.SetActive(true); InfErrors_Button.SetActive(true);
            InfErrors_Button.GetComponent<Button>().interactable = false;

            yield return new WaitForSeconds(3);     // Espera 3 segundos (mostrando el aviso)

            InfErrors_Button.GetComponent<Button>().interactable = true;
            Error_Warns.SetActive(false); InfErrors_Button.SetActive(false);

            // Mueve la modificación del texto aquí, después de que el objeto se desactive
            Error_Warns.GetComponent<Text>().text = "Errors  have   been   detected";
        }
    }                     // Activa el Comunicado 
    private void ResetEverything()
    {
        Utils.Reset();
        Errors_Panel.transform.GetChild(1).GetComponent<Text>().text = "";
    }                // Resetea valores
    private void Warn_Active(bool active)
    {
        InfErrors_Button.SetActive(active);
        Error_Warns.SetActive(active);

        if (active)
            for (int i = 0; i < Utils.errors.Count; i++)
                Errors_Panel.transform.GetChild(1).GetComponent<Text>().text += $"{i + 1}) " + Utils.errors[i] + "\n";
    }         // Muestra los errores
    public void Button_ErrorsWarn()
    {
        Errors_Panel.SetActive(true);
    }               // Activa el panel de errores
    public void Button_BackCompiler()
    {
        Errors_Panel.SetActive(false);
    }             // Desactiva el panel de errores

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
