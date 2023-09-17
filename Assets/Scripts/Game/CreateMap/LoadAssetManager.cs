using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Object = System.Object;

public class LoadAssetManager : Singleton<LoadAssetManager>
{
    class LoadData<T>
        where T : Component
    {
        public UnityEngine.Object ass = null;
        public Dictionary<int, T> objs = new();
        public LoadAsyncResult result = LoadAsyncResult.Start;
    }
    Dictionary<AssetKey, LoadData<ObjectPoolBase>> m_DicAsset = new();

    public async UniTask<T> LoadAsync<T>(AssetKey f_ID) where T : ObjectPoolBase
    {
        if (!GTools.TableMgr.TryGetAssetPath(f_ID, out var path)) return null;
        T com = null;
        if (m_DicAsset.TryGetValue(f_ID, out var value))
        {
            await UniTask.WaitUntil(() => value.result != LoadAsyncResult.Loading);
            if (value.result != LoadAsyncResult.Succeed)
            {
                return null;
            }
            if (value.ass is GameObject original)
            {
                var obj = GameObject.Instantiate(original);
                if (obj.TryGetComponent<T>(out com))
                {
                }
            }
        }
        else
        {
            var assetPath = $"{path}";
            value = new();
            value.result = LoadAsyncResult.Loading;

            m_DicAsset.Add(f_ID, value);

            var asset = await LoadAsync<GameObject>(assetPath);
            if (asset != null && asset is GameObject targetObj)
            {
                var obj = GameObject.Instantiate(targetObj);
                if (obj.TryGetComponent<T>(out com))
                {
                    value.ass = asset;
                    value.result = LoadAsyncResult.Succeed;
                }
                else
                {
                    m_DicAsset.Remove(f_ID);
                }
            }
            else
            {
                m_DicAsset.Remove(f_ID);
            }
        }

        if (com == null)
        {
            return null;
        }

        com.AssetKey = f_ID;
        com.SaveID = com.GetInstanceID();
        await com.OnLoadAsync();
        if (com.UpdateInteractable)
        {
            GTools.LifecycleMgr.AddUpdate(com);
        }
        value.objs.Add(com.SaveID, com);
        return com;
    }

    public void UnLoad(ObjectPoolBase f_Asset)
    {
        if (!(m_DicAsset.TryGetValue(f_Asset.AssetKey, out var value) && value.objs.ContainsKey(f_Asset.SaveID))) return;

        if (f_Asset.UpdateInteractable) GTools.LifecycleMgr.RemoveUpdate(f_Asset);

        GTools.RunUniTask(f_Asset.OnUnLoadAsync());
        value.objs.Remove(f_Asset.SaveID);
        GameObject.Destroy(f_Asset.gameObject);
        //if (value.objs.Count == 0)
        //{
        //    UnLoad(value.ass);
        //    m_DicAsset.Remove(f_Asset.AssetKey);
        //}
    }

    public async UniTask<T> LoadAsync<T>(string f_Path)
        where T: UnityEngine.Object
    {
        var result = await Resources.LoadAsync<T>(f_Path) as T;
        return result;
    }
    public void UnLoad<T>(T f_AssetObj)
    where T : UnityEngine.Object
    {
        //Resources.UnloadAsset(f_AssetObj);
    }


}