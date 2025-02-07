using UnityEngine;
using System.Collections.Generic;

public class UnoManager : MonoBehaviour
{
    public GameObject cartaBasePrefab;
    public Transform manoJugador;
    public Transform pilaDescarte;
    public string carpetaSprites = "SpritesCartas";

    private List<Sprite> spritesCartas = new List<Sprite>();
    private List<GameObject> cartasMano = new List<GameObject>();
    private int cartaSeleccionada = 0;
    private int ordenPila = 0; // 🔥 Controla el orden de las cartas en la pila de descarte

    private float radioAbanico = 3.5f;
    private float anguloSeparacion = 15f;
    private const int MAX_CARTAS = 15; // 🔥 Límite máximo de cartas en la mano

    void Start()
    {
        CargarSprites();
        for (int i = 0; i < 7; i++)
        {
            GenerarCartaAleatoria();
        }
        ActualizarSeleccion();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cartaSeleccionada = (cartaSeleccionada + 1) % cartasMano.Count;
            ActualizarSeleccion();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cartaSeleccionada = (cartaSeleccionada - 1 + cartasMano.Count) % cartasMano.Count;
            ActualizarSeleccion();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JugarCarta();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GenerarCartaAleatoria();
        }
    }

    void CargarSprites()
    {
        Sprite[] cargados = Resources.LoadAll<Sprite>(carpetaSprites);
        if (cargados.Length == 0)
        {
            Debug.LogError($"No se encontraron sprites en '{carpetaSprites}'.");
            return;
        }
        spritesCartas.AddRange(cargados);
    }

    void GenerarCartaAleatoria()
    {
        if (spritesCartas.Count == 0 || cartaBasePrefab == null) return;

        // 🔥 Verifica si el jugador ya tiene el máximo de cartas
        if (cartasMano.Count >= MAX_CARTAS)
        {
            Debug.Log("¡Máximo de 15 cartas alcanzado! No puedes robar más.");
            return;
        }

        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        GameObject nuevaCarta = Instantiate(cartaBasePrefab, manoJugador);
        SpriteRenderer sr = nuevaCarta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteAleatorio;
            sr.sortingOrder = 20 + cartasMano.Count;
        }

        // Asignar número y color a la carta
        Carta cartaScript = nuevaCarta.GetComponent<Carta>();
        if (cartaScript != null)
        {
            cartaScript.numero = Random.Range(0, 10); // Asignar un número aleatorio entre 0 y 9
            cartaScript.color = (ColorCarta)Random.Range(0, 4); // Asignar un color aleatorio (Rojo, Verde, Azul, Amarillo)
        }

        cartasMano.Add(nuevaCarta);
        ReorganizarCartasEnAbanico();
    }

    void ActualizarSeleccion()
    {
        for (int i = 0; i < cartasMano.Count; i++)
        {
            SpriteRenderer sr = cartasMano[i].GetComponent<SpriteRenderer>();
            sr.color = (i == cartaSeleccionada) ? Color.yellow : Color.white;
        }
    }

    void JugarCarta()
    {
        if (cartasMano.Count == 0) return;

        GameObject carta = cartasMano[cartaSeleccionada];
        Carta cartaSeleccionadaScript = carta.GetComponent<Carta>();

        if (cartaSeleccionadaScript == null || !EsCartaValida(cartaSeleccionadaScript))
        {
            Debug.LogError("¡La carta seleccionada no es válida para descartar!");
            return;
        }

        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;

        // 🔥 Asegurar que la carta esté ENCIMA de las demás en la pila
        SpriteRenderer sr = carta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 100 + ordenPila; // Mayor orden = encima de todas
        }
        ordenPila++; // Incrementa el orden para la siguiente carta

        cartasMano.RemoveAt(cartaSeleccionada);

        ReorganizarCartasEnAbanico();
        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasMano.Count - 1);
        ActualizarSeleccion();
    }

    bool EsCartaValida(Carta cartaSeleccionada)
    {
        // Si la pila de descarte está vacía, la primera carta puede ser cualquier carta
        if (pilaDescarte.childCount == 0)
            return true;

        // La carta en la pila de descarte
        GameObject cartaPila = pilaDescarte.GetChild(0).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

        Debug.Log("Carta pila: num col " + cartaPilaScript.numero + " " + cartaPilaScript.color);

        // Si la carta en la pila de descarte es un +4 (cambio de color), cualquier carta es válida
        if (cartaPilaScript.numero == 0 && cartaPilaScript.color == ColorCarta.Ninguno)
        {
            return true;
        }

        // La carta es válida si coincide en color o número con la carta en la pila de descarte
        if (cartaSeleccionada.color == cartaPilaScript.color || cartaSeleccionada.numero == cartaPilaScript.numero)
        {
            return true;
        }

        return false; // Si no coincide en número ni en color, no es válida
    }

    void ReorganizarCartasEnAbanico()
    {
        int totalCartas = cartasMano.Count;
        if (totalCartas == 0) return;

        float anguloInicial = -anguloSeparacion * (totalCartas - 1) / 2f;

        for (int i = 0; i < totalCartas; i++)
        {
            float angulo = anguloInicial + (i * anguloSeparacion);
            float x = Mathf.Sin(angulo * Mathf.Deg2Rad) * radioAbanico;
            float y = Mathf.Cos(angulo * Mathf.Deg2Rad) * -radioAbanico;

            cartasMano[i].transform.localPosition = new Vector3(x, y, 0);
            cartasMano[i].transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }
}