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
        
        Debug.Log("Funcionalidad de cargar partida pendiente.");
    }

    
    public void SalirJuego()
    {
        
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}



