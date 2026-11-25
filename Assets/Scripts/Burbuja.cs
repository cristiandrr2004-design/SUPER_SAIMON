using TMPro;
using UnityEngine;

// Globo de dialogo que flota encima de un personaje (NPC, Monsai).
// Se crea con Burbuja.Crear(...); no hay que ponerlo en la escena.
[DefaultExecutionOrder(100)]   // despues de la camara, para no temblar
public class Burbuja : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento = new Vector3(0, 2.2f, 0);

    public TextMeshPro texto;      // editable en la escena (fuente, tamano, color)
    public SpriteRenderer fondo;
    public float margen = 0.25f;    // espacio minimo con el borde de la pantalla
    Vector2 caja = new Vector2(2, 0.8f);

    public static Burbuja Crear(Transform objetivo, Vector3 desplazamiento, TMP_FontAsset fuente, Sprite spriteFondo, Color colorFondo, Color colorTexto, float tamano = 3f, string textoInicial = null)
    {
        GameObject go = new GameObject("Burbuja");
        var b = go.AddComponent<Burbuja>();
        b.objetivo = objetivo;
        b.desplazamiento = desplazamiento;

        GameObject f = new GameObject("Fondo");
        f.transform.SetParent(go.transform, false);
        b.fondo = f.AddComponent<SpriteRenderer>();
        b.fondo.sprite = spriteFondo;
        b.fondo.color = colorFondo;
        b.fondo.sortingOrder = 30;
        if (spriteFondo != null && spriteFondo.border != Vector4.zero) b.fondo.drawMode = SpriteDrawMode.Sliced;

        GameObject t = new GameObject("Texto");
        t.transform.SetParent(go.transform, false);
        b.texto = t.AddComponent<TextMeshPro>();
        if (fuente != null) b.texto.font = fuente;
        b.texto.fontSize = tamano;
        b.texto.color = colorTexto;
        b.texto.alignment = TextAlignmentOptions.Center;
        b.texto.textWrappingMode = TextWrappingModes.Normal;
        b.texto.rectTransform.sizeDelta = new Vector2(4.6f, 1f);
        b.texto.sortingOrder = 31;

        go.transform.position = objetivo.position + desplazamiento;
        if (!string.IsNullOrEmpty(textoInicial)) b.Mostrar(textoInicial);
        else b.Ocultar();
        return b;
    }

    public void Mostrar(string mensaje)
    {
        gameObject.SetActive(true);
        texto.text = mensaje;
        texto.ForceMeshUpdate();
        Vector2 tam = texto.GetRenderedValues(false);
        if (tam.x <= 0 || tam.y <= 0 || float.IsNaN(tam.x)) tam = new Vector2(2, 0.5f);
        caja = tam + new Vector2(0.5f, 0.35f);
        if (fondo.drawMode == SpriteDrawMode.Sliced) fondo.size = caja;
        else if (fondo.sprite != null)
            fondo.transform.localScale = new Vector3(caja.x / fondo.sprite.bounds.size.x, caja.y / fondo.sprite.bounds.size.y, 1);
        // el globo crece hacia arriba
        texto.transform.localPosition = new Vector3(0, caja.y / 2, 0);
        fondo.transform.localPosition = new Vector3(0, caja.y / 2, 0);
        Seguir();
    }

    public void Ocultar() { gameObject.SetActive(false); }

    void LateUpdate()
    {
        if (objetivo == null) { Destroy(gameObject); return; }
        Seguir();
    }

    void Seguir()
    {
        if (objetivo == null) return;
        Vector3 p = objetivo.position + desplazamiento;

        // Mantener el globo completo dentro de la pantalla
        // (solo si el personaje esta a la vista; si Saimon esta lejos no importa)
        Camera cam = Camera.main;
        if (Application.isPlaying && cam != null && cam.orthographic)
        {
            float alto = cam.orthographicSize, ancho = alto * cam.aspect;
            Vector3 c = cam.transform.position;
            float izq = c.x - ancho, der = c.x + ancho, arriba = c.y + alto, abajo = c.y - alto;
            bool visible = objetivo.position.x > izq - 0.5f && objetivo.position.x < der + 0.5f;
            if (visible)
            {
                float medio = caja.x / 2 + margen;
                p.x = (der - izq > 2 * medio) ? Mathf.Clamp(p.x, izq + medio, der - medio) : c.x;
                p.y = Mathf.Min(p.y, arriba - caja.y - margen);
                p.y = Mathf.Max(p.y, abajo + margen);
            }
        }
        transform.position = p;
    }
}
