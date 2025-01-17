using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard != null)
        {
            droppedCard.transform.position = transform.position; // La carta se queda en el cuadrado central
        }
    }
}

