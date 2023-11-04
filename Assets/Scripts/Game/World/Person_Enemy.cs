using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Person_EnemyData : WorldObjectBaseData
{
    public Person_EnemyData() : base()
    {
        CurStatus = EPersonStatusType.None;
    }

    public override EEntityType EntityType => EEntityType.Person;
    public abstract EHeroCardType CardType { get; }
    public int StartIndex { get; protected set; }
    public int TargetIndex { get; protected set; }
    public int CurTargetIndex { get; protected set; }
    public Person_Enemy TargetMonster => GetCom<Person_Enemy>();
    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;
    public ListStack<PathElementData> m_PathPoint = null;
    public override bool IsUpdateEnable => true;


    // 复活
    public override void Resurgence()
    {
        base.Resurgence();
        UpdatePathPoint(TargetIndex);
    }



    public override void OnUpdate()
    {
        base.OnUpdate();




        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                {
                    DitectionNextAction();
                }
                break;
            case EPersonStatusType.Walk:
                {
                    var distance = Vector3.Magnitude(m_MoveToTarget - WorldPosition);
                    if (distance > 0.001f)
                    {
                        var value = Vector3.MoveTowards(WorldPosition, m_MoveToTarget, m_MoveSpeed * Time.deltaTime);
                        var forward = Vector3.MoveTowards(Forward, value - WorldPosition, m_MoveSpeed * Time.deltaTime * 50);
                        SetForward(forward);
                        SetPosition(value);
                    }
                    else if (!DitectionNextAction())
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Attack:
                {
                    if (GTools.UnityObjectIsVaild(m_CurTarget)
                        && WorldMapMgr.Ins.IsInNearByObject(CurrentIndex, m_CurTarget, AtkRange))
                    {
                        if (MagicPercent >= 1)
                        {
                            SetPersonStatus(EPersonStatusType.Skill);
                        }
                        else
                        {
                            var value = Vector3.MoveTowards(Forward, m_CurTarget.WorldPosition - WorldPosition, m_MoveSpeed * Time.deltaTime * 10);
                            SetForward(value);
                        }
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Skill:
                {
                    if (CurrentMagic == 0)
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
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
    public bool DitectionNextAction()
    {
        // 攻击
        if (TryGetAttackTarget() && m_CurTarget.CurStatus != EPersonStatusType.Die)
        {
            if (m_CurTarget.CurrentIndex != CurTargetIndex)
            {
                UpdatePathPoint(m_CurTarget.CurrentIndex);
            }
            SetPersonStatus(EPersonStatusType.Attack);
            return true;
        }
        // 行走
        else if (MoveNext())
        {
            if (m_CurTarget != null && m_CurTarget.CurStatus != EPersonStatusType.Die)
            {

            }
            else if (CurTargetIndex != TargetIndex)
            {
                UpdatePathPoint(TargetIndex);
            }
            SetPersonStatus(EPersonStatusType.Walk);
            return true;
        }
        else
        {
            return false;
        }
    }
    // -- 
    // ----------------------------------===============----------------------------------
    // ------------------------------------- 移动篇
    // ----------------------------------===============----------------------------------
    // -- 
    // 移动到目标点
    private Vector3 m_MoveToTarget = Vector3.zero;
    // 移动速度
    private float m_MoveSpeed = 1;

    public Func<int, bool> GetPathCondition
    {
        get => PathCondition.GetCondition(AssetPrefabID, out var condition)
            ? condition
            : PathCondition.GetCondition(EAssetKey.Person_Enemy, out condition)
                ? condition
                : condition;
    }
    public bool PathCondition1(int f_Index)
    {
        if (WorldMapMgr.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.CurObjectType == EWorldObjectType.Road)
            {
                return true;
            }
        }
        return false;
    }
    public bool PathCondition2(int f_Index)
    {
        if (WorldMapMgr.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if ((chunkData.CurObjectType & EWorldObjectType.Road) != 0
                || ((chunkData.CurObjectType & EWorldObjectType.Construction) != 0)
                )
            {
                return true;
            }
        }
        return false;
    }
    // 设置开始索引
    public void SetStartIndex(int f_StartIndex)
    {
        StartIndex = f_StartIndex;
        CurrentIndex = StartIndex = f_StartIndex;

        var point = WorldMapMgr.Ins.GetChunkPoint(f_StartIndex);
        m_MoveToTarget = point;
        SetPosition(point);
    }
    // 上一个目标地图快索引
    private int m_LastTargetIndex = -1;
    private float m_LastTime = 0;
    public void UpdatePathPoint(int f_TargetIndex)
    {
        m_LastTime = Time.time;
        m_LastTargetIndex = f_TargetIndex;
        CurTargetIndex = f_TargetIndex;
        if (PathManager.Ins.TryGetAStarPath(CurrentIndex, f_TargetIndex, out m_PathPoint, m_PathPoint != null ? PathCondition1 : PathCondition2))
        {

        }
    }
    // 移动到下一个目标点
    public bool NextPathPoint(out Entity_Chunk1Data f_PathElement)
    {
        f_PathElement = null;

        Entity_Chunk1Data chunkData = null;
        PathElementData pathData = null;
        while (m_PathPoint != null && m_PathPoint.TryPop(out pathData) && WorldMapMgr.Ins.TryGetChunkData(pathData.ChunkIndex, out chunkData) && chunkData.Index == CurrentIndex)
        {

        }
        if (pathData != null)
        {
            if (chunkData != null && PathCondition1(chunkData.Index))
            {
                f_PathElement = chunkData;
                return true;
            }
            m_PathPoint.Push(pathData);
        }


        return false;
    }
    // 尝试移动到下一个目标点
    public bool MoveNext(bool f_Force = false)
    {
        if (NextPathPoint(out var chunkData))
        {
            m_MoveToTarget = chunkData.PointUp;
            WorldMapMgr.Ins.MoveChunkElement(this, chunkData.Index);
            return true;
        }
        else if (CurrentIndex != CurTargetIndex && WorldMapMgr.Ins.TryGetChunkData(CurTargetIndex, out chunkData))
        {
            if (m_LastTargetIndex == CurTargetIndex && Time.time - m_LastTime < 1.0f)
            {
                return false;
            }
            UpdatePathPoint(CurTargetIndex);
            return MoveNext(f_Force);
        }
        return false;
    }
    public void ForceSetPosition(Vector3 f_ToPosition)
    {
        m_MoveToTarget = f_ToPosition;
        SetPosition(f_ToPosition);
    }



    // -- 
    // ----------------------------------===============----------------------------------
    // ------------------------------------- 攻击篇
    // ----------------------------------===============----------------------------------
    // -- 
    // 攻击范围
    protected virtual int AtkRange => 1;
    protected virtual float AtkSpeed => 1;
    // 当前攻击目标
    protected WorldObjectBaseData m_CurTarget = null;
    // 当前技能需要消耗的能量值
    protected float m_CurrectSkillEnergy = 0;
    // 最大技能能量值
    protected float m_MaxSkillEnergy = 3.0f;
    // 技能消耗的能量值
    protected float m_SkillEnergyExpend = 1.0f;
    // 上一次使用技能的时间
    protected float m_LastSkillTime = 0;
    // 技能使用时间间隔
    protected float m_SkillTimeInterval = 2.0f;
    // 上一次攻击的时间
    protected float m_LastAttackTime = 0;
    // 攻击间隔
    protected virtual float AttackTimeInterval => 0.5f;
    // 尝试攻击目标
    public bool TryGetAttackTarget()
    {
        WorldObjectBaseData target = null;
        float curMinDis = float.MaxValue;
        if (WorldMapMgr.Ins.TryGetRangeChunkByIndex(CurrentIndex, out var result, GetAttackTargetCondition, false, AtkRange))
        {
            // ��������Ŀ��
            while (result.TryPop(out var index))
            {
                if (WorldMapMgr.Ins.TryGetChunkObjectByLayer(index, AttackLayerMask, out var targetData))
                {
                    foreach (var item in targetData)
                    {
                        var dis = Vector3.Magnitude(item.Value.WorldPosition - WorldPosition);
                        if (curMinDis > dis)
                        {
                            if (item.Value is WorldObjectBaseData worldObj)
                            {
                                target = worldObj;
                                curMinDis = dis;
                            }
                        }
                    }
                }
            }
        }
        m_CurTarget = target;
        return target != null;
    }

    private bool GetAttackTargetCondition(int f_Index)
    {
        if (WorldMapMgr.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.GetWorldObjectByLayer(AttackLayerMask, out var result) && result.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public override void AttackTarget()
    {
        base.AttackTarget();
        if (GTools.UnityObjectIsVaild(m_CurTarget))
        {
            GTools.MathfMgr.EntityDamage(this, m_CurTarget, EDamageType.Physical, -CurHarm, false);
            GTools.WorldWindowMgr.CreateAttackEffect(m_CurTarget.CentralPoint, EAttackEffectType.Default1);
        }
    }
    public virtual int DiePrice { get; } = 10;
    public override void AnimatorCallback000()
    {
        base.AnimatorCallback000();
        switch (CurStatus)
        {
            case EPersonStatusType.Die:
                {
                    GTools.PlayerMgr.Increases(DiePrice);
                }
                break;
            default:
                break;
        }
    }

}


public abstract class Person_Enemy : WorldObjectBase
{
    private Person_EnemyData m_MonsterData => TryGetData<Person_EnemyData>(out var data) ? data : null;
    public Vector3 TargetPosition = Vector3.zero;

}
