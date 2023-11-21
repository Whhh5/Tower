using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity_Supplementary1SkillEffectData : UnityObjectData
{
    public Entity_Supplementary1SkillEffectData() : base(0)
    {
    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Supplementary1SkillEffect;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    private Entity_Supplementary1SkillEffect EntityTarget => GetCom<Entity_Supplementary1SkillEffect>();

    public Color CurMainColorStart = Color.white;
    public Color CurMainColor = Color.white;
    public override void AfterLoad()
    {
        base.AfterLoad();
        SetMainColor(CurMainColorStart);
    }

    public async void Play(Entity_Supplementary1Data f_HeroData)
    {
        var scale = f_HeroData.UtilWidth * (f_HeroData.SkillRange + 1) * f_HeroData.WeaponStartScale;
        SetLocalScale(scale);

        var time = f_HeroData.SkillToTime + f_HeroData.SkillFromTime1;
        var curColor = new Color(CurMainColorStart.r, CurMainColorStart.g, CurMainColorStart.b, 0);
        var toColor = new Color(curColor.r, curColor.g, curColor.b, 0.5f);

        await DOTween.To(() => 0.0f, slider =>
          {
              var color = Color.Lerp(curColor, toColor, slider);
              SetMainColor(color);

          }, 1.0f, time)
            .SetEase(Ease.InExpo);
        await UniTask.Delay(Mathf.CeilToInt(f_HeroData.SkillFromTime2 * 1000));
        await DOTween.To(() => 0.0f, slider =>
        {
            var color = Color.Lerp(toColor, curColor, slider);
            SetMainColor(color);

        }, 1.0f, f_HeroData.SkillFromTime3);


        ILoadPrefabAsync.UnLoad(this);
    }

    public void SetMainColor(Color? f_Color = null)
    {
        CurMainColor = f_Color ?? CurMainColor;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
}

public class Entity_Supplementary1SkillEffect : ObjectPoolBase
{
    [SerializeField]
    private SpriteRenderer m_MainSprite = null;
    [SerializeField]
    private SpriteRenderer m_MainSprite2 = null;

    private Entity_Supplementary1SkillEffectData EntityData => GetData<Entity_Supplementary1SkillEffectData>();

    public void SetMainColor()
    {
        m_MainSprite.color = EntityData.CurMainColor;
    }
}
