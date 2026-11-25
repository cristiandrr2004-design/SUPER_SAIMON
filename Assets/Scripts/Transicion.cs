using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Transicion suave entre escenas: la pantalla se oscurece, cambia la escena y vuelve a aclarar.
// No hay que ponerla en ninguna escena: se crea sola la primera vez que se usa.
// Uso desde cualquier script:  Transicion.CambiarEscena("Nivel1");
public class Transicion : MonoBehaviour
{
    public static float duracion = 0.6f;   // segundos que dura oscurecer (y lo mismo aclarar)

    static Transicion instancia;
    CanvasGroup grupo;
    bool cambiando = false;

    public static void CambiarEscena(string escena)
    {
        Crear();
        if (!instancia.cambiando) instancia.StartCoroutine(instancia.Cambiar(escena));
    }

    static void Crear()
    {
        if (instancia != null) return;

        GameObject go = new GameObject("Transicion");
        DontDestroyOnLoad(go);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;   // siempre encima de todo

        GameObject negro = new GameObject("Negro", typeof(RectTransform));
        negro.transform.SetParent(go.transform, false);
        var rt = (RectTransform)negro.transform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        negro.AddComponent<Image>().color = Color.black;

        instancia = go.AddComponent<Transicion>();
        instancia.grupo = go.AddComponent<CanvasGroup>();
        instancia.grupo.alpha = 0;
        instancia.grupo.blocksRaycasts = false;
    }

    IEnumerator Cambiar(string escena)
    {
        cambiando = true;
        grupo.blocksRaycasts = true;   // evita clics dobles durante la transicion
        yield return Fundir(0, 1);

        Time.timeScale = 1;
        AsyncOperation carga = SceneManager.LoadSceneAsync(escena);
        while (!carga.isDone) yield return null;

        yield return Fundir(1, 0);
        grupo.blocksRaycasts = false;
        cambiando = false;
    }

    IEnumerator Fundir(float desde, float hasta)
    {
        float t = 0;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;   // funciona aunque el juego este en pausa
            grupo.alpha = Mathf.Lerp(desde, hasta, t / duracion);
            yield return null;
        }
        grupo.alpha = hasta;
    }

    // Para fundir paneles dentro de una misma escena (por ejemplo el menu y los creditos)
    public static IEnumerator FundirPanel(CanvasGroup panel, float desde, float hasta, float segundos)
    {
        float t = 0;
        while (t < segundos)
        {
            t += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(desde, hasta, t / segundos);
            yield return null;
        }
        panel.alpha = hasta;
    }
}
