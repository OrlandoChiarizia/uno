using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Canvas canvas;

    private void Start()
    {
        // Buscar el Canvas automáticamente
        canvas = FindObjectOfType<Canvas>();
    }

    // Método para asignar la imagen de la carta
    public void SetCardSprite(Sprite newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    // Al empezar a arrastrar la carta
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
    }

    // Mientras arrastras la carta
    public void OnDrag(PointerEventData eventData)
    {
        // Convertir la posición del mouse a coordenadas del mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        mousePosition.z = 0; // Asegurarse de que la carta esté en el plano 2D
        transform.position = mousePosition;
    }

    // Al soltar la carta
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            transform.position = originalPosition; // Si no está sobre un objeto válido, vuelve a su posición original
        }
    }
}
