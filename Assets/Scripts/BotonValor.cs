using TMPro;
using UnityEngine;

// Boton de un VALOR (ÉTICA, RESPETO, INTEGRALIDAD...). Saimon lo pisa para activarlo.
// Nivel 2: neutraliza todas las burlas y silencia a Monsai unos segundos (se puede reusar).
// Nivel 3: se activa una sola vez y debilita al jefe Monsai.
// Necesita: SpriteRenderer y un Collider2D con "Is Trigger".
public class BotonValor : MonoBehaviour
{
    public string valor = "RESPETO";
    public bool unaSolaVez = false;          // Nivel 3
    public float silencio = 4f;              // segundos sin burlas (Nivel 2)
    public float recarga = 6f;               // segundos para volver a usarlo (Nivel 2)
    public Color colorApagado = new Color(0.85f, 0.25f, 0.25f);
    public Color colorActivo = new Color(1f, 0.82f, 0.25f);

    public TMP_FontAsset fuente;
    public Material materialContorno;
    public Sprite chispa;
    public AudioClip sonido;

    SpriteRenderer sprite;
    public TextMeshPro etiqueta;   // texto del valor (TextMeshPro) ya puesto en la escena
    float listoEn = 0;
    Color colorEtiqueta;
    Vector3 alturaEtiqueta;
    bool usado = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = colorApagado;

        if (etiqueta == null) etiqueta = CrearEtiqueta(valor, transform.position, fuente, materialContorno);
        colorEtiqueta = etiqueta.color;
        alturaEtiqueta = etiqueta.transform.position - transform.position;
    }

    public static TextMeshPro CrearEtiqueta(string valor, Vector3 posBoton, TMP_FontAsset fuente, Material contorno)
    {
        GameObject t = new GameObject("Texto " + valor);
        t.transform.position = posBoton + new Vector3(0, 0.75f, 0);
        var e = t.AddComponent<TextMeshPro>();
        if (fuente != null) e.font = fuente;
        if (contorno != null) e.fontSharedMaterial = contorno;
        e.text = valor;
        e.fontSize = 3.2f;
        e.color = new Color(1f, 0.94f, 0.8f);
        e.alignment = TextAlignmentOptions.Center;
        e.textWrappingMode = TextWrappingModes.NoWrap;
        e.sortingOrder = 20;
        return e;
    }

    void Update()
    {
        if (etiqueta == null) return;
        bool listo = !usado && Time.time >= listoEn;
        // la etiqueta brinca cuando el boton esta listo para usarse
        float brinco = listo ? Mathf.Abs(Mathf.Sin(Time.time * 3f)) * 0.15f : 0;
        etiqueta.transform.position = transform.position + alturaEtiqueta + new Vector3(0, brinco, 0);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;
        if (usado || Time.time < listoEn) return;

        if (unaSolaVez) usado = true;
        else listoEn = Time.time + recarga;

        if (sprite != null) sprite.color = colorActivo;
        if (etiqueta != null) etiqueta.color = colorActivo;
        if (chispa != null) Destello.Crear(transform.position + Vector3.up * 0.3f, chispa, colorActivo, 14);
        if (sonido != null && Camera.main != null) AudioSource.PlayClipAtPoint(sonido, Camera.main.transform.position);

        // Todas las burlas en el aire se vuelven frases amables
        foreach (Burla b in FindObjectsByType<Burla>(FindObjectsSortMode.None)) b.Neutralizar();

        LanzadorBurlas lanzador = FindFirstObjectByType<LanzadorBurlas>();
        if (lanzador != null) lanzador.Silenciar(silencio);

        JefeMonsai jefe = FindFirstObjectByType<JefeMonsai>();
        if (jefe != null && unaSolaVez) jefe.Debilitar(valor);

        if (!unaSolaVez) Invoke(nameof(Restaurar), recarga);
    }

    void Restaurar()
    {
        if (etiqueta != null) etiqueta.color = colorEtiqueta;
        if (sprite != null) sprite.color = colorApagado;
    }
}
