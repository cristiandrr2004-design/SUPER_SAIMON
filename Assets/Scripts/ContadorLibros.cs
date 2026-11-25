using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Interfaz del nivel: libros recogidos, vidas, y las pantallas de victoria y derrota.
// Nivel 1: al recoger todos los libros, gana.
// Nivel 2: hay que entregar los libros al compañero (ganarConLibros = false).
// Nivel 3: no hay libros; el marcador muestra los valores activados (usarMarcadorPropio).
// Va en el Canvas de la escena.
public class ContadorLibros : MonoBehaviour
{
    public Graphic textoLibros;      // "Libros: 2 / 5" (Text o TextMeshPro)
    public Graphic textoVidas;       // "Vidas: 3"
    public GameObject panelVictoria; // aparece al ganar (empieza oculto)
    public GameObject panelDerrota;  // aparece al perder (empieza oculto)

    [Header("Niveles")]
    public bool ganarConLibros = true;      // false = hay que entregarlos al compañero
    public bool usarMarcadorPropio = false; // true = otro script escribe el marcador
    public string siguienteEscena = "";     // escena del boton de victoria (vacio = repetir)

    [Header("Sonidos")]
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    private int recogidos = 0;
    private int total = 0;
    private bool terminado = false;

    void Awake()
    {
        // Cuenta solos los libros que hay en la escena: no hay que escribir el total
        total = FindObjectsByType<Libro>(FindObjectsSortMode.None).Length;
    }

    void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);

        Time.timeScale = 1;
        ActualizarTexto();
    }

    // La llama cada Libro al ser recogido
    public void SumarLibro()
    {
        recogidos = recogidos + 1;
        ActualizarTexto();

        if (ganarConLibros && recogidos >= total)
            Ganar();
    }

    public int Recogidos() { return recogidos; }
    public int Total() { return total; }
    public bool TieneTodos() { return recogidos >= total; }
    public bool Terminado() { return terminado; }

    // La llama Super Saimon cuando cambian sus vidas
    public void MostrarVidas(int vidas)
    {
        if (textoVidas != null)
            Textos.Poner(textoVidas, "Vidas: " + Mathf.Max(vidas, 0));
    }

    public void PonerMarcador(string texto)
    {
        if (textoLibros != null) Textos.Poner(textoLibros, texto);
    }

    void ActualizarTexto()
    {
        if (usarMarcadorPropio) return;
        if (textoLibros != null)
            Textos.Poner(textoLibros, "Libros: " + recogidos + " / " + total);
    }

    public void Ganar()
    {
        if (terminado) return;
        terminado = true;
        Sonar(sonidoVictoria);
        foreach (Burla b in FindObjectsByType<Burla>(FindObjectsSortMode.None)) Destroy(b.gameObject);
        if (panelVictoria != null) panelVictoria.SetActive(true);
        Time.timeScale = 0;   // detiene el juego
    }

    // La llama Super Saimon cuando se queda sin vidas
    public void Perder()
    {
        if (terminado) return;
        terminado = true;
        Sonar(sonidoDerrota);
        if (panelDerrota != null) panelDerrota.SetActive(true);
        Time.timeScale = 0;
    }

    void Sonar(AudioClip clip)
    {
        if (clip != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    // Para el boton "REINTENTAR"
    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Para el boton de victoria: pasa al siguiente nivel
    public void Continuar()
    {
        Time.timeScale = 1;
        if (string.IsNullOrEmpty(siguienteEscena)) Reiniciar();
        else Transicion.CambiarEscena(siguienteEscena);
    }
}
