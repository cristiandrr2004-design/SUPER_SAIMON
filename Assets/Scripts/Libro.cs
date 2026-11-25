using UnityEngine;

// Libro perdido por Anti Super Saimon. Al tocarlo, Super Saimon lo recoge.
// Necesita: SpriteRenderer y un Collider2D con "Is Trigger" marcado.
public class Libro : MonoBehaviour
{
    public float altura = 0.15f;     // cuanto sube y baja flotando
    public float rapidez = 2f;       // que tan rapido flota

    [Header("Efectos")]
    public AudioClip sonidoRecoger;
    public Sprite chispa;            // dibujo de las chispas que salen al recogerlo
    public Color colorChispa = new Color(1f, 0.85f, 0.3f);

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Flota suavemente para que se note que se puede recoger
        transform.position = posicionInicial + new Vector3(0, Mathf.Sin(Time.time * rapidez) * altura, 0);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            ContadorLibros contador = FindFirstObjectByType<ContadorLibros>();
            if (contador != null)
                contador.SumarLibro();

            if (sonidoRecoger != null)
                AudioSource.PlayClipAtPoint(sonidoRecoger, Camera.main.transform.position);

            if (chispa != null)
                Destello.Crear(transform.position, chispa, colorChispa, 10);

            Destroy(gameObject);
        }
    }
}
