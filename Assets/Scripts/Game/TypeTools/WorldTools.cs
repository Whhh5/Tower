using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


// enum

public enum EDamageType
{
    None,

    /// <summary>
    /// 物理
    /// </summary>
    Physical,

    /// <summary>
    /// 物理暴击
    /// </summary>
    PhCritical,

    /// <summary>
    /// 魔法
    /// </summary>
    Magic,

    /// <summary>
    /// 魔法暴击
    /// </summary>
    MaCritical,

    /// <summary>
    /// 真实伤害
    /// </summary>
    True,

    /// <summary>
    /// 增加血量
    /// </summary>
    AddBlood,
}

public interface IExecute
{
    // 模块开始执行
    UniTask StartExecute();

    // 模块停止执行
    UniTask StopExecute();
}

public interface IButton3DClick
{
    UniTask OnClickAsync();
    UniTask OnClick2Async();
}

#region 对象池相关

public interface IObjectPoolBase : IOnDestroyAsync, IUpdateBase
{
    string PoolKey { get; set; }
    int AssetName { get; set; }
    int AssetLable { get; set; }
    bool IsActively { get; set; }
}

public enum LoadAsyncResult
{
    Start,
    UnLoad,
    Succeed,
    Loading,
    Defeated,
}

public interface ILoadPrefabAsync
{
    public int LoadKey { get; set; }
    public ObjectPoolBase PrefabTarget { get; set; }
    public LoadAsyncResult LoadResult { get; set; }
    public AssetKey AssetPrefabID { get; }

    public void AfterLoad();
    public void OnUnLoad();
    public static async UniTask LoadAsync<TLoad>(TLoad f_Target)
        where TLoad : UnityObjectData
    {
        if (f_Target.LoadResult is LoadAsyncResult.Loading or LoadAsyncResult.Succeed or LoadAsyncResult.UnLoad)
        {
            return;
        }
        var lastLoadKey = Random.Range(int.MinValue + 1, int.MaxValue);
        f_Target.LoadKey = lastLoadKey;
        f_Target.LoadResult = LoadAsyncResult.Loading;
        var result = await LoadAssetManager.Ins.LoadAsync<ObjectPoolBase>(f_Target.AssetPrefabID);
        if (result != null)
        {
            if (f_Target.LoadResult is LoadAsyncResult.UnLoad || lastLoadKey != f_Target.LoadKey)
            {
                LoadAssetManager.Ins.UnLoad(result);
            }
            else if (f_Target.LoadResult is LoadAsyncResult.Loading)
            {
                f_Target.LoadResult = LoadAsyncResult.Succeed;
                f_Target.PrefabTarget = result;
                result.SetUnityObjectData(f_Target);
                await result.OnStartAsync();
                f_Target.AfterLoad();
            }
        }
        else
        {
            f_Target.LoadResult = LoadAsyncResult.Defeated;
        }
    }

    public static void UnLoad<TLoad>(TLoad f_Target)
        where TLoad : UnityObjectData
    {
        f_Target.OnUnLoad();
        f_Target.LoadKey = int.MinValue;
        if (f_Target.LoadResult == LoadAsyncResult.Succeed)
        {
            LoadAssetManager.Ins.UnLoad(f_Target.PrefabTarget);
            f_Target.PrefabTarget = null;
        }

        f_Target.LoadResult = LoadAsyncResult.UnLoad;

    }

    // public async UniTask LoadPrefabAsync()
    // {
    //     if (LoadResult is LoadAsyncResult.Loading or LoadAsyncResult.Succeed or LoadAsyncResult.UnLoad)
    //     {
    //         return;
    //     }
    //
    //     var lastLoadKey = Random.Range(int.MinValue + 1, int.MaxValue);
    //     LoadKey = lastLoadKey;
    //     LoadResult = LoadAsyncResult.Loading;
    //     var result = await LoadAssetManager.Ins.LoadAsync<ObjectPoolBase>(AssetPrefabID());
    //     if (result != null)
    //     {
    //         if (LoadResult is LoadAsyncResult.UnLoad || lastLoadKey != LoadKey)
    //         {
    //             LoadAssetManager.Ins.UnLoad(result);
    //         }
    //         else if (LoadResult is LoadAsyncResult.Loading)
    //         {
    //             LoadResult = LoadAsyncResult.Succeed;
    //             PrefabTarget = result;
    //         }
    //     }
    //     else
    //     {
    //         LoadResult = LoadAsyncResult.Defeated;
    //     }
    // }
    //
    // public void UnLoadPrefab()
    // {
    //     LoadKey = int.MinValue;
    //     if (LoadResult == LoadAsyncResult.Succeed)
    //     {
    //         LoadAssetManager.Ins.UnLoad(PrefabTarget);
    //         PrefabTarget = null;
    //     }
    //
    //     LoadResult = LoadAsyncResult.UnLoad;
    // }
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
    public Animator CurAnim => GetComponent<Animator>();
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
        where T: UnityObjectData
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
        transform.localRotation = UnityObjectData.LocalRotation;
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

#endregion