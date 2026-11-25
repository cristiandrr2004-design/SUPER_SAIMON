using UnityEngine;

// Libro que brilla (Nivel 3): un halo dorado que late y chispas de vez en cuando,
// para encontrarlo entre los libros revueltos por Monsai. Va junto al script Libro.
public class LibroBrillante : MonoBehaviour
{
    public Sprite brillo;              // halo suave
    public Sprite chispa;
    public Color color = new Color(1f, 0.85f, 0.3f);
    public float tamano = 1.8f;
    public float cadaChispa = 0.7f;

    Transform halo;
    SpriteRenderer srHalo;
    float siguiente;

    void Start()
    {
        if (brillo != null)
        {
            GameObject h = new GameObject("Brillo");
            h.transform.SetParent(transform, false);
            srHalo = h.AddComponent<SpriteRenderer>();
            srHalo.sprite = brillo;
            srHalo.color = color;
            var mio = GetComponent<SpriteRenderer>();
            srHalo.sortingOrder = mio != null ? mio.sortingOrder - 1 : 1;
            halo = h.transform;
        }
        siguiente = Time.time + Random.Range(0f, cadaChispa);
    }

    void Update()
    {
        float pulso = 0.5f + 0.5f * Mathf.Sin(Time.time * 4f);
        if (halo != null && brillo != null)
        {
            float s = tamano * (0.85f + 0.3f * pulso) / brillo.bounds.size.x;
            Vector3 padre = transform.lossyScale;
            halo.localScale = new Vector3(s / Mathf.Max(padre.x, 0.0001f), s / Mathf.Max(padre.y, 0.0001f), 1);
            srHalo.color = new Color(color.r, color.g, color.b, 0.55f + 0.45f * pulso);
        }
        if (chispa != null && Time.time >= siguiente)
        {
            siguiente = Time.time + cadaChispa;
            Destello.Crear(transform.position, chispa, color, 4);
        }
    }
}
