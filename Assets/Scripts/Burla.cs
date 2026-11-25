using TMPro;
using UnityEngine;

// Burla que lanza Monsai: una palabra hiriente que vuela hacia Saimon y le quita una vida.
// Con un boton de VALORES se neutraliza y se vuelve una frase amable.
public class Burla : MonoBehaviour
{
    public Vector2 velocidad;
    public float vida = 6f;
    bool neutralizada = false;
    public TextMeshPro texto;      // editable en la plantilla de la escena
    public SpriteRenderer fondo;
    float nacio;

    static readonly string[] Amables = { "¡Tú importas!", "¡Respeto!", "¡Eres valioso!", "¡Bien hecho!", "¡Ánimo!" };

    public static Burla Crear(Vector3 desde, Vector3 hacia, float rapidez, string frase, TMP_FontAsset fuente, Sprite spriteFondo)
    {
        GameObject go = new GameObject("Burla");
        go.transform.position = desde;
        var b = go.AddComponent<Burla>();
        b.velocidad = ((Vector2)(hacia - desde)).normalized * rapidez;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.35f;

        GameObject f = new GameObject("Fondo");
        f.transform.SetParent(go.transform, false);
        b.fondo = f.AddComponent<SpriteRenderer>();
        b.fondo.sprite = spriteFondo;
        b.fondo.color = new Color(0.75f, 0.1f, 0.12f, 0.92f);
        b.fondo.sortingOrder = 25;
        if (spriteFondo != null && spriteFondo.border != Vector4.zero) b.fondo.drawMode = SpriteDrawMode.Sliced;

        GameObject t = new GameObject("Texto");
        t.transform.SetParent(go.transform, false);
        b.texto = t.AddComponent<TextMeshPro>();
        if (fuente != null) b.texto.font = fuente;
        b.texto.fontSize = 2.6f;
        b.texto.color = Color.white;
        b.texto.alignment = TextAlignmentOptions.Center;
        b.texto.textWrappingMode = TextWrappingModes.NoWrap;
        b.texto.sortingOrder = 26;
        b.PonerTexto(frase);
        return b;
    }

    // Copia la plantilla de la escena (asi se puede editar su fuente, tamano y colores)
    public static Burla Lanzar(Burla plantilla, Vector3 desde, Vector3 hacia, float rapidez, string frase)
    {
        Burla b = Instantiate(plantilla, desde, Quaternion.identity);
        b.name = "Burla";
        b.velocidad = ((Vector2)(hacia - desde)).normalized * rapidez;
        b.gameObject.SetActive(true);
        b.PonerTexto(frase);
        return b;
    }

    void PonerTexto(string frase)
    {
        texto.text = frase;
        texto.ForceMeshUpdate();
        Vector2 tam = texto.GetRenderedValues(false);
        if (tam.x <= 0 || float.IsNaN(tam.x)) tam = new Vector2(1.5f, 0.4f);
        Vector2 caja = tam + new Vector2(0.35f, 0.25f);
        if (fondo.drawMode == SpriteDrawMode.Sliced) fondo.size = caja;
        else if (fondo.sprite != null)
            fondo.transform.localScale = new Vector3(caja.x / fondo.sprite.bounds.size.x, caja.y / fondo.sprite.bounds.size.y, 1);
    }

    void Start() { nacio = Time.time; }

    void Update()
    {
        transform.position += (Vector3)(velocidad * Time.deltaTime);
        // se mueve un poquito en ola para que se vea viva
        transform.position += new Vector3(0, Mathf.Sin((Time.time - nacio) * 8f) * 0.01f, 0);
        if (Time.time - nacio > vida) Destroy(gameObject);
    }

    // La llama un boton de VALORES: la burla se convierte en una frase amable y sube
    public void Neutralizar()
    {
        if (neutralizada) return;
        neutralizada = true;
        GetComponent<Collider2D>().enabled = false;
        fondo.color = new Color(0.12f, 0.55f, 0.3f, 0.95f);
        PonerTexto(Amables[Random.Range(0, Amables.Length)]);
        velocidad = new Vector2(0, 1.2f);
        nacio = Time.time;
        vida = 1.6f;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (neutralizada || !otro.CompareTag("Player")) return;
        SuperSaimon s = otro.GetComponent<SuperSaimon>();
        if (s != null)
        {
            if (s.EsInvencible()) return;
            s.RecibirGolpe(transform.position);
        }
        Destroy(gameObject);
    }
}
