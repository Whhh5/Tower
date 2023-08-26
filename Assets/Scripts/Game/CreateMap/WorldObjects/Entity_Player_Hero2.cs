using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EffMoveData
{

}
public class Entity_Player_Hero2Data : Entity_HeroBaseData
{
    public Entity_Player_Hero2Data(int f_Index, int f_TargetIndex) : base(f_Index, f_TargetIndex)
    {

    }
    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero2;

    private Entity_Player_Hero2 Target => GetCom<Entity_Player_Hero2>();

    public Vector3 SkillPoint2 => Target != null && Target.m_SkillPoint2 != null ? Target.m_SkillPoint2.position : WorldPosition;
    public override int AtkRange => 5;
    public override float AtkSpeed => 8;
    public ESkillStage3 CurrentStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;

    public override int HarmBase => 5;

    public override EHeroCradType HeroCradType => EHeroCradType.Hero2;

    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                {
                    var eff = new TestTimeLineData( this, AttackPoint, CurAttackTarget, -HarmBase, true, DirectorWrapMode.Loop);
                    var data = new Effect_Track_Line<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (CurrentStage)
                    {
                        case ESkillStage3.Stage1:
                            SkillStage1();
                            break;
                        case ESkillStage3.Stage2:
                            SkillStage2();
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
    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();

        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                {
                    switch (CurrentStage)
                    {
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

    public void SkillStage1()
    {
        var eff = new TestTimeLineData( this, AttackPoint, CurAttackTarget, -Mathf.CeilToInt(HarmBase * 1), false, DirectorWrapMode.Loop);
        var data = new Effect_Track_Line<TestTimeLineData>(eff, AtkSpeed);
        GTools.RunUniTask(data.StartExecute());
    }

    public async void SkillStage2()
    {
        var targets = GTools.MathfMgr.GetTargstsByForwardAngle<WorldObjectBaseData>(SkillPoint2, Forward, 30, AtkRange + 3, ELayer.Enemy);
        for (int i = 0; i < 1; i++)
        {
            foreach (var item in targets)
            {
                var rangeSpeed = GTools.MathfMgr.GetRandomValue(6.0f, 10.0f);
                var eff = new TestTimeLineData( this, SkillPoint2, item, -Mathf.CeilToInt(HarmBase * 1.5f), false, DirectorWrapMode.Loop);
                var data = new Effect_Track_Line<TestTimeLineData>(eff, rangeSpeed);
                GTools.RunUniTask(data.StartExecute());
            }

            await UniTask.Delay(100);
        }
    }
    public async void SkillStage3()
    {
        var targets = GTools.MathfMgr.GetTargstsByForwardAngle<WorldObjectBaseData>(SkillPoint2, Forward, 30, AtkRange + 6, ELayer.Enemy);
        for (int i = 0; i < 5; i++)
        {
            foreach (var item in targets)
            {
                var rangeSpeed = GTools.MathfMgr.GetRandomValue(AtkSpeed - 2.0f, AtkSpeed + 3.0f);
                var eff = new TestTimeLineData( this, SkillPoint2, item, -HarmBase * 2, false, DirectorWrapMode.Loop);
                var data = new Effect_Track_Line<TestTimeLineData>(eff, rangeSpeed);
                GTools.RunUniTask(data.StartExecute());
            }

            await UniTask.Delay(100);
        }
    }
}
public class Entity_Player_Hero2 : Entity_HeroBase
{
    [SerializeField]
    public Transform m_SkillPoint1 = null;
    [SerializeField]
    public Transform m_SkillPoint2 = null;
    [SerializeField]
    public Transform m_SkillPoint3 = null;
}
