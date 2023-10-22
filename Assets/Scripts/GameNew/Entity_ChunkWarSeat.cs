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

    public override AssetKey AssetPrefabID => AssetKey.Entity_ChunkWarSeat;
    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    public EHeroCardType CurHeroType = EHeroCardType.EnumCount;
    public void InitData()
    {

    }
    public void AddHeroCard()
    {

    }
    public void ClearHeroCard()
    {

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


    private void OnMouseEnter()
    {
        StartEnter();
    }
    private void OnMouseExit()
    {
        StopEnter();
    }

    private void StartEnter()
    {
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
