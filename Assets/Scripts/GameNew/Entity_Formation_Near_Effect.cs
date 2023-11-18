using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Formation_Near_EffectData : UnityObjectData
{
    public Entity_Formation_Near_EffectData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Formation_Near_Effect;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public Entity_Formation_Near_Effect EntityTarget => GetCom<Entity_Formation_Near_Effect>();

    private Color StartMainColor = Color.white;
    public Color MainColor = Color.white;
    public override void AfterLoad()
    {
        base.AfterLoad();
        SetMainColor(StartMainColor);
    }

    public void SetMainColorAlpha(float f_ToColor)
    {
        Color color = MainColor;
        color.a = f_ToColor;
        SetMainColor(color);
    }
    public void SetMainColor(Color f_ToColor)
    {
        MainColor = f_ToColor;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
}
public class Entity_Formation_Near_Effect : ObjectPoolBase
{
    private Entity_Formation_Near_EffectData EntityData => GetData<Entity_Formation_Near_EffectData>();
    [SerializeField]
    private List<SpriteRenderer> m_MainSprites = new();

    public void SetMainColor()
    {
        foreach (var item in m_MainSprites)
        {
            item.color = EntityData.MainColor;
        }
    }
}
