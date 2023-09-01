using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public abstract class UnityObjectData : Base, ILoadPrefabAsync, IUpdateBase
{
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
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
    public virtual bool IsUpdateEnable => false;
    public virtual void OnUpdate()
    {
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
    //                                catalogue -- 预制体属性篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public Transform Parent { get; private set; } = null;
    public Vector3 WorldPosition { get; private set; } = new Vector3();
    public Vector3 LocalPosition { get; private set; } = new Vector3();
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
    public void SetLocalPosition(Vector3 f_ToPosition)
    {
        LocalPosition = f_ToPosition;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetLocalPosition();
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
    public virtual void SetPersonStatus(EPersonStatusType f_ToStatus, EAnimatorStatus f_AnimaStatus = EAnimatorStatus.Loop)
    {
        if (CurStatus != f_ToStatus)
        {
            CurStatus = f_ToStatus;
            CurAnimaNormalizedTime = 0;
            AnimaStatus = f_AnimaStatus;
            UpdateCallbackStatus();
            PlayerAnimation();
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
    public virtual float AtkSpeedBase { get; set; } = 1;
    protected float m_AddAtkSpeed = 0;
    public float CurAnimaSpeed => Mathf.Clamp(AtkSpeedBase + m_AddAtkSpeed, 0.1f, 10);
    public void ChangeReleaseSpeed(float f_Value)
    {
        var value = f_Value * AtkSpeedBase;
        m_AddAtkSpeed += value;
    }
    // 移动速度
    protected virtual float m_BaseMoveSpeed { get; set; } = 1;
    protected float m_AddMoveSpeed = 0;
    public float CurMoveSpeed => Mathf.Clamp(m_BaseMoveSpeed + m_AddMoveSpeed, 0.1f, 10);
    public void ChangeMoveSpeed(float f_Value)
    {
        var value = f_Value * m_BaseMoveSpeed;
        m_AddMoveSpeed += value;
    }
    // 动画状态
    public EAnimatorStatus AnimaStatus = EAnimatorStatus.None;
    private Dictionary<int, AnomatorCallback> m_DicAnimaCallBack = new();
    private float CurAnimationTime => PrefabTarget != null ? PrefabTarget.CurAnimationTime : 1;
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
        var speed = 1.0f;
        if (CurStatus is EPersonStatusType.Attack or EPersonStatusType.Skill)
        {
            speed = CurAnimaSpeed;
        }
        else if (CurStatus is EPersonStatusType.Walk)
        {
            speed = CurMoveSpeed;
        }
        var value = CurAnimaNormalizedTime + UpdateDelta * speed / CurAnimationTime;
        if (CurStatus == EPersonStatusType.Die && GetType() == typeof(Entity_Incubator1Data))
        {
            Log("");
        }
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
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
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
    public float CurAnimationTime
    {
        get
        {
            var value = 1.0f;
            if (CurAnim != null)
            {
                var curArr = CurAnim.GetCurrentAnimatorClipInfo(0);
                value = curArr != null && curArr.Length > 0 ? curArr[0].clip.length : 1;
            }
            return value;
        }
    }
    public virtual async UniTask OnUnLoadAsync()
    {

    }
    // 加载完成之后调用  queue 1
    public virtual async UniTask OnLoadAsync()
    {
        if (CurAnim != null)
        {
            CurAnim.StopPlayback();
        }
        m_AnimatorGraph = PlayableGraph.Create();
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
    public void SetLocalPosition()
    {
        transform.localPosition = UnityObjectData.LocalPosition;
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
    private string m_LastAnimaClipName = "";
    private PlayableGraph m_AnimatorGraph;
    public void PlayerAnimation()
    {
        var animaName = UnityObjectData.GetCurrentAnimationName();
        if (CurAnim != null)
        {

            CurAnim.Play(animaName, 0, UnityObjectData.CurAnimaNormalizedTime);


            //if (m_LastAnimaClipName != "" && m_LastAnimaClipName != animaName
            //    && TryGetAnimationClip(m_LastAnimaClipName, out var lastAnim) 
            //    && TryGetAnimationClip(animaName, out var curAnima))
            //{
            //    var animOutput = AnimationPlayableOutput.Create(m_AnimatorGraph, "AnimationOutout", CurAnim);

            //    var mixerPlayable = AnimationMixerPlayable.Create(m_AnimatorGraph, 2);

            //    var clipPlayableA = AnimationClipPlayable.Create(m_AnimatorGraph, lastAnim);
            //    var clipPlayableB = AnimationClipPlayable.Create(m_AnimatorGraph, curAnima);


            //    m_AnimatorGraph.Connect(clipPlayableA, 0, mixerPlayable, 0);
            //    m_AnimatorGraph.Connect(clipPlayableB, 0, mixerPlayable, 1);

            //    animOutput.SetSourcePlayable(mixerPlayable);

            //    mixerPlayable.SetInputWeight(0, 1);
            //    mixerPlayable.SetInputWeight(1, 1);

            //    m_AnimatorGraph.Play();

            //}
            m_LastAnimaClipName = animaName;
        }
    }
    private bool TryGetAnimationClip(string f_Name, out AnimationClip f_Clip, int f_Layer = 0)
    {
        var clips = CurAnim.runtimeAnimatorController.animationClips;
        f_Clip = null;
        foreach (var item in clips)
        {
            var hashCode = item.GetHashCode();
            if (!item.name.Contains(f_Name))
            {
                continue;
            }
            //f_Clip = item;
            break;
        }
        return f_Clip != null;
    }

    protected sealed override void Awake()
    {
        base.Awake();
    }
    protected sealed override void Start()
    {
        base.Start();
    }
    protected sealed override void Update()
    {
        base.Update();
    }
}
