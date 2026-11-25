using TMPro;
using UnityEngine.UI;

// Pone texto en un Text normal o en un TextMeshPro, el que tenga el objeto.
public static class Textos
{
    public static void Poner(Graphic destino, string texto)
    {
        if (destino is TMP_Text tmp) tmp.text = texto;
        else if (destino is Text legacy) legacy.text = texto;
    }
}
