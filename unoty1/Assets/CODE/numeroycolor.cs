using UnityEngine;

public enum ColorCarta
{
    Rojo,
    Verde,
    Azul,
    Amarillo,
    Ninguno // Esto es para las cartas especiales como +4 y cambio de color
}

public class Carta : MonoBehaviour
{
    public int numero;       // Número de la carta (por ejemplo, 0-9 o cartas especiales)
    public ColorCarta color; // Color de la carta (Rojo, Verde, Azul, Amarillo, Ninguno)
}
