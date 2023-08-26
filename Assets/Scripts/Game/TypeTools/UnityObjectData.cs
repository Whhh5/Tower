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
    //                                catalogue -- ��Դ����ƪ
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
    }
    public T GetCom<T>()
        where T : Component
    {
        return PrefabTarget != null ? PrefabTarget.GetComponent<T>() : null;
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Update ƪ
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
    //                                catalogue -- ��������ƪ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int Index { get; private set; }
    public abstract EWorldObjectType ObjectType { get; }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Ԥ��������ƪ
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

    public  void SetLocalScale(Vector3 f_ToLocalScale)
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
    public virtual void SetPersonStatus(EPersonStatusType f_ToStatus, EAnimatorStatus f_AnimaStatus = EAnimatorStatus.Loop)
    {
        if (CurStatus != f_ToStatus)
        {
            CurStatus = f_ToStatus;
            CurAnimaNormalizedTime = 0;
            AnimaStatus = f_AnimaStatus;
            UpdateCallbackStatus();
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ����ƪ
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
    // ��ǰ�������Ž���
    private float m_CurAnimaNormalizedTime = 0;
    public float CurAnimaNormalizedTime
    {
        get => m_CurAnimaNormalizedTime;
        set => m_CurAnimaNormalizedTime = Mathf.Clamp01(value);
    }

    // ��ǰ���������ٶ�
    protected virtual float m_BaseSpeed { get; set; } = 1;
    protected float m_AddAnimaSpeed = 0;
    public float CurAnimaSpeed => Mathf.Clamp(m_BaseSpeed + m_AddAnimaSpeed, 0.1f, 10);
    public void ChangeReleaseSpeed(float f_Value)
    {
        var value = f_Value * m_BaseSpeed;
        m_AddAnimaSpeed += value;
    }
    // �ƶ��ٶ�
    protected virtual float m_BaseMoveSpeed { get; set; } = 1;
    protected float m_AddMoveSpeed = 0;
    public float CurMoveSpeed => Mathf.Clamp(m_BaseMoveSpeed + m_AddMoveSpeed, 0.1f, 10);
    public void ChangeMoveSpeed(float f_Value)
    {
        var value = f_Value * m_BaseMoveSpeed;
        m_AddMoveSpeed += value;
    }
    // ����״̬
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
        // Ŀ�����
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

        // �����ص�
        foreach (var item in m_DicAnimaCallBack)
        {
            if (item.Value.Status && item.Value.NormalizedTime <= value)
            {
                item.Value.Invok();
            }
        }

        // ���½���
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
// ��������常��
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
    public float CurAnimationTime = 1;
    public virtual async UniTask OnUnLoadAsync()
    {

    }
    // �������֮�����  queue 1
    public virtual async UniTask OnLoadAsync()
    {

    }
    // �������֮�����  queue 2
    public virtual void OnUpdate()
    {

    }
    // �ڼ������֮����� queue 3
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
    // �ڼ������֮����� queue 4
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
            var curArr = CurAnim.GetCurrentAnimatorClipInfo(0);
            CurAnimationTime = curArr != null && curArr.Length > 0 ? curArr[0].clip.length : 1;
        }
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
