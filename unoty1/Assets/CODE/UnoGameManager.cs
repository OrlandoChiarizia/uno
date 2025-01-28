using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnoManager : MonoBehaviour
{
    // Configuración general
    public GameObject cartaBasePrefab; // Prefab de la carta base
    public Transform manoJugador;      // Contenedor de las cartas del jugador
    public Transform pilaDescarte;    // Contenedor de la pila de descarte
    public string carpetaSprites = "SpritesCartas"; // Carpeta de los sprites en Resources

    // Listas para manejar los sprites y las cartas
    private List<Sprite> spritesCartas = new List<Sprite>();  // Sprites cargados
    private List<GameObject> cartasMano = new List<GameObject>(); // Cartas en la mano
    private int cartaSeleccionada = 0; // Índice de la carta seleccionada

    void Start()
    {
        // Cargar todos los sprites desde la carpeta
        CargarSprites();

        // Generar 7 cartas iniciales
        for (int i = 0; i < 7; i++)
        {
            GenerarCartaAleatoria();
        }

        // Actualizar la selección inicial
        ActualizarSeleccion();
    }

    void Update()
    {
        // Navegar entre cartas con las flechas del teclado
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

        // Seleccionar carta y moverla a la pila de descarte
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeleccionarCarta();
        }
    }

    // Método para cargar los sprites de la carpeta
    void CargarSprites()
    {
        Sprite[] cargados = Resources.LoadAll<Sprite>(carpetaSprites);

        if (cargados.Length == 0)
        {
            Debug.LogError($"No se encontraron sprites en la carpeta '{carpetaSprites}'.");
            return;
        }

        spritesCartas.AddRange(cargados);
        Debug.Log($"Se cargaron {spritesCartas.Count} sprites.");
    }

    // Método para generar una carta aleatoria
    void GenerarCartaAleatoria()
    {
        if (spritesCartas.Count == 0 || cartaBasePrefab == null || manoJugador == null)
        {
            Debug.LogError("Faltan referencias o sprites para generar cartas.");
            return;
        }

        // Elegir un sprite aleatorio
        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        // Instanciar la carta base
        GameObject nuevaCarta = Instantiate(cartaBasePrefab, manoJugador);

        // Configurar el sprite de la carta
        Image cartaImagen = nuevaCarta.GetComponentInChildren<Image>();
        if (cartaImagen != null)
        {
            cartaImagen.sprite = spriteAleatorio;
        }

        // Ajustar posición en la mano
        nuevaCarta.transform.localPosition = new Vector3(cartasMano.Count * 1.5f, 0, 0);
        cartasMano.Add(nuevaCarta);
    }

    // Actualizar la visualización de la carta seleccionada
    void ActualizarSeleccion()
    {
        for (int i = 0; i < cartasMano.Count; i++)
        {
            var outline = cartasMano[i].GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = (i == cartaSeleccionada); // Activar outline si es la carta seleccionada
            }
        }
    }

    // Mover carta seleccionada a la pila de descarte
    void SeleccionarCarta()
    {
        if (cartasMano.Count == 0) return;

        // Obtener la carta seleccionada
        GameObject carta = cartasMano[cartaSeleccionada];

        // Mover la carta a la pila de descarte
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;

        // Eliminar la carta de la mano
        cartasMano.RemoveAt(cartaSeleccionada);

        // Ajustar las posiciones de las cartas restantes
        for (int i = 0; i < cartasMano.Count; i++)
        {
            cartasMano[i].transform.localPosition = new Vector3(i * 1.5f, 0, 0);
        }

        // Ajustar índice de selección
        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasMano.Count - 1);
        ActualizarSeleccion();
    }
}
