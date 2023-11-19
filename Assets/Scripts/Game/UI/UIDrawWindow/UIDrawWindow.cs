using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;

public class UIDrawWindow : UIWindow
{
    [SerializeField]
    private RectTransform m_ImgRoot = null;
    [SerializeField]
    private RectTransform m_DownImg = null;
    [SerializeField]
    private RectTransform m_LineImg = null;
    [SerializeField]
    private RectTransform m_MouseImg = null;
    public override async UniTask AwakeAsync()
    {
        m_IsMouseDown = false;
        OnMouseUp();
    }

    public override async UniTask OnShowAsync()
    {
        
    }

    private bool m_IsMouseDown = false;
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            m_IsMouseDown = true;
            OnMouseDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_IsMouseDown = false;
            OnMouseUp();
        }
        if (m_IsMouseDown)
        {
            OnMouseDrag();
        }
    }

    private Vector2 m_StartPosition = Vector3.zero;
    private void OnMouseDown()
    {
        Vector2 mousePos = Input.mousePosition;
        m_ImgRoot.gameObject.SetActive(true);
        m_StartPosition = 
            m_LineImg.anchoredPosition = 
            m_MouseImg.anchoredPosition = 
            m_DownImg.anchoredPosition = mousePos;
    }
    private void OnMouseUp()
    {
        m_ImgRoot.gameObject.SetActive(false);
    }
    private void OnMouseDrag()
    {
        Vector2 mousePos = Input.mousePosition;
        m_MouseImg.anchoredPosition = mousePos;
        var sizeX = Vector2.Distance(mousePos, m_StartPosition);
        m_LineImg.sizeDelta = new Vector2(sizeX, m_LineImg.sizeDelta.y);
        m_LineImg.right = (mousePos - m_StartPosition).normalized;
    }
}
