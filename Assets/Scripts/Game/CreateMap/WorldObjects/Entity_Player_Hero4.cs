using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Player_Hero4Data : Entity_HeroBaseData
{
    public Entity_Player_Hero4Data(int f_Index, int f_ChunkIndex, EHeroCradStarLevel f_HeroStarLvevl) : base(f_Index, f_ChunkIndex, f_HeroStarLvevl)
    {

    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero4;

    public override int AtkRange => 5;
    public override float AtkSpeed => 10;

    public override int HarmBase => 1;
    public override EHeroCradType HeroCradType => EHeroCradType.Hero4;


    public ESkillStage3 m_SkillStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;

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
                        case ESkillStage3.EnumCount:
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

        var eff = new TestTimeLineData(this, AttackPoint, CurAttackTarget, -HarmBase, true, DirectorWrapMode.Loop);
        var data = new Effect_Track_Line<TestTimeLineData>(eff, AtkSpeed * CurAnimaSpeed);
        GTools.RunUniTask(async () =>
        {
            await data.StartExecute();
            var range = GTools.MathfMgr.GetRandomValue(0, 100);
            if (range > 50)
            {
                IBuffUtil.InflictionBuff(EBuffType.AddBlood, this, CurAttackTarget);
            }
            else
            {
                IBuffUtil.InflictionBuff(EBuffType.Poison, this, CurAttackTarget);
            }
        });
    }
    private void SkillStage1()
    {
        IGainUtil.InflictionGain(EGainType.Launch1, this, this);
    }
    private void SkillStage2()
    {
        IGainUtil.InflictionGain(EGainType.Launch1, this, this);
    }
    private void SkillStage3()
    {
        IGainUtil.InflictionGain(EGainType.Launch1, this, this);
    }
}
public class Entity_Player_Hero4 : Entity_HeroBase
{

}
