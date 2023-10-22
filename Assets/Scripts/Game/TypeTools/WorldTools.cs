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

public interface ILoadSpriteAsync
{
    public static async UniTask<Sprite> LoadAsync(string f_Path)
    {
        var request = await Resources.LoadAsync<Sprite>(f_Path) as Sprite;
        return request;
    }
    public static async UniTask<Sprite> LoadAsync(AssetKey f_SpriteKey)
    {
        Sprite result = null;
        if (GTools.TableMgr.TryGetAssetPath(f_SpriteKey, out var path))
        {
            result = await LoadAsync(path);
        }
        return result;
    }
    public static void UnLoad(Object f_Assets)
    {
        Resources.UnloadAsset(f_Assets);
    }
}
public interface ILoadPrefabAsync
{
    public int LoadKey { get; set; }
    public ObjectPoolBase PrefabTarget { get; set; }
    public LoadAsyncResult LoadResult { get; set; }
    public AssetKey AssetPrefabID { get; }

    public void AfterLoad();
    public void OnUnLoad();





    private static int m_LoadKey = int.MinValue;
    private static ListStack<int> m_LoadKeyList = new("load key", 200);
    private static int GetLoadKey()
    {
        if (!m_LoadKeyList.TryPop(out var key))
        {
            key = ++m_LoadKey;
        }
        return key;
    }

    private static Dictionary<EWorldObjectType, Dictionary<int, UnityObjectData>> m_DicEntity = new();
    public static async UniTask LoadAsync<TLoad>(TLoad f_Target)
        where TLoad : UnityObjectData
    {
        if (f_Target.LoadResult is LoadAsyncResult.Loading or LoadAsyncResult.Succeed)
        {
            return;
        }
        var lastLoadKey = GetLoadKey();
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
                if (!m_DicEntity.TryGetValue(f_Target.ObjectType, out var list))
                {
                    list = new();
                    m_DicEntity.Add(f_Target.ObjectType, list);
                }
                if (!list.ContainsKey(lastLoadKey))
                {
                    list.Add(lastLoadKey, f_Target);
                }
                f_Target.LoadResult = LoadAsyncResult.Succeed;
                f_Target.PrefabTarget = result;
                result.SetUnityObjectData(f_Target);
                await result.OnStartAsync();
                f_Target.AfterLoad();

                if (f_Target is WorldObjectBaseData data)
                {
                    WeatherMgr.Ins.InflictionGain(data);
                }
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
        var loadKey = f_Target.LoadKey;
        f_Target.LoadKey = int.MinValue;
        if (f_Target.LoadResult == LoadAsyncResult.Succeed)
        {
            if (m_DicEntity.TryGetValue(f_Target.ObjectType, out var list) && list.ContainsKey(loadKey))
            {
                list.Remove(loadKey);
            }
            m_LoadKeyList.Push(loadKey);
            LoadAssetManager.Ins.UnLoad(f_Target.PrefabTarget);
            f_Target.PrefabTarget = null;
        }

        f_Target.LoadResult = LoadAsyncResult.UnLoad;

    }
    public static bool TryGetEntityByType<T>(EWorldObjectType f_ObjectType, out Dictionary<int, T> f_Result)
        where T : UnityObjectData
    {
        f_Result = new();
        foreach (var item in m_DicEntity)
        {
            if ((item.Key & f_ObjectType) != 0)
            {
                foreach (var entity in item.Value)
                {
                    if (entity.Value is T target)
                    {
                        f_Result.Add(entity.Key, target);
                    }
                }
            }
        }
        return f_Result.Count > 0;
    }
}


#endregion