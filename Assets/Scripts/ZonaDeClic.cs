using UnityEngine;
using UnityEngine.UI;

// Area donde un boton recibe el clic. Es un hijo del boton con una Image invisible.
// Muevela y cambia su tamano con la herramienta Rect (tecla T) o en el RectTransform.
// En el editor se ve de color para poder ajustarla; al darle Play queda invisible.
[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ZonaDeClic : MonoBehaviour
{
    public bool verEnEditor = true;
    public Color colorEnEditor = new Color(0f, 1f, 0.3f, 0.35f);

    void Update()
    {
        var img = GetComponent<Image>();
        img.raycastTarget = true;
        Color c = (!Application.isPlaying && verEnEditor) ? colorEnEditor : new Color(0, 0, 0, 0);
        if (img.color != c) img.color = c;
    }
}
