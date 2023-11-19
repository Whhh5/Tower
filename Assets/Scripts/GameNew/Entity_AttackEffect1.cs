using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_AttackEffect1Data : UnityObjectData
{
    public Entity_AttackEffect1Data() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_AttackEffect1;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public Entity_AttackEffect1 EntityTarget => GetCom<Entity_AttackEffect1>();
    public float MainColorAlpha;
    public string EDGID => $"{EDGWorldID.AttackEffect1Data}_{LoadKey}";
    public override async void AfterLoad()
    {
        base.AfterLoad();

        SetMainColorAlpha(0);

        var dirX = GTools.MathfMgr.GetRandomValue(0.0f, 360.0f);
        SetLocalRotation(Vector3.forward * dirX);

        var moveTime = 0.2f;
        await DOTween.To(() => 0.0f, slider =>
          {
              SetMainColorAlpha(slider);

          }, 1.0f, moveTime * 0.8f)
            .SetId(EDGID);
        await DOTween.To(() => 0.0f, slider =>
        {
            SetMainColorAlpha(1 - slider);

        }, 1.0f, moveTime * 0.2f)
            .SetId(EDGID);
        ILoadPrefabAsync.UnLoad(this);
    }

    public void SetMainColorAlpha(float f_ToColor)
    {
        MainColorAlpha = f_ToColor;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColorAlpha();
        }
    }
}
public class Entity_AttackEffect1 : ObjectPoolBase
{
    private Entity_AttackEffect1Data EntityData => GetData<Entity_AttackEffect1Data>();
    [SerializeField]
    private List<SpriteRenderer> m_MainRenderer = null;

    private Color m_Color = Color.white;
    public void SetMainColorAlpha()
    {
        m_Color.a = EntityData.MainColorAlpha;
        foreach (var item in m_MainRenderer)
        {
            item.color = m_Color;
        }
    }
}
