using TMPro;
using UnityEngine;

// Personaje no jugable (estudiante, compañero): cuando Saimon se acerca, habla con un globo.
// Si es el compañero del Nivel 2, al llegar con todos los libros se los entrega y se gana el nivel.
// Necesita: SpriteRenderer. El trigger de cercania se calcula solo (no necesita collider).
public class NPC : MonoBehaviour
{
    [TextArea] public string[] dialogos = { "¡Hola, Saimón!" };
    public float segundosPorFrase = 3.2f;
    public float distanciaHablar = 2.6f;

    [Header("Compañero que recibe los libros (Nivel 2)")]
    public bool recibeLibros = false;
    [TextArea] public string faltanLibros = "¡Todavía faltan libros! Te espero aquí.";
    [TextArea] public string agradecimiento = "¡Gracias, Saimón! Tú sí me respetas.";

    [Header("Globo")]
    public TMP_FontAsset fuente;
    public Sprite spriteGlobo;
    public float altoGlobo = 1.3f;

    Transform jugador;
    SpriteRenderer sprite;
    public Burbuja globo;          // globo de texto (TextMeshPro) ya puesto en la escena
    float baseY;
    int frase = 0;
    float siguienteCambio = 0;
    bool cerca = false;
    bool entregado = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;
        baseY = transform.position.y;
        float alto = sprite != null ? sprite.bounds.extents.y : 1f;
        if (globo == null)
            globo = Burbuja.Crear(transform, new Vector3(0, alto + 0.15f, 0), fuente, spriteGlobo,
                new Color(1f, 0.97f, 0.88f, 0.95f), new Color(0.15f, 0.07f, 0.03f), 2.8f);
        else globo.Ocultar();
    }

    void Update()
    {
        // respira un poquito para que se note que esta vivo
        transform.position = new Vector3(transform.position.x, baseY + Mathf.Abs(Mathf.Sin(Time.time * 2.2f + transform.position.x)) * 0.05f, 0);
        if (jugador == null || globo == null) return;

        // mira hacia Saimon (los dibujos miran a la derecha)
        if (sprite != null) sprite.flipX = jugador.position.x < transform.position.x;

        float d = Vector2.Distance(jugador.position, transform.position);
        bool ahora = d < distanciaHablar;
        if (ahora && !cerca) { frase = 0; siguienteCambio = 0; }
        cerca = ahora;

        if (!cerca) { if (!entregado) globo.Ocultar(); return; }

        if (recibeLibros)
        {
            ContadorLibros c = FindFirstObjectByType<ContadorLibros>();
            if (c != null && c.TieneTodos() && !entregado)
            {
                entregado = true;
                globo.Mostrar(agradecimiento);
                Invoke(nameof(Ganar), 1.6f);
                return;
            }
            if (entregado) return;
            if (c != null && c.Recogidos() > 0) { globo.Mostrar(faltanLibros + " (" + c.Recogidos() + "/" + c.Total() + ")"); return; }
        }

        if (Time.time >= siguienteCambio && dialogos != null && dialogos.Length > 0)
        {
            globo.Mostrar(dialogos[frase % dialogos.Length]);
            frase++;
            siguienteCambio = Time.time + segundosPorFrase;
        }
    }

    void Ganar()
    {
        ContadorLibros c = FindFirstObjectByType<ContadorLibros>();
        if (c != null) c.Ganar();
    }
}
