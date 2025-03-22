using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class UnoGameManager : MonoBehaviour
{
    public GameObject cartaBasePrefab;
    public Transform manoJugador;
    public Transform manoIA;
    public Transform pilaDescarte;
    public string carpetaSprites = "SpritesCartas";

    private List<Sprite> spritesCartas = new List<Sprite>();
    private List<GameObject> cartasManoJugador = new List<GameObject>();
    private List<GameObject> cartasManoIA = new List<GameObject>();

    private int cartaSeleccionada = 0;
    private int ordenPila = 0;
    private bool esTurnoJugador = true; // Se empieza con el turno del jugador

    private float radioAbanico = 3.5f;
    private float anguloSeparacion = 15f;
    private const int MAX_CARTAS = 15;
    public TextMeshProUGUI debugTexto;  // ✅ Correcto para UI


    void Start()
    {
        CargarSprites();
        GenerarCartaInicial();
        RepartirCartasIniciales();
        ActualizarSeleccion();
        MostrarDebug("🎮 ¡Juego Iniciado!");
    }


    public void MostrarDebug(string mensaje)
    {
        if (debugTexto != null)
        {
            debugTexto.text = mensaje; // 🔹 Muestra el mensaje en la UI
        }
        else
        {
            Debug.Log("DEBUG: " + mensaje); // 🔹 Si no hay UI, muestra en la consola
        }
    }
    void Update()
    {
        if (!esTurnoJugador || cartasManoJugador.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cartaSeleccionada = (cartaSeleccionada + 1) % cartasManoJugador.Count;
            ActualizarSeleccion();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cartaSeleccionada = (cartaSeleccionada - 1 + cartasManoJugador.Count) % cartasManoJugador.Count;
            ActualizarSeleccion();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JugarCartaJugador();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RobarCartaJugador();
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


    public void GenerarCartaInicial()
    {
        if (spritesCartas.Count == 0 || cartaBasePrefab == null) return;

        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        GameObject cartaInicial = Instantiate(cartaBasePrefab, pilaDescarte);
        cartaInicial.transform.position = new Vector3(-19, 5, 0); // Colocar en el centro del tablero

        SpriteRenderer sr = cartaInicial.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteAleatorio;
            sr.sortingOrder = 0; // Asegurar que quede por debajo de las demás cartas
        }

        Carta cartaScript = cartaInicial.GetComponent<Carta>();
        if (cartaScript != null)
        {
            string[] parts = spriteAleatorio.name.Split('_');
            cartaScript.color = parts[0];
            cartaScript.numero = parts.Length > 1 ? parts[1] : null;
        }
    }

    void RepartirCartasIniciales()
    {
        for (int i = 0; i < 7; i++)
        {
            // 🚀 Usamos una versión de robar sin restricciones para el reparto inicial
            RobarCartaInicial(manoJugador, cartasManoJugador);
            RobarCartaInicial(manoIA, cartasManoIA);
        }
    }

    void RobarCartaInicial(Transform mano, List<GameObject> cartasMano)
    {
        if (cartasMano.Count >= MAX_CARTAS) return;
        GenerarCarta(mano, cartasMano);
        ReorganizarCartasEnAbanico(cartasMano);
    }




    public void RobarCartaJugador()
    {
        // 🛑 Si el jugador tiene una carta jugable, no puede robar
        if (TieneCartaJugable(cartasManoJugador))
        {
            MostrarDebug("❌ No puedes robar porque tienes cartas jugables.");
            return;
        }

        if (cartasManoJugador.Count >= MAX_CARTAS) return;

        GenerarCarta(manoJugador, cartasManoJugador);
        ReorganizarCartasEnAbanico(cartasManoJugador);

        // 🛠 Verifica si la carta robada ahora es jugable
        if (TieneCartaJugable(cartasManoJugador))
        {
            MostrarDebug("✅ Has robado una carta jugable.");
        }
        else
        {
            MostrarDebug("🚫 Has robado una carta, pero aún no puedes jugar.");
        }

        // Si no puede jugar, cambiar turno a la IA
        CambiarTurno();
    }




    void RobarCartaIA()
    {
        // 🛑 Si la IA tiene una carta jugable, no debe robar
        if (TieneCartaJugable(cartasManoIA))
        {
            MostrarDebug("❌ La IA no puede robar porque tiene cartas jugables.");
            CambiarTurno();
            return;
        }

        if (cartasManoIA.Count >= MAX_CARTAS) return;

        GenerarCarta(manoIA, cartasManoIA);

        // 🛠 Verifica si la IA ahora puede jugar
        if (TieneCartaJugable(cartasManoIA))
        {
            MostrarDebug("🤖 La IA ha robado una carta jugable.");
        }
        else
        {
            MostrarDebug("🤖 La IA ha robado pero sigue sin cartas jugables.");
        }
    }
    bool TieneCartaJugable(List<GameObject> mano)
    {
        foreach (GameObject carta in mano)
        {
            Carta cartaScript = carta.GetComponent<Carta>();
            if (EsCartaValida(cartaScript))
            {
                return true;
            }
        }
        return false;
    }


    void GenerarCarta(Transform mano, List<GameObject> cartasMano)
    {
        if (spritesCartas.Count == 0 || cartaBasePrefab == null) return;

        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        GameObject nuevaCarta = Instantiate(cartaBasePrefab, mano);
        SpriteRenderer sr = nuevaCarta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteAleatorio;
            sr.sortingOrder = 20 + cartasMano.Count;
        }

        Carta cartaScript = nuevaCarta.GetComponent<Carta>();
        if (cartaScript != null)
        {
            string[] parts = spriteAleatorio.name.Split('_');
            cartaScript.color = parts[0];
            cartaScript.numero = parts.Length > 1 ? parts[1] : null;
        }

        cartasMano.Add(nuevaCarta);
    }

    public void JugarCartaJugador()
    {
        if (cartasManoJugador.Count == 0) return;

        GameObject carta = cartasManoJugador[cartaSeleccionada];
        Carta cartaSeleccionadaScript = carta.GetComponent<Carta>();

        if (cartaSeleccionadaScript == null || !EsCartaValida(cartaSeleccionadaScript))
        {
            MostrarDebug("¡La carta seleccionada no es válida!");
            return;
        }

        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;
        carta.GetComponent<SpriteRenderer>().sortingOrder = 100 + ordenPila;
        ordenPila++;

        cartasManoJugador.RemoveAt(cartaSeleccionada);
        ReorganizarCartasEnAbanico(cartasManoJugador);
        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasManoJugador.Count - 1);
        ActualizarSeleccion();

        CambiarTurno();
    }

    void TurnoIA()
    {
        if (esTurnoJugador) return; // Asegurar que sea el turno de la IA

        GameObject cartaJugada = null;

        // La IA intenta jugar una carta válida
        foreach (GameObject carta in cartasManoIA)
        {
            Carta cartaScript = carta.GetComponent<Carta>();
            if (EsCartaValida(cartaScript))
            {
                cartaJugada = carta;
                break;
            }
        }

        if (cartaJugada != null)
        {
            // ✅ La IA juega la carta y cambia de turno inmediatamente
            JugarCartaIA(cartaJugada);
            CambiarTurno();
            return;
        }

        // Si no tiene carta válida, roba UNA sola carta
        MostrarDebug("IA no tiene carta jugable y robará una.");
        RobarCartaIA();

        // Revisa si la carta robada es jugable
        Carta cartaRobada = cartasManoIA[cartasManoIA.Count - 1].GetComponent<Carta>();
        if (EsCartaValida(cartaRobada))
        {
            MostrarDebug("IA ha robado una carta jugable y la jugará.");
            JugarCartaIA(cartasManoIA[cartasManoIA.Count - 1]);
        }
        else
        {
            MostrarDebug("IA ha robado pero no puede jugar. Turno del jugador.");
        }

        // ✅ Cambio de turno después de jugar o robar
        CambiarTurno();
    }

    void JugarCartaIA(GameObject carta)
    {
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;
        carta.GetComponent<SpriteRenderer>().sortingOrder = 100 + ordenPila;
        ordenPila++;
        cartasManoIA.Remove(carta);

        MostrarDebug("🤖 IA ha jugado una carta.");
    }


    bool EsCartaValida(Carta cartaSeleccionada)
    {
        if (pilaDescarte.childCount == 0) return true;

        GameObject cartaPila = pilaDescarte.GetChild(pilaDescarte.childCount - 1).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

        // La carta es válida si el color o el número coinciden con la carta en la pila
        return cartaSeleccionada.color == cartaPilaScript.color || cartaSeleccionada.numero == cartaPilaScript.numero;
    }


    void ReorganizarCartasEnAbanico(List<GameObject> cartasMano)
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

    void ActualizarSeleccion()
    {
        for (int i = 0; i < cartasManoJugador.Count; i++)
        {
            SpriteRenderer sr = cartasManoJugador[i].GetComponent<SpriteRenderer>();
            sr.color = (i == cartaSeleccionada) ? new Color(0.7f, 0.7f, 0.7f, 1f) : Color.white;
        }
    }

    void CambiarTurno()
    {
        esTurnoJugador = !esTurnoJugador;

        if (!esTurnoJugador)
        {
            Invoke("TurnoIA", 1.0f);
        }
    }
}