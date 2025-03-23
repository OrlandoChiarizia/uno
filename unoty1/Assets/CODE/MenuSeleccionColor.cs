using UnityEngine;

public class MenuSeleccionColor : MonoBehaviour
{
    public GameObject menuSeleccionColor; // Referencia al panel del menú

    public void AbrirMenu()
    {
        if (menuSeleccionColor != null)
        {
            menuSeleccionColor.SetActive(true); // Mostrar el menú
        }
        else
        {
            Debug.LogError("❌ Error: No se asignó el menú de selección de color.");
        }
    }

    public void SeleccionarColor(string color)
    {
        UnoGameManager gameManager = FindObjectOfType<UnoGameManager>();

        if (gameManager != null)
        {
            gameManager.SeleccionarColor(color); // Llamamos a la función en UnoGameManager
            menuSeleccionColor.SetActive(false); // Ocultamos el menú
        }
        else
        {
            Debug.LogError("❌ Error: No se encontró el UnoGameManager en la escena.");
        }
    }
}
