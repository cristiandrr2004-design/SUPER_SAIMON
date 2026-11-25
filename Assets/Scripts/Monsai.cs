using UnityEngine;

// Monsai, el antagonista: camina de un lado a otro por el piso.
// Si toca a Super Saimon, le quita una vida y lo empuja.
// Necesita: SpriteRenderer, un Collider2D con "Is Trigger" y un Rigidbody2D en Kinematic.
public class Monsai : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float limiteIzquierdo = -3f;     // hasta donde camina (posicion X)
    public float limiteDerecho = 3f;

    [Header("Animacion de caminar (cuadros del sprite Enemigo)")]
    public Sprite[] cuadros;
    public float cuadrosPorSegundo = 8f;

    private SpriteRenderer sprite;
    private int direccion = -1;             // -1 = izquierda, 1 = derecha

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Caminar
        transform.position += new Vector3(direccion * velocidad * Time.deltaTime, 0, 0);
        if (transform.position.x < limiteIzquierdo) direccion = 1;
        if (transform.position.x > limiteDerecho) direccion = -1;

        // El dibujo mira a la derecha: se voltea cuando camina a la izquierda
        if (sprite != null)
        {
            sprite.flipX = direccion < 0;
            if (cuadros != null && cuadros.Length > 0)
                sprite.sprite = cuadros[(int)(Time.time * cuadrosPorSegundo) % cuadros.Length];
        }
    }

    void OnTriggerEnter2D(Collider2D otro) { Golpear(otro); }
    void OnTriggerStay2D(Collider2D otro) { Golpear(otro); }

    void Golpear(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;
        SuperSaimon saimon = otro.GetComponent<SuperSaimon>();
        if (saimon != null) saimon.RecibirGolpe(transform.position);
    }
}
