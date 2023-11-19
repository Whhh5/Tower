using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_ChunkWarSeatData : UnityObjectData
{
    public Entity_ChunkWarSeatData() : base(0)
    {
    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_ChunkWarSeat;
    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    public EHeroCardType CurHeroType = EHeroCardType.EnumCount;
    private Entity_HeroBaseNewData m_CurHeroCardData;
    public void InitData()
    {

    }
    public bool TryGetHeroData(out Entity_HeroBaseNewData f_HeroData)
    {
        f_HeroData = m_CurHeroCardData;
        return f_HeroData != null;
    }
    public void SetCurHeroCard(Entity_HeroBaseNewData f_HeroData)
    {
        m_CurHeroCardData = f_HeroData;
    }
    public void ClearHeroCard()
    {
        m_CurHeroCardData = null;
    }
    public void OnMouseDown()
    {
        GTools.HeroCardPoolMgr.EnterWarSeat(this);

        DOTween.Kill(DgID);
        if (m_CurHeroCardData == null)
        {
            return;
        }
        m_CurHeroCardData.SetPosition(WorldPosition);
    }
    public void OnMouseUp()
    {
        if (GTools.HeroCardPoolMgr.WarSeatUpClick(this))
        {
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_ChangeWarSeat);
        }
    }
    private string DgID => "Entity_ChunkWarSeatData";
    public void ResetHeroPosition(bool f_Force = false)
    {
        if (m_CurHeroCardData == null)
        {
            return;
        }
        DOTween.Kill(DgID);
        var curPos = m_CurHeroCardData.WorldPosition;
        var time = f_Force ? 0.0f : Vector3.Distance(WorldPosition, curPos);
        DOTween.To(() => 0.0f, value =>
          {
              var target = Vector3.Lerp(curPos, WorldPosition, value);
              m_CurHeroCardData.SetPosition(target);

          }, 1.0f, time)
            .SetId(DgID);
    }
}
public class Entity_ChunkWarSeat : ObjectPoolBase
{
    private string DGID => SaveID.ToString();
    private Entity_ChunkWarSeatData Data => GetData<Entity_ChunkWarSeatData>();
    [SerializeField]
    private List<SpriteRenderer> m_SpriteRenders = new();
    [SerializeField]
    private Color m_EnterToColor = Color.cyan;
    [SerializeField]
    private Color m_OriginalColor;
    [SerializeField]
    private Color m_CurColor;
    [SerializeField]
    private bool m_IsEnter = false;
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        m_CurColor = m_OriginalColor;
        UpdateColor();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }

    private void OnMouseDown()
    {
        Data.OnMouseDown();
    }
    private void OnMouseUp()
    {

    }
    private void OnMouseEnter()
    {
        m_IsEnter = true;
        StartEnter();
    }
    private void OnMouseExit()
    {
        m_IsEnter = false;
        StopEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_IsEnter && Input.GetMouseButtonUp(0))
        {
            Data.OnMouseUp();
        }
    }

    private void StartEnter()
    {

        GTools.AudioMgr.PlayAudio(EAudioType.Scene_EnterWarSeat);

        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_EnterToColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
          {
              m_CurColor = Color.Lerp(curColor, m_EnterToColor, value);
              UpdateColor();

          }, 1.0f, moveTime * 0.1f)
            .SetId(DGID)
            .SetEase(Ease.OutExpo);
    }
    private void StopEnter()
    {
        GTools.AudioMgr.PlayAudio(EAudioType.Scene_ExitWarSeat);

        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_OriginalColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
        {
            m_CurColor = Color.Lerp(curColor, m_OriginalColor, value);
            UpdateColor();

        }, 1.0f, moveTime)
            .SetId(DGID);
    }
    private void UpdateColor()
    {
        foreach (var item in m_SpriteRenders)
        {
            item.color = m_CurColor;
        }
    }
}
