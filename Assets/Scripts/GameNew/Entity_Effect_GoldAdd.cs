using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Entity_Effect_GoldAddData : UnityObjectData
{
    public Entity_Effect_GoldAddData() : base(0)
    {

    }
    private Entity_Effect_GoldAdd EntityTarget => GetCom<Entity_Effect_GoldAdd>();
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Effect_GoldAdd;

    private string DGID => $"{EDGWorldID.Entity_Effect_GoldAddData}_{LoadKey}";

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public override async void AfterLoad()
    {
        base.AfterLoad();
        DOTween.Kill(DGID);
        var startPos = WorldPosition;
        var toPosY = startPos.y + 0.5f;
        await DOTween.To(() => 0.0f, slider =>
          {
              var posY = Mathf.Lerp(startPos.y, toPosY, slider);
              var pos = startPos;
              pos.y = posY;
              SetPosition(pos);
              SetMainColor(slider);

          }, 1.0f, 0.2f)
            .SetId(DGID);
        await UniTask.Delay(300);
        startPos = WorldPosition;
        toPosY = startPos.y + 0.5f;
        await DOTween.To(() => 0.0f, slider =>
        {
            var posY = Mathf.Lerp(startPos.y, toPosY, slider);
            var pos = startPos;
            pos.y = posY;
            SetPosition(pos);
            SetMainColor(1 - slider);

        }, 1.0f, 0.2f)
            .SetId(DGID);

        ILoadPrefabAsync.UnLoad(this);
    }
    public override void UnLoad()
    {
        DOTween.Kill(DGID);
        base.UnLoad();
    }

    public float MainAlpha = 0.0f;
    public void SetMainColor(float f_ToAlpha)
    {
        MainAlpha = f_ToAlpha;
        if (PrefabTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
}
public class Entity_Effect_GoldAdd : ObjectPoolBase
{
    private Entity_Effect_GoldAddData EntityData => GetData<Entity_Effect_GoldAddData>();
    [SerializeField]
    private SpriteRenderer m_MainRootGroup = null;
    [SerializeField]
    private TextMeshPro m_TextMesh = null;
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

        
    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
        SetMainColor();
    }
    public void SetMainColor()
    {
        var color = m_MainRootGroup.color;
        color.a = EntityData.MainAlpha;
        m_MainRootGroup.color = color;

        color = m_TextMesh.color;
        color.a = EntityData.MainAlpha;
        m_TextMesh.color = color;
    }
}
