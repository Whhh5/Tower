using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ESkillStage2
{
    Stage1,
    Stage2,
    EnumCount,
}
public enum ESkillStage3
{
    Stage1,
    Stage2,
    Stage3,
    EnumCount,
}
public enum ESkillStage4
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    EnumCount,
}
public enum ESkillStage5
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    EnumCount,
}
public abstract class Entity_HeroBaseData : WorldObjectBaseData
{
    protected enum EMoveStatus
    {
        Play,
        Pause,
        Stop,
    }
    public Entity_HeroBaseData(int f_index, int f_ChunkIndex) : base(f_index, f_ChunkIndex)
    {

    }

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public sealed override EWorldObjectType ObjectType => EWorldObjectType.Preson;
    public Entity_HeroBase HeroData => GetCom<Entity_HeroBase>();

    public Vector3 AttackPoint => HeroData.m_AttackPoint != null ? HeroData.m_AttackPoint.position : WorldPosition;

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override void AfterLoad()
    {
        base.AfterLoad();
        Resurgence();
    }
    public override void OnUnLoad()
    {
        Death();
        base.OnUnLoad();
    }
    // 复活
    public void Resurgence()
    {
        SetPersonStatus(EPersonStatusType.Idle);
        WorldWindowManager.Ins.UpdateBloodHint(this);
    }
    public void Death()
    {
        WorldWindowManager.Ins.RemoveBloodHint(this);
    }
    public override int ChangeBlood(ChangeBloodData f_Increment)
    {
        var value = base.ChangeBlood(f_Increment);
        WorldWindowManager.Ins.UpdateBloodHint(this);

        if (value <= 0)
        {
            Death();
        }
        return value;
    }
    public override int ChangeMagic(int f_Increment)
    {
        var value = base.ChangeMagic(f_Increment);
        WorldWindowManager.Ins.UpdateBloodHint(this);
        return value;
    }
    public override bool IsUpdateEnable => true;

    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                {
                    if (false)
                    {

                    }
                    else if (TryGetNextPoint(out var pathPoint))
                    {
                        WorldMapManager.Ins.MoveChunkElement(this, pathPoint.ChunkIndex);
                        m_MoveToTarget = pathPoint.WorldPos;
                        SetPersonStatus(EPersonStatusType.Walk);
                    }
                    else if (TryDetectionAttack(out var target))
                    {
                        CurAttackTarget = target;
                        SetPersonStatus(EPersonStatusType.Attack);
                    }
                }
                break;
            case EPersonStatusType.Walk:
                {
                    var distance = Vector3.Magnitude(m_MoveToTarget - WorldPosition);
                    if (distance > 0.001f)
                    {
                        var value = Vector3.MoveTowards(WorldPosition, m_MoveToTarget, CurMoveSpeed * Time.deltaTime);
                        SetForward(value - WorldPosition);
                        SetPosition(value);
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Attack:
                {
                    if (MagicPercent >= 1)
                    {
                        SetPersonStatus(EPersonStatusType.Skill);
                        SetForward(CurAttackTarget.WorldPosition - WorldPosition);
                    }
                    if (!GTools.UnityObjectIsActive(CurAttackTarget))
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Skill:
                if (CurrentMagic == 0)
                {
                    SetPersonStatus(EPersonStatusType.Idle);
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

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 路径篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    protected EMoveStatus m_CurMoveStatus = EMoveStatus.Stop;
    private ListStack<PathElementData> m_CurPathList = new("");
    private Vector3 m_MoveToTarget = Vector3.zero;
    public void SetPath(ListStack<PathElementData> f_CurPath)
    {
        m_CurPathList = f_CurPath;
        SetPersonStatus(EPersonStatusType.Idle);
    }
    public void Play()
    {

        m_CurMoveStatus = EMoveStatus.Play;
    }
    public void Pause()
    {

        m_CurMoveStatus = EMoveStatus.Pause;
    }
    public void Stop()
    {

        m_CurMoveStatus = EMoveStatus.Stop;
    }
    public bool TryGetNextPoint(out PathElementData f_PathPoint)
    {
        return m_CurPathList.TryPop(out f_PathPoint);
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public virtual int AtkRange { get; } = 1;
    public virtual float AtkSpeed { get; } = 1.0f;
    public WorldObjectBaseData CurAttackTarget = null;
    public bool TryDetectionAttack(out WorldObjectBaseData f_AttackTarget)
    {
        f_AttackTarget = null;
        if (WorldMapManager.Ins.TryGetRangeChunkByIndex(CurrentIndex, out var list, AttackTargetCondition, false, AtkRange))
        {
            var minDis = float.MaxValue;
            foreach (var item in list.GetEnumerator())
            {
                if (WorldMapManager.Ins.TryGetChunkData(item.Value, out var chunkData))
                {
                    if (chunkData.GetWorldObjectByLayer(AttackLayerMask, out var targets))
                    {
                        foreach (var target in targets)
                        {
                            var dis = Vector3.Magnitude(target.Value.WorldPosition - WorldPosition);
                            if (GTools.UnityObjectIsActive(target.Value) && minDis > dis)
                            {
                                f_AttackTarget = target.Value;
                                minDis = dis;
                            }
                        }
                    }
                }
            }
        }
        return f_AttackTarget != null;
    }
    public bool AttackTargetCondition(int f_Index)
    {
        if (WorldMapManager.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            return chunkData.IsAlreadyLayer(AttackLayerMask);
        }
        return false;
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 技能篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int m_CurSkillCount = 0;
    public virtual int SkillStageCount { get; } = 1;
    public int CurStage => m_CurSkillCount % SkillStageCount;
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
    public override string GetCurrentAnimationName()
    {
        var curName = base.GetCurrentAnimationName();

        if (CurStatus == EPersonStatusType.Skill)
        {
            curName = $"{curName}_{(int)CurStage + 1}";
        }
        return curName;
    }
}
public abstract class Entity_HeroBase : WorldObjectBase
{
    private Entity_HeroBaseData DataEntity => GetData<Entity_HeroBaseData>();
    public Transform m_AttackPoint = null;

    private void OnMouseDown()
    {
        MoveCardMgr.Ins.SetCurSelectHero(DataEntity);
    }

}
