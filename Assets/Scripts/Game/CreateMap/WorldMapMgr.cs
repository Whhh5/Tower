using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Flags] // 方向枚举
public enum EDirection : int
{
    None = 0,
    LeftUp = 1 << 0,
    RightUp = 1 << 1,
    LeftBottom = 1 << 2,
    RightBottom = 1 << 3,
    Left = 1 << 4,
    Right = 1 << 5,
    EnumCount = 6,
}

// 路径点数据
public class PathElementData
{
    /// <summary>
    /// 地图快索引
    /// </summary>
    public int Index { get; private set; }

    public int ChunkIndex => Index;
    public PathElementData LastElement { get; private set; }
    public float ToPrice { get; private set; }
    public int FormPrice { get; private set; }
    public float PriceCount => ToPrice + 1;

    public Vector3 WorldPos => WorldMapMgr.Ins.TryGetChunkData(ChunkIndex, out var data) ? data.PointUp : Vector3.zero;

    public PathElementData(int f_Index, PathElementData f_LastElement, int f_FormPrice, float f_Price)
    {
        Index = f_Index;
        LastElement = f_LastElement;
        ToPrice = f_Price;
        FormPrice = f_FormPrice;
    }

    public static bool operator >(PathElementData f_Data1, PathElementData f_Data2)
    {
        return f_Data1.PriceCount > f_Data2.PriceCount;
    }

    public static bool operator <(PathElementData f_Data1, PathElementData f_Data2)
    {
        return f_Data1.PriceCount < f_Data2.PriceCount;
    }

    public static bool GetMinPrice(out PathElementData f_Result, params PathElementData[] f_Elements)
    {
        f_Result = null;
        if (f_Elements.Length > 0)
        {
            f_Result = f_Elements[0];
            foreach (var item in f_Elements)
            {
                if (f_Result > item)
                {
                    f_Result = item;
                }
            }

            return true;
        }

        return false;
    }

    public static bool GetMinPrice(List<PathElementData> f_Elements, out PathElementData f_Result)
    {
        f_Result = null;
        if (f_Elements.Count > 0)
        {
            f_Result = f_Elements[0];
            foreach (var item in f_Elements)
            {
                if (f_Result > item)
                {
                    f_Result = item;
                }
            }

            return true;
        }

        return false;
    }

    public static bool GetMaxPrice(List<PathElementData> f_Elements, out PathElementData f_Result)
    {
        f_Result = null;
        if (f_Elements.Count > 0)
        {
            f_Result = f_Elements[0];
            foreach (var item in f_Elements)
            {
                if (f_Result < item)
                {
                    f_Result = item;
                }
            }

            return true;
        }

        return false;
    }
}

public class WorldMapMgr : B1.Singleton<WorldMapMgr>
{
    //                                ------------------------------------------------
    //                                --------------------Catalogue 生命周期函数
    //                                ------------------------------------------------
    public override void Awake()
    {
        base.Awake();

        CreateChunkTest();
        InitMonsterSpawnPointData();
        CreateRoadExtend();
    }


    //                                ------------------------------------------------
    //                                --------------------Catalogue 地图块相关
    //                                ------------------------------------------------
    // 当前地图唯一 id
    public int CurrentMapKey { get; private set; } = Int32.MinValue;

    // 一个 unity 单位相当于多少个像素
    private const int m_UnityUnitToPixelRatio = 10;

    // 整个世界地图的长宽
    private Vector2Int m_WorldMapSize = new Vector2Int(100, 100);

     // scaleX: x-y, ScaleZ: z-w  块大小范围
    private Vector4 ChunkSizeRandom = new Vector4(1, 2, 1, 2);

     // 地图快行列
    public Vector2Int RowCol = new(10, 20);

     // 地图快间隔
    public Vector2 Interval = new(0.2f, 0.2f);

    // 存放所有的快
    private Dictionary<int, Entity_Chunk1Data> m_DicChunk = new();

    // 地图快总数
    public int ChunkCount => m_DicChunk.Count;

    //protected override void Awake()
    //{
    //    base.Awake();


    //    m_MapTexture = new Texture2D(m_WorldMapSize.x * m_UnityUnitToPixelRatio,
    //        m_WorldMapSize.y * m_UnityUnitToPixelRatio, TextureFormat.RGBA64, false, true);
    //}

    // 开始异步清除所有的块
    private async UniTask ClearAllChunkAsync()
    {
        m_DicMainRoad.Clear();
        m_DicMonsterSpawnPoint.Clear();
        m_DicRoad.Clear();


        var tempDic = m_DicChunk;
        m_DicChunk = new();
        foreach (var item in tempDic)
        {
            ILoadPrefabAsync.UnLoad(item.Value);
            await UniTask.Delay(2);
        }
    }

    [Button] // 初始化块数据
    public async void CreateChunkTest(Vector2Int? f_RowCol = null, Vector2? f_Interval = null)
    {
        var rowCol = f_RowCol ?? RowCol;
        var interval = f_Interval ?? Interval;
        UniTask[] tasks =
        {
            // 清楚数据
            UniTask.Create(async () => await ClearAllChunkAsync()),
            // 创建
            UniTask.Create(async () =>
            {
                CurrentMapKey++;
                // 初始化数据
                for (var i = 0; i < rowCol.x; i++)
                {
                    for (var j = 0; j < rowCol.y; j++)
                    {
                        var pos = new Vector2
                        (
                            j * interval.x + (j + 1 + (i % 2 /*奇数行偏移*/ - 1 /*行偏移*/) * 0.5f) *
                            Entity_Chunk1Data.ChunkSize.x,
                            i * interval.y + (3 * i + 2) / 4.0f * Entity_Chunk1Data.ChunkSize.y
                        );
                        var key = i * rowCol.y + j;
                        var chunk = new Entity_Chunk1Data(key);
                        chunk.SetPosition(new Vector3(pos.x, 0, pos.y));

                        m_DicChunk.Add(key, chunk);
                    }
                }

                await CreateChunkAsync();
            }),
        };

        await UniTask.WhenAll(tasks);
    }

    // 创建实例化块对象
    public async UniTask CreateChunkAsync()
    {
        foreach (var item in m_DicChunk)
        {
            await ILoadPrefabAsync.LoadAsync(item.Value);
            await UniTask.Delay(2);
        }
    }


    // 根据索引获得行列
    public Vector2Int GetRowCol(int f_Index)
    {
        if (f_Index < 0 || f_Index >= ChunkCount)
        {
            return -1 * Vector2Int.one;
        }

        var x = f_Index / RowCol.y;
        var y = f_Index % RowCol.y;
        return new Vector2Int(x, y);
    }

    // 根据行列获得索引
    public int GetIndex(Vector2Int f_RowCol)
    {
        if (f_RowCol.x < 0 || f_RowCol.x >= RowCol.x || f_RowCol.y < 0 || f_RowCol.y >= RowCol.y)
        {
            return -1;
        }

        var index = f_RowCol.x * RowCol.y + f_RowCol.y;
        return index;
    }

    // 获得索引周围的索引
    public bool GetDirectionChunk(int f_Index, EDirection f_Dir, out int f_Result)
    {
        var curRowCol = GetRowCol(f_Index);
        if (curRowCol.x < 0 || curRowCol.y < 0)
        {
            f_Result = -1;
            return false;
        }


        if ((f_Dir & EDirection.Left) != EDirection.None)
        {
            curRowCol.y--;
        }

        if ((f_Dir & EDirection.Right) != EDirection.None)
        {
            curRowCol.y++;
        }

        if ((f_Dir & EDirection.LeftUp) != EDirection.None)
        {
            if (curRowCol.x % 2 == 0)
            {
                curRowCol.y--;
            }

            curRowCol.x++;
        }

        if ((f_Dir & EDirection.LeftBottom) != EDirection.None)
        {
            if (curRowCol.x % 2 == 0)
            {
                curRowCol.y--;
            }

            curRowCol.x--;
        }

        if ((f_Dir & EDirection.RightUp) != EDirection.None)
        {
            if (curRowCol.x % 2 != 0)
            {
                curRowCol.y++;
            }

            curRowCol.x++;
        }

        if ((f_Dir & EDirection.RightBottom) != EDirection.None)
        {
            if (curRowCol.x % 2 != 0)
            {
                curRowCol.y++;
            }

            curRowCol.x--;
        }

        f_Result = GetIndex(curRowCol);
        return m_DicChunk.ContainsKey(f_Result);
    }

    [Button] // A*测试代码
    public async UniTask CreatePath_AStar(int f_Start, int f_End)
    {
    }

    [Button] // 清理块
    public async void CreateMapChunk_Clear()
    {
        await ClearAllChunkAsync();
    }

    // 设置块的颜色
    public void SetChunkColor(int f_Index, Color f_ToColor)
    {
        var chunk = m_DicChunk[f_Index];
    }
    // 获得某个块的参考点
    public Vector3 GetChunkPoint(int f_Index)
    {
        return TryGetChunkData(f_Index, out var chunkData) ? chunkData.PointUp : Vector3.zero;
    }
    // 获得某个块的参考点
    public Vector3 GetChunkPoint(Vector2Int f_RowCol)
    {
        var index = GetIndex(f_RowCol);
        return GetChunkPoint(index);
    }
    // 根据索引获得某个块
    public bool TryGetChunkData(int f_Index, out Entity_Chunk1Data fObjectBaseChunkData)
    {
        return m_DicChunk.TryGetValue(f_Index, out fObjectBaseChunkData);
    }
    // 遍历所有的块
    public void LoopChunk(Action<KeyValuePair<int, Entity_Chunk1Data>> f_Loop)
    {
        foreach (var VARIABLE in m_DicChunk)
        {
            f_Loop.Invoke(VARIABLE);
        }
    }
    // 获得块上某一类型的物体
    public bool TryGetWorldObjectByType(EWorldObjectType f_ObjectType, out Dictionary<int, DependChunkData> f_Result, bool f_IsInclude = false)
    {
        Dictionary<int, DependChunkData> data = new();
        LoopChunk(VARIABLE =>
        {
            if (((f_IsInclude && (VARIABLE.Value.CurObjectType & f_ObjectType) == f_ObjectType)
                || (!f_IsInclude && (VARIABLE.Value.CurObjectType & f_ObjectType) != 0))
                && VARIABLE.Value.GetWorldObjectByType(f_ObjectType, out var list))
            {
                data.AddRange(list);
            }
        });

        f_Result = data.Count > 0 ? data : null;

        return f_Result != null;
    }
    // 获得块上某一类型的物体
    public bool TryGetWorldObjectByLayer(ELayer f_Layer, out Dictionary<int, DependChunkData> f_Result)
    {
        Dictionary<int, DependChunkData> data = new();
        LoopChunk(VARIABLE =>
        {
            if (VARIABLE.Value.GetWorldObjectByLayer(f_Layer, out var list))
            {
                data.AddRange(list);
            }
        });

        f_Result = data.Count > 0 ? data : null;

        return f_Result != null;
    }
    // 获得块上某一层级的物体
    public bool TryGetChunkObjectByType(int f_Index, EWorldObjectType f_ObjectType, out Dictionary<int, DependChunkData> f_Result)
    {
        if (TryGetChunkData(f_Index, out var chunkData) && chunkData.GetWorldObjectByType(f_ObjectType, out f_Result))
        {
            return true;
        }
        f_Result = null;
        return false;
    }
    public bool TryGetChunkObjectByLayer(int f_Index, ELayer f_ObjectType, out Dictionary<int, DependChunkData> f_Result)
    {
        if (TryGetChunkData(f_Index, out var chunkData) && chunkData.GetWorldObjectByLayer(f_ObjectType, out f_Result))
        {
            return true;
        }
        f_Result = null;
        return false;
    }
    // 获得周围的块
    public bool TryGetRangeChunkByIndex(int f_Index, out ListStack<int> f_IndexList, Func<int, bool> f_Condition = null,
        bool f_IsThis = false, int f_Extend = 1)
    {
        // 最终结果列表
        ListStack<int> resultList = new("", f_Extend * 6 + 1 + (f_IsThis ? 1 : 0));
        // 已经遍历过的列表
        Dictionary<int, bool> already = new();
        var loopNum = 0;

        void Loop(Dictionary<int, bool> f_In)
        {
            if (loopNum++ >= f_Extend) return;
            // 下一次循环需要便利的列表
            Dictionary<int, bool> nextDic = new();
            foreach (var target in f_In)
            {
                for (var i = 0; i < (int)EDirection.EnumCount; i++)
                {
                    var dir = (EDirection)(1 << i);
                    if (!GetDirectionChunk(target.Key, dir, out var result)) continue;

                    if (already.ContainsKey(result)) continue;

                    nextDic.Add(result, true);
                    already.Add(result, true);

                    if (f_Condition != null && !f_Condition.Invoke(result)) continue;

                    resultList.Push(result);
                }
            }

            if (loopNum >= f_Extend) return;

            Loop(nextDic);
        }

        Loop(new() { { f_Index, true } });
        if (f_IsThis && (f_Condition == null || f_Condition.Invoke(f_Index)))
        {
            resultList.Push(f_Index);
        }

        f_IndexList = resultList;
        return resultList.Count > 0;
    }
    public bool IsInNearByIndex(int f_Index, int f_Target, int f_Range = 1)
    {
        if (TryGetRangeChunkByIndex(f_Index, out var result, index => true, false, f_Range))
        {
            return result.Contains(f_Target);
        }
        return false;
    }
    // 在块上添加一个元素对象
    public bool AddChunkElement<T>(T f_Element, bool f_IsForce = false)
        where T : DependChunkData
    {
        if (TryGetChunkData(f_Element.CurrentIndex, out var chunk))
        {
            var result = chunk.AddElement(f_Element, f_IsForce);
            return result;
        }
        return false;
    }
    // 在块上删除一个元素对象
    public void RemoveChunkElement<T>(T f_Element)
        where T : DependChunkData
    {
        if (TryGetChunkData(f_Element.CurrentIndex, out var chunk))
        {
            chunk.RemoveElement(f_Element);
        }
    }
    public void MoveChunkElement<T>(T f_Element, int f_ToChunkIndex)
        where T : DependChunkData
    {
        RemoveChunkElement(f_Element);
        f_Element.SetCurrentChunkIndex(f_ToChunkIndex);
        AddChunkElement(f_Element);
    }
    public bool TryGetChunkByType(EWorldObjectType f_Type, out Dictionary<int, Entity_Chunk1Data> f_Result, bool f_IsInclude = false)
    {
        Dictionary<int, Entity_Chunk1Data> newData = new();
        LoopChunk(value =>
        {
            var type = value.Value.CurObjectType & f_Type;
            if ((f_IsInclude && type != 0) || (!f_IsInclude && type == f_Type))
            {
                newData.Add(value.Key, value.Value);
            }
        });
        f_Result = newData;
        return newData.Count > 0;
    }
    //                                ------------------------------------------------
    //                                --------------------Catalogue 山体相关
    //                                ------------------------------------------------
    [Button("创建山体")] // 创建山体
    public async void CreateAltsData()
    {
        if (TryGetChunkByType(EWorldObjectType.None, out var dataList))
        {
            foreach (var data in dataList)
            {
                var range = Random.Range(0.0f, 100.0f);
                if (!(range < 30))
                {
                    continue;
                }

                var height = 1 + range / 30.0f * 2;
                var worldObject = new WorldObjectBaseObjectDataAlt(data.Key, data.Key, height);
            }
            await CreateAltsAsync();
        }
    }

    public async UniTask CreateAltsAsync()
    {
        if (TryGetWorldObjectByType(EWorldObjectType.Wall, out var altData))
        {
            foreach (var VARIABLE in altData)
            {
                await ILoadPrefabAsync.LoadAsync(VARIABLE.Value);
                await UniTask.Delay(100);
            }
        }
    }

    //                                ------------------------------------------------
    //                                --------------------Catalogue 路面相关
    //                                ------------------------------------------------
    // 存放所有的路径点 key: 块索引, value: 路径数据类
    private Dictionary<int, WorldBaseObjectDataRoad> m_DicRoad = new();
    // 存放主路线 key: 块索引, value: 路径数据类
    private Dictionary<int, ListStack<PathElementData>> m_DicMainRoad = new();
    [Button("创建路面")] // 创建路面测试
    public void CreateRoadDataTest(int f_Start, int f_End, int f_Width)
    {
        Dictionary<int, bool> passIndex = new();
        if (!PathManager.Ins.TryGetAStarPath(f_Start, f_End, out var aStarPath, CreateRoadCondition))
        {
            return;
        }

        CreateRoadData(aStarPath);
    }
    // 创建路面需要的条件
    public bool CreateRoadCondition(int f_Index)
    {
        if (TryGetChunkData(f_Index, out var chunkData))
        {
            return chunkData.CurObjectType == EWorldObjectType.None;
        }
        else
        {
            return false;
        }
    }
    // 创建路面数据
    public async void CreateRoadData(ListStack<PathElementData> f_PathData)
    {
        if (f_PathData.Count <= 0) return;
        foreach (var item in f_PathData.GetEnumerator())
        {
            var pathData = item.Value;
            if (TryGetChunkData(pathData.ChunkIndex, out var chunkData) && chunkData.CurObjectType == EWorldObjectType.None)
            {
                var roadData = new WorldBaseObjectDataRoad(pathData.Index, pathData.ChunkIndex);
                roadData.ApplyCurrentChunk();
                m_DicRoad.TryAdd(pathData.ChunkIndex, roadData);
            }
        }

        await CreateRoadAsync();
    }
    // 创建路面实体
    public async UniTask CreateRoadAsync()
    {
        if (TryGetWorldObjectByType(EWorldObjectType.Road, out var roadData))
        {
            foreach (var VARIABLE in roadData)
            {
                await ILoadPrefabAsync.LoadAsync(VARIABLE.Value);

                await UniTask.Delay(100);
            }
        }
    }

    // 扩展路面
    public async void CreateRoadExtend()
    {
        if (!TryGetWorldObjectByType(EWorldObjectType.Road, out var roadData)) return;

        Dictionary<int, int> addChunk = new();
        foreach (var VARIABLE in roadData)
        {
            if (!TryGetRangeChunkByIndex(VARIABLE.Value.Index, out var list, CreateRoadCondition)) continue;

            while (list.TryPop(out var element))
            {
                addChunk.TryAdd(element, element);
            }
        }

        foreach (var VARIABLE in addChunk)
        {
            if (!TryGetChunkData(VARIABLE.Value, out var chunkData)) continue;

            var roadData2 = new WorldBaseObjectDataRoad(VARIABLE.Key, VARIABLE.Value);
            roadData2.ApplyCurrentChunk();
        }

        await CreateRoadAsync();
    }
    // 获得路径点数据
    public bool TryGetPathElementData(int f_ChunkIndex, out WorldBaseObjectDataRoad f_PathElemtnData)
    {
        return m_DicRoad.TryGetValue(f_ChunkIndex, out f_PathElemtnData);
    }
    // 设置当前主路线
    public void SaveCurrentToMainRoad()
    {
        if (!TryGetChunkByType(EWorldObjectType.Road, out var dataList, true))
            return;

    }
    // 判断是否为主路
    public bool IsMainRoad(int f_Index)
    {
        return m_DicMainRoad.ContainsKey(f_Index);
    }

    //                                ------------------------------------------------
    //                                --------------------Catalogue 怪物出生点相关
    //                                ------------------------------------------------
    public int m_MonsterSpawnPointCount = 2;
    private int SpawnPointIndex = 0;
    private Dictionary<int, Entity_SpawnPointMonsterData> m_DicMonsterSpawnPoint = new();
    private Dictionary<int, Entity_SpawnPointPlayerData> m_DicPlayerSpawnPoint = new();

    public int HomeIndex { get; private set; } = -1;

    public Dictionary<int, Entity_SpawnPointMonsterData> MonsterSpawnPoint => m_DicMonsterSpawnPoint;
    public Dictionary<int, Entity_SpawnPointPlayerData> PlayerSpawnPoint => m_DicPlayerSpawnPoint;
    public async void InitMonsterSpawnPointData()
    {
        List<int> leftPoint = new();
        List<int> rightPoint = new();
        Dictionary<int, bool> curPathPoint = new();
        Dictionary<int, ListStack<PathElementData>> pathList = new();
        Dictionary<int, EDirection> monsterSpawnPoint = new();
        Dictionary<int, EDirection> playerSpawnPoint = new();

        bool Condition(int f_ChunkIndex)
        {
            var condition = false;
            if (TryGetChunkData(f_ChunkIndex, out var chunkData))
            {
                condition = (chunkData.CurObjectType == EWorldObjectType.None) &&
                            !(curPathPoint.ContainsKey(f_ChunkIndex) && !(leftPoint.Contains(f_ChunkIndex) ||
                                                                          rightPoint.Contains(f_ChunkIndex)));
            }

            return condition;
        }

        // 左侧行列
        for (var i = 0; i < RowCol.x; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                var startPoint = GetIndex(new Vector2Int(i, j));
                if (!Condition(startPoint))
                {
                    continue;
                }

                leftPoint.Add(startPoint);
            }
        }

        // 右侧行列
        for (var u = 0; u < RowCol.x; u++)
        {
            for (var v = RowCol.y - 2; v < RowCol.y; v++)
            {
                var endPoint = GetIndex(new Vector2Int(u, v));
                if (!Condition(endPoint))
                {
                    continue;
                }

                rightPoint.Add(endPoint);
            }
        }

        var centreIndex = Mathf.CeilToInt((leftPoint.Count - 1) / 2);
        for (var i = 0; i < centreIndex + 1; i++)
        {
            List<ListStack<PathElementData>> pathList2 = new();
            for (var j = -1; j < 2; j += 2)
            {
                var itemIndex = centreIndex + j * i;
                if (itemIndex >= leftPoint.Count)
                {
                    continue;
                }

                var item = leftPoint[itemIndex];


                // 右侧正向遍历
                var k = 0;
                for (k = 0; k < rightPoint.Count; k++)
                {
                    var rightItem = rightPoint[k];
                    if (PathManager.Ins.TryGetAStarPath(item, rightItem, out var pathData, Condition))
                    {
                        pathList2.Add(pathData);
                        break;
                    }
                }


                if (pathList2.Count < 1) continue;
                // 右侧反向遍历
                for (var l = rightPoint.Count - 1; l > k; l--)
                {
                    var rightItem = rightPoint[l];
                    if (PathManager.Ins.TryGetAStarPath(item, rightItem, out var pathData, Condition))
                    {
                        pathList2.Add(pathData);
                        break;
                    }
                }

                if (pathList2.Count == 2)
                {
                    break;
                }
            }

            if (pathList2.Count == 2)
            {
                for (int j = 0; j < pathList2.Count; j++)
                {
                    var VARIABLE = pathList2[j];
                    m_DicMainRoad.Add(j, VARIABLE);
                    foreach (var VARIABLE2 in VARIABLE.GetEnumerator())
                    {
                        curPathPoint.TryAdd(VARIABLE2.Value.ChunkIndex, true);
                    }

                    // 敌方阵营小兵出生点
                    pathList.Add(pathList.Count, VARIABLE);
                    var dir = CompareIndexDirection(VARIABLE[0].ChunkIndex, VARIABLE[1].ChunkIndex);
                    monsterSpawnPoint.TryAdd(VARIABLE[0].ChunkIndex, dir);

                    // 玩家阵营小兵出生点
                    var homeIndex = pathList2[0][pathList2[0].Count - 1].ChunkIndex;
                    var nextHomeIndex = pathList2[0][pathList2[0].Count - 2].ChunkIndex;
                    var dir2 = CompareIndexDirection(homeIndex, nextHomeIndex);
                    playerSpawnPoint.TryAdd(homeIndex, dir2);
                }

                break;
            }
        }


        // 遍历所有通路
        // Dictionary<int, ListStack<PathElementData>> pathDic = new();
        // foreach (var leftValue in leftPoint)
        // {
        //     if (pathList.Count >= m_MonsterSpawnPointCount) break;
        //     foreach (var rightValue in rightPoint)
        //     {
        //         if (pathList.Count >= m_MonsterSpawnPointCount) break;
        //         if (!PathManager.Ins.TryGetAStarPath(rightValue, rightValue, out var result, Condition))
        //             continue;
        //
        //         foreach (var VARIABLE in result.GetEnumerator())
        //         {
        //             curPathPoint.TryAdd(VARIABLE.Value.ChunkIndex, true);
        //         }
        //
        //         pathList.Add(pathList.Count, result);
        //         var dir = CompareIndexDirection(result[^1].ChunkIndex, result[^2].ChunkIndex);
        //         monsterSpawnPoint.TryAdd(result[^1].ChunkIndex, dir);
        //     }
        // }

        // 生成路径
        foreach (var VARIABLE in pathList)
        {
            CreateRoadData(VARIABLE.Value);
        }

        // 生成玩家点
        m_DicPlayerSpawnPoint.Clear();
        foreach (var VARIABLE in playerSpawnPoint)
        {
            var key = SpawnPointIndex++;
            var spawnPointData = new Entity_SpawnPointPlayerData(key, VARIABLE.Key, VARIABLE.Value);

            m_DicPlayerSpawnPoint.Add(key, spawnPointData);
        }



        // 生成怪物点
        m_DicMonsterSpawnPoint.Clear();
        foreach (var VARIABLE in monsterSpawnPoint)
        {
            var key = SpawnPointIndex++;
            var spawnPointData = new Entity_SpawnPointMonsterData(key, VARIABLE.Key, VARIABLE.Value);

            m_DicMonsterSpawnPoint.Add(key, spawnPointData);
        }

        foreach (var VARIABLE in m_DicPlayerSpawnPoint)
        {
            await ILoadPrefabAsync.LoadAsync(VARIABLE.Value);
        }

        foreach (var VARIABLE in m_DicMonsterSpawnPoint)
        {
            await ILoadPrefabAsync.LoadAsync(VARIABLE.Value);
        }
    }

    public EDirection CompareIndexDirection(int f_Index1, int f_Index2)
    {
        // return f_Index2 > f_Index1
        //     ? (f_Index1 + 1 == f_Index2 ? EDirection.Right : EDirection.Up)
        //     : (f_Index1 - 1 == f_Index2 ? EDirection.Left : EDirection.Bottom);
        return EDirection.Left;
    }

    //                                ------------------------------------------------
    //                                --------------------Catalogue 当前鼠标触碰块相关
    //                                ------------------------------------------------
    private int m_CurMouseEnableIndex = -1;
    public void SetCurMouseEnable(int f_Index)
    {
        m_CurMouseEnableIndex = f_Index;
    }
    public int GetCurMouseEnable()
    {
        return m_CurMouseEnableIndex;
    }
    public void ClearCurMouseEnable()
    {
        m_CurMouseEnableIndex = -1;
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 防御塔 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<int, EntityTowerBaseData> m_DicTowers = new();
    public void CreateTowerLight()
    {
        var interval = 10;
        foreach (var item in m_DicMainRoad)
        {
            var count = Mathf.Max(item.Value.Count / interval - 1, 0);
            for (int i = 0; i < count; i++)
            {
                var curIndex = (i + 1) * interval;
                var pathInfo = item.Value[curIndex];
                if (TryGetChunkData(pathInfo.ChunkIndex, out var chunkData) && chunkData.CurObjectType == EWorldObjectType.Road)
                {
                    var key = m_DicTowers.Count;
                    var towerData = new Entity_Tower_Light1Data(key, chunkData.Index);
                    m_DicTowers.Add(key, towerData);
                }
            }
        }

        CreateTowerLightAsync();
    }
    public async void CreateTowerLightAsync()
    {
        foreach (var item in m_DicTowers)
        {
            await ILoadPrefabAsync.LoadAsync(item.Value);
            await UniTask.Delay(500);
        }
    }
    public void CreateTowerDark()
    {
    }
}