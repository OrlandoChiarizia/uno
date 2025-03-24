    using UnityEngine;
    using System.Collections.Generic;
    using TMPro;
    using System.Linq;
using System.Collections;


    public class UnoGameManager : MonoBehaviour
    {
        public GameObject cartaBasePrefab;
        public Transform manoJugador;
        public Transform manoIA;
        public Transform pilaDescarte;
        public string carpetaSprites = "SpritesCartas";

        private MenuSeleccionColor menuSeleccionColor;
        public TextMeshProUGUI debugTexto;
        public GameObject panelSeleccionColor;



    private List<Sprite> spritesCartas = new List<Sprite>();
        private List<GameObject> cartasManoJugador = new List<GameObject>();
        private List<GameObject> cartasManoIA = new List<GameObject>();

        private int cartaSeleccionada = 0;
        private int ordenPila = 0;
        private bool esTurnoJugador = true;
        private string colorActual;

        private float radioAbanico = 3.5f;
        private float anguloSeparacion = 15f;
        private const int MAX_CARTAS = 15;




    void Start()
    {
        CargarSprites();
        GenerarCartaInicial();
        RepartirCartasIniciales();
        ActualizarSeleccion();
        MostrarDebug("🎮 ¡Juego Iniciado!");

        panelSeleccionColor.SetActive(false);  // 🔹 Ocultar panel al inicio
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
                sr.sortingOrder = 1; // Asegurar que quede por debajo de las demás cartas
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

        // 🃏 Generamos una nueva carta y la agregamos a la mano
        GenerarCarta(manoJugador, cartasManoJugador);
        ReorganizarCartasEnAbanico(cartasManoJugador);

        // ✅ Verificamos si la carta robada es jugable
        if (TieneCartaJugable(cartasManoJugador))
        {
            MostrarDebug("✅ Has robado una carta jugable. Puedes jugarla.");
            return; // 🚀 Permite que el jugador juegue en lugar de cambiar el turno
        }
        else
        {
            MostrarDebug("🚫 Has robado una carta, pero aún no puedes jugar.");
        }

        // 🔄 Si no puede jugar, cambiar turno a la IA
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

                // Si la carta es Wild, aseguramos que su color sea "wild"
                if (parts[0].ToLower() == "wild")
                {
                    cartaScript.color = "wild";
                    cartaScript.numero = null; // No tiene número
                }
                else
                {
                    cartaScript.color = parts[0];
                    cartaScript.numero = parts.Length > 1 ? parts[1] : null;
                }

                Debug.Log($"🃏 Carta generada: {spriteAleatorio.name} | Color: {cartaScript.color} | Número: {cartaScript.numero}");
            }

            cartasMano.Add(nuevaCarta);
        }


    public void JugarCartaJugador()
    {
        if (cartasManoJugador.Count == 0) return;

        GameObject carta = cartasManoJugador[cartaSeleccionada];
        Carta cartaSeleccionadaScript = carta.GetComponent<Carta>();

        if (cartaSeleccionadaScript == null)
        {
            MostrarDebug("⚠️ Error: La carta seleccionada es nula.");
            return;
        }

        if (!EsCartaValida(cartaSeleccionadaScript))
        {
            MostrarDebug("❌ ¡La carta seleccionada no es válida!");
            return;
        }

        // 🔹 Mover la carta al área de descarte
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;
        carta.GetComponent<SpriteRenderer>().sortingOrder = 100 + ordenPila;
        ordenPila++;

        cartasManoJugador.RemoveAt(cartaSeleccionada);
        ReorganizarCartasEnAbanico(cartasManoJugador);
        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasManoJugador.Count - 1);
        ActualizarSeleccion();

        // 🔹 Verificar si al jugador le queda una sola carta
        VerificarCartasJugador();

        // 🃏 Si la carta es Wild, abrir la selección de color
        if (cartaSeleccionadaScript.color == "wild")
        {
            MostrarDebug("🎨 Elige un color para continuar.");
            panelSeleccionColor.SetActive(true);
            return;  // ⛔ No cambiar turno hasta que el jugador elija color
        }

        // ✅ Si la carta es Skip o Reverse, permitir que el jugador juegue otra carta
        if (cartaSeleccionadaScript.numero == "Skip" || cartaSeleccionadaScript.numero == "Reverse")
        {
            MostrarDebug($"🔄 Jugaste una carta {cartaSeleccionadaScript.numero}. ¡Puedes jugar otra carta del mismo color o número!");
            CambiarTurno(true);  // 🔥 Permite jugar otra vez
            return;
        }

        CambiarTurno();  // 🔄 Pasar el turno si no es Skip ni Reverse
    }

  



    void VerificarCartasJugador()
    {
        if (cartasManoJugador.Count == 1)
        {
            MostrarDebug("⚠️ ¡Te queda solo UNA carta! Juega sabiamente. 🃏");
        }
    }


    public void SeleccionarColorRojo()
    {
        SeleccionarColor("Red");
    }

    public void SeleccionarColorAzul()
    {
        SeleccionarColor("Blue");
    }

    public void SeleccionarColorVerde()
    {
        SeleccionarColor("Green");
    }
    public void SeleccionarColorAmarillo()
    {
        SeleccionarColor("Yellow");
    }

    public void AbrirMenuSeleccionColor()
    {
        if (menuSeleccionColor != null)
        {
            panelSeleccionColor.SetActive(true); // ✅ Mostrar panel
        }
    }

    // ✅ Método para seleccionar el color de una carta Wild
    public void SeleccionarColor(string nuevoColor)
    {
        Debug.Log($"🎨 Color seleccionado: {nuevoColor}");

        if (pilaDescarte.childCount == 0) return;

        GameObject cartaPila = pilaDescarte.GetChild(pilaDescarte.childCount - 1).gameObject;
        Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

        if (cartaPilaScript.color == "wild")
        {
            if (nuevoColor == "Red" || nuevoColor == "Blue" || nuevoColor == "Green" || nuevoColor == "Yellow")
            {
                cartaPilaScript.color = nuevoColor; // ✅ Asignar nuevo color
                MostrarDebug($"El color ha cambiado a {nuevoColor}");

                panelSeleccionColor.SetActive(false); // ❌ Ocultar el panel después de seleccionar

                CambiarTurno(); // ✅ Pasar el turno al siguiente jugador
            }
            else
            {
                Debug.LogError("❌ Color inválido seleccionado.");
            }
        }
    }






    void TurnoIA()
    {
        if (esTurnoJugador) return;

        GameObject cartaJugada = null;
        Carta cartaJugadaScript = null;

        // 🔹 Primero, buscar una carta jugable por color o número
        foreach (GameObject carta in cartasManoIA)
        {
            Carta cartaScript = carta.GetComponent<Carta>();
            if (EsCartaValida(cartaScript) && cartaScript.color != "wild") // No jugar Wild aún
            {
                cartaJugada = carta;
                cartaJugadaScript = cartaScript;
                break;
            }
        }

        // 🔹 Si no encontró una carta jugable normal, buscar un Wild
        if (cartaJugada == null)
        {
            foreach (GameObject carta in cartasManoIA)
            {
                Carta cartaScript = carta.GetComponent<Carta>();
                if (cartaScript.color == "wild")  // Jugar Wild solo si no tiene otra opción
                {
                    cartaJugada = carta;
                    cartaJugadaScript = cartaScript;
                    break;
                }
            }
        }

        IEnumerator EsperarYMostrarDebug(GameObject cartaJugada, Carta cartaJugadaScript, float tiempoEspera)
        {
            // 🃏 Primero, jugar la carta
            JugarCartaIA(cartaJugada);

            // ⏳ Esperar un poco para que la animación de jugar la carta ocurra
            yield return new WaitForSeconds(tiempoEspera);

            // 🃏 Si la carta es Wild, elegir y mostrar el color
            if (cartaJugadaScript.color == "wild")
            {
                colorActual = ElegirMejorColorIA();
                MostrarDebug($"🤖 La IA ha jugado un Wild y ha elegido el color {colorActual.ToUpper()} 🎨");

                // Aplicar el color seleccionado
                cartaJugadaScript.color = colorActual;

                yield return new WaitForSeconds(5f); // ⏳ Esperar antes de continuar
            }

            // 🛑 Si la IA juega Skip o Reverse, repetir turno
            if (cartaJugadaScript.numero == "Skip" || cartaJugadaScript.numero == "Reverse")
            {
                MostrarDebug("🤖 La IA te ha cancelado el turno y volverá a jugar.");
                yield return new WaitForSeconds(5f);
                TurnoIA();
                yield break;
            }

            // 🔹 Pasar el turno normalmente después de esperar
            CambiarTurno();
        }

        // 🔹 Si encontró una carta jugable (normal o Wild), jugarla
        if (cartaJugada != null)
        {
            StartCoroutine(EsperarYMostrarDebug(cartaJugada, cartaJugadaScript, 0.5f)); // ⏳ Pequeña pausa antes del mensaje
            return;
        }

        // 🔹 Si no tiene cartas jugables NI Wild, entonces roba una carta
        MostrarDebug("🤖 La IA no tiene carta jugable y robará una.");
        RobarCartaIA(); // ✅ Se llama sin asignarla a una variable

        // 🔹 Intentar jugar la carta robada inmediatamente
        foreach (GameObject carta in cartasManoIA)
        {
            Carta cartaScript = carta.GetComponent<Carta>();
            if (EsCartaValida(cartaScript))
            {
                JugarCartaIA(carta);

                // 🛑 Si la carta robada es Skip o Reverse, repetir turno
                if (cartaScript.numero == "Skip" || cartaScript.numero == "Reverse")
                {
                    MostrarDebug("🤖 La IA te ha cancelado el turno y volverá a jugar.");
                    TurnoIA(); // 🔥 La IA vuelve a jugar
                    return;
                }

                CambiarTurno();
                return;
            }
        }

        // 🔹 Si después de robar aún no tiene jugada, pasa el turno
        CambiarTurno();
    }


    // 🃏 La IA elige el mejor color basado en sus cartas
    string ElegirMejorColorIA()
{
    Dictionary<string, int> contadorColores = new Dictionary<string, int>()
    {
        { "Red", 0 },
        { "Blue", 0 },
        { "Green", 0 },
        { "Yellow", 0 }
    };

    // Contar cuántas cartas tiene de cada color
    foreach (GameObject carta in cartasManoIA)
    {
        Carta cartaScript = carta.GetComponent<Carta>();
        if (contadorColores.ContainsKey(cartaScript.color))
        {
            contadorColores[cartaScript.color]++;
        }
    }

    // 🏆 Elegir el color con más cartas; si hay empate, elegir uno al azar
    int maxCantidad = contadorColores.Values.Max();
    List<string> mejoresColores = contadorColores
        .Where(c => c.Value == maxCantidad)
        .Select(c => c.Key)
        .ToList();

    return mejoresColores[UnityEngine.Random.Range(0, mejoresColores.Count)];
}

    void JugarCartaIA(GameObject carta)
    {
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;
        carta.GetComponent<SpriteRenderer>().sortingOrder = 100 + ordenPila;
        ordenPila++;
        cartasManoIA.Remove(carta);

        MostrarDebug($"🤖 IA ha jugado una carta {carta.GetComponent<Carta>().color}_{carta.GetComponent<Carta>().numero}.");

        // 🔹 Verificar si a la IA le queda una sola carta
        if (cartasManoIA.Count == 1)
        {
            MostrarDebug("⚠️ ¡A la IA le queda solo UNA carta! 🃏");
        }
    }




    bool EsCartaValida(Carta cartaSeleccionada)
        {
            if (cartaSeleccionada.color == "wild")
            {
                Debug.Log("🃏 Carta Wild detectada. Siempre es válida.");
                return true; // Wild siempre se puede jugar
            }

            if (pilaDescarte.childCount == 0) return true;

            GameObject cartaPila = pilaDescarte.GetChild(pilaDescarte.childCount - 1).gameObject;
            Carta cartaPilaScript = cartaPila.GetComponent<Carta>();

            bool esValida = (cartaSeleccionada.color == cartaPilaScript.color ||
                             cartaSeleccionada.numero == cartaPilaScript.numero);

            Debug.Log($"✅ Validación de carta: {cartaSeleccionada.color} vs {cartaPilaScript.color} | {cartaSeleccionada.numero} vs {cartaPilaScript.numero} | Es válida: {esValida}");

            return esValida;
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
        
    public void CambiarTurno(bool turnoExtra = false)
    {
        // Si hay un turno extra, el mismo jugador juega de nuevo
        if (turnoExtra)
        {
            MostrarDebug("🔄 Turno extra activado. El mismo jugador juega otra vez.");

            // Si es el turno del jugador, permitimos que juegue otra carta
            if (esTurnoJugador)
            {
                return; // El jugador puede jugar otra vez
            }
            else
            {
                // Si es la IA, hacemos que juegue otra vez inmediatamente
                Invoke("TurnoIA", 1.0f);
                return;
            }
        }

        // Cambiar el turno normalmente
        esTurnoJugador = !esTurnoJugador;

        if (!esTurnoJugador)
        {
            MostrarDebug("🤖 Turno de la IA.");
            Invoke("TurnoIA", 1.0f); // Esperar 1 segundo antes de que la IA juegue
        }
        else
        {
            MostrarDebug("👤 Turno del jugador.");
        }
    }

}


    