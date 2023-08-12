using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Person_EnemyData : WorldObjectBaseData
{
    public Person_EnemyData(int f_Index, int f_TargetIndex, Entity_SpawnPointData f_TargetSpawnPoint) : base(f_Index, f_TargetSpawnPoint.CurrentIndex)
    {
        SpawnPointData = f_TargetSpawnPoint;
        TargetIndex = f_TargetIndex;
        CurStatus = EPersonStatusType.None;
    }

    public int StartIndex { get; protected set; }
    public int TargetIndex { get; protected set; }
    public int CurTargetIndex { get; protected set; }
    public Entity_SpawnPointData SpawnPointData { get; protected set; }
    public Person_Enemy TargetMonster => GetCom<Person_Enemy>();
    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;
    public ListStack<PathElementData> m_PathPoint = null;
    public override bool IsUpdateEnable => true;

    public override void AfterLoad()
    {
        base.AfterLoad();
        Resurgence();
    }
    public override void OnUnLoad()
    {
        CurStatus = EPersonStatusType.None;
        Death();
        base.OnUnLoad();
    }
    // 复活
    public void Resurgence()
    {
        CurStatus = EPersonStatusType.Idle;
        UpdatePathPoint(TargetIndex);

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
    public override void OnUpdate()
    {
        base.OnUpdate();




        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                {
                    // 攻击
                    if (TryGetAttackTarget() && m_CurTarget.CurStatus != EPersonStatusType.Die)
                    {
                        if (m_CurTarget.CurrentIndex != CurTargetIndex)
                        {
                            UpdatePathPoint(m_CurTarget.CurrentIndex);
                        }
                        SetPersonStatus(EPersonStatusType.Attack);
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
                    }
                }
                break;
            case EPersonStatusType.Walk:
                {
                    var distance = Vector3.Magnitude(m_MoveToTarget - WorldPosition);
                    if (distance > 0.001f)
                    {
                        var value = Vector3.MoveTowards(WorldPosition, m_MoveToTarget, m_MoveSpeed * Time.deltaTime);
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
                    if(m_CurTarget != null && m_CurTarget.CurStatus != EPersonStatusType.Die && WorldMapManager.Ins.IsInNearByIndex(CurrentIndex, m_CurTarget.CurrentIndex, AtkRange))
                    {
                        if (MagicPercent >= 1)
                        {
                            SetPersonStatus(EPersonStatusType.Skill);
                        }
                        else
                        {
                            SetForward(m_CurTarget.WorldPosition - WorldPosition);
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
        PlayerAnimation();
    }
    // -- 
    // ----------------------------------===============----------------------------------
    // ------------------------------------- �ƶ�ƪ
    // ----------------------------------===============----------------------------------
    // -- 
    // Ŀ���
    private Vector3 m_MoveToTarget = Vector3.zero;
    // һ���ƶ�����
    private float m_MoveSpeed = 1;

    public Func<int, bool> GetPathCondition
    {
        get => PathCondition.GetCondition(AssetPrefabID, out var condition)
            ? condition
            : PathCondition.GetCondition(AssetKey.Person_Enemy, out condition)
                ? condition
                : condition;
    }
    public bool PathCondition1(int f_Index)
    {
        if (WorldMapManager.Ins.TryGetChunkData(f_Index, out var chunkData))
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
        if (WorldMapManager.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if ((chunkData.CurObjectType & EWorldObjectType.Road) != 0
                && ((chunkData.CurObjectType & EWorldObjectType.Construction) == 0)
                )
            {
                return true;
            }
        }
        return false;
    }
    // ���ÿ�ʼ����
    public void SetStartIndex(int f_StartIndex)
    {
        StartIndex = f_StartIndex;
        CurrentIndex = StartIndex = f_StartIndex;

        var point = WorldMapManager.Ins.GetChunkPoint(f_StartIndex);
        m_MoveToTarget = point;
        SetPosition(point);
    }
    // ���µ�ǰ·��
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
    // ��ȡ��һ��·����
    public bool NextPathPoint(out Entity_Chunk1Data f_PathElement)
    {
        f_PathElement = null;

        Entity_Chunk1Data chunkData = null;
        PathElementData pathData = null;
        while (m_PathPoint != null && m_PathPoint.TryPop(out pathData) && WorldMapManager.Ins.TryGetChunkData(pathData.ChunkIndex, out chunkData) && chunkData.Index == CurrentIndex)
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
    // �ƶ�����һ����
    public bool MoveNext(bool f_Force = false)
    {
        if (NextPathPoint(out var chunkData))
        {
            m_MoveToTarget = chunkData.PointUp;
            WorldMapManager.Ins.MoveChunkElement(this, chunkData.Index);
            return true;
        }
        else if (CurrentIndex != CurTargetIndex && WorldMapManager.Ins.TryGetChunkData(CurTargetIndex, out chunkData))
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
    // ------------------------------------- ����ƪ
    // ----------------------------------===============----------------------------------
    // -- 
    // ������Χ
    protected virtual int AtkRange => 1;
    protected virtual float AtkSpeed => 1;
    // ��ǰ����Ŀ��
    protected WorldObjectBaseData m_CurTarget = null;
    // ��ǰ��������ֵ ��λ 1
    protected float m_CurrectSkillEnergy = 0;
    // �����������ֵ
    protected float m_MaxSkillEnergy = 3.0f;
    // ����������������
    protected float m_SkillEnergyExpend = 1.0f;
    // �ϴμ���ʹ��ʱ��
    protected float m_LastSkillTime = 0;
    // ����ʹ��ʱ����
    protected float m_SkillTimeInterval = 2.0f;
    // �ϴι���ʹ��ʱ��
    protected float m_LastAttackTime = 0;
    // ����ʱ����
    protected virtual float AttackTimeInterval => 0.5f;
    // ��⹥��
    public bool TryGetAttackTarget()
    {
        WorldObjectBaseData target = null;
        float curMinDis = float.MaxValue;
        if (WorldMapManager.Ins.TryGetRangeChunkByIndex(CurrentIndex, out var result, GetAttackTargetCondition, false, AtkRange))
        {
            // ��������Ŀ��
            while (result.TryPop(out var index))
            {
                if (WorldMapManager.Ins.TryGetChunkObjectByLayer(index, AttackLayerMask, out var targetData))
                {
                    foreach (var item in targetData)
                    {
                        var dis = Vector3.Magnitude(item.Value.WorldPosition - WorldPosition);
                        if (curMinDis > dis)
                        {
                            curMinDis = dis;
                            target = item.Value;
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
        if (WorldMapManager.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.GetWorldObjectByLayer(AttackLayerMask, out var result) && result.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void AttackTarget()
    {
        if (GTools.UnityObjectIsActive(m_CurTarget))
        {
            GTools.MathfMgr.EntityDamage(this, m_CurTarget, EDamageType.Physical, -HarmBase, false);
        }
    }

}


public abstract class Person_Enemy : WorldObjectBase
{
    private Person_EnemyData m_MonsterData => TryGetData<Person_EnemyData>(out var data) ? data : null;
    public Vector3 TargetPosition = Vector3.zero;

}
