using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldObjectType
{
    public EWorldObjectType ObjectType { get; }
}
public class Entity_ChunkMapData : UnityObjectData
{
    public Entity_ChunkMapData() : base(0)
    {

    }
    public int ChunkIndex { get; private set; }
    public override AssetKey AssetPrefabID => AssetKey.Entity_ChunkMap;

    public override EWorldObjectType ObjectType => EWorldObjectType.None;

    public EWorldObjectType CurChunkObjType { get; private set; }


    private Dictionary<EWorldObjectType, List<DependChunkData>> m_ObjDataList = new();

    public void InitData(int f_Index)
    {
        ChunkIndex = f_Index;
        CurChunkObjType = EWorldObjectType.Road;
    }
    public bool AddObject(DependChunkData f_Obj, bool f_isForce = false)
    {
        var objType = f_Obj.ObjectType;
        if (f_isForce || !IsExistObj(objType))
        {
            CurChunkObjType |= objType;
            if (!m_ObjDataList.TryGetValue(objType, out var list))
            {
                list = new();
                m_ObjDataList.Add(objType, list);
            }
            list.Add(f_Obj);
            return true;
        }
        return false;
    }
    public bool ClearObject(DependChunkData f_Obj)
    {
        var objType = f_Obj.ObjectType;
        if (!m_ObjDataList.TryGetValue(objType, out var list))
        {
            return false;
        }
        var result = list.Remove(f_Obj);
        if (result && list.Count == 0)
        {
            m_ObjDataList.Remove(objType);
        }
        return result;
    }
    public bool IsPass()
    {
        return (CurChunkObjType == EWorldObjectType.Road);
    }
    public bool IsExistObj(EWorldObjectType f_DetectionTarget)
    {
        return (CurChunkObjType & f_DetectionTarget) != EWorldObjectType.None;
    }
    public bool GetObjectByType(out List<DependChunkData> f_DataList, EWorldObjectType? f_ObjType = null)
    {
        var objType = f_ObjType;
        f_DataList = new();
        foreach (var item in m_ObjDataList)
        {
            if (objType != null)
            {
                if (objType == item.Key)
                {
                    f_DataList = item.Value;
                }
            }
            else
            {
                f_DataList.AddRange(item.Value);
            }
        }
        return f_DataList.Count > 0;
    }
    public bool IsExistObj(DependChunkData f_ObjData)
    {
        if (m_ObjDataList.TryGetValue(f_ObjData.ObjectType, out var list))
        {
            return list.Contains(f_ObjData);
        }
        return false;
    }
}
public class Entity_ChunkMap : ObjectPoolBase
{
    private string DGID => SaveID.ToString();
    private Entity_ChunkMapData Data => GetData<Entity_ChunkMapData>();
    [SerializeField]
    private List<SpriteRenderer> m_SpriteRenders = new();
    [SerializeField]
    private Color m_EnterToColor = Color.cyan;
    [SerializeField]
    private Color m_OriginalColor;
    [SerializeField]
    private Color m_CurColor;

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        m_CurColor = m_OriginalColor;
        UpdateColor();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }


    private void OnMouseEnter()
    {
        StartEnter();
    }
    private void OnMouseExit()
    {
        StopEnter();
    }

    private void StartEnter()
    {
        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_EnterToColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
        {
            m_CurColor = Color.Lerp(curColor, m_EnterToColor, value);
            UpdateColor();

        }, 1.0f, moveTime * 0.1f)
            .SetId(DGID)
            .SetEase(Ease.OutExpo);
    }
    private void StopEnter()
    {
        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_OriginalColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
        {
            m_CurColor = Color.Lerp(curColor, m_OriginalColor, value);
            UpdateColor();

        }, 1.0f, moveTime)
            .SetId(DGID);
    }
    private void UpdateColor()
    {
        foreach (var item in m_SpriteRenders)
        {
            item.color = m_CurColor;
        }
    }
}
