using UnityEngine;
using System.Collections.Generic;

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

    void Start()
    {
        CargarSprites();
        GenerarCartaInicial();
        RepartirCartasIniciales();
        ActualizarSeleccion();
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

    
    void GenerarCartaInicial()
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
            RobarCartaJugador();
            RobarCartaIA();
        }
    }

    public void RobarCartaJugador()
    {
        if (cartasManoJugador.Count >= MAX_CARTAS) return;
        GenerarCarta(manoJugador, cartasManoJugador);
        ReorganizarCartasEnAbanico(cartasManoJugador);
    }

    void RobarCartaIA()
    {
        if (cartasManoIA.Count >= MAX_CARTAS) return;
        GenerarCarta(manoIA, cartasManoIA);
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
            Debug.Log("¡La carta seleccionada no es válida!");
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
        GameObject cartaJugada = null;

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
            cartaJugada.transform.SetParent(pilaDescarte);
            cartaJugada.transform.localPosition = Vector3.zero;
            cartaJugada.GetComponent<SpriteRenderer>().sortingOrder = 100 + ordenPila;
            ordenPila++;
            cartasManoIA.Remove(cartaJugada);
        }
        else
        {
            RobarCartaIA();
        }

        CambiarTurno();
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
