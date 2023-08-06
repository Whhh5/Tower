using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class UnityObjectData : Base, ILoadPrefabAsync, IUpdateBase
{
    protected UnityObjectData(int f_Index)
    {
        Index = f_Index;
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 资源加载篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int LoadKey { get; set; }

    public ObjectPoolBase PrefabTarget { get; set; }
    public LoadAsyncResult LoadResult { get; set; }
    public abstract AssetKey AssetPrefabID { get; }
    public EPersonStatusType CurStatus { get; protected set; } = EPersonStatusType.Idle;
    public virtual void AfterLoad()
    {
        InitAnimatorParams();
        SetPersonStatus(EPersonStatusType.Idle);
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

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Update 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
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
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 基础数据篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int Index { get; private set; }
    public abstract EWorldObjectType ObjectType { get; }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 参考点篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
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


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 预制体属性篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public Transform Parent { get; private set; } = null;
    public Vector3 WorldPosition { get; private set; } = new Vector3();
    public Vector3 Forward { get; private set; } = new Vector3(0, 0, 1);
    public Vector3 LocalRotation { get; private set; } = new Vector3();
    public Vector3 LocalScale { get; private set; } = new Vector3(1, 1, 1);

    public Color Color { get; private set; } = Color.white;
    // --

    public Transform Tran => PrefabTarget != null ? PrefabTarget.MainTran : null;
    public Vector3 PointUp => PrefabTarget != null && PrefabTarget.PointUp != null
        ? PrefabTarget.PointUp.position
        : WorldPosition;

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

    public void SetLocalRotation(Vector3 f_ToLocalRotation)
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
            UpdateCallbackStatus();
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 动画篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
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
        if (value >= 1)
        {
            CurAnimaNormalizedTime = AnimaStatus == EAnimatorStatus.Loop ? value - 1 : 1;
            UpdateCallbackStatus();
        }
        else
        {
            CurAnimaNormalizedTime = value;
        }


        //AnimationClip clip = 
        //clip.events
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
// 世界池物体父类
public abstract class ObjectPoolBase : TransformBase, IObjectPoolBase
{
    public bool UpdateInteractable = false;
    public AssetKey AssetKey { get; set; }
    public int SaveID { get; set; }
    public string PoolKey { get; set; }
    public int AssetName { get; set; }
    public int AssetLable { get; set; }
    public bool IsActively { get; set; }
    public int UpdateLevelID { get; set; }
    public virtual EUpdateLevel UpdateLevel => GTools.PrefabPoolUpdateLevel;

    public EAssetName EAssetName => (EAssetName)AssetName;
    public EAssetLable EAssetLable => (EAssetLable)AssetLable;
    [SerializeField]
    private Animator m_CurAnim = null;
    public Animator CurAnim => m_CurAnim != null ? m_CurAnim : GetComponent<Animator>();
    public virtual async UniTask OnUnLoadAsync()
    {

    }
    // 加载完成之后调用  queue 1
    public virtual async UniTask OnLoadAsync()
    {

    }
    // 加载完成之后调用  queue 2
    public virtual void OnUpdate()
    {

    }
    // 在加载完成之后调用 queue 3
    public void SetUnityObjectData(UnityObjectData f_ObjectData)
    {
        UnityObjectData = f_ObjectData;
    }
    public bool TryGetData<T>(out T f_Com)
        where T : UnityObjectData
    {
        f_Com = null;
        if (UnityObjectData is T data)
        {
            f_Com = data;
            return true;
        }
        return false;
    }
    // 在加载完成之后调用 queue 4
    public virtual async UniTask OnStartAsync(params object[] f_Params)
    {
        SetParent();
        SetLocalScale();
        SetPosition();
        SetForward();
        SetLocalRotation();
        SetColor();
        PlayerAnimation();
    }


    public virtual async UniTask StartExecute()
    {

    }

    public virtual async UniTask StopExecute()
    {

    }
    // point
    [SerializeField] private Transform m_CentralPoint = null;
    public Transform CentralPoint => m_CentralPoint;
    [SerializeField] private Transform m_BeHitPoint = null;
    public Transform BeHitPoint => m_BeHitPoint;
    [SerializeField] private Transform m_BuffPoint = null;
    public Transform BuffPoint => m_BuffPoint;
    [SerializeField] private Transform m_EffectPoint = null;
    public Transform EffectPoint => m_EffectPoint;
    [SerializeField] private Transform m_TrailPoint = null;
    public Transform TrailPoint => m_TrailPoint;
    [SerializeField] protected Transform m_WeaponPoint = null;
    public Transform WeaponPoint => m_WeaponPoint;



    [SerializeField] private GameObject m_MainBody = null;
    [SerializeField] private GameObject m_MainObject = null;
    [SerializeField] private Transform m_PointUp = null;
    public Transform PointUp => m_PointUp;
    public Transform MainTran => m_MainBody != null ? m_MainBody.transform : transform;
    public virtual UnityObjectData UnityObjectData { get; set; }

    public T GetData<T>()
        where T : UnityObjectData
    {
        return UnityObjectData as T;
    }

    public void SetParent()
    {
        transform.SetParent(UnityObjectData.Parent);
    }

    public void SetLocalScale()
    {
        transform.localScale = UnityObjectData.LocalScale;
    }

    public void SetLocalRotation()
    {
        transform.localRotation = Quaternion.Euler(UnityObjectData.LocalRotation);
    }

    public void SetPosition()
    {
        transform.position = UnityObjectData.WorldPosition;
    }
    public void SetForward()
    {
        transform.forward = UnityObjectData.Forward;
    }
    public void SetColor()
    {
        if (m_MainObject != null && m_MainObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.material.color = UnityObjectData.Color;
        }
    }
    public void PlayerAnimation()
    {
        var animaName = UnityObjectData.GetCurrentAnimationName();
        if (CurAnim != null)
        {
            var animaClip = CurAnim.GetCurrentAnimatorClipInfo(0);
            var animaState = CurAnim.GetCurrentAnimatorStateInfo(0);

            CurAnim.Play(animaName, 0, UnityObjectData.CurAnimaNormalizedTime);
        }
    }

}
