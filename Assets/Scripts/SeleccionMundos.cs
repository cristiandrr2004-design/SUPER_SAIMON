using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Seleccion de mundos: las 4 ciudades del juego, una por una, con flechas.
// Solo las ciudades desbloqueadas se pueden jugar; las demas muestran el aviso de bloqueado.
// Va en el Canvas de la escena Mundos.
public class SeleccionMundos : MonoBehaviour
{
    [System.Serializable]
    public class Mundo
    {
        public string nombre;        // Dignitown
        public string principio;     // Dignidad humana
        public string valores;       // Etica e integralidad
        public string escenario;     // Aula de clase
        public string slogan;        // ¡Aqui gane porque tu importas!
        public Sprite imagen;        // imagen de la ciudad
        public Sprite marco;         // panel con la descripcion ya dibujada (MarcoDignitown...)
        public bool desbloqueado;    // solo Dignitown por ahora
        public string escena;        // escena que se abre al entrar
    }

    public Mundo[] mundos;

    [Header("Interfaz")]
    public Image imagenCiudad;       // ciudad que se ve
    public Image imagenFundido;      // capa encima para pasar de una ciudad a otra suavemente
    public Image imagenPanel;        // panel de abajo: usa el marco del mundo si lo tiene
    public Sprite panelVacio;        // panel sin texto (Pannel) para cuando no hay marco
    public Graphic textoTitulo;         // "MUNDO 1 · DIGNITOWN" (arriba del panel)
    public Graphic textoDescripcion;    // principio, valores, escenario y slogan
    public Graphic textoEstado;         // "Haz clic en la ciudad para entrar" o "BLOQUEADO"
    public GameObject flechaIzquierda;
    public GameObject flechaDerecha;
    public CanvasGroup avisoBloqueado;
    public Graphic textoAviso;
    public string escenaInicio = "Inicio";

    [Header("Tiempos")]
    public float duracionCambio = 0.35f;
    public float duracionAviso = 1.8f;

    int actual = 0;
    bool cambiando = false;
    Coroutine rutinaAviso;

    void Start()
    {
        Time.timeScale = 1;
        if (avisoBloqueado != null) avisoBloqueado.alpha = 0;
        if (imagenFundido != null) imagenFundido.color = new Color(1, 1, 1, 0);
        Mostrar(0, false);
    }

    void Update()
    {
        var teclado = Keyboard.current;
        if (teclado == null) return;
        if (teclado.leftArrowKey.wasPressedThisFrame || teclado.aKey.wasPressedThisFrame) Anterior();
        if (teclado.rightArrowKey.wasPressedThisFrame || teclado.dKey.wasPressedThisFrame) Siguiente();
        if (teclado.enterKey.wasPressedThisFrame || teclado.spaceKey.wasPressedThisFrame) Entrar();
    }

    public void Siguiente()
    {
        if (actual < mundos.Length - 1) Mostrar(actual + 1, true);
    }

    public void Anterior()
    {
        if (actual > 0) Mostrar(actual - 1, true);
    }

    // Clic sobre la ciudad
    public void Entrar()
    {
        if (cambiando || mundos == null || mundos.Length == 0) return;
        Mundo m = mundos[actual];
        if (m.desbloqueado && !string.IsNullOrEmpty(m.escena))
        {
            Transicion.CambiarEscena(m.escena);
        }
        else
        {
            if (textoAviso != null)
                Textos.Poner(textoAviso, "¡" + m.nombre.ToUpper() + " ESTÁ BLOQUEADO!\nPrimero completa " + mundos[0].nombre + ".");
            if (rutinaAviso != null) StopCoroutine(rutinaAviso);
            rutinaAviso = StartCoroutine(MostrarAviso());
        }
    }

    public void VolverAlInicio()
    {
        Transicion.CambiarEscena(escenaInicio);
    }

    void Mostrar(int indice, bool conFundido)
    {
        if (mundos == null || mundos.Length == 0 || cambiando) return;
        actual = indice;
        Mundo m = mundos[actual];

        bool conMarco = m.marco != null && imagenPanel != null;
        if (imagenPanel != null) imagenPanel.sprite = conMarco ? m.marco : panelVacio;
        foreach (Graphic g in new[] { textoTitulo, textoDescripcion, textoEstado })
            if (g != null) g.gameObject.SetActive(!conMarco);

        if (textoTitulo != null)
            Textos.Poner(textoTitulo, "MUNDO " + (actual + 1) + " DE " + mundos.Length + "  ·  " + m.nombre.ToUpper());
        if (textoDescripcion != null)
            Textos.Poner(textoDescripcion,
                "<color=#FFCC66>Principio:</color> " + m.principio + "\n" +
                "<color=#FFCC66>Valores:</color> " + m.valores + "\n" +
                "<color=#FFCC66>Escenario:</color> " + m.escenario + "\n" +
                "<i>" + m.slogan + "</i>");
        if (textoEstado != null)
        {
            Textos.Poner(textoEstado, m.desbloqueado ? "Haz clic en la ciudad para entrar" : "BLOQUEADO  ·  Próximamente");
            textoEstado.color = m.desbloqueado ? new Color(0.55f, 1f, 0.55f) : new Color(1f, 0.45f, 0.4f);
        }
        if (flechaIzquierda != null) flechaIzquierda.SetActive(actual > 0);
        if (flechaDerecha != null) flechaDerecha.SetActive(actual < mundos.Length - 1);

        if (avisoBloqueado != null) avisoBloqueado.alpha = 0;

        if (conFundido && imagenFundido != null) StartCoroutine(Fundir(m.imagen));
        else if (imagenCiudad != null) imagenCiudad.sprite = m.imagen;
    }

    IEnumerator Fundir(Sprite nueva)
    {
        cambiando = true;
        imagenFundido.sprite = nueva;
        float t = 0;
        while (t < duracionCambio)
        {
            t += Time.unscaledDeltaTime;
            imagenFundido.color = new Color(1, 1, 1, Mathf.Clamp01(t / duracionCambio));
            yield return null;
        }
        imagenCiudad.sprite = nueva;
        imagenFundido.color = new Color(1, 1, 1, 0);
        cambiando = false;
    }

    IEnumerator MostrarAviso()
    {
        yield return Transicion.FundirPanel(avisoBloqueado, avisoBloqueado.alpha, 1, 0.2f);
        float t = 0;
        while (t < duracionAviso) { t += Time.unscaledDeltaTime; yield return null; }
        yield return Transicion.FundirPanel(avisoBloqueado, 1, 0, 0.35f);
    }
}
