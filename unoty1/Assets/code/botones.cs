using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Necesario para manejar las escenas

public class Botones : MonoBehaviour
{
    public UnoManager unoManager;  // Referencia al script UnoManager (asegúrate de asignarlo en el Inspector)
    public Button botonJugar;  // Botón para jugar la carta
    public Button botonRobar;  // Botón para robar carta
    public Button botonSalir;  // Botón para salir del juego
    public Button botonNuevoJuego;  // Botón para iniciar un nuevo juego

    void Start()
    {
        // Asegurarse de que los botones están correctamente configurados
        if (botonJugar != null)
            botonJugar.onClick.AddListener(JugarCarta);  // Asignar función al botón de jugar

        if (botonRobar != null)
            botonRobar.onClick.AddListener(RobarCarta);  // Asignar función al botón de robar

        if (botonSalir != null)
            botonSalir.onClick.AddListener(SalirDelJuego);  // Asignar función al botón de salir

        if (botonNuevoJuego != null)
            botonNuevoJuego.onClick.AddListener(IniciarNuevoJuego);  // Asignar función al botón de nuevo juego
    }

    // Función para jugar una carta (llama al método de UnoManager para jugar la carta seleccionada)
    void JugarCarta()
    {
        if (unoManager != null)
        {
            unoManager.JugarCarta();  // Llama al método JugarCarta en UnoManager
        }
    }

    // Función para robar una carta (llama al método de UnoManager para robar una carta)
    void RobarCarta()
    {
        if (unoManager != null)
        {
            unoManager.GenerarCartaAleatoria();  // Llama al método GenerarCartaAleatoria en UnoManager
        }
    }

    // Función para salir del juego (cierra la aplicación o vuelve al menú principal)
    void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();  // Cierra la aplicación (en una compilación)

        // Si estás en el editor de Unity, para salir del juego:
        // UnityEditor.EditorApplication.isPlaying = false;  // Descomentar para usar en el Editor
    }

    // Función para iniciar un nuevo juego (carga la escena del tablero)
    void IniciarNuevoJuego()
    {
        Debug.Log("Iniciando nuevo juego...");
        SceneManager.LoadScene("tablero");  // Cambia "tablero" al nombre de tu escena de juego
    }
}
