using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public interface IParsing
{
    ProgramCompiler Parse();
}                                                // Interface para analizar sintaxis
public interface ISemantic
{
    public bool CheckSemantic(IScope scope);
}                                               // Interface para analizar semántica
public interface IScope
{
    // Properties
    public IScope? Parent { get; set; }                                     // Padre de este objeto en el árbol de Scope
    public Dictionary<string, GeneralStatement?> Defined { get; set; }      // Variables almacenadas (nombre y valores)

    // Methods
    public bool IsDefined(string? search);                                  // Verifica si la variable está almacenada
    public void Define(Variable variable);                                  // Agrega la variable
    public IScope CreateChild();                                            // Retorna un nuevo hijo de este Scope
    public Utils.ReturnType? GetType(string? search, IScope scope);         // Retorna el tipo de variable (String, Bool, Digit)
}                                                  // Alcance de las variables (Proceso Semántico)                                                                          //
public interface IVisitor
{
    // Properties
    public IVisitor? Parent { get; set; }                                   // Padre de este objeto en el árbol de Visitor
    public IScope? Scope { get; set; }                                      // Scope asociado al alcance (Máscara)
    public Dictionary<string, object> Defined { get; set; }                 // Variables almacenadas (nombre y valores)
    public List<string> IncreaseVariables { get; set; }                     // Variables a incrementadas dentro del alcance
    public Assig? GetAssig();                                               // Retorna el objeto auxiliar

    // Methods
    public bool IsDefined(string search);                                   // Verifica si la variable está almacenada
    public void Define(Variable variable);                                  // Agrega la variable
    public void Define(string? name, object? value);                        // Agrega la variable (nombre y valor)
    public object? GetValue(string? search);                                // Devuelve el valor específico de un tipo (variable)
    public IVisitor CreateChild();                                          // Retorna un nuevo hijo de este Visitor
    public IVisitor CreateChild(IScope? scope);                             // Retorna un hijo de este objeto y le asigna un Scope
    public void AddIncrease();                                              // Agrega incremento a las con "++" dentro del alcance
    public void AddInstance();                                              // Agrega las variables del Scope
    public Assig? Assig { get; set; }                                       // Objeto auxiliar (usado para asignar)
}                                                // Alcance de las variables (Proceso para Evaluar)
