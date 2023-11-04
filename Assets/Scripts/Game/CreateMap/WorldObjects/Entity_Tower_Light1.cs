using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Tower_Light1Data : EntityTowerBaseData
{
    public Entity_Tower_Light1Data() : base()
    {


    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Tower_Light1;

    public override ETowerType TowerType => ETowerType.Light;

    public override ELayer LayerMask => ELayer.Player;

    private ESkillStage3 m_SkillStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;

    protected override float AtkIntervalOriginal => 1.0f;
    public Entity_Tower_Light1 TowerMono => GetCom<Entity_Tower_Light1>();
    public Vector3 AtkPoint => TowerMono.AtkPoint != null ? TowerMono.AtkPoint.position : WorldPosition;

    protected override int AtkRange => 6;
    public override int BranchOutIndex => 0;

    public List<Vector3> GetSkill2Points()
    {
        List<Vector3> points = new();
        if (TowerMono.Skill2Point != null)
        {
            foreach (var item in TowerMono.Skill2Point)
            {
                points.Add(item.position);
            }
        }
        else
        {
            points.Add(WorldPosition);
        }
        return points;
    }


    public override void Death()
    {
        base.Death();
        m_IsStartSkill3 = false;
    }

    public override void OnUnLoad()
    {
        m_IsStartSkill3 = false;
        base.OnUnLoad();

        SetTowerStatus(ETowerStatus.Start);
    }

    private Entity_Effect_Tower_Light1_AttackData GetAttackBullet(Vector3 f_StartPoint, int f_DamageValue, bool f_IsAddMagic)
    {
        var bullet = new Entity_Effect_Tower_Light1_AttackData();
        bullet.Initialization(f_StartPoint, this, f_DamageValue, f_IsAddMagic);
        return bullet;
    }
    private void AtkTargets(List<WorldObjectBaseData> f_Targets, int? f_DamageValue = null, bool f_IsAddMagic = false, Vector3? f_StartPoint = null, float? f_Speed = null)
    {
        var bullet = GetAttackBullet(f_StartPoint ?? AtkPoint, f_DamageValue ?? -CurHarm, f_IsAddMagic);
        var trackData = new Effect_Track_Points<Entity_Effect_Tower_Light1_AttackData>(bullet, f_Targets, f_Speed ?? AtkSpeed);
        GTools.RunUniTask(trackData.StartExecute());
    }
    public override void AttackTarget()
    {
        base.AttackTarget();
        List<WorldObjectBaseData> target = new()
        {
            m_AttackTarget,
        };
        AtkTargets(target, f_IsAddMagic: true);
    }
    public override void AnimatorCallback000()
    {
        base.AnimatorCallback000();
    }
    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();
        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                {
                    switch (m_SkillStage)
                    {
                        case ESkillStage3.Stage1:
                            break;
                        case ESkillStage3.Stage2:
                            break;
                        case ESkillStage3.Stage3:
                            m_IsStartSkill3 = true;
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
    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();
        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                {
                    switch (m_SkillStage)
                    {
                        case ESkillStage3.Stage1:
                            {
                                var targets = GTools.MathfMgr.GetTargets_Sphere(WorldPosition, AtkRange + 2, AttackLayerMask);
                                if (targets.Count > 0)
                                {
                                    AtkTargets(targets, f_IsAddMagic: false);
                                }
                            }
                            break;
                        case ESkillStage3.Stage2:
                            {
                                var targets = GTools.MathfMgr.GetTargets_Sphere(WorldPosition, AtkRange + 2, AttackLayerMask);
                                if (targets.Count > 0)
                                {
                                    var points = GetSkill2Points();
                                    for (int i = 0; i < points.Count; i++)
                                    {
                                        var point = points[i];
                                        var item = targets[i % targets.Count];
                                        List<WorldObjectBaseData> target = new()
                                        {
                                            item,
                                        };
                                        AtkTargets(target, -CurHarm * 2, f_StartPoint: point);

                                    }
                                }
                            }
                            break;
                        case ESkillStage3.Stage3:
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
    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();
        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                {
                    switch (m_SkillStage)
                    {
                        case ESkillStage3.Stage1:
                            break;
                        case ESkillStage3.Stage2:
                            break;
                        case ESkillStage3.Stage3:
                            m_IsStartSkill3 = false;
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
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();
    }

    private bool m_IsStartSkill3 = false;
    private float m_LastSkill3Time = 0;
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_IsStartSkill3 && (Time.time - m_LastSkill3Time) > AtkInterval * 0.5f)
        {
            m_LastSkill3Time = Time.time;
            StartSkill3();
        }
    }
    private async void StartSkill3()
    {
        var targets = GTools.MathfMgr.GetTargets_Sphere(WorldPosition, AtkRange + 2, AttackLayerMask);

        if (targets.Count == 0)
        {
            return;
        }

        foreach (var item in targets)
        {
            List<WorldObjectBaseData> target = new()
            {
                item,
            };
            AtkTargets(target, -CurHarm * 3, false, AtkPoint, AtkSpeed * 3);
            var interval = AtkInterval / targets.Count;
            await UniTask.Delay(Mathf.CeilToInt(interval * 1000));
            if (!m_IsStartSkill3) break;
        }
    }
}
public class Entity_Tower_Light1 : EntityTowerBase
{
    public Transform AtkPoint = null;
    public List<Transform> Skill2Point = new();
}
