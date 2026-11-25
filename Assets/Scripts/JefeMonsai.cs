using TMPro;
using UnityEngine;

// Nivel 3: Monsai como jefe. Tiene tanta "fuerza" como botones de VALORES hay.
// Cada boton activado lo debilita; sin fuerza, se arrepiente y Saimon gana.
// Va en Monsai (junto con Monsai y LanzadorBurlas).
public class JefeMonsai : MonoBehaviour
{
    public int fuerza = 3;
    public bool contarLibros = false;   // Nivel 3: cada libro brillante recogido lo debilita
    public TMP_FontAsset fuente;
    public Sprite spriteGlobo;
    public Sprite pixel;                 // para la barra de fuerza
    [TextArea] public string frasePerdon = "Perdón, Saimón... me equivoqué. Todos merecemos respeto.";
    [TextArea] public string fraseInicio = "¡Nadie podrá detener mis burlas!";

    public float distanciaPerdon = 3f;     // que tan cerca debe llegar Saimon para oir el perdon
    public float segundosPerdon = 3f;      // cuanto dura el mensaje antes de terminar el nivel
    int fuerzaMax;
    bool esperandoSaimon = false;
    Transform saimon;
    int activados = 0;
    SpriteRenderer sprite;
    Monsai patrulla;
    LanzadorBurlas lanzador;
    public Burbuja globo;          // globo de texto (TextMeshPro) ya puesto en la escena
    Transform barra, relleno;
    float aturdidoHasta = 0;
    bool vencido = false;

    void Start()
    {
        fuerzaMax = Mathf.Max(1, fuerza);
        sprite = GetComponent<SpriteRenderer>();
        patrulla = GetComponent<Monsai>();
        lanzador = GetComponent<LanzadorBurlas>();
        float alto = sprite != null ? sprite.bounds.extents.y : 1.3f;

        if (globo == null)
            globo = Burbuja.Crear(transform, new Vector3(0, alto + 0.6f, 0), fuente, spriteGlobo,
                new Color(0.25f, 0.08f, 0.1f, 0.95f), new Color(1f, 0.9f, 0.8f), 3f);
        globo.Mostrar(fraseInicio);
        Invoke(nameof(OcultarGlobo), 3f);

        // Barra de fuerza encima de la cabeza
        if (pixel != null)
        {
            barra = new GameObject("BarraFuerza").transform;
            var fondo = barra.gameObject.AddComponent<SpriteRenderer>();
            fondo.sprite = pixel; fondo.color = new Color(0.1f, 0.05f, 0.05f, 0.9f); fondo.sortingOrder = 22;
            fondo.drawMode = SpriteDrawMode.Simple;
            barra.localScale = new Vector3(2.4f / pixel.bounds.size.x, 0.28f / pixel.bounds.size.y, 1);

            relleno = new GameObject("Relleno").transform;
            var r = relleno.gameObject.AddComponent<SpriteRenderer>();
            r.sprite = pixel; r.color = new Color(0.85f, 0.15f, 0.2f); r.sortingOrder = 23;
        }
        ActualizarMarcador();
    }

    void Update()
    {
        // Ya vencido: el perdon sale solo cuando Saimon se acerca
        if (esperandoSaimon)
        {
            if (saimon == null) { var p = GameObject.FindGameObjectWithTag("Player"); if (p != null) saimon = p.transform; }
            if (saimon != null && Vector2.Distance(saimon.position, transform.position) < distanciaPerdon)
            {
                esperandoSaimon = false;
                globo.Mostrar(frasePerdon);
                Invoke(nameof(Ganar), segundosPerdon);
            }
            return;
        }
        if (!contarLibros || vencido) return;
        ContadorLibros c = FindFirstObjectByType<ContadorLibros>();
        if (c != null && c.Recogidos() > activados) Debilitar("");
    }

    void OcultarGlobo() { if (globo != null && !vencido) globo.Ocultar(); }

    void LateUpdate()
    {
        if (barra == null || sprite == null) return;
        float alto = sprite.bounds.extents.y;
        Vector3 cabeza = transform.position + new Vector3(0, alto + 0.3f, 0);
        barra.position = cabeza;
        float ancho = 2.3f * fuerza / (float)fuerzaMax;
        relleno.localScale = new Vector3(Mathf.Max(ancho, 0.0001f) / pixel.bounds.size.x, 0.2f / pixel.bounds.size.y, 1);
        relleno.position = cabeza + new Vector3(-1.15f + ancho / 2, 0, -0.01f);
        relleno.gameObject.SetActive(fuerza > 0);
        barra.gameObject.SetActive(!vencido);
        relleno.gameObject.SetActive(!vencido && fuerza > 0);

        // parpadea rojo mientras esta aturdido
        if (!vencido)
        {
            bool aturdido = Time.time < aturdidoHasta;
            sprite.color = aturdido && Mathf.Repeat(Time.time * 8, 1) > 0.5f ? new Color(1f, 0.5f, 0.5f) : Color.white;
            if (patrulla != null) patrulla.enabled = !aturdido;
        }
    }

    public void Debilitar(string valor)
    {
        if (vencido) return;
        activados++;
        fuerza = Mathf.Max(0, fuerza - 1);
        aturdidoHasta = Time.time + 2f;
        ActualizarMarcador();

        if (fuerza <= 0) { Vencer(); return; }

        // cada vez se pone mas rapido (esta desesperado)
        if (lanzador != null) { lanzador.cadaSegundos *= 0.8f; lanzador.rapidez *= 1.12f; }
        if (patrulla != null) patrulla.velocidad *= 1.15f;
        globo.Mostrar(string.IsNullOrEmpty(valor) ? "¡¿Encontraste otro libro?! ¡Me debilito!" : "¡¿" + valor + "?! ¡Eso me debilita!");
        CancelInvoke(nameof(OcultarGlobo));
        Invoke(nameof(OcultarGlobo), 2f);
    }

    void ActualizarMarcador()
    {
        ContadorLibros c = FindFirstObjectByType<ContadorLibros>();
        if (c != null) c.PonerMarcador((contarLibros ? "Libros: " : "Valores: ") + activados + " / " + fuerzaMax);
    }

    void Vencer()
    {
        vencido = true;
        if (patrulla != null) patrulla.enabled = false;
        if (lanzador != null) lanzador.enabled = false;
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        foreach (Burla b in FindObjectsByType<Burla>(FindObjectsSortMode.None)) b.Neutralizar();
        if (sprite != null) sprite.color = new Color(0.8f, 0.85f, 1f);
        CancelInvoke(nameof(OcultarGlobo));
        globo.Ocultar();
        esperandoSaimon = true;
        ContadorLibros contador = FindFirstObjectByType<ContadorLibros>();
        if (contador != null) contador.PonerMarcador("¡Ve con Monsai!");
    }

    void Ganar()
    {
        ContadorLibros c = FindFirstObjectByType<ContadorLibros>();
        if (c != null) c.Ganar();
    }
}
