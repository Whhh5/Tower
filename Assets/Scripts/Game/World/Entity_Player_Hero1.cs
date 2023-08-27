using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Player_Hero1Data : Entity_HeroBaseData
{
    public Entity_Player_Hero1Data(int f_Index, int f_TargetIndex, EHeroCradStarLevel f_StarLevel) : base(f_Index, f_TargetIndex, f_StarLevel)
    {

    }
    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero1;

    public override EHeroCradType HeroCradType => EHeroCradType.Hero1;

    private ESkillStage3 m_SkillStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;


    private Emitter_SwordBaseData m_SwordBase = null;

    public override int AtkRange => 5;
    public override void AfterLoad()
    {
        base.AfterLoad();


        var weaponData = new Emitter_SwordLowData(0, this);
        m_SwordBase = weaponData;
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(weaponData));
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
        GTools.MathfMgr.EntityDamage(this, CurAttackTarget, EDamageType.Physical, -HarmBase, true);

        GTools.RunUniTask(m_SwordBase.StartExecute());
    }

    public void SkillStage1()
    {

    }

    public void SkillStage2()
    {
        GTools.RunUniTask(m_SwordBase.StopExecute());
    }

    public void SkillStage3()
    {

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
