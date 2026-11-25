using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Pruebas automaticas (testing) de las mecanicas y fisicas del prototipo.
// Durante el Play presiona T: Saimon hace cada prueba solo y aparece la lista con
// APROBADA o FALLIDA. Los resultados tambien salen en la Console.
// Presiona R para reiniciar el nivel despues de las pruebas.
// Va en un objeto vacio de la escena llamado "Pruebas".
public class PruebasFisicas : MonoBehaviour
{
    private SuperSaimon saimon;
    private Rigidbody2D rb;
    private ContadorLibros contador;
    private Vector3 inicio;
    private readonly List<string> resultados = new List<string>();
    private bool corriendo = false;
    private int aprobadas = 0, total = 0;

    void Start()
    {
        saimon = FindFirstObjectByType<SuperSaimon>();
        contador = FindFirstObjectByType<ContadorLibros>();
        if (saimon != null)
        {
            rb = saimon.GetComponent<Rigidbody2D>();
            inicio = saimon.transform.position;
        }
    }

    void Update()
    {
        Keyboard t = Keyboard.current;
        if (t == null) return;
        if (t.tKey.wasPressedThisFrame && !corriendo && saimon != null)
            StartCoroutine(Probar());
        if (t.rKey.wasPressedThisFrame && !corriendo && resultados.Count > 0)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator Probar()
    {
        corriendo = true;
        resultados.Clear();
        aprobadas = 0; total = 0;
        Debug.Log("===== PRUEBAS DE MECANICAS Y FISICAS: SUPER SAIMON =====");

        // 1. Gravedad: si lo soltamos en el aire, cae y aterriza en el piso
        Mover(inicio + new Vector3(0, 3, 0));
        float yAntes = saimon.transform.position.y;
        yield return EsperarSuelo(3f);
        Anotar("Gravedad: cae y aterriza en el piso", saimon.enSuelo && saimon.transform.position.y < yAntes - 2f);

        // 2. Movimiento horizontal
        float xAntes = saimon.transform.position.x;
        saimon.entradaSimulada = 1;
        yield return new WaitForSeconds(0.5f);
        saimon.entradaSimulada = 0;
        float avance = saimon.transform.position.x - xAntes;
        Anotar("Movimiento: camina a la derecha (" + avance.ToString("0.0") + " u)", avance > 1.5f);

        // 3. Salto: sube mas de 2 unidades
        yield return EsperarSuelo(2f);
        float ySuelo = saimon.transform.position.y, yMax = ySuelo;
        saimon.saltoSimulado = true;
        for (float tiempo = 0; tiempo < 1f; tiempo += Time.deltaTime)
        {
            yMax = Mathf.Max(yMax, saimon.transform.position.y);
            yield return null;
        }
        Anotar("Salto: altura " + (yMax - ySuelo).ToString("0.0") + " u", yMax - ySuelo > 2f);

        // 4. Plataformas: se puede parar encima de un mueble
        Transform plataformas = GameObject.Find("Plataformas") != null ? GameObject.Find("Plataformas").transform : null;
        if (plataformas != null && plataformas.childCount > 0)
        {
            Collider2D mueble = plataformas.GetChild(0).GetComponent<Collider2D>();
            Mover(new Vector3(mueble.bounds.center.x, mueble.bounds.max.y + 1f, 0));
            yield return EsperarSuelo(2f);
            bool encima = saimon.enSuelo && saimon.transform.position.y > mueble.bounds.max.y - 0.2f;
            Anotar("Plataforma: se para sobre '" + mueble.name + "'", encima);

            // 5. Plataforma de un solo lado: la atraviesa saltando desde abajo
            Mover(new Vector3(mueble.bounds.center.x, mueble.bounds.min.y - 1.8f, 0));
            yield return EsperarSuelo(2f);
            saimon.saltoSimulado = true;
            yield return new WaitForSeconds(0.25f);
            Anotar("Plataforma de un solo lado: la atraviesa desde abajo", saimon.transform.position.y > mueble.bounds.min.y);
            yield return EsperarSuelo(2f);
        }

        // 6. Recoger un libro: el contador sube
        Libro libro = FindFirstObjectByType<Libro>();
        if (libro != null && contador != null)
        {
            int antes = contador.Recogidos();
            Mover(libro.transform.position);
            yield return new WaitForSeconds(0.3f);
            Anotar("Interaccion: recoge un libro (" + antes + " -> " + contador.Recogidos() + ")", contador.Recogidos() == antes + 1);
        }

        // 7. Dano: Monsai le quita una vida
        Monsai monsai = FindFirstObjectByType<Monsai>();
        if (monsai != null)
        {
            int vidasAntes = saimon.vidas;
            Mover(monsai.transform.position);
            yield return new WaitForSeconds(0.2f);
            Anotar("Dano: Monsai quita una vida (" + vidasAntes + " -> " + saimon.vidas + ")", saimon.vidas == vidasAntes - 1);

            // 8. Invencibilidad: un segundo toque seguido no quita otra vida
            int vidasDespues = saimon.vidas;
            Mover(monsai.transform.position);
            yield return new WaitForSeconds(0.2f);
            Anotar("Invencibilidad: no pierde otra vida de inmediato", saimon.vidas == vidasDespues && saimon.EsInvencible());
        }

        // 9. Caida al vacio: pierde una vida y vuelve al inicio
        int vidasCaida = saimon.vidas;
        Mover(new Vector3(inicio.x, saimon.limiteCaida - 1f, 0));
        yield return new WaitForSeconds(0.2f);
        bool volvio = Vector2.Distance(saimon.transform.position, inicio) < 2f;
        Anotar("Caida: vuelve al inicio y pierde una vida", volvio && saimon.vidas == vidasCaida - 1);

        Debug.Log("===== RESULTADO: " + aprobadas + " de " + total + " pruebas aprobadas =====");
        corriendo = false;
    }

    void Mover(Vector3 posicion)
    {
        rb.linearVelocity = Vector2.zero;
        rb.position = posicion;
        saimon.transform.position = posicion;
    }

    IEnumerator EsperarSuelo(float maximo)
    {
        yield return new WaitForSeconds(0.1f);
        for (float t = 0; t < maximo && !saimon.enSuelo; t += Time.deltaTime)
            yield return null;
        yield return new WaitForSeconds(0.1f);
    }

    void Anotar(string prueba, bool ok)
    {
        total++;
        if (ok) aprobadas++;
        string linea = (ok ? "APROBADA  " : "FALLIDA   ") + prueba;
        resultados.Add(linea);
        if (ok) Debug.Log(linea); else Debug.LogWarning(linea);
    }

    // Lista de resultados en pantalla
    void OnGUI()
    {
        if (resultados.Count == 0 && !corriendo) return;   // no se muestra nada hasta presionar T
        var estilo = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 18 };
        estilo.normal.textColor = Color.white;
        string texto = "PRUEBAS DE MECANICAS Y FISICAS\n\n" + string.Join("\n", resultados);
        texto += corriendo ? "\n\nProbando..." : "\n\nResultado: " + aprobadas + " de " + total + " aprobadas   (R = reiniciar)";
        GUI.Box(new Rect(Screen.width - 620, 20, 600, 60 + 26 * (resultados.Count + 3)), texto, estilo);
    }
}
