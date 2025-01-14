using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void NuevaPartida()
    {
        SceneManager.LoadScene("Juego");
    }

    public void CargarPartida()
    {
        Debug.Log("Cargar Partida: Funcionalidad pendiente...");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}

