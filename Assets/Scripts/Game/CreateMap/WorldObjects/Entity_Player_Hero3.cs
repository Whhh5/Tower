using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Entity_Player_Hero3_AttackEffect<T> : EffectMoveBaseData
    where T : EntityEffectBaseData
{
    public Entity_Player_Hero3_AttackEffect(T f_EffectData, List<WorldObjectBaseData> f_PathList, float f_UnitSpeed = 1) : base(f_EffectData, f_EffectData.WorldPosition, f_EffectData.WorldPosition, f_UnitSpeed)
    {
        m_CurPathListIndex = 0;
        m_Initiator = f_EffectData.Initiator;
        m_TargetEnemy = f_PathList[m_CurPathListIndex];
        m_MovePathList = f_PathList;
    }
    public T Effect = null;
    private WorldObjectBaseData m_Initiator = null;
    private WorldObjectBaseData m_TargetEnemy = null;

    private List<WorldObjectBaseData> m_MovePathList = null;
    private int m_CurPathListIndex = 0;
    public override bool GetExecuteCondition()
    {
        var result = true;
        if (Vector3.Magnitude(CurPosition - TargetPosition) < 0.001f)
        {
            Effect.ExecuteEffect(m_TargetEnemy);
            m_CurPathListIndex++;
            if (m_CurPathListIndex < m_MovePathList.Count)
            {
                m_TargetEnemy = m_MovePathList[m_CurPathListIndex];
            }
            else
            {
                result = false;
            }
        }



        return result;
    }

    public override Vector3? GetPosition()
    {
        return m_TargetEnemy != null ? m_TargetEnemy.CentralPoint : TargetPosition;
    }
}
public class Entity_Player_Hero3Data : Entity_HeroBaseData
{
    public Entity_Player_Hero3Data(int f_index, int f_ChunkIndex) : base(f_index, f_ChunkIndex)
    {

    }
    public Entity_Player_Hero3 EntityMono => GetCom<Entity_Player_Hero3>();
    public Vector3 AttackPoint => EntityMono.m_AttackPoint != null ? EntityMono.m_AttackPoint.position : WorldPosition;

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero3;
    public override int SkillStageCount => 3;
    public override int AtkRange => 5;

    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();

        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                if (CurStage == 2)
                {
                    CurAnimaSpeed = 0.2f;
                }
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
            case EPersonStatusType.Attack:
                {
                    var eff = new TestTimeLineData(0, this, AttackPoint, CurAttackTarget, -HarmBase, DirectorWrapMode.Loop);
                    var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (CurStage)
                    {
                        case 0:
                            Skill1();
                            break;
                        case 1:
                            Skill2();
                            break;
                        case 2:
                            Skill3();
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
    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();
        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                if (CurStage == 2)
                {
                    CurAnimaSpeed = 1.0f;
                }
                break;
            default:
                break;
        }
    }

    public void Skill1()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            var tareget = result[0];
            var eff = new Hero3SkillEffectData(0, AttackPoint, HarmBase, this);
            var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, 5.0f);
            GTools.RunUniTask(moveData.StartExecute());
        }
    }
    public void Skill2()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            var tareget = result[0];
            var eff = new Hero3SkillEffectData(0, AttackPoint, HarmBase, this);
            var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, 5.0f);
            GTools.RunUniTask(moveData.StartExecute());
        }
    }
    public void Skill3()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            var tareget = result[0];
            var eff = new Hero3SkillEffectData(0, AttackPoint, HarmBase, this);
            var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, 5.0f);
            GTools.RunUniTask(moveData.StartExecute());
        }
    }

}
public class Entity_Player_Hero3 : Entity_HeroBase
{
    public Entity_Player_Hero3Data Data => GetData<Entity_Player_Hero3Data>();

    public Transform m_AttackPoint = null;
}


