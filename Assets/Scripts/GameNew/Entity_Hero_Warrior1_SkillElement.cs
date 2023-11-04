using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Hero_Warrior1_SkillElementData : Entity_SkillElementBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Hero_Warrior1_SkillElement;

    public Entity_Hero_Warrior1_SkillElement EntityTarget => GetCom<Entity_Hero_Warrior1_SkillElement>();

    public Color StartMainColor => Color.white;
    public Color MainColor;

    public override void AfterLoad()
    {
        base.AfterLoad();
        SetMainColor(StartMainColor);
    }

    public void SetMainColor(Color? f_Color = null)
    {
        MainColor = f_Color ?? StartMainColor;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
}

public class Entity_Hero_Warrior1_SkillElement : Entity_SkillElementBase
{
    public Entity_Hero_Warrior1_SkillElementData SkillData => GetData<Entity_Hero_Warrior1_SkillElementData>();
    [SerializeField]
    private List<SpriteRenderer> m_MainRenderList = new();
    public void SetMainColor()
    {
        foreach (var item in m_MainRenderList)
        {
            item.color = SkillData.MainColor;
        }
    }
}
