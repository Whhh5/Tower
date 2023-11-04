using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Player_Hero1Data : Entity_HeroBaseData
{
    public Entity_Player_Hero1Data(EHeroCradStarLevel f_StarLevel) : base(f_StarLevel)
    {

    }
    public override EEntityType EntityType => EEntityType.Person;
    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Player_Hero1;

    public override EHeroCardType HeroCradType => EHeroCardType.Hero1;

    private ESkillStage3 m_SkillStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;


    private Emitter_SwordLowData m_SwordLow = null;
    public Emitter_SwordLowData SwordLow => m_SwordLow;

    public override int AtkRange => 5;
    public override float AtkSpeed => 5.0f;
    protected override float AtkSpeedBase => 3.0f;

    public override void AfterLoad()
    {
        base.AfterLoad();


        var weaponData = new Emitter_SwordLowData(0, this);
        m_SwordLow = weaponData;
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(weaponData));
    }
    public void StartExecuteSword()
    {
        GTools.RunUniTask(m_SwordLow.StartExecute());
    }
    public void StopExecuteSword()
    {
        GTools.RunUniTask(m_SwordLow.StopExecute());
    }

    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                {
                    AttackTarget();
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (m_SkillStage)
                    {
                        case ESkillStage3.Stage1:
                            SkillStage1();
                            break;
                        case ESkillStage3.Stage2:
                            SkillStage2();
                            break;
                        case ESkillStage3.Stage3:
                            SkillStage3();
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    public override void AttackTarget()
    {
        base.AttackTarget();
        GTools.MathfMgr.EntityDamage(this, CurAttackTarget, EDamageType.Physical, -CurHarm, CurSkillCount > 0);

        GTools.RunUniTask(m_SwordLow.StartExecute());
    }

    public void SkillStage1()
    {
        var skillInfo = GetSkill(ESkillIndex.Skill_1);
        skillInfo.SkillData.StartExecute();
    }

    public void SkillStage2()
    {
        //GTools.RunUniTask(m_SwordLow.StopExecute());
        var skillInfo = GetSkill(ESkillIndex.Skill_2);
        skillInfo.SkillData.StartExecute();
    }

    public void SkillStage3()
    {
        var skillInfo = GetSkill(ESkillIndex.Skill_3);
        skillInfo.SkillData.StartExecute();
    }
}
public class Entity_Player_Hero1 : Entity_HeroBase
{
    public Entity_Player_Hero1Data Data => GetData<Entity_Player_Hero1Data>();
    public override async UniTask OnClickAsync()
    {
        await base.OnClickAsync();


    }
    public override async UniTask OnClick2Async()
    {
        await base.OnClick2Async();

    }


    public void Run()
    {

    }
}
