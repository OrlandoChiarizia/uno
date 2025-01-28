using UnityEngine;
using System.Collections.Generic;

public class UnoManager : MonoBehaviour
{
    public GameObject cartaBasePrefab; // Prefab base de la carta
    public Transform manoJugador;      // Donde aparecen las cartas en la mano
    public Transform pilaDescarte;     // Donde se ponen las cartas jugadas
    public string carpetaSprites = "SpritesCartas"; // Carpeta en Resources

    private List<Sprite> spritesCartas = new List<Sprite>();
    private List<GameObject> cartasMano = new List<GameObject>();
    private int cartaSeleccionada = 0;

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

        int indice = Random.Range(0, spritesCartas.Count);
        Sprite spriteAleatorio = spritesCartas[indice];

        GameObject nuevaCarta = Instantiate(cartaBasePrefab, manoJugador);
        SpriteRenderer sr = nuevaCarta.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteAleatorio;
            sr.sortingOrder = 20 + cartasMano.Count; // Asegura que esté sobre el tablero
        }

        nuevaCarta.transform.localPosition = new Vector3(cartasMano.Count * 1.5f, 0, 0);
        cartasMano.Add(nuevaCarta);
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
        carta.transform.SetParent(pilaDescarte);
        carta.transform.localPosition = Vector3.zero;
        cartasMano.RemoveAt(cartaSeleccionada);

        for (int i = 0; i < cartasMano.Count; i++)
        {
            cartasMano[i].transform.localPosition = new Vector3(i * 1.5f, 0, 0);
        }

        cartaSeleccionada = Mathf.Clamp(cartaSeleccionada, 0, cartasMano.Count - 1);
        ActualizarSeleccion();
    }
}
