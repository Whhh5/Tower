using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EffMoveData
{

}
public class Entity_Player_Hero2Data : Person_EnemyData
{
    public enum ESkillStage
    {
        Stage1,
        Stage2,
        Stage3,
        EnumCount,
    }
    public Entity_Player_Hero2Data(int f_Index, int f_TargetIndex, Entity_SpawnPointData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {

    }
    public override ELayer LayerMask => ELayer.Player;
    public override ELayer AttackLayerMask => ELayer.Enemy;
    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero2;

    private Entity_Player_Hero2 Target => GetCom<Entity_Player_Hero2>();

    public Vector3 AttackPoint => Target != null && Target.m_AttackPoint != null ? Target.m_AttackPoint.position : WorldPosition;
    public Vector3 SkillPoint2 => Target != null && Target.m_SkillPoint2 != null ? Target.m_SkillPoint2.position : WorldPosition;
    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                {
                    var eff = new TestTimeLineData(0, this, AttackPoint, m_CurTarget, -HarmBase, DirectorWrapMode.Loop);
                    var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, 5);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (m_CurStage)
                    {
                        case ESkillStage.Stage1:
                            SkillStage1();
                            break;
                        case ESkillStage.Stage2:
                            SkillStage2();
                            break;
                            break;
                        case ESkillStage.EnumCount:
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
                    switch (m_CurStage)
                    {
                        case ESkillStage.Stage3:
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
    public int m_CurSkillCount = 0;
    public ESkillStage m_CurStage => (ESkillStage)(m_CurSkillCount % (int)ESkillStage.EnumCount);
    public override string GetCurrentAnimationName()
    {
        var curName = base.GetCurrentAnimationName();

        if (CurStatus == EPersonStatusType.Skill)
        {
            curName = $"{curName}_{(int)m_CurStage + 1}";
        }
        return curName;
    }
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();

        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                {
                    CurrentMagic = 0;
                    m_CurSkillCount++;
                }
                break;
            default:
                break;
        }
    }

    public void SkillStage1()
    {
        var eff = new TestTimeLineData(0, this, AttackPoint, m_CurTarget, -Mathf.CeilToInt(HarmBase * 1.5f), DirectorWrapMode.Loop);
        var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, 5);
        GTools.RunUniTask(data.StartExecute());
    }

    public async void SkillStage2()
    {
        var targets = GTools.MathfMgr.GetTargstsByForwardAngle<WorldObjectBaseData>(SkillPoint2, Forward, 30, 5, ELayer.Enemy);
        for (int i = 0; i < 1; i++)
        {
            foreach (var item in targets)
            {
                var rangeSpeed = GTools.MathfMgr.GetRandomValue(5, 8.0f);
                var eff = new TestTimeLineData(0, this, SkillPoint2, item, -HarmBase * 3, DirectorWrapMode.Loop);
                var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, rangeSpeed);
                GTools.RunUniTask(data.StartExecute());
            }

            await UniTask.Delay(100);
        }
    }
    public async void SkillStage3()
    {
        var targets = GTools.MathfMgr.GetTargstsByForwardAngle<WorldObjectBaseData>(SkillPoint2, Forward, 30, 5, ELayer.Enemy);
        for (int i = 0; i < 5; i++)
        {
            foreach (var item in targets)
            {
                var rangeSpeed = GTools.MathfMgr.GetRandomValue(5, 8.0f);
                var eff = new TestTimeLineData(0, this, SkillPoint2, item, -HarmBase * 3, DirectorWrapMode.Loop);
                var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, rangeSpeed);
                GTools.RunUniTask(data.StartExecute());
            }

            await UniTask.Delay(100);
        }
    }
}
public class Entity_Player_Hero2 : Person_Enemy
{
    [SerializeField]
    public Transform m_AttackPoint = null;
    [SerializeField]
    public Transform m_SkillPoint1 = null;
    [SerializeField]
    public Transform m_SkillPoint2 = null;
    [SerializeField]
    public Transform m_SkillPoint3 = null;
}
