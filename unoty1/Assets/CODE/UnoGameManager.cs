using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnoGameManager : MonoBehaviour
{
    public List<Sprite> cardSprites; // Asigna aquí las imágenes de las cartas.
    public Transform playerHand;    // Contenedor para las cartas del jugador.
    public Transform discardPile;   // Contenedor para la pila de descarte.
    public GameObject cardPrefab;   // Prefab de una carta (con Image).

    private List<GameObject> playerCards = new List<GameObject>();
    private int selectedCardIndex = 0;

    void Start()
    {
        // Inicialización opcional
        RobarCarta(); // Roba una carta al inicio para probar.
    }

    void Update()
    {
        NavegarCartas();
        SeleccionarCarta();
    }

    // Espawnea una carta aleatoria y la añade a la mano del jugador.
    public void RobarCarta()
    {
       
        if (cardSprites.Count == 0) return;

        // Seleccionar una carta aleatoria.
        Sprite randomCard = cardSprites[Random.Range(0, cardSprites.Count)];
        
        // Crear la carta en la mano del jugador.
        GameObject newCard = Instantiate(cardPrefab, playerHand);
        newCard.GetComponent<Image>().sprite = randomCard;


        // Añadir la carta a la lista de cartas del jugador.
        playerCards.Add(newCard);

        // Ajustar selección al robar.
        if (playerCards.Count == 1) selectedCardIndex = 0;
    }

    // Navegar entre cartas con las flechas del teclado.
    void NavegarCartas()
    {
        if (playerCards.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedCardIndex = (selectedCardIndex + 1) % playerCards.Count;
            ActualizarSeleccionVisual();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedCardIndex = (selectedCardIndex - 1 + playerCards.Count) % playerCards.Count;
            ActualizarSeleccionVisual();
        }
    }

    // Resaltar la carta seleccionada.
    void ActualizarSeleccionVisual()
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            var cardImage = playerCards[i].GetComponent<Image>();
            cardImage.color = i == selectedCardIndex ? Color.yellow : Color.white;
        }
    }

    // Seleccionar la carta activa y moverla a la pila de descarte.
    void SeleccionarCarta()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerCards.Count > 0)
        {
            GameObject selectedCard = playerCards[selectedCardIndex];

            // Mover la carta a la pila de descarte.
            selectedCard.transform.SetParent(discardPile);
            selectedCard.transform.localPosition = Vector3.zero; // Posición centrada.

            // Quitar la carta de la mano.
            playerCards.RemoveAt(selectedCardIndex);
            Destroy(selectedCard);

            // Ajustar el índice de selección.
            selectedCardIndex = Mathf.Clamp(selectedCardIndex, 0, playerCards.Count - 1);
            ActualizarSeleccionVisual();
        }
    }
}
