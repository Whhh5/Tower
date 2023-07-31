using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = System.Object;

namespace B1
{
    public class AssetsMgr : Singleton<AssetsMgr>
    {
        private string GetAssetKey(EAssetName f_Key, EAssetLable f_Lable) => $"{f_Key}.{f_Lable}";

        /// <summary>
        /// 场景加载的资源列表
        /// </summary>
        Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)> m_DicAssets = new();
        #region 公用方法
        /// <summary>
        /// 加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_Key"></param>
        /// <returns></returns>
        private async UniTask<T> LoadAsync<T>(EAssetName f_Key, EAssetLable f_Lable) where T : class
        {
            var asset = await Addressables.LoadAssetAsync<T>(f_Key.ToString());

            #region Console
            var color = asset != null ? "00FF00FF" : "FF0000FF";
            LogWarning($"加载资源  result = {asset != null}   <color=#{color}>path = {f_Key} </color>   ");
            #endregion
            return asset;
        }
        private void UnLoadAsync<T>(T f_Asset) where T: UnityEngine.Object
        {
            Addressables.Release<T>(f_Asset);
        }
        /// <summary>
        /// 卸载一个资源
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="f_Key"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnLoadAsync(EAssetName f_Asset, EAssetLable f_Lable, int f_Key)
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Key = {f_Key}");
                return;
            }


            // 判断 该对象是否引用了该资源
            if (value.objs.ContainsKey(f_Key))
            {
                value.objs.Remove(f_Key);
                // 判断当前资源是否是实例化对象
            }
            else
            {
                LogError($"该对象没有引用 当前资源      dicKey = {disKey}  ");
            }

            // 判断是否从内存中移除资源
            if (value.objs.Count <= 0)
            {
                UnLoadAsync(value.assets as GameObject);
                m_DicAssets.Remove(disKey);
            }
        }
        /// <summary>
        /// 卸载一个继承 mono 的预制体资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_Asset"></param>
        /// <param name="f_Lable"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnLoadPrefabAsync<T>(EAssetName f_Asset, EAssetLable f_Lable, T f_Obj) where T : UnityEngine.MonoBehaviour, IOnDestroyAsync
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Obj = {f_Obj}");
                return;
            }

            // 设置是否卸载实例化的对象
            var insID = f_Obj.GetInstanceID();

            // 判断 该对象是否引用了该资源
            if (value.objs.ContainsKey(insID))
            {
                var coms = f_Obj.GetComponents<IOnDestroyAsync>();
                foreach (var com in coms)
                {
                    await com.OnUnLoadAsync();
                }
                GameObject.Destroy(f_Obj.gameObject);


                await UnLoadAsync(f_Asset, f_Lable, insID);

            }
        }
        public async UniTask UnLoadMonoAsync<T>(EAssetName f_Asset, T f_Obj) where T : UnityEngine.MonoBehaviour, IOnDestroyAsync
        {
            await UnLoadPrefabAsync(f_Asset, EAssetLable.Prefab, f_Obj);
        }
        /// <summary>
        /// 卸载一个类型全部资源
        /// </summary>
        /// <param name="f_Asset"></param>
        /// <param name="f_Lable"></param>
        /// <returns></returns>
        public async UniTask UnLoadByTypeAsync(EAssetName f_Asset, EAssetLable f_Lable)
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Lable = {f_Lable}");
                return;
            }
            if (value.isIns)
            {
                List<UniTask> tasks = new();
                foreach (var item in value.objs)
                {
                    var tempItem = item;
                    tasks.Add(UniTask.Create(async () =>
                    {
                        var coms = tempItem.Value.GetComponents<IOnDestroyAsync>();
                        foreach (var com in coms)
                        {
                            await com.OnUnLoadAsync();
                        }
                        GameObject.Destroy(tempItem.Value);

                        await UnLoadAsync(f_Asset, f_Lable, tempItem.Key);

                    }));
                }
                await UniTask.WhenAll(tasks);
            }
        }

        public async UniTask<(bool tResult, TAsset tObj)> LoadAssetAsync<TAsset>(EAssetName f_Name, EAssetLable f_Lable, int f_key)
            where TAsset : UnityEngine.Object
        {

            var dicKey = GetAssetKey(f_Name, f_Lable);
            var resuult = false;
            TAsset obj = null;
            if (m_DicAssets.TryGetValue(dicKey, out var value))
            {
                if (value.assets is TAsset)
                {
                    obj = value.assets as TAsset;
                    if (!value.objs.ContainsKey(f_key))
                    {
                        value.objs.Add(f_key, null);
                    }
                    else
                    {
                        LogWarning("该 key 已经请求过该资源    请检查此处是否合理，合理则忽略该消息");
                    }
                    resuult = true;
                }
                else
                {
                    LogError($"加载资源失败， 传入类型和已经存在的资源类型不符， 已经存在的资源：{value.assets.GetType()},  请求类型：{typeof(TAsset)}");
                }
            }
            else
            {
                var loadRes = await LoadAsync<TAsset>(f_Name, f_Lable);
                if (loadRes != null)
                {
                    obj = loadRes;

                    m_DicAssets.Add(dicKey, (typeof(TAsset), false, loadRes, new() { { f_key, null } }));

                    resuult = true;
                }
            }

            return (resuult, obj);
        }
        #endregion



        #region 预制体相关
        /// <summary>
        /// 加载一个继承 mono 的预制体, 该方法加载的预制体只能存在一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_EPrefab"></param>
        /// <param name="f_Parent"></param>
        /// <returns></returns>
        public async UniTask<(bool result, T obj)> LoadPrefabAsync<T>(EAssetName f_EPrefab, Transform f_Parent = null) where T : MonoBehaviour
        {
            T obj = default(T);
            bool result = false;
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (!m_DicAssets.TryGetValue(dicKey, out var value))
            {
                var asset = await LoadAsync<GameObject>(f_EPrefab, EAssetLable.Prefab);
                var com = asset?.GetComponent<T>();
                if (com != null)
                {
                    result = true;
                    obj = GameObject.Instantiate<T>(com, f_Parent);
                    m_DicAssets.Add(dicKey, (typeof(T), true, asset, new()));
                    m_DicAssets[dicKey].objs.Add(obj.GetInstanceID(), obj.gameObject);
                    LogWarning($"加载预制体实例化成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
                }
            }
            else if (value.objs.Count > 0)
            {
                var firstElement = value.objs.First();
                if (firstElement.Value.TryGetComponent(out obj))
                {
                    LogWarning("该资源已经加载过还没有卸载  当前读取的是之前就已经加载过的实例");
                }
                else
                {
                    LogWarning($"该对象没有该组件   id = {firstElement.Key}  object name = {firstElement.Value?.name}   Component = {typeof(T)}");
                }
            }
            else
            {
                if (value.assets as T != null)
                {
                    result = true;
                    obj = GameObject.Instantiate<T>(value.assets as T);
                    value.objs.Add(obj.gameObject.GetInstanceID(), obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            return (result, obj);
        }
        /// <summary>
        /// 该方法加载预制体允许存在多个, 回收会被销毁掉，个数 = 0 会卸载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_EPrefab"></param>
        /// <param name="f_Parent"></param>
        /// <returns></returns>
        public async UniTask<T> LoadPrefabsAsync<T>(EAssetName f_EPrefab, Transform f_Parent = null) where T : MonoBehaviour, IOnDestroyAsync
        {
            T obj = default(T);
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (m_DicAssets.TryGetValue(dicKey, out var value))
            {
                if ((value.assets as GameObject).TryGetComponent<T>(out var com))
                {
                    obj = GameObject.Instantiate<T>(com, f_Parent);
                    await obj.OnLoadAsync();
                    m_DicAssets[dicKey].objs.Add(obj.GetInstanceID(), obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            else
            {
                var result = await LoadPrefabAsync<T>(f_EPrefab, f_Parent);
                if (result.result)
                {
                    obj = result.obj;
                    await obj.OnLoadAsync();
                    LogWarning($"加载实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
                }
            }
            return obj;
        }




        private Dictionary<string, PrefabPool> m_DicPrefabPool = new();
        /// <summary>
        /// 该方法加载的预制体永不会被销毁，只有调用 Clear 才会被销毁
        /// </summary>
        /// <returns></returns>
        public async UniTask<T> LoadPrefabPoolAsync<T>(EAssetName f_EPrefab, Transform f_Parent = null, Vector3? f_LocalPos = null, bool f_Active = true, params object[] f_Params) 
            where T : ObjectPoolBase
        {
            var pool = await UpdatePrefanPool(f_EPrefab);
            var insObject = await pool.PopObject<T>(f_Parent ?? GTools.TestRoot, f_LocalPos, f_Active);
            await insObject.OnStartAsync(f_Params);
            return insObject;
        }
        public async UniTask UnLoadPrefabPoolAsync<T>(T f_Target) where T : ObjectPoolBase
        {
            if (m_DicPrefabPool.TryGetValue(f_Target.PoolKey, out var prefabPool))
            {
                f_Target.gameObject.SetActive(false);
                await prefabPool.PushObject(f_Target);
            }
            else
            {
                LogError($"池中不存在该对象 target = {f_Target.name}, key = {f_Target.PoolKey}, type = {f_Target.EAssetName}, lable = {f_Target.EAssetLable}");
            }
        }
        public async UniTask<T> LoadPrefabPoolAsync<T>(Transform f_Parent = null, Vector3? f_LocalPos = null, bool f_Active = true, params object[] f_Params) where T : ObjectPoolBase
        {
            var f_EPrefab = (EAssetName)System.Enum.Parse(typeof(EAssetName), typeof(T).Name);
            return await LoadPrefabPoolAsync<T>(f_EPrefab, f_Parent, f_LocalPos, f_Active, f_Params);
        }
        public async UniTask ClearPrefabPoolAsync()
        {
            List<UniTask> tasks = new();
            foreach (var item in m_DicPrefabPool)
            {
                var tempItem = item;
                tasks.Add(UniTask.Create(async () =>
                {
                    await tempItem.Value.Clear();

                    UnLoadAsync(tempItem.Value.Target.gameObject);
                }));
            }
            m_DicPrefabPool.Clear();
            await UniTask.WhenAll(tasks);
        }
        public async UniTask InitLoadPrefabPoolAsync<T>(EAssetName f_EPrefab, uint f_Count = GTools.PrefabPoolDefaultCount) where T : ObjectPoolBase
        {
            var pool = await UpdatePrefanPool(f_EPrefab);
            pool.InitObj(f_Count);
        }
        public async UniTask<PrefabPool> UpdatePrefanPool(EAssetName f_EPrefab)
        {
            var lable = EAssetLable.Prefab;
            var assetKey = GetAssetKey(f_EPrefab, lable);
            if (!m_DicPrefabPool.TryGetValue(assetKey, out var pool))
            {
                m_DicPrefabPool.Add(assetKey, null);
                var asset = await LoadAsync<GameObject>(f_EPrefab, lable);
                var com = asset.GetComponent<ObjectPoolBase>();

                pool = new(com, assetKey, f_EPrefab);
                m_DicPrefabPool[assetKey] = pool;
            }
            if (pool == null)
            {
                var curTime = GTools.CurTime;
                await UniTask.WaitUntil(() =>
                {
                    if (GTools.CurTime - curTime > 1.0f)
                    {
                        LogError($"等待加载超时 请检查 对象池 {f_EPrefab}");
                        return true;
                    }
                    else
                    {
                        return (pool = m_DicPrefabPool[assetKey]) != null;
                    }
                });
            }
            return pool;
        }


        #endregion
    }
    public class PrefabPool
    {
        private string m_PoolKey;
        private EAssetName m_AssetName;
        public PrefabPool(ObjectPoolBase f_ObjectPoolBase, string f_PoolKey, EAssetName f_AssetName)
        {
            Target = f_ObjectPoolBase;
            m_PoolKey = f_PoolKey;
            m_AssetName = f_AssetName;
        }
        public ObjectPoolBase Target;
        public Dictionary<int, ObjectPoolBase> UseList = new();
        public Dictionary<int, ObjectPoolBase> PoolList = new();


        private bool m_IsUpdate;
        private ObjectPoolBase GetIns(Transform f_Parent = null)
        {
            var obj = GameObject.Instantiate(Target, f_Parent ?? GTools.PoolRoot);
            obj.PoolKey = m_PoolKey;
            obj.AssetName = (int)m_AssetName;
            obj.AssetLable = (int)EAssetLable.Prefab;
            return obj;
        }

        public void InitObj(uint f_Count)
        {
            var count = Mathf.Max(0, f_Count - UseList.Count - PoolList.Count);
            for (int i = 0; i < count; i++)
            {
                var obj = GetIns();
                obj.gameObject.SetActive(false);
                var key = obj.UpdateLevelID;
                PoolList.Add(key, obj);
            }
        }

        public async UniTask<T> PopObject<T>(Transform f_Parent = null, Vector3? f_LocalPos = null, bool f_Active = true) where T : ObjectPoolBase
        {
            T retIns;
            if (PoolList.Count > 0)
            {
                var obj = PoolList.First();
                PoolList.Remove(obj.Key);
                obj.Value.transform.SetParent(f_Parent);
                retIns = obj.Value as T;
            }
            else
            {
                var obj = GetIns(f_Parent);
                retIns = obj as T;
                retIns.name += $" - {obj.GetInstanceID()}";
            }

            retIns.gameObject.SetActive(f_Active);
            retIns.transform.localPosition = f_LocalPos ?? Vector3.zero;

            var key = retIns.GetInstanceID();
            UseList.Add(key, retIns);
            await retIns.OnLoadAsync();

            m_IsUpdate = retIns.UpdateInteractable;
            if (m_IsUpdate)
            {
                GTools.LifecycleMgr.AddUpdate(retIns);
            }

            retIns.IsActively = true;

            return retIns;
        }
        public async UniTask PushObject(ObjectPoolBase f_Obj)
        {
            f_Obj.IsActively = false;
            if (m_IsUpdate)
            {
                GTools.LifecycleMgr.RemoveUpdate(f_Obj);
            }
            await f_Obj.OnUnLoadAsync();
            f_Obj.gameObject.SetActive(false);
            f_Obj.transform.SetParent(GTools.PoolRoot);


            var key = f_Obj.UpdateLevelID;
            if (UseList.ContainsKey(key))
            {
                UseList.Remove(key);
            }
            PoolList.Add(key, f_Obj);
        }
        public async UniTask Clear()
        {
            var tasks = new UniTask[UseList.Count + PoolList.Count];
            var index = 0;
            foreach (var item in UseList)
            {
                var tempItem = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    GameObject.Destroy(tempItem.Value.gameObject);
                });
            }
            foreach (var item in PoolList)
            {
                var tempItem = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    GameObject.Destroy(tempItem.Value.gameObject);
                });
            }
            UseList.Clear();
            PoolList.Clear();
            await UniTask.WhenAll(tasks);
        }
    }
}
