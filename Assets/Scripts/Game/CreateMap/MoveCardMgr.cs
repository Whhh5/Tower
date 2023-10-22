using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using UnityEngine.UI;

public class MoveCardMgr : MonoSingleton<MoveCardMgr>, IUpdateBase
{
    private EQualityType m_CurSelectHero = EQualityType.EnumCount;
    public int UpdateLevelID { get; set; }
    [SerializeField]
    private RectTransform m_TargetIcon = null;
    [SerializeField]
    private RectTransform m_TargetArrow = null;
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }

    [SerializeField]
    private Entity_HeroBaseData m_CurSelectHeroEntity = null;
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level1;

    protected override void Awake()
    {
        base.Awake();

        m_TargetIcon.gameObject.SetActive(false);
        m_TargetArrow.gameObject.SetActive(false);
    }
    private Vector2 m_MouseClickDownPos = Vector2.zero;
    public void SetCurSelectHero(EQualityType f_Quality)
    {
        if (f_Quality != m_CurSelectHero)
        {
            if (TryGetCurrentMousePos(out var pos))
            {
                m_MouseClickDownPos = pos;
                m_TargetArrow.anchoredPosition = pos;
            }
            m_CurSelectHero = f_Quality;
            GTools.LifecycleMgr.AddUpdate(this);
            m_TargetIcon.gameObject.SetActive(true);
            m_TargetArrow.gameObject.SetActive(true);
        }
    }
    public void SetCurSelectHero(Entity_HeroBaseData f_HeroType)
    {
        return;
        if (TryGetCurrentMousePos(out var pos))
        {
            m_MouseClickDownPos = pos;
            m_TargetArrow.anchoredPosition = pos;
        }
        m_CurSelectHeroEntity = f_HeroType;
        GTools.LifecycleMgr.AddUpdate(this);
        m_TargetArrow.gameObject.SetActive(true);

    }
    private void EndCurSelectHero()
    {
        m_TargetIcon.gameObject.SetActive(false);
        m_TargetArrow.gameObject.SetActive(false);
        m_CurSelectHeroEntity = null;
        var curSelectChunk = WorldMapMgr.Ins.GetCurMouseEnable();
        if (WorldMapMgr.Ins.TryGetChunkData(curSelectChunk, out var chunkData))
        {
            if (chunkData.CurObjectType == EWorldObjectType.Road)
            {
                
            }
        }
        m_CurSelectHero = EQualityType.EnumCount;
    }
    public void SetTargetIcon(Sprite f_Sprite)
    {
        m_TargetIcon.GetComponent<Image>().sprite = f_Sprite;
    }

    public void OnUpdate()
    {
        if (TryGetCurrentMousePos(out var pos))
        {
            if (m_CurSelectHero != EQualityType.EnumCount)
            {
                m_TargetIcon.anchoredPosition = pos;
            }
            if (m_CurSelectHeroEntity != null)
            {

            }
            var forward = pos - m_MouseClickDownPos;
            m_TargetArrow.up = forward.normalized;
            var size = m_TargetArrow.sizeDelta;
            size.y = Vector2.Distance(forward, Vector2.zero);
            m_TargetArrow.sizeDelta = size;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurSelectHeroEntity != null && PathManager.Ins.TryGetAStarPath(m_CurSelectHeroEntity.CurrentIndex, WorldMapMgr.Ins.GetCurMouseEnable(), out var path))
            {
                m_CurSelectHeroEntity.SetPath(path);
            }
            EndCurSelectHero();
            GTools.LifecycleMgr.RemoveUpdate(this);
        }
    }

    [SerializeField]
    private Camera m_TargetCamera = null;
    [SerializeField]
    private RectTransform m_TargetRect = null;
    private bool TryGetCurrentMousePos(out Vector2 f_Pos)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(m_TargetRect, Input.mousePosition, m_TargetCamera, out f_Pos);
    }
}
