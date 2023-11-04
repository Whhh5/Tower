using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;
using System;

public class PathManager : Singleton<PathManager>
{
    private Vector2Int GetRowCol(int f_Index) => GTools.CreateMapNew.IndexToRowCol(f_Index);
    private int ChunkCount => GameDataMgr.MapWH.x * GameDataMgr.MapWH.y;

    public bool TryGetAStarPath(int f_Start, int f_End, out ListStack<PathElementData> f_Result, Func<int, bool> f_ExtraCondition = null)
    {
        var startRowCol = GetRowCol(f_Start);
        var endRowCol = GetRowCol(f_End);
        var startPrice = GetPrice(startRowCol);
        var element = new PathElementData(f_Start, null, 0, startPrice);

        ListStack<PathElementData> priorityQueue1 = new("全局优先队列", ChunkCount,
            (item1, item2) => item1.PriceCount > item2.PriceCount, ESortType.Back);
        Dictionary<int, bool> backIndexDix = new();
        PathElementData endElement = null;
        Loop(element);

        var pathStr = "";
        ListStack<PathElementData> priorityQueue = new("路径队列", endElement != null ? endElement.FormPrice : 0);
        var tempPathPoint = endElement;
        if (tempPathPoint != null)
        {
            while (tempPathPoint != null)
            {
                pathStr += $"{tempPathPoint.Index} <- ";

                priorityQueue.Push(tempPathPoint);
                tempPathPoint = tempPathPoint.LastElement;
            }

            f_Result = priorityQueue;
            Debug.Log(pathStr);
            return true;
        }

        pathStr = $"尝试获取路径失败 start = {f_Start}, end = {f_End}";
        f_Result = null;
        Debug.Log(pathStr);
        return false;


        void Loop(PathElementData f_Target)
        {
            if (endElement != null)
            {
                return;
            }

            if (f_End == f_Target.Index)
            {
                endElement = f_Target;
                return;
            }

            void AddList(EDirection f_Dir)
            {
                var isNext = GTools.CreateMapNew.GetDirectionChunk(f_Target.Index, f_Dir, out var result);
                if (!isNext || backIndexDix.ContainsKey(result) ||
                    (!(result == f_End || (f_ExtraCondition?.Invoke(result) ?? true)))) return;
                
                var rowCol = GetRowCol(result);
                var price = GetPrice(rowCol);
                var ele = new PathElementData(result, f_Target, f_Target.FormPrice + 1, price);

                backIndexDix.Add(ele.Index, true);

                priorityQueue1.Push(ele);
            }

            for (var i = 0; i < (int)EDirection.EnumCount; i++)
            {
                AddList((EDirection)(1 << i));
            }

            if (priorityQueue1.TryPop(out var ele))
            {
                Loop(ele);
            }
        }


        float GetPrice(Vector2Int f_RowCol)
        {
            var index = GTools.CreateMapNew.RowColToIndex(f_RowCol);
            var index2 = GTools.CreateMapNew.RowColToIndex(endRowCol);
            if (GTools.CreateMapNew.TryGetChunkData(index, out var pos1))
            {

            }
            if (GTools.CreateMapNew.TryGetChunkData(index, out var pos2))
            {

            }
            //var pos1 = GTools.CreateMapNew.GetChunkPoint(f_RowCol);
            //var pos2 = GTools.CreateMapNew.GetChunkPoint(endRowCol);
            return Mathf.Abs(f_RowCol.x - endRowCol.x) + Mathf.Abs(f_RowCol.y - endRowCol.y);
        }
    }
}



public static class PathCondition
{
    private static Dictionary<EAssetKey, Func<int, bool>> m_DicCondition = new()
    {
        { EAssetKey.SpawnPointMonster1, Person_Enemy },
        { EAssetKey.Person_Enemy, Person_Enemy }
    };
    public static bool GetCondition(EAssetKey f_Target, out Func<int, bool> f_Condition)
    {
        if (m_DicCondition.TryGetValue(f_Target, out var value))
        {
            f_Condition = value;
            return true;
        }
        else
        {
            f_Condition = index => true;
            return false;
        }
    }
    public static bool Person_Enemy(int f_Index)
    {
        if (WorldMapMgr.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.CurObjectType == EWorldObjectType.Road)
            {
                return true;
            }
        }
        return false;
    }
}