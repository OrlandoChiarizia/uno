using System.Collections.Generic;
using UnityEngine;

public class IAUno : MonoBehaviour
{
    public List<GameObject> cartasManoIA = new List<GameObject>();
    private UnoManager unoManager;

    void Start()
    {
        unoManager = FindObjectOfType<UnoManager>();
    }

    public void JugarTurno()
    {
        // Obtener la carta superior de la pila de descarte
        GameObject cartaPila = unoManager.pilaDescarte.GetChild(unoManager.pilaDescarte.childCount - 1).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

        bool cartaEncontrada = false;

        // Buscar una carta válida en la mano de la IA
        for (int i = 0; i < cartasManoIA.Count; i++)
        {
            Carta cartaIA = cartasManoIA[i].GetComponent<Carta>();

            if (EsCartaValida(cartaIA, cartaPilaScript))
            {
                // Jugar la carta encontrada
                JugarCarta(cartasManoIA[i]);
                cartaEncontrada = true;
                break;
            }
        }

        // Si no hay cartas válidas, robar una carta
        if (!cartaEncontrada)
        {
            unoManager.GenerarCartaAleatoriaIA();
        }
    }

    private bool EsCartaValida(Carta cartaIA, Carta cartaPila)
    {
        return cartaIA.color == cartaPila.color || cartaIA.numero == cartaPila.numero;
    }

    private void JugarCarta(GameObject carta)
    {
        // Mover la carta a la pila de descarte
        carta.transform.SetParent(unoManager.pilaDescarte);
        carta.transform.localPosition = Vector3.zero;

        // Asegurar que la carta esté ENCIMA de las demás en la pila
        SpriteRenderer sr = carta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 100 + unoManager.ordenPila; // Mayor orden = encima de todas
        }
        unoManager.ordenPila++; // Incrementa el orden para la siguiente carta

        cartasManoIA.Remove(carta);
    }
}