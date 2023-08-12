using Cysharp.Threading.Tasks;
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
        Effect = f_EffectData;
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

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero3;
    public ESkillStage3 CurrentStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;
    public override int AtkRange => 5;
    public override float AtkSpeed => 8;
    public int AddBloodValue = 19;
    public override int HarmBase => 2;

    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();

        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                switch (CurrentStage)
                {
                    case ESkillStage3.Stage3:
                        Skill3();
                        break;
                    default:
                        break;
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
                    var eff = new TestTimeLineData(0, this, AttackPoint, CurAttackTarget, -HarmBase, true, DirectorWrapMode.Loop);
                    var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (CurrentStage)
                    {
                        case ESkillStage3.Stage1:
                            Skill1();
                            break;
                        case ESkillStage3.Stage2:
                            Skill2();
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
                switch (CurrentStage)
                {
                    case ESkillStage3.Stage3:
                        break;
                    default:
                        break;
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
            var eff = new Hero3SkillEffectData(0, AttackPoint, AddBloodValue, this);
            var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, AtkSpeed * 2);
            GTools.RunUniTask(moveData.StartExecute());
        }
    }
    public void Skill2()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                var speed = AtkSpeed * GTools.MathfMgr.GetRandomValue(2, 3.0f);
                var eff = new Hero3SkillEffectData(0, AttackPoint, AddBloodValue, this);
                var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, speed);
                GTools.RunUniTask(moveData.StartExecute());
            }
        }
    }
    public async void Skill3()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                var speed = AtkSpeed * GTools.MathfMgr.GetRandomValue(1.5f, 3.0f);
                var eff = new Hero3SkillEffectData(0, AttackPoint, AddBloodValue, this);
                var moveData = new Entity_Player_Hero3_AttackEffect<Hero3SkillEffectData>(eff, result, speed);
                GTools.RunUniTask(moveData.StartExecute());
                await UniTask.Delay(200);
            }
        }
    }

}
public class Entity_Player_Hero3 : Entity_HeroBase
{
    public Entity_Player_Hero3Data Data => GetData<Entity_Player_Hero3Data>();

}


