using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapNew : Singleton<CreateMapNew>
{
    private Vector2Int MapWH => GameDataMgr.MapWH;
    private Dictionary<EBarrierType, List<BarrierData>> BarrierData => GameDataMgr.BarrierData;
    private float MapChunkLength => GameDataMgr.MapChunkLength;
    private Vector2 MapChunkInterval => GameDataMgr.MapChunkInterval;


    private Transform m_MapRoot = null;
    public override void Awake()
    {
        base.Awake();

        var obj = new GameObject("Root_MapChunk");
        m_MapRoot = obj.transform;
        m_MapRoot.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }



    public void CreateMap()
    {
        ClearMapAssets();
        CreateMapAssets();

        CreateMapEntityAsync();
    }




    private Dictionary<int, Entity_ChunkMapData> m_ChunkDataList = new();
    private void CreateMapAssets()
    {
        for (int i = 0; i < MapWH.x; i++)
        {
            for (int j = 0; j < MapWH.y; j++)
            {
                var chunkIndex = RowColToIndex(new Vector2Int(i, j));
                CreateMapChunk(i, j, chunkIndex);
            }
        }

        CreateMapBarrizer();
    }
    private void ClearMapAssets()
    {
        ClearMapBarrizer();
        ClearMapChunk();
    }
    private async void CreateMapEntityAsync()
    {
        await CreateMapChunkEntityAsync();
        await CreateMapBarrizerEntityAsync();
    }
    private void CreateMapChunk(int f_Row, int f_Col, int f_Index)
    {
        var chunkData = new Entity_ChunkMapData();
        chunkData.SetParent(m_MapRoot);
        chunkData.InitData(f_Index);
        m_ChunkDataList.Add(f_Index, chunkData);

        // pos math
        var xInterval = Mathf.Pow(0.75f * MapChunkLength, 0.5f) * 2;
        var yInterval = MapChunkLength * 2;
        var pos = new Vector3(
            xInterval * 0.5f + f_Col * (yInterval + MapChunkInterval.x),
            yInterval * 0.5f + f_Row * (xInterval + MapChunkInterval.y), 0);
        var posOffsetX = f_Row % 2 * (xInterval + MapChunkInterval.x) * 0.5f;
        pos.x += posOffsetX;
        chunkData.SetPosition(pos + m_MapRoot.position);
    }
    private void ClearMapChunk()
    {
        ClearMapChunkEntityAsync();
        m_ChunkDataList.Clear();
    }
    private async UniTask CreateMapChunkEntityAsync()
    {
        List<UniTask[]> tasksList = new();
        for (int j = 0; j < MapWH.y; j++)
        {
            UniTask[] tasks = new UniTask[MapWH.x];
            for (int i = 0; i < MapWH.x; i++)
            {
                var chunkIndex = RowColToIndex(new Vector2Int(i, j));
                if (TryGetChunkData(chunkIndex, out var chunkData))
                {
                    UniTask task = UniTask.Create(async () =>
                    {
                        await ILoadPrefabAsync.LoadAsync(chunkData);
                    });
                    tasks[i] = task;
                }
            }
            tasksList.Add(tasks);
        }

        foreach (var item in tasksList)
        {
            await UniTask.WhenAll(item);
            await UniTask.Delay(100);
        }
    }
    private void ClearMapChunkEntityAsync()
    {
        foreach (var item in m_ChunkDataList)
        {
            if (item.Value.GetObjectByType(out var list))
            {
                foreach (var element in list)
                {
                    ILoadPrefabAsync.UnLoad(element);
                }
            }
            ILoadPrefabAsync.UnLoad(item.Value);
        }
    }
    private void CreateMapBarrizer()
    {
        foreach (var item in BarrierData)
        {
            if (item.Key == EBarrierType.Massif)
            {
                foreach (var data in item.Value)
                {
                    var marrizerData = new Entity_MassifData();
                    var index = RowColToIndex(data.Index);
                    marrizerData.InitData(index);
                    marrizerData.SetParent(m_MapRoot);
                }
            }
        }
    }
    private void ClearMapBarrizer()
    {

    }
    private async UniTask CreateMapBarrizerEntityAsync()
    {
        if (!TryGetAllObject(EWorldObjectType.Wall, out var dataList))
        {
            return;
        }
        foreach (var item in dataList)
        {
            await ILoadPrefabAsync.LoadAsync(item);
            await UniTask.Delay(200);
        }
    }
    private void ClearMapBarrizerEntityAsync()
    {
        if (!TryGetAllObject(EWorldObjectType.Wall, out var dataList))
        {
            return;
        }
        foreach (var item in dataList)
        {
            ILoadPrefabAsync.UnLoad(item);
        }
    }
    public int RowColToIndex(Vector2Int f_RowCol)
    {
        return f_RowCol.x * MapWH.y + f_RowCol.y;
    }
    public Vector2Int IndexToRowCol(int f_Index)
    {
        return new Vector2Int(f_Index / MapWH.y, f_Index % MapWH.y);
    }
    public bool TryGetChunkData(int f_Index, out Entity_ChunkMapData f_ChunkData)
    {
        return m_ChunkDataList.TryGetValue(f_Index, out f_ChunkData);
    }
    public bool AddChunkElement(DependChunkData f_Elementdata, int? f_ChunkIndex = null, bool f_IsForce = false)
    {
        var chunkIndex = f_ChunkIndex ?? f_Elementdata.CurrentIndex;
        if (chunkIndex == f_Elementdata.CurrentIndex)
        {
            return false;
        }
        if (!TryGetChunkData(chunkIndex, out var chunkData))
        {
            return false;
        }
        var result1 = chunkData.AddObject(f_Elementdata, f_IsForce);
        return result1;
    }
    public bool ClearChunkElement(DependChunkData f_Elementdata, int? f_ChunkIndex = null)
    {
        var chunkIndex = f_ChunkIndex ?? f_Elementdata.CurrentIndex;

        if (!TryGetChunkData(chunkIndex, out var chunkData))
        {
            return false;
        }
        if (!chunkData.IsExistObj(f_Elementdata))
        {
            return false;
        }
        chunkData.ClearObject(f_Elementdata);
        f_Elementdata.SetCurrentChunkIndex(-1);
        return true;
    }
    public bool MoveToChunk(DependChunkData f_Elementdata, int f_Index)
    {
        ClearChunkElement(f_Elementdata);

        var result = AddChunkElement(f_Elementdata, f_Index);

        return result;
    }
    public bool TryGetAllObject(EWorldObjectType f_ObjectType, out List<DependChunkData> f_DataList)
    {
        f_DataList = new();
        foreach (var item in m_ChunkDataList)
        {
            if (!item.Value.GetObjectByType(out var list, f_ObjectType))
            {
                continue;
            }
            foreach (var data in list)
            {
                f_DataList.Add(data);
            }
        }
        return f_DataList.Count > 0;
    }
}