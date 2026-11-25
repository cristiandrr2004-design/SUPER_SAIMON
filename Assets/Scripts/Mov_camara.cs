using UnityEngine;

// Camara que sigue al Jugador de forma suave. Va en la Main Camera.
// Con "Usar Limites" no se sale del fondo del escenario.
public class Mov_camara : MonoBehaviour
{
    public Transform objetivo;                               // arrastra aqui al Jugador
    public float velocidadCamara = 5f;                       // mas alto = mas pegada
    public Vector3 desplazamiento = new Vector3(0, 1, -10);  // Z debe quedar en -10

    [Header("Limites (los bordes del fondo)")]
    public bool usarLimites = false;
    public Vector2 limiteMin = new Vector2(-12, -7);         // esquina de abajo a la izquierda
    public Vector2 limiteMax = new Vector2(12, 7);           // esquina de arriba a la derecha

    private Camera cam;

    // Al empezar el nivel, la camara aparece de una vez sobre el Jugador
    void Start()
    {
        cam = GetComponent<Camera>();
        if (objetivo != null)
            transform.position = Limitar(objetivo.position + desplazamiento);
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = Limitar(objetivo.position + desplazamiento);
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara * Time.deltaTime);
    }

    // Evita que la camara muestre lo que hay fuera del fondo
    Vector3 Limitar(Vector3 p)
    {
        if (!usarLimites || cam == null) return p;

        float altoMitad = cam.orthographicSize;
        float anchoMitad = altoMitad * cam.aspect;

        p.x = LimitarEje(p.x, limiteMin.x + anchoMitad, limiteMax.x - anchoMitad);
        p.y = LimitarEje(p.y, limiteMin.y + altoMitad, limiteMax.y - altoMitad);
        return p;
    }

    float LimitarEje(float valor, float min, float max)
    {
        if (min > max) return (min + max) / 2;   // el fondo es mas pequeno que la pantalla
        return Mathf.Clamp(valor, min, max);
    }
}
