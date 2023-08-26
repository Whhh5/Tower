using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Player_Hero4Data : Entity_HeroBaseData
{
    public Entity_Player_Hero4Data(int f_index, int f_ChunkIndex) : base(f_index, f_ChunkIndex)
    {

    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero4;

    public override int AtkRange => 5;
    public override float AtkSpeed => 10;

    public override int HarmBase => 1;
    public override EHeroCradType HeroCradType => EHeroCradType.Hero4;

    public override void AnimatorCallback000()
    {
        base.AnimatorCallback000();
        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                break;
            case EPersonStatusType.Walk:
                break;
            case EPersonStatusType.Attack:
                break;
            case EPersonStatusType.Skill:
                break;
            case EPersonStatusType.Die:
                break;
            case EPersonStatusType.Control:
                break;
            default:
                break;
        }
    }

    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();

        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                break;
            case EPersonStatusType.Walk:
                break;
            case EPersonStatusType.Attack:
                {
                    AttackTarget();
                }
                break;
            case EPersonStatusType.Skill:
                {
                    IGainUtil.InflictionGain(EGainType.Launch1, this, this);
                }
                break;
            case EPersonStatusType.Die:
                break;
            case EPersonStatusType.Control:
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
}
public class Entity_Player_Hero4 : Entity_HeroBase
{

}
