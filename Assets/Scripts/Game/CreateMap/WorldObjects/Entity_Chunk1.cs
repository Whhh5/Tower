using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Entity_Chunk1Data : WorldObjectBaseData
{
    public Entity_Chunk1Data(int f_Index): base(f_Index, f_Index)
    {
        CurObjectType = EWorldObjectType.None;
    }
    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    public override ELayer LayerMask => ELayer.Terrain;
    public override ELayer AttackLayerMask => ELayer.Default;

    public override AssetKey AssetPrefabID => AssetKey.Chunk1;
    public override void AfterLoad()
    {
        base.AfterLoad();
    }
    public override void OnUnLoad()
    {
        base.OnUnLoad();
        RemoveAllElement();
    }

    // 块索引
    public Vector2 ChunkIndex { get; private set; }

    // 六边形边长
    public static readonly float ChunkRideLength = 1.0f;
    // 块大小
    public static Vector2 ChunkSize => new Vector2(Mathf.Pow(Mathf.Pow(ChunkRideLength, 2) - Mathf.Pow(ChunkRideLength * 0.5f, 2), 0.5f) * 2, ChunkRideLength * 2);


    // 目标实例化的块对象
    private Entity_Chunk1 m_TargetPrefab = null;

    // 当前块上所有物体的类型
    public EWorldObjectType CurObjectType { get; private set; }

    // 当前块上的所有对象
    private Dictionary<ELayer, Dictionary<int, WorldObjectBaseData>> m_AllObjectsLayer = new();
    private Dictionary<EWorldObjectType, Dictionary<int, WorldObjectBaseData>> m_AllObjectsType = new();


    public bool GetWorldObjectByLayer(ELayer f_ObjectType, out Dictionary<int, WorldObjectBaseData> f_Result)
    {
        return m_AllObjectsLayer.TryGetValue(f_ObjectType, out f_Result);
    }
    public bool GetWorldObjectByType(EWorldObjectType f_ObjectType, out Dictionary<int, WorldObjectBaseData> f_Result)
    {
        return m_AllObjectsType.TryGetValue(f_ObjectType, out f_Result);
    }

    private void UpdateColor()
    {
        if (!GTools.TableMgr.TryGetColorByObjectType(EWorldObjectType.None, out var blendColor))
        {
            return;;
        }
        var count = (int)EWorldObjectType.EnumCount;
        for (var i = 0; i < count; i++)
        {
            var type = (EWorldObjectType)(i * Mathf.Clamp01(i));
            if ((CurObjectType & type) != 0 && GTools.TableMgr.TryGetColorByObjectType(type, out var value))
            {
                blendColor += value / count;
            }
        }

        SetColor(blendColor);
        
    }

    public bool AddElement<T>(T f_ObjectData, bool f_IsForce = false) where T : WorldObjectBaseData
    {
        if ((CurObjectType & f_ObjectData.ObjectType) != 0 && !f_IsForce)
        {
            Log($"添加删除快对象 --》 添加失败 已存在相同类型对象 : index = {f_ObjectData.Index}");
            return false;
        }

        if (!m_AllObjectsType.TryGetValue(f_ObjectData.ObjectType, out var list))
        {
            list = new();
            m_AllObjectsType.Add(f_ObjectData.ObjectType, list);
        }
        if (!m_AllObjectsLayer.TryGetValue(f_ObjectData.LayerMask, out var list2))
        {
            list2 = new();
            m_AllObjectsLayer.Add(f_ObjectData.LayerMask, list2);
        }

        if (list.TryAdd(f_ObjectData.Index, f_ObjectData))
        {
            list2.Add(f_ObjectData.Index, f_ObjectData);
            AddChunkObjectType(f_ObjectData.ObjectType);
            return true;
        }
        else
        {
            Log($"添加删除快对象 --》 添加失败 重复添加 : index = {f_ObjectData.Index}");
            return false;
        }
    }

    public void RemoveElement(WorldObjectBaseData fObjectBaseData)
    {
        if (m_AllObjectsType.TryGetValue(fObjectBaseData.ObjectType, out var list) && list.ContainsKey(fObjectBaseData.Index))
        {
            if (list.Count > 1)
            {
                list.Remove(fObjectBaseData.Index);
            }
            else
            {
                m_AllObjectsType.Remove(fObjectBaseData.ObjectType);
                RemoveChunkObjectType(fObjectBaseData.ObjectType);
            }

            var list2 = m_AllObjectsLayer[fObjectBaseData.LayerMask];
            if (list2.Count > 1)
            {
                list2.Remove(fObjectBaseData.Index);
            }
            else
            {
                m_AllObjectsLayer.Remove(fObjectBaseData.LayerMask);
            }

        }
        else
        {
            Log($"添加删除快对象 --》 删除失败 不存在索引 : index = {fObjectBaseData.Index}");
        }
    }

    public void RemoveAllElement()
    {
        m_AllObjectsLayer.Clear();
        foreach (var VARIABLE2 in m_AllObjectsType.SelectMany(VARIABLE => VARIABLE.Value))
        {
            ILoadPrefabAsync.UnLoad(VARIABLE2.Value);
        }
    }

    private void AddChunkObjectType(EWorldObjectType fEWorldObjectType)
    {
        CurObjectType |= fEWorldObjectType;
        UpdateColor();
    }

    private void RemoveChunkObjectType(EWorldObjectType fEWorldObjectType)
    {
        CurObjectType &= ~fEWorldObjectType;
        UpdateColor();
    }

    public bool IsAlreadyType(EWorldObjectType f_ObjectType)
    {
        return (CurObjectType & f_ObjectType) != 0;
    }
    public bool IsAlreadyLayer(ELayer f_Layer)
    {
        return m_AllObjectsLayer.ContainsKey(f_Layer);
    }
}
public class Entity_Chunk1 : WorldObjectBase
{
    // 文字部分
    [SerializeField]
    private TextMeshPro m_TMPIndex = null;
    [SerializeField]
    private TextMeshPro m_TMPRowCol = null;
    [SerializeField]
    private TextMeshPro m_TMPHeight = null;
    [SerializeField]
    private Transform m_TargetRoot = null;

    private List<Material> m_Materials = new();

    public Entity_Chunk1Data ChunkData => TryGetData<Entity_Chunk1Data>(out var data) ? data : null;

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        m_TMPIndex.text = $"{ChunkData.Index}";
        m_TMPRowCol.text = $"{WorldMapManager.Ins.GetRowCol(ChunkData.Index)}";
        m_TMPHeight.text = $"H:{ChunkData.LocalScale.y:0.00}";

        foreach (Transform item in m_TargetRoot)
        {
            if (item.TryGetComponent<MeshRenderer>(out var com))
            {
                m_Materials.Add(com.material);
            }
        }
        OnMouseExit();
    }
    public override async UniTask StartExecute()
    {
        
    }

    public override async UniTask StopExecute()
    {
        
    }

    private void SetColor(Color f_Color)
    {
        foreach (var item in m_Materials)
        {
            item.color = f_Color;
        }
    }
    private void OnMouseEnter()
    {
        SetColor(Color.yellow);
        WorldMapManager.Ins.SetCurMouseEnable(ChunkData?.Index ?? -1);
    }
    private void OnMouseExit()
    {
        SetColor(Color.gray);
        WorldMapManager.Ins.ClearCurMouseEnable();
    }
}
