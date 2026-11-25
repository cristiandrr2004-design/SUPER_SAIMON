using UnityEngine;

// Escena de creditos: el boton VOLVER regresa al inicio con un fundido.
// Va en el Canvas de la escena Creditos.
public class PantallaCreditos : MonoBehaviour
{
    public string escenaInicio = "Inicio";

    void Start()
    {
        Time.timeScale = 1;
    }

    public void Volver()
    {
        Transicion.CambiarEscena(escenaInicio);
    }
}
