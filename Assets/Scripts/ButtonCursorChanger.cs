using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.SetSelectionCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.SetIdleCursor();
    }
}
