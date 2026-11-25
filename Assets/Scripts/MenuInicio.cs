using UnityEngine;

// Pantalla de inicio: letreros INICIO y CREDITOS. Cada uno abre su escena con un fundido.
// Va en el Canvas de la escena Inicio.
public class MenuInicio : MonoBehaviour
{
    public string escenaJuego = "Nivel1";      // escena que se abre con INICIO
    public string escenaCreditos = "Creditos"; // escena que se abre con CREDITOS

    void Start()
    {
        Time.timeScale = 1;
    }

    public void Jugar()
    {
        Transicion.CambiarEscena(escenaJuego);
    }

    public void VerCreditos()
    {
        Transicion.CambiarEscena(escenaCreditos);
    }
}
