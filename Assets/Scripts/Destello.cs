using UnityEngine;

// Efecto visual: chispas que salen disparadas y se desvanecen.
// No se pone en ningun objeto: lo llama el Libro con Destello.Crear(...).
public class Destello : MonoBehaviour
{
    private Vector3 velocidad;
    private float vida = 0.6f;
    private float tiempo = 0;
    private SpriteRenderer sprite;
    private Color color;

    public static void Crear(Vector3 posicion, Sprite dibujo, Color color, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject chispa = new GameObject("Chispa");
            chispa.transform.position = posicion;
            var sr = chispa.AddComponent<SpriteRenderer>();
            sr.sprite = dibujo;
            sr.color = color;
            sr.sortingOrder = 10;
            var d = chispa.AddComponent<Destello>();
            float angulo = (i / (float)cantidad) * Mathf.PI * 2 + Random.Range(-0.2f, 0.2f);
            d.velocidad = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0) * Random.Range(2.5f, 4f);
            d.sprite = sr;
            d.color = color;
        }
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        transform.position += velocidad * Time.deltaTime;
        velocidad *= 0.92f;
        float t = tiempo / vida;
        transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.2f, t);
        sprite.color = new Color(color.r, color.g, color.b, 1 - t);
        if (tiempo >= vida) Destroy(gameObject);
    }
}
