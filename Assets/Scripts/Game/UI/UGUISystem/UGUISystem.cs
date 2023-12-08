using System.Collections;
using System.Collections.Generic;
using B1.UI;
using UnityEngine;
using B1;
public class UGUISystem : MonoSingleton<UGUISystem>
{
    public Camera UICamera = null;
    public Canvas MainCanvas = null;
    public RectTransform CanvasRect = null;


    public Vector2 GetUGUIPosByMouse()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect,
            Input.mousePosition, UICamera, out var uipos);
        return uipos;
    }
}
