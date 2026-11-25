using UnityEngine;
using UnityEngine.UI;

// El boton solo recibe el clic dentro de su BoxCollider2D.
// Para ajustar el area: selecciona el boton, en el BoxCollider2D pulsa "Edit Collider"
// y arrastra los puntos verdes en la escena (o cambia Offset y Size).
[RequireComponent(typeof(BoxCollider2D))]
public class AreaDeClic : MonoBehaviour, ICanvasRaycastFilter
{
    BoxCollider2D area;

    public bool IsRaycastLocationValid(Vector2 puntoPantalla, Camera camara)
    {
        if (area == null) area = GetComponent<BoxCollider2D>();
        if (area == null || !area.enabled) return true;
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, puntoPantalla, camara, out local))
            return false;
        Vector2 d = local - area.offset;
        return Mathf.Abs(d.x) <= area.size.x / 2 && Mathf.Abs(d.y) <= area.size.y / 2;
    }
}
