using UnityEngine;
using System.Collections.Generic;

public class UnoManager : MonoBehaviour
{
    public GameObject cartaBasePrefab; // Prefab base de la carta
    public Transform manoJugador;      // Contenedor de las cartas del jugador
    public Transform pilaDescarte;     // Contenedor de la pila de descarte
    public string carpetaSprites = "SpritesCartas"; // Carpeta de los sprites en Resources

    private List<Sprite> spritesCartas = new List<Sprite>(); // Sprites disponibles
    private List<GameObject> cartasMano = new List<GameObject>(); // Cartas en la mano
    private int cartaSeleccionada = 0; // Índice de la carta seleccionada

    void Start()
    {
        CargarSprites();
        if (spritesCartas.Count == 0 || cartaBasePrefab == null || manoJugador == null)
        {
            Debug.LogError("Faltan referencias o no hay sprites disponibles.");
            return;
        }

        GenerarCartasIniciales(7);
        ActualizarSeleccion();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoverSeleccion(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoverSeleccion(-1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            DescartarCartaSeleccionada();
        }
    }

    // Carga los sprites desde la carpeta en Resources
    void CargarSprites()
    {
        spritesCartas.AddRange(Resources.LoadAll<Sprite>(carpetaSprites));
        if (spritesCartas.Count == 0)
            Debug.LogError($"No se encontraron sprites en '{carpetaSprites}'");
    }

    // Genera una cantidad de cartas aleatorias
    void GenerarCartasIniciales(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GenerarCartaAleatoria();
        }
    }

    // Genera una carta con un sprite aleatorio
    void GenerarCartaAleatoria()
    {
        if (spritesCartas.Count == 0) return;

        Sprite spriteAleatorio = spritesCartas[Random.Range(0, spritesCartas.Count)];
        GameObject nuevaCarta = Instantiate(cartaBasePrefab, manoJugador);
        AsignarSprite(nuevaCarta, spriteAleatorio);
        cartasMano.Add(nuevaCarta);
        ReposicionarCartas();
    }

    // Asigna el sprite y ajusta el orden de renderizado
    void AsignarSprite(GameObject carta, Sprite sprite)
    {
        SpriteRenderer sr = carta.GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.sprite = sprite;
            sr.sortingOrder = 10; // Asegurar que la carta esté por encima del fondo
        }
        else
        {
            Debug.LogError("El prefab de la carta no tiene un componente SpriteRenderer.");
        }
    }

    // Mueve la selección de la carta
    void MoverSeleccion(int direccion)
    {
        if (cartasMano.Count == 0) return;
        cartaSeleccionada = (cartaSeleccionada + direccion + cartasMano.Count) % cartasMano.Count;
        ActualizarSeleccion();
    }

    // Actualiza la visualización de la carta seleccionada
    void ActualizarSeleccion()
    {
        for (int i = 0; i < cartasMano.Count; i++)
        {
            SpriteRenderer sr = cartasMano[i].GetComponent<SpriteRenderer>();
            if (sr) sr.color = (i == cartaSeleccionada) ? Color.yellow : Color.white;
        }
    }

    // Descarta la carta seleccionada
    void DescartarCartaSeleccionada()
    {
        if (cartasMano.Count == 0) return;

        GameObject carta = cartasMano[cartaSeleccionada];
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;

        // Cambiar el sortingOrder para que la carta descartada esté arriba
        SpriteRenderer sr = carta.GetComponent<SpriteRenderer>();
        if (sr) sr.sortingOrder = 15;

        cartasMano.RemoveAt(cartaSeleccionada);

        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasMano.Count - 1);
        ReposicionarCartas();
        ActualizarSeleccion();
    }

    // Reposiciona las cartas en la mano
    void ReposicionarCartas()
    {
        for (int i = 0; i < cartasMano.Count; i++)
        {
            cartasMano[i].transform.localPosition = new Vector3(i * 1.5f, 0, 0);
        }
    }
}
