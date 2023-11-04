using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_EnergyCrystal1_Effect1Data : UnityObjectData
{
    public Entity_EnergyCrystal1_Effect1Data() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_EnergyCrystal1_Effect1;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public Entity_EnergyCrystal1_Effect1 EntityTarget => GetCom<Entity_EnergyCrystal1_Effect1>();
    public string DGIDPlayEffect => $"{EDGWorldID.EnergyCrystalEffect}_{LoadKey}";

    public override async void AfterLoad()
    {
        base.AfterLoad();

        await EntityTarget.PlayDoTweenAsync();
        ILoadPrefabAsync.UnLoad(this);
    }
    public override void OnUnLoad()
    {
        DOTween.Kill(DGIDPlayEffect);
        base.OnUnLoad();
    }
}
public class Entity_EnergyCrystal1_Effect1 : ObjectPoolBase
{
    public Entity_EnergyCrystal1_Effect1Data DataTarget => GetData<Entity_EnergyCrystal1_Effect1Data>();
    [SerializeField]
    private SpriteRenderer m_MainSpriteRender = null;

    public async UniTask PlayDoTweenAsync()
    {
        UpdateAlpha(0);
        var tempTimeNode = 0.2f;
        var delayTime = 500;
        var vanishTime = 1.0f;
        await DOTween.To(() => 0.0f, slider =>
          {
              UpdateAlpha(slider);
          }, 1.0f, tempTimeNode)
            .SetId(DataTarget.DGIDPlayEffect);
        await UniTask.Delay(delayTime);
        await DOTween.To(() => 0.0f, slider =>
        {
            var alpha = 1 - slider;
            UpdateAlpha(alpha);
        }, 1.0f, vanishTime)
            .SetId(DataTarget.DGIDPlayEffect);
    }
    private void UpdateAlpha(float f_Alpha)
    {
        var color = m_MainSpriteRender.color;
        color.a = f_Alpha;

        m_MainSpriteRender.color = color;
    }
}