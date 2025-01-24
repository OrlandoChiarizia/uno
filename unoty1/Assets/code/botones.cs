using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Métodos públicos para asignar a los botones desde el Inspector

    // Método para cargar la escena del juego
    public void StartGame()
    {
        SceneManager.LoadScene("tablero"); // Cambia "GameScene" al nombre exacto de tu escena del juego
    }

    // Método para la funcionalidad "Continue"
    public void ContinueGame()
    {
        Debug.Log("La funcionalidad 'Continue' está pendiente de implementación.");
    }

    // Método para salir del juego
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
