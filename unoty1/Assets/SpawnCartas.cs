using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;          // Prefab de la carta
    public List<Transform> spawnPoints;    // Lista de puntos de generación (spawners)
    public List<Sprite> cardSprites;       // Lista de imágenes de las cartas

    private void Start()
    {
        GenerateRandomCards();
    }

    // Genera cartas aleatorias en cada uno de los spawners
    void GenerateRandomCards()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            // Instanciar la carta en el spawner correspondiente
            GameObject newCard = Instantiate(cardPrefab, spawnPoints[i].position, Quaternion.identity);

            // Asignar un sprite aleatorio a la carta
            Sprite randomSprite = cardSprites[Random.Range(0, cardSprites.Count)];
            newCard.GetComponent<SpriteRenderer>().sprite = randomSprite;

            // Añadir el script de arrastre a la carta
            //newCard.AddComponent<CardDragHandler>();
        }
    }
}
