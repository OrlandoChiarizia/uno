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
        if (botonJugar != null)
            botonJugar.onClick.AddListener(JugarCarta);

        if (botonRobar != null)
            botonRobar.onClick.AddListener(RobarCarta);

        if (botonSalir != null)
            botonSalir.onClick.AddListener(SalirDelJuego);

        if (botonNuevoJuego != null)
            botonNuevoJuego.onClick.AddListener(IniciarNuevoJuego);
    }

    void JugarCarta()
    {
        if (unoManager != null)
        {
            unoManager.JugarCarta();
        }
    }

    void RobarCarta()
    {
        if (unoManager != null)
        {
            unoManager.GenerarCartaAleatoria();
        }
    }

    void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    void IniciarNuevoJuego()
    {
        Debug.Log("Iniciando nuevo juego...");
        SceneManager.LoadScene("tablero");
    }
}
