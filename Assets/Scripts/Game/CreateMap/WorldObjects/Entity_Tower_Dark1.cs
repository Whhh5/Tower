using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Tower_Dark1Data : EntityTowerBaseData
{
    public Entity_Tower_Dark1Data() : base()
    {
    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Tower_Dark1;

    public override ETowerType TowerType =>  ETowerType.Dark;

    public override ELayer LayerMask => ELayer.Player;

    private ESkillStage3 m_SkillStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;

    protected override float AtkIntervalOriginal => 1.0f;

    public override void AttackTarget()
    {
        base.AttackTarget();
    }
    public override void AnimatorCallback000()
    {
        base.AnimatorCallback000();
    }
    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();
    }
    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();
    }
    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();
    }
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();
    }
}
public class Entity_Tower_Dark1 : EntityTowerBase
{

}
