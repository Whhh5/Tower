using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Hero_EnchanterEffectData : UnityObjectData
{
    public Entity_Hero_EnchanterEffectData() : base(0)
    {
    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Hero_EnchanterEffect;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public Entity_Hero_EnchanterEffect EntityTarget => GetCom<Entity_Hero_EnchanterEffect>();

    public string DGID_Move => $"{EDGWorldID.EffectMove}_{LoadKey}";

    public override void OnUnLoad()
    {
        base.OnUnLoad();

        SetMainColor(StartMainColor);
    }
    
    public async void DestroyAsync()
    {
        var delayTime = Mathf.CeilToInt(GetDurationTime()) * 1000;
        await UniTask.Delay(delayTime);
        ILoadPrefabAsync.UnLoad(this);
    }
    public float GetDurationTime()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetDurationTime();
        }
        return 0;
    }
    public Color StartMainColor = Color.white;
    public Color MainColor = Color.white;
    public void SetMainColor(Color? f_Color = null)
    {
        MainColor = f_Color ?? StartMainColor;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
}
public class Entity_Hero_EnchanterEffect : ObjectPoolBase
{
    [SerializeField]
    private TrailRenderer m_TrailRender = null;
    [SerializeField]
    private SpriteRenderer m_MainSprite = null;

    private Entity_Hero_EnchanterEffectData EntityData => GetData<Entity_Hero_EnchanterEffectData>();
    public float GetDurationTime()
    {
        var time = m_TrailRender.time;
        return time;
    }
    public void SetMainColor()
    {
        m_MainSprite.color = m_TrailRender.startColor = EntityData.MainColor;
    }
}
