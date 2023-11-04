using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public abstract class UnityObjectData : Base, ILoadPrefabAsync, IUpdateBase, IWorldObjectType
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
    public abstract EAssetKey AssetPrefabID { get; }
    public EPersonStatusType CurStatus { get; protected set; } = EPersonStatusType.Idle;
    public virtual void AfterLoad()
    {
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
        
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 基础数据篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int Index { get; protected set; }
    public abstract EWorldObjectType ObjectType { get; }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 预制体属性篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public string PrefabName = null;
    public bool Active = true;
    public Transform Parent { get; private set; } = null;
    public Vector3 WorldPosition { get; private set; } = new Vector3();
    public Vector3 LocalPosition { get; private set; } = new Vector3(); 
    public Vector3 Forward { get; private set; } = new Vector3(0, 0, 1);
    public Vector3 Up { get; private set; } = new Vector3(0, 1, 0);
    public Vector3 LocalRotation { get; private set; } = new Vector3();
    public Vector3 LocalScale { get; private set; } = new Vector3(1, 1, 1);

    public Color Color { get; private set; } = Color.white;
    // --

    public Transform Tran => PrefabTarget != null ? PrefabTarget.MainTran : null;
    public Vector3 PointUp => PrefabTarget != null && PrefabTarget.PointUp != null
        ? PrefabTarget.PointUp.position
        : WorldPosition;
    public void SetName(string f_Name)
    {
        PrefabName = f_Name;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetName();
        }
    }
    public void SetActive(bool f_ToActive)
    {
        var active = f_ToActive;
        Active = active;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetActive();
        }
    }
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

    public void SetUp(Vector3 f_ToUp)
    {
        Up = f_ToUp.normalized;
        if (PrefabTarget != null)
        {
            PrefabTarget.SetUp();
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
}
// 世界池物体父类
public abstract class ObjectPoolBase : TransformBase, IObjectPoolBase
{
    public bool Active { get; set; }
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
    public bool UpdateInteractable = false;
    public EAssetKey AssetKey { get; set; }
    public int SaveID { get; set; }
    public string PoolKey { get; set; }
    public int AssetName { get; set; }
    public int AssetLable { get; set; }
    public bool IsActively { get; set; }
    public int UpdateLevelID { get; set; }
    public virtual EUpdateLevel UpdateLevel => GTools.PrefabPoolUpdateLevel;

    public EAssetName EAssetName => (EAssetName)AssetName;
    public EAssetLable EAssetLable => (EAssetLable)AssetLable;

    [SerializeField] private GameObject m_MainBody = null;
    [SerializeField] private GameObject m_MainObject = null;
    [SerializeField] private Transform m_PointUp = null;
    public Transform PointUp => m_PointUp;
    public Transform MainTran => m_MainBody != null ? m_MainBody.transform : transform;
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
        SetActive();
        SetName();
    }


    public virtual async UniTask StartExecute()
    {

    }

    public virtual async UniTask StopExecute()
    {

    }



    public virtual UnityObjectData UnityObjectData { get; set; }

    public T GetData<T>()
        where T : UnityObjectData
    {
        return UnityObjectData as T;
    }
    public void SetName()
    {
        name = UnityObjectData.PrefabName ?? name;
    }
    public void SetActive()
    {
        gameObject.SetActive(UnityObjectData.Active);
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
    public void SetUp()
    {
        transform.up = UnityObjectData.Up;
    }
    public void SetColor()
    {
        if (m_MainObject != null && m_MainObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.material.color = UnityObjectData.Color;
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
