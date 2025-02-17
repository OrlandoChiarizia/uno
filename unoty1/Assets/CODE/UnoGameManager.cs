using System.Collections.Generic;
using UnityEngine;

public class UnoManager : MonoBehaviour
{
    public GameObject cartaBasePrefab;
    public Transform manoJugador;
    public Transform pilaDescarte;
    public string carpetaSprites = "SpritesCartas";

    private List<Sprite> spritesCartas = new List<Sprite>();
    public List<GameObject> cartasMano = new List<GameObject>(); // Cambiado a público
    private int cartaSeleccionada = 0;
    public int ordenPila = 0; // Cambiado a público

    private float radioAbanico = 3.5f;
    private float anguloSeparacion = 15f;
    private const int MAX_CARTAS = 15; // 🔥 Límite máximo de cartas en la mano

    public IAUno iaUno;

    private bool esTurnoJugador = true;
    private bool cambioSentido = false;

    void Start()
    {
        CargarSprites();
        for (int i = 0; i < 7; i++)
        {
            GenerarCartaAleatoria();
        }
        ActualizarSeleccion();

        // Inicializa la mano de la IA
        for (int i = 0; i < 7; i++)
        {
            GenerarCartaAleatoriaIA();
        }
    }

    void Update()
    {
        if (esTurnoJugador)
        {
            // Control de turnos del jugador
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
                CambiarTurno();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                GenerarCartaAleatoria();
            }
        }
        else
        {
            // Turno de la IA
            iaUno.JugarTurno();
            CambiarTurno();
        }

        // Verificar si el jugador o la IA han ganado
        if (cartasMano.Count == 0)
        {
            Debug.Log("¡El jugador ha ganado!");
            // Reiniciar el juego o mostrar mensaje de victoria
        }
        else if (iaUno.cartasManoIA.Count == 0)
        {
            Debug.Log("¡La IA ha ganado!");
            // Reiniciar el juego o mostrar mensaje de derrota
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

    public void GenerarCartaAleatoria() // Cambiado a público
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

    public void GenerarCartaAleatoriaIA() // Cambiado a público
    {
        if (spritesCartas.Count == 0 || cartaBasePrefab == null) return;

        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        GameObject nuevaCarta = Instantiate(cartaBasePrefab, iaUno.transform);
        SpriteRenderer sr = nuevaCarta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteAleatorio;
            sr.sortingOrder = 20 + iaUno.cartasManoIA.Count;
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

        iaUno.cartasManoIA.Add(nuevaCarta);
    }

    void ActualizarSeleccion()
    {
        for (int i = 0; i < cartasMano.Count; i++)
        {
            SpriteRenderer sr = cartasMano[i].GetComponent<SpriteRenderer>();
            sr.color = (i == cartaSeleccionada) ? new Color(0.9f, 0.9f, 0.9f, 1f) : Color.white;
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

        // Lógica para cartas especiales
        if (cartaSeleccionadaScript.numero == "Draw2")
        {
            GenerarCartaAleatoria();
            GenerarCartaAleatoria();
        }
        else if (cartaSeleccionadaScript.numero == "Skip")
        {
            CambiarTurno();
        }
        else if (cartaSeleccionadaScript.numero == "Reverse")
        {
            cambioSentido = !cambioSentido;
        }
        else if (cartaSeleccionadaScript.numero == "Wild")
        {
            // Cambio de color
        }
    }

    bool EsCartaValida(Carta cartaSeleccionada)
    {
        if (pilaDescarte.childCount == 0)
            return true;

        GameObject cartaPila = pilaDescarte.GetChild(pilaDescarte.childCount - 1).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

        if (string.IsNullOrEmpty(cartaPilaScript.numero) && string.IsNullOrEmpty(cartaPilaScript.color))
        {
            return true;
        }

        if (string.IsNullOrEmpty(cartaSeleccionada.numero) && string.IsNullOrEmpty(cartaSeleccionada.color))
        {
            return true;
        }

        return cartaSeleccionada.color == cartaPilaScript.color || cartaSeleccionada.numero == cartaPilaScript.numero;
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

    private void CambiarTurno()
    {
        esTurnoJugador = !esTurnoJugador;
    }
}