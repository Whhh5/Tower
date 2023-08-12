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
        if (f_Target.LoadResult is LoadAsyncResult.Loading or LoadAsyncResult.Succeed)
        {
            return;
        }
        var lastLoadKey = Random.Range(int.MinValue + 1, int.MaxValue);
        f_Target.LoadKey = lastLoadKey;
        f_Target.LoadResult = LoadAsyncResult.Loading;
        var result = await LoadAssetManager.Ins.LoadAsync<ObjectPoolBase>(f_Target.AssetPrefabID);
        if (result != null)
        {
            if (lastLoadKey != f_Target.LoadKey)
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
}


#endregion