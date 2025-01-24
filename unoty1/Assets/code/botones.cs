using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // M�todos p�blicos para asignar a los botones desde el Inspector

    // M�todo para cargar la escena del juego
    public void StartGame()
    {
        SceneManager.LoadScene("tablero"); // Cambia "GameScene" al nombre exacto de tu escena del juego
    }

    // M�todo para la funcionalidad "Continue"
    public void ContinueGame()
    {
        Debug.Log("La funcionalidad 'Continue' est� pendiente de implementaci�n.");
    }

    // M�todo para salir del juego
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
