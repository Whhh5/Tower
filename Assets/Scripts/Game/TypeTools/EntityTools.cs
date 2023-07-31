using System;
using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public enum EAnimatorStatus
{
    None,
    One,
    Loop,
    Stop,
}
public abstract class TransformBase : MonoBase
{
    public Vector3 Position => transform.position;

    public Vector3 LocalPostion => transform.localPosition;

    public Vector3 Forward => transform.forward;

    public Vector3 Up => transform.up;

    public Vector3 Right => transform.right;

    public Vector3 LocalRotation => transform.localRotation.eulerAngles;
    public Quaternion Rotation => transform.rotation;


    public virtual void SetWorldPos(Vector3 f_WorldPos)
    {
        transform.position = f_WorldPos;
    }

    public virtual void SetLocalPos(Vector3 f_LocalPos)
    {
        transform.localPosition = f_LocalPos;
    }

    public virtual void SetForward(Vector3 f_Direction)
    {
        transform.forward = f_Direction;
    }

    public virtual void SetUp(Vector3 f_Direction)
    {
        transform.up = f_Direction;
    }

    public virtual void SetRight(Vector3 f_Direction)
    {
        transform.right = f_Direction;
    }

    public virtual void SetLocalRotation(Vector3 f_Angle)
    {
        transform.localRotation = Quaternion.Euler(f_Angle);
    }
}

public abstract class VirtualEntityData : UnityObjectData
{
    protected VirtualEntityData(int f_Index) : base(f_Index)
    {
    }
}
public abstract class VirtualEntity : ObjectPoolBase, IExecute
{
    public abstract UniTask StartExecute();

    public abstract UniTask StopExecute();
}

public enum EntityCommandType
{
}

public class EntityCommand<T>
{
    private T data;
    private ListQueue<Action<T>> m_List;

    public EntityCommand(T f_Target, int f_CacheCount)
    {
        data = f_Target;
        m_List = new("命令队列", f_CacheCount);
    }

    public void AddCommand()
    {
    }

    public void Invoke()
    {
    }
}


public abstract class UnityObjectData : Base, ILoadPrefabAsync, IUpdateBase
{
    protected UnityObjectData(int f_Index)
    {
        Index = f_Index;
    }
    


    //                                ------------------------------------------------
    //                                --------------------资源加载 篇
    //                                ------------------------------------------------
    public int LoadKey { get; set; }

    public ObjectPoolBase PrefabTarget { get; set; }
    public LoadAsyncResult LoadResult { get; set; }
    public abstract AssetKey AssetPrefabID { get; }
    public EPersonStatusType CurStatus { get; protected set; } = EPersonStatusType.Idle;
    public virtual void AfterLoad()
    {
        SetPersonStatus(EPersonStatusType.Idle);
        InitAnimatorParams();
        AnimaStatus = EAnimatorStatus.Loop;
        LastUpdateTime = Time.time;
        if (IsUpdateEnable)
        {
            GTools.LifecycleMgr.AddUpdate(this);
        }
    }
    public virtual void OnUnLoad()
    {
        if (IsUpdateEnable)
        {
            GTools.LifecycleMgr.RemoveUpdate(this);
        }
        SetPersonStatus(EPersonStatusType.Die);        
    }
    public T GetCom<T>()
        where T : Component
    {
        return PrefabTarget != null ? PrefabTarget.GetComponent<T>() : null;
    }
    //                                ------------------------------------------------
    //                                --------------------Update 篇
    //                                ------------------------------------------------
    public int UpdateLevelID { get; set; }
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    public float LastUpdateTime { get; private set; }
    public float UpdateDelta { get; private set; }
    public virtual bool IsUpdateEnable => false;
    public virtual void OnUpdate()
    {
        UpdateDelta = Time.time - LastUpdateTime;
        LastUpdateTime = Time.time;
        UpdateAnimator();
    }
    //                                ------------------------------------------------
    //                                --------------------基础数据
    //                                ------------------------------------------------
    public int Index { get; private set; }
    public abstract EWorldObjectType ObjectType { get; }

    //                                ------------------------------------------------
    //                                --------------------参考点篇
    //                                ------------------------------------------------
    public Vector3 CentralPoint => PrefabTarget != null && PrefabTarget.CentralPoint != null
        ? PrefabTarget.CentralPoint.position
        : WorldPosition;

    public Vector3 BeHitPoint => PrefabTarget != null && PrefabTarget.BeHitPoint != null
        ? PrefabTarget.BeHitPoint.position
        : WorldPosition;

    public Vector3 BuffPoint => PrefabTarget != null && PrefabTarget.BuffPoint != null
        ? PrefabTarget.BuffPoint.position
        : WorldPosition;

    public Vector3 EffectPoint => PrefabTarget != null && PrefabTarget.EffectPoint != null
        ? PrefabTarget.EffectPoint.position
        : WorldPosition;

    public Vector3 TrailPoint => PrefabTarget != null && PrefabTarget.TrailPoint != null
        ? PrefabTarget.TrailPoint.position
        : WorldPosition;

    public Vector3 WeaponPoint => PrefabTarget != null && PrefabTarget.WeaponPoint != null
        ? PrefabTarget.WeaponPoint.position
        : WorldPosition;


    //                                ------------------------------------------------
    //                                --------------------预制体属性
    //                                ------------------------------------------------
    public Transform Parent { get; private set; } = null;
    public Vector3 WorldPosition { get; private set; } = new Vector3();
    public Vector3 Forward { get; private set; } = new Vector3(0, 0, 1);
    public Quaternion LocalRotation { get; private set; } = new Quaternion();
    public Vector3 LocalScale { get; private set; } = new Vector3(1, 1, 1);

    public Color Color { get; private set; } = Color.white;
    // --

    public Transform Tran => PrefabTarget != null ? PrefabTarget.MainTran : null;
    public Vector3 PointUp => PrefabTarget != null && PrefabTarget.PointUp != null
        ? PrefabTarget.PointUp.position
        : WorldPosition + new Vector3(0, 1, 0);

    public void SetParent(Transform f_ToParent)
    {
        Parent = f_ToParent;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetParent();
        }
    }

    public void SetLocalScale(Vector3 f_ToLocalScale)
    {
        LocalScale = f_ToLocalScale;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetLocalScale();
        }
    }

    public void SetLocalRotation(Quaternion f_ToLocalRotation)
    {
        LocalRotation = f_ToLocalRotation;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetLocalRotation();
        }
    }

    public void SetPosition(Vector3 f_ToPosition)
    {
        WorldPosition = f_ToPosition;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetPosition();
        }
    }
    public void SetForward(Vector3 f_ToForward)
    {
        Forward = f_ToForward.normalized;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetForward();
        }
    }

    public void SetColor(Color f_ToColor)
    {
        Color = f_ToColor;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetColor();
        }
    }
    public void SetPersonStatus(EPersonStatusType f_ToStatus, EAnimatorStatus f_AnimaStatus = EAnimatorStatus.Loop)
    {
        if (CurStatus != f_ToStatus)
        {
            CurStatus = f_ToStatus;
            CurAnimaNormalizedTime = 0;
            CurAnimaSpeed = 1;
            AnimaStatus = f_AnimaStatus;
        }
    }
    //                                ------------------------------------------------
    //                                --------------------动画篇
    //                                ------------------------------------------------
    class AnomatorCallback
    {
        public float NormalizedTime = 0;
        public Action Action = null;
        public bool Status = false;
        public void Init()
        {
            Status = true;
        }
        public void Invok()
        {
            Status = false;
            Action?.Invoke();
        }
    }
    // 当前动画播放进度
    private float m_CurAnimaNormalizedTime = 0;
    public float CurAnimaNormalizedTime
    {
        get => m_CurAnimaNormalizedTime;
        set => m_CurAnimaNormalizedTime = Mathf.Clamp01(value);
    }

    // 当前动画播放速度
    public float CurAnimaSpeed = 1;
    // 动画状态
    public EAnimatorStatus AnimaStatus = EAnimatorStatus.None;
    private Dictionary<int, AnomatorCallback> m_DicAnimaCallBack = null;
    public void InitAnimatorParams()
    {
        int index = 0;
        m_DicAnimaCallBack = new()
        {
            {
                index++,
                new()
                {
                    NormalizedTime = 0.0f,
                    Status = true,
                    Action = AnimatorCallback000,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.2f,
                    Status = true,
                    Action = AnimatorCallback020,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.5f,
                    Status = true,
                    Action = AnimatorCallback050,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.7f,
                    Status = true,
                    Action = AnimatorCallback070,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 1.0f,
                    Status = true,
                    Action = AnimatorCallback100,
                }
            },
        };
    }
    public void UpdateCallbackStatus()
    {
        foreach (var item in m_DicAnimaCallBack)
        {
            item.Value.Init();
        }
    }
    public void PlayerAnimation(bool f_IsForce = false)
    {
        if (PrefabTarget != null && !f_IsForce)
        {
            PrefabTarget.PlayerAnimation();
        }
    }
    public virtual string GetCurrentAnimationName()
    {
        return $"{CurStatus}";
    }
    private void UpdateAnimator()
    {
        switch (AnimaStatus)
        {
            case EAnimatorStatus.None:
                break;
            case EAnimatorStatus.One:
                {
                    if (CurAnimaNormalizedTime == 1)
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                    UpdateCurAnimaNormalizedTime();
                }
                break;
            case EAnimatorStatus.Loop:
                {
                    UpdateCurAnimaNormalizedTime();
                }
                break;
            case EAnimatorStatus.Stop:
                {
                    if (CurAnimaNormalizedTime != 1)
                    {
                        UpdateCurAnimaNormalizedTime();
                    }
                }
                break;
            default:
                break;
        }
        PlayerAnimation();
    }

    private void UpdateCurAnimaNormalizedTime()
    {
        // 目标进度
        var value = CurAnimaNormalizedTime + UpdateDelta * CurAnimaSpeed;

        // 动画回调
        foreach (var item in m_DicAnimaCallBack)
        {
            if (item.Value.Status && item.Value.NormalizedTime <= value)
            {
                item.Value.Invok();
            }
        }

        // 更新进度
        if (value > 1)
        {
            CurAnimaNormalizedTime = AnimaStatus == EAnimatorStatus.Loop ? value - 1 : 1;
            UpdateCallbackStatus();
        }
        else
        {
            CurAnimaNormalizedTime = value;
        }
    }
    public virtual void AnimatorCallback000()
    {
        
    }
    public virtual void AnimatorCallback020()
    {

    }
    public virtual void AnimatorCallback050()
    {
        
    }
    public virtual void AnimatorCallback070()
    {
        
    }
    public virtual void AnimatorCallback100()
    {
        
    }
}

public abstract class EntityData : UnityObjectData
{
    protected EntityData(int f_index) : base(f_index)
    {
        CommandQueue = new(this, 100);
    }

    public int CurrentBlood { get; private set; } = 400;
    public int MaxBlood { get; private set; } = 523;
    public int CurrentMagic { get; private set; } = 300;
    public int MaxMagic { get; private set; } = 653;

    // buff 列表
    public Dictionary<ushort, BuffBase> BuffDic { get; private set; } = new();
    public ListStack<ushort> AddBuffDic = new("", 5);
    public ListStack<ushort> RemoveBuffDic = new("", 5);

    // 免疫列表
    public List<ushort> ImmuneBuffList = new();

    // buff 列表
    protected Dictionary<uint, WorldBuffHint> BuffHintDic = new();

    // destroy list
    public Dictionary<int, IEntityDestry> IEntityDestroyList = new();

    // 命令队列
    public EntityCommand<EntityData> CommandQueue = null;

    public virtual int ChangeBlood(int f_Increment)
    {
        var value = CurrentBlood + f_Increment;

        CurrentBlood = Mathf.Clamp(value, 0, MaxBlood);

        return CurrentBlood;
    }
    public virtual int ChangeMagic(int f_Increment)
    {
        var value = CurrentMagic + f_Increment;

        CurrentMagic = Mathf.Clamp(value, 0, MaxMagic);

        return CurrentMagic;
    }
}

// base class
public abstract class Entity : ObjectPoolBase
{
    // 开始执行行为
    public abstract UniTask StartExecute();

    // 结束执行行为
    public abstract UniTask StopExecute();

    // CommandQueue
    [SerializeField] protected Dictionary<ushort, BuffBase> m_BuffDic = new();
    protected ListStack<ushort> m_AddBuffDic = new("", 5);
    protected ListStack<ushort> m_RemoveBuffDic = new("", 5);
    [SerializeField] protected List<ushort> m_ImmuneBuffList = new();
    protected Dictionary<uint, WorldBuffHint> m_BuffHintDic = new();

    // destroy list
    protected Dictionary<int, IEntityDestry> m_IEntityDestroyList = new();


    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        await GTools.ParallelTaskAsync(GTools.BuffMgr.BuffMap, async (key, value) =>
        {
            var target = GTools.BuffMgr.GetBuff(key, this);
            if (target.Result == EResult.Succeed)
            {
                m_BuffDic.Add(key, target.Value);
            }
        });
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        await GTools.ParallelTaskAsync(m_IEntityDestroyList,
            async (key, value) => { await value.CurEntityDestroyAsync(); });
        m_IEntityDestroyList.Clear();

        m_BuffDic.Clear();
    }

    public override async void OnUpdate()
    {
        {
            var index = 0;
            var buffLocalPos = BuffPoint.localPosition;
            Vector3 localPos = new Vector3(
                buffLocalPos.x > 0 ? 0.2f / buffLocalPos.x : 0,
                buffLocalPos.y,
                buffLocalPos.z > 0 ? 0.2f / buffLocalPos.y : 0);
            foreach (var item in m_BuffDic)
            {
                if (item.Value.CurRatio > 0)
                {
                    if (!m_BuffHintDic.TryGetValue(item.Key, out var target))
                    {
                        await GTools.AddDicElementAsync(m_BuffHintDic, item.Key, async () =>
                        {
                            target = await GTools.LoadWorldBuffHintAsync(item.Value);
                            return target;
                        });
                    }

                    var i = index++;
                    var pos = buffLocalPos + new Vector3
                    (
                        buffLocalPos.x * (localPos.x * i),
                        0,
                        buffLocalPos.z * (localPos.z * i)
                    );
                    target?.SetLocalPos(pos + Position);
                }
                else
                {
                    if (m_BuffHintDic.ContainsKey(item.Key))
                    {
                        await GTools.UnLoadWorldBuffHintAsync(m_BuffHintDic[item.Key]);
                        m_BuffHintDic.Remove(item.Key);
                    }
                }
            }
        }
    }

    // 加血
    public async UniTask AddBloodAsync(uint f_Value)
    {
        GTools.WorldMgr.DamageText(f_Value, EDamageType.AddBlood, BeHitPoint.position);

        await ChangeBloodAsync(f_Value, EDamageType.AddBlood);
    }

    // 减血
    public async UniTask RediuceBlood(uint f_Value, EDamageType f_Type)
    {
        GTools.WorldMgr.DamageText(f_Value, f_Type, BeHitPoint.position);

        await ChangeBloodAsync(f_Value, f_Type);
    }

    // 改变血量
    public async UniTask ChangeBloodAsync(uint f_Value, EDamageType f_Type)
    {
    }

    // 受击
    public async UniTask BeHitAsync(uint f_Value, EDamageType f_Type)
    {
        await RediuceBlood(f_Value, f_Type);
    }


    // ===============------      ------===============
    //                       CommandQueue 
    // ===============------      ------===============
    // 添加 CommandQueue
    public async UniTask AddBuffAsync(ushort f_BuffID, ushort f_Level)
    {
        if (m_ImmuneBuffList.Contains(f_BuffID))
        {
            GTools.WorldMgr.DamageText("免疫", EDamageType.None, UnityObjectData.BeHitPoint);
        }
        else if (m_BuffDic.TryGetValue(f_BuffID, out var IBuffBase))
        {
            await IBuffBase.AddAsync(f_Level);
        }
    }

    // 移除 CommandQueue
    public async UniTask RemoveBuffAsync<T>(T f_IBuffBase) where T : BuffBase
    {
    }

    // 清除所有 CommandQueue
    public async UniTask ClearBuffsAsync<T>(T f_IBuffBase) where T : BuffBase
    {
    }





}