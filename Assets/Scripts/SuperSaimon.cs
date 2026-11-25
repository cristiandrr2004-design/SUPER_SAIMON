using UnityEngine;
using UnityEngine.InputSystem;   // tu proyecto usa el Input System nuevo

// Super Saimon: camina con flechas o A/D y salta con Espacio (o W / flecha arriba).
// Tiene vidas: Monsai le quita una al tocarlo, y caerse del escenario tambien.
// Sin vidas, aparece la pantalla de derrota.
// Necesita: Rigidbody2D, un Collider2D, Tag "Player", AudioSource y (opcional) un Animator.
public class SuperSaimon : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;          // velocidad al caminar
    public float fuerzaSalto = 12f;       // fuerza del salto
    public float gravedad = 3f;           // mas alto = cae mas rapido

    [Header("Vidas y dano")]
    public int vidas = 3;
    public float tiempoInvencible = 1.5f; // segundos sin recibir dano despues de un golpe
    public float empujeGolpe = 7f;        // que tan lejos sale volando al recibir un golpe

    [Header("Caida")]
    public float limiteCaida = -10f;      // si baja de esta altura Y, pierde una vida y vuelve al inicio

    [Header("Sonidos")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoDano;

    [Header("Solo para ver en el Inspector")]
    public bool enSuelo = false;

    // Las pruebas automaticas usan esto para "presionar" teclas
    [HideInInspector] public float entradaSimulada = 0;
    [HideInInspector] public bool saltoSimulado = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private AudioSource audioSource;
    private Vector3 puntoInicio;
    private float invencibleHasta = 0;
    private float sinControlHasta = 0;    // despues de un golpe no se controla por un momento
    private bool derrotado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rb.freezeRotation = true;
        rb.gravityScale = gravedad;
        puntoInicio = transform.position;    // donde lo pusiste en la escena
        AvisarVidas();
    }

    void Update()
    {
        if (Time.timeScale == 0 || derrotado) return;   // juego detenido (ganaste o perdiste)

        Keyboard teclado = Keyboard.current;

        // Caminar: flechas o A/D
        float x = entradaSimulada;
        bool saltar = saltoSimulado;
        saltoSimulado = false;
        if (teclado != null)
        {
            if (teclado.leftArrowKey.isPressed || teclado.aKey.isPressed) x = -1;
            if (teclado.rightArrowKey.isPressed || teclado.dKey.isPressed) x = 1;
            if (teclado.spaceKey.wasPressedThisFrame || teclado.wKey.wasPressedThisFrame || teclado.upArrowKey.wasPressedThisFrame)
                saltar = true;
        }

        if (Time.time > sinControlHasta)
            rb.linearVelocity = new Vector2(x * velocidad, rb.linearVelocity.y);

        // Saltar (solo si esta en el suelo)
        if (saltar && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            enSuelo = false;
            Sonar(sonidoSalto);
        }

        // Voltear hacia donde camina
        if (x != 0)
            transform.localScale = new Vector3(Mathf.Sign(x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Animaciones: quieto, caminar, saltar (sube) y caer (baja)
        if (animator != null)
        {
            animator.SetBool("Movimiento", x != 0);
            animator.SetBool("EnSuelo", enSuelo);
            animator.SetFloat("VelY", rb.linearVelocity.y);
        }

        // Parpadeo mientras es invencible
        if (sprite != null)
            sprite.enabled = Time.time > invencibleHasta || Mathf.Repeat(Time.time * 10, 1) > 0.5f;

        // Se cayo del escenario: pierde una vida y vuelve al inicio
        if (transform.position.y < limiteCaida)
        {
            VolverAlInicio();
            PerderVida();
        }
    }

    // Solo cuenta como suelo lo que esta debajo (no las paredes)
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.GetContact(0).normal.y > 0.5f)
            enSuelo = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        enSuelo = false;
    }

    // La llama Monsai al tocarlo: pierde una vida y sale empujado
    public void RecibirGolpe(Vector3 desde)
    {
        if (derrotado || Time.time < invencibleHasta) return;   // todavia parpadeando

        float lado = Mathf.Sign(transform.position.x - desde.x);
        if (lado == 0) lado = 1;
        rb.linearVelocity = new Vector2(lado * empujeGolpe, empujeGolpe * 0.8f);
        sinControlHasta = Time.time + 0.3f;
        PerderVida();
    }

    void PerderVida()
    {
        vidas = vidas - 1;
        invencibleHasta = Time.time + tiempoInvencible;
        Sonar(sonidoDano);
        AvisarVidas();

        if (vidas <= 0)
        {
            derrotado = true;
            if (sprite != null) sprite.enabled = true;
            ContadorLibros contador = FindFirstObjectByType<ContadorLibros>();
            if (contador != null) contador.Perder();
        }
    }

    void AvisarVidas()
    {
        ContadorLibros contador = FindFirstObjectByType<ContadorLibros>();
        if (contador != null) contador.MostrarVidas(vidas);
    }

    public bool EsInvencible()
    {
        return Time.time < invencibleHasta;
    }

    public void VolverAlInicio()
    {
        transform.position = puntoInicio;
        rb.linearVelocity = Vector2.zero;
    }

    void Sonar(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
