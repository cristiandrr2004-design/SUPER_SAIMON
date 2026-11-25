using TMPro;
using UnityEngine;

// Hace que Monsai lance burlas hacia Saimon cada cierto tiempo.
// Va en Monsai. Los botones de VALORES lo silencian un rato.
public class LanzadorBurlas : MonoBehaviour
{
    public string[] frases = { "¡Raro!", "¡Lento!", "¡Cuatro ojos!", "¡Perdedor!", "¡No sabes nada!", "¡Nadie te escucha!" };
    public float cadaSegundos = 2.4f;
    public float rapidez = 4.5f;
    public float alcance = 13f;
    public float alturaBoca = 0.7f;

    public TMP_FontAsset fuente;
    public Sprite spriteGlobo;
    public Burla plantilla;        // "Plantilla Burla" de la escena (TextMeshPro editable)

    Transform jugador;
    float siguiente = 2f;
    float silencioHasta = 0;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;
        siguiente = Time.time + 2f;
    }

    public void Silenciar(float segundos)
    {
        silencioHasta = Mathf.Max(silencioHasta, Time.time + segundos);
        siguiente = Mathf.Max(siguiente, silencioHasta + 0.5f);
    }

    public bool Silenciado() { return Time.time < silencioHasta; }

    void Update()
    {
        if (jugador == null || !enabled || Time.time < siguiente || Silenciado()) return;
        siguiente = Time.time + cadaSegundos * Random.Range(0.8f, 1.2f);
        if (Mathf.Abs(jugador.position.x - transform.position.x) > alcance) return;

        Vector3 boca = transform.position + new Vector3(0, alturaBoca, 0);
        Vector3 objetivo = jugador.position + new Vector3(0, 0.8f, 0);
        string frase = frases[Random.Range(0, frases.Length)];
        if (plantilla != null) Burla.Lanzar(plantilla, boca, objetivo, rapidez, frase);
        else Burla.Crear(boca, objetivo, rapidez, frase, fuente, spriteGlobo);
    }
}
