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
        var cartaRenderer = nuevaCarta.GetComponent<SpriteRenderer>();
        if (cartaScript != null)
        {
            string nombre_carta = cartaRenderer.sprite.name;
            print(nombre_carta);

            // Split the name into parts
            string[] parts = nombre_carta.Split('_');

            // Handle regular cards (Blue_9, Blue_Skip, etc.)
            if (parts.Length == 2)
            {
                cartaScript.color = parts[0]; // The first part is the color (e.g., Blue)
                cartaScript.numero = parts[1]; // The second part is the number or action (e.g., 9, Skip, Reverse)
            }
            // Handle special cards (Draw, Wild_Draw, etc.)
            else if (parts.Length == 1)
            {
                // Special cards like Draw or Wild_Draw
                cartaScript.color = null; // The whole part is the color (e.g., Draw, Wild_Draw)
                cartaScript.numero = null; // No specific number or action, so set it to null or a default value
            }
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
            Debug.Log("¡La carta seleccionada no es válida para descartar!");
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
        GameObject cartaPila = pilaDescarte.GetChild(pilaDescarte.childCount-1).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();


        if (string.IsNullOrEmpty(cartaPilaScript.numero) && string.IsNullOrEmpty(cartaPilaScript.color))
        {
            return true;
        }

        if (string.IsNullOrEmpty(cartaSeleccionada.numero) && string.IsNullOrEmpty(cartaSeleccionada.color))
        {
            return true; // Las cartas especiales seleccionadas también son válidas
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