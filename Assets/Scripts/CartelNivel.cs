using System.Collections;
using UnityEngine;

// Cartel grande al empezar el nivel ("NIVEL 2: El pasillo de las burlas") que se desvanece solo.
// Va en un objeto del Canvas con CanvasGroup.
public class CartelNivel : MonoBehaviour
{
    public float segundos = 3.5f;

    IEnumerator Start()
    {
        var grupo = GetComponent<CanvasGroup>();
        if (grupo == null) grupo = gameObject.AddComponent<CanvasGroup>();
        grupo.alpha = 1;
        grupo.blocksRaycasts = false;
        yield return new WaitForSecondsRealtime(segundos);
        yield return Transicion.FundirPanel(grupo, 1, 0, 0.8f);
        gameObject.SetActive(false);
    }
}
