using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public interface IUIUtil
{
    public static RectTransform rect => UIMgr.Ins.UICanvasRect;
    public static Camera cam => UIMgr.Ins.UICamera;
    public static Vector2 GetScreenPosByWorld(Vector3 f_WorldPos)
    {
        var cam = UIMgr.Ins.UICamera;
        var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, f_WorldPos);
        return screenPos;
    }
    public static Vector2 GetUGUIPosByWorld(Vector3 f_WorldPos, bool f_IsCentre = false)
    {
        var screenPos = GetScreenPosByWorld(f_WorldPos);
        var uguiPos = GetUGUIPositionByScreen(screenPos) + (f_IsCentre ? rect.sizeDelta * 0.5f : Vector2.zero);
        return uguiPos;
    }
    public static Vector2 GetUGUIPositionByScreen(Vector2 f_ScreenPos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, f_ScreenPos, cam, out var pos))
        {
            return pos;
        }
        return Vector2.zero;
    }
    public static Vector2 GetMouseUGUIPosition()
    {
        return GetUGUIPositionByScreen(Input.mousePosition);
    }
    public static Vector3 GetWorldPosBuyUGUIPos(Vector2 f_UGUIPos)
    {
        var ratio = f_UGUIPos / rect.sizeDelta;
        var screenPos = ratio * new Vector2(Screen.width, Screen.height);
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, cam, out var worldPos))
        {
            return worldPos;
        }
        return Vector3.zero;
    }
}
public class UIMgr : Singleton<UIMgr>
{
    public Canvas UICanvas = null;
    public RectTransform UICanvasRect => UICanvas.GetComponent<RectTransform>();
    public Camera UICamera => UICanvas.worldCamera;
    public override void Awake()
    {
        base.Awake();

        UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
    }
}
