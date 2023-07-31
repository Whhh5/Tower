
using B1;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button3D : MonoBehaviour
{
    [SerializeField]
    Vector3 m_OriginScale = Vector3.one;
    [SerializeField]
    Vector3 m_ToScale = new Vector3(1.1f, 1.1f, 1.1f);

    [SerializeField]
    float m_ClickTime = 0.0f;
    [SerializeField]
    float m_Click2Interval = 1;


    List<IButton3DClick> m_ClickList = new();
    List<IButton3DClick> m_ClickList2 = new();
    private void OnMouseEnter()
    {
        DOTween.Kill(EUIMapDGIDType.Button3DOnMouseEnter);
        var curSize = transform.localScale;
        var moveTime = Vector3.Distance(curSize, m_ToScale);
        DOTween.To(() => 0.0f, (value) =>
              {
                  transform.localScale = Vector3.Lerp(curSize, m_ToScale, value);

              }, 1.0f, moveTime)
                .SetId(EUIMapDGIDType.Button3DOnMouseEnter);
    }
    private void OnMouseExit()
    {
        DOTween.Kill(EUIMapDGIDType.Button3DOnMouseEnter);
        var curSize = transform.localScale;
        var moveTime = Vector3.Distance(curSize, m_OriginScale);
        DOTween.To(() => 0.0f, (value) =>
        {
            transform.localScale = Vector3.Lerp(curSize, m_OriginScale, value);

        }, 1.0f, moveTime)
                .SetId(EUIMapDGIDType.Button3DOnMouseEnter);
    }
    private async void OnMouseUpAsButton()
    {
        var curTime = Time.time;
        if (Mathf.Abs(curTime - m_ClickTime) > m_Click2Interval)
        {
            await GTools.ParallelTaskAsync(m_ClickList, async (value) => await value.OnClickAsync());
        }
        else
        {
            await GTools.ParallelTaskAsync(m_ClickList2, async (value) => await value.OnClick2Async());
        }
        m_ClickTime = curTime;
    }

    public void AddListener(IButton3DClick f_Func)
    {
        if (!m_ClickList.Contains(f_Func))
        {
            m_ClickList.Add(f_Func);
        }
    }
    public void RemoveListener(IButton3DClick f_Func)
    {
        if (m_ClickList.Contains(f_Func))
        {
            m_ClickList.Remove(f_Func);
        }
    }
    public void AddListener2(IButton3DClick f_Func)
    {
        if (!m_ClickList2.Contains(f_Func))
        {
            m_ClickList2.Add(f_Func);
        }
    }
    public void RemoveListener2(IButton3DClick f_Func)
    {
        if (m_ClickList2.Contains(f_Func))
        {
            m_ClickList2.Remove(f_Func);
        }
    }
}
