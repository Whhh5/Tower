using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CreateMapNew : Singleton<CreateMapNew>
{
    private Vector2Int MapWH => GameDataMgr.MapWH;
    private Dictionary<EBarrierType, List<BarrierData>> BarrierData => GameDataMgr.BarrierData;
    private Vector2 MapChunkSize => GameDataMgr.MapChunkSize;
    private Vector2 MapChunkInterval => GameDataMgr.MapChunkInterval;


    private Transform m_MapRoot = null;
    public override void Awake()
    {
        base.Awake();

        var obj = new GameObject("Root_MapChunk");
        m_MapRoot = obj.transform;
        m_MapRoot.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }



    public void CreateMapData()
    {
        InitWaveCache();
        // ��������
        ClearMapAssets();
        // ������ͼ������
        CreateMapAssets();
        // ���ܼ��ϰ�������
        CreateMapBarrizer();
        // ����ˮ������
        CreateEnergyCrystalAssets();
        // ������������

        CreateMonsterAssets();

        CreateMapEntityAssets();
    }
    public async void CreateMapEntityAssets()
    {
        // ������ͼ����Դ
        await CreateMapChunkEntityAsync();
        // �����ϰ���
        await CreateMapBarrizerEntityAsync();
        // ��������ˮ��
        await CreateEnergyCrystalEntityAsync();
        // ���ܼ�����
        await CreateMonsterEntityAsync();
    }
    public void ClearMapAssets()
    {
        ClearMapBg();
        ClearMonster();
        ClearEnergyCrystal();
        ClearMapBarrizer();
        ClearMapChunk();
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ��ͼ��
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<int, Entity_ChunkMapData> m_ChunkDataList = new();
    private void CreateMapAssets()
    {
        CreateMapBg();
        for (int i = 0; i < MapWH.x; i++)
        {
            for (int j = 0; j < MapWH.y; j++)
            {
                var chunkIndex = RowColToIndex(new Vector2Int(i, j));
                CreateMapChunk(i, j, chunkIndex);
            }
        }

    }

    public static GameObject bgPrefabAsset;
    public static GameObject bgPrefabPrefabPrefab;
    private void CreateMapBg(){
        string prefabPath = "Prefabs/UI/BG";
        if(bgPrefabAsset == null){
            bgPrefabAsset = Resources.Load<GameObject>(prefabPath);
        }
        bgPrefabPrefabPrefab = GameObject.Instantiate(bgPrefabAsset);

    }
    private void ClearMapBg(){
        GameObject.Destroy(bgPrefabPrefabPrefab);
    }


    private void CreateMapChunk(int f_Row, int f_Col, int f_Index)
    {
        var chunkData = new Entity_ChunkMapData();
        chunkData.SetParent(m_MapRoot);
        chunkData.InitData(f_Index);
        m_ChunkDataList.Add(f_Index, chunkData);

        // pos math
        var xInterval = MapChunkSize.x;
        var yInterval = MapChunkSize.y * 0.75f;
        var pos = new Vector3(
            xInterval * 0.5f + f_Col * (xInterval + MapChunkInterval.x),
            yInterval * 0.5f + f_Row * (yInterval + MapChunkInterval.y), 0);
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
            await UniTask.Delay(10);
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
                    ClearChunkElement(element);
                    ILoadPrefabAsync.UnLoad(element);
                }
            }
            ILoadPrefabAsync.UnLoad(item.Value);
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ��ͼ�ϰ���
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private void CreateMapBarrizer()
    {
        foreach (var item in BarrierData)
        {
            if (item.Key != EBarrierType.Massif)
            {
                continue;
            }
            foreach (var data in item.Value)
            {
                var marrizerData = new Entity_MassifData();
                var index = RowColToIndex(data.Index);
                marrizerData.InitData(index);
                marrizerData.SetParent(m_MapRoot);
            }
        }
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
            await UniTask.Delay(10);
        }
    }
    private void ClearMapBarrizer()
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
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ����ˮ��
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<int, Entity_EnergyCrystalBaseData> m_CurEnergyCrtstalLust = new();
    public int GetCurActiveEnergyCount()
    {
        var count = 0;
        foreach (var item in m_CurEnergyCrtstalLust)
        {
            count += GTools.UnityObjectIsVaild(item.Value) ? 1 : 0;
        }
        return count;
    }
    public int GetCurMaxExergyCount()
    {
        return m_CurEnergyCrtstalLust.Count;
    }
    public void CreateEnergyCrystalAssets()
    {
        m_CurEnergyCrtstalLust.Clear();
        var listDic = GameDataMgr.EnergyCrystalData;
        foreach (var item in listDic)
        {
            var index = item.Value.StartIndex;
            var quality = item.Value.Quality;
            if (!GTools.TableMgr.TryGetEnergyCrystalData(quality, out var data))
            {
                LogError("������ˮ��ʵ����Ϣ");
                continue;
            }
            if (!TryGetChunkData(index, out var chunkData))
            {
                LogError($"������ˮ���� index = {index}");
                continue;
            }
            data.InitData(index);
            data.SetPosition(chunkData.WorldPosition);
            data.SetObjBehaviorStatus(true);
            m_CurEnergyCrtstalLust.Add(item.Key, data);
        }
    }
    public async UniTask CreateEnergyCrystalEntityAsync()
    {
        if (!TryGetAllObject(EWorldObjectType.Resource, out var dataList))
        {
            return;
        }
        foreach (var item in dataList)
        {
            await ILoadPrefabAsync.LoadAsync(item);
            await UniTask.Delay(10);
        }
    }
    public void ClearEnergyCrystal()
    {
        if (!TryGetAllObject(EWorldObjectType.Resource, out var dataList))
        {
            return;
        }
        foreach (var item in dataList)
        {
            ILoadPrefabAsync.UnLoad(item);
        }
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ����
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private int CurWaveCount = -1;
    public float LastWaveTime = 0.0f;
    public Dictionary<int, LevelWaveInfo> MonsterData => GameDataMgr.MonsterData;
    private Dictionary<int, List<Entity_MonsterBaseNewData>> m_CurWaveMonsterList = new();
    public int GetCurWaveCount()
    {
        return CurWaveCount;
    }
    public int GetMaxWaveCount()
    {
        return m_CurWaveMonsterList.Count;
    }
    public float GetCurWaveTime()
    {
        if (MonsterData.TryGetValue(CurWaveCount + 1, out var waveInfo))
        {
            return waveInfo.ActiveTime;
        }
        return 0;
    }
    public bool TryGetNextWaveTime(out float f_NextWaveTime)
    {
        var curWaveTime = GetCurWaveTime();
        f_NextWaveTime = curWaveTime > 0 ? LastWaveTime + curWaveTime : 0;
        return f_NextWaveTime > 0;
    }
    public int GetCurWaveMonsterCount()
    {
        if (!m_CurWaveMonsterList.TryGetValue(GetCurWaveCount(), out var list))
        {
            return 0;
        }
        return list.Count;
    }
    public int GetCurWaveMonsterActiveCount()
    {
        if (!m_CurWaveMonsterList.TryGetValue(GetCurWaveCount(), out var list))
        {
            return 0;
        }
        var count = 0;
        foreach (var item in list)
        {
            count += GTools.UnityObjectIsVaild(item) ? 1 : 0;
        }
        return count;
    }
    public void InitWaveCache()
    {
        CurWaveCount = -1;
        LastWaveTime = Time.time;
        m_CurWaveMonsterList.Clear();
    }
    public bool TrySetWaveCount(int f_WaveCount)
    {
        if (CurWaveCount == f_WaveCount)
        {
            LogError($"��ͬ��������Ҫ�л� wave count = {CurWaveCount}");
            return false;
        }
        if (!MonsterData.ContainsKey(f_WaveCount))
        {
            LogError($"�����ڹ��ﲨ�� WaveCount = {f_WaveCount}");
            return false;
        }
        if (!TryGetNextWaveTime(out var _))
        {
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_LastWave);
        }
        else
        {
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_NextWave);
        }
        Log($"��һ������ ��ǰ���� = {f_WaveCount}");
        CurWaveCount = f_WaveCount;
        LastWaveTime = Time.time;
        return true;
    }
    public bool NextWave()
    {
        return TrySetWaveCount(CurWaveCount + 1);
    }
    public void CreateMonsterAssets()
    {
        m_CurWaveMonsterList.Clear();
        foreach (var list in MonsterData)
        {
            foreach (var item in list.Value.MonsterList)
            {
                var index = item.StartIndex;
                var monsterType = item.MonsterType;
                var monsterData = GTools.HeroCardPoolMgr.CreateMonsterEntity(monsterType, item.AttributeInfoOffset, f_TargetIndex: index);
                if (!m_CurWaveMonsterList.TryGetValue(list.Key, out var curList))
                {
                    curList = new();
                    m_CurWaveMonsterList.Add(list.Key, curList);
                }
                curList.Add(monsterData);
            }
        }
    }
    public async UniTask CreateMonsterEntityAsync()
    {
        foreach (var item in m_CurWaveMonsterList)
        {
            foreach (var data in item.Value)
            {
                await ILoadPrefabAsync.LoadAsync(data);
                await UniTask.Delay(10);
            }
        }
    }
    public void SetWaveMonsterActive(int? f_WaveCount = null, bool f_Status = true)
    {
        var waveCount = f_WaveCount ?? GetCurWaveCount();
        if (!m_CurWaveMonsterList.TryGetValue(waveCount, out var list))
        {
            return;
        }
        foreach (var item in list)
        {
            item.SetObjBehaviorStatus(f_Status);
        }
    }
    public void ClearMonster()
    {
        if (!TryGetAllObject(ELayer.Enemy, out var list))
        {
            return;
        }
        foreach (var item in list)
        {
            ILoadPrefabAsync.UnLoad(item);
        }
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ת��
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int RowColToIndex(Vector2Int f_RowCol)
    {
        if (f_RowCol.x < 0 || f_RowCol.y < 0 || f_RowCol.x >= MapWH.x || f_RowCol.y >= MapWH.y)
        {
            return -1;
        }
        return f_RowCol.x * MapWH.y + f_RowCol.y;
    }
    public Vector2Int IndexToRowCol(int f_Index)
    {
        if (f_Index < 0 || f_Index >= MapWH.x * MapWH.y)
        {
            return Vector2Int.one * -1;
        }
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
        f_Elementdata.SetCurrentChunkIndex(f_Index);

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
    public bool TryGetAllObject(ELayer f_ObjectLayer, out List<DependChunkData> f_DataList)
    {
        f_DataList = new();
        foreach (var item in m_ChunkDataList)
        {
            if (!item.Value.IsExistObj(f_ObjectLayer, out var list))
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
    // �����������һ��Χ�ڵķ���
    public bool GetDirectionByIndex(int f_Original, int f_TargetIndex, out EDirection f_Result)
    {
        f_Result = EDirection.None;
        for (int i = 0; i < (int)EDirection.EnumCount; i++)
        {
            var dir = (EDirection)(1 << i);
            if (!GetDirectionChunk(f_Original, dir, out var index))
            {
                continue;
            }
            if (f_TargetIndex != index)
            {
                continue;
            }
            f_Result = dir;
            return true;
        }
        return false;
    }
    // ���ݷ����ȡ������
    public bool GetDirectionChunk(int f_Index, EDirection f_Dir, out int f_Result)
    {
        var curRowCol = IndexToRowCol(f_Index);
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

        f_Result = RowColToIndex(curRowCol);
        return m_ChunkDataList.ContainsKey(f_Result);
    }
    // �����Χ�Ŀ�
    public bool TryGetRangeChunkByIndex(int f_Index, out List<int> f_IndexList, Func<int, bool> f_Condition = null,
        bool f_IsThis = false, int f_Extend = 1)
    {
        // ���ս���б�
        List<int> resultList = new();
        // �Ѿ����������б�
        Dictionary<int, bool> already = new();
        var loopNum = 0;

        void Loop(Dictionary<int, bool> f_In)
        {
            if (loopNum++ >= f_Extend) return;
            // ��һ��ѭ����Ҫ�������б�
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

                    resultList.Add(result);
                }
            }

            if (loopNum >= f_Extend) return;

            Loop(nextDic);
        }

        Loop(new() { { f_Index, true } });
        if (f_IsThis && (f_Condition == null || f_Condition.Invoke(f_Index)))
        {
            resultList.Add(f_Index);
        }

        f_IndexList = resultList;
        return resultList.Count > 0;
    }
    public bool TryGetRandomTargetByWorldObjectType(int f_Index, ELayer f_TargetType, out List<WorldObjectBaseData> f_Targets, int f_Random)
    {
        List<WorldObjectBaseData> tempData = new();
        f_Targets = tempData;
        if (!TryGetRangeChunkByIndex(f_Index, out var listdata, index =>
        {
            if (TryGetChunkData(index, out var chunkData))
            {
                if (chunkData.IsExistObj(f_TargetType, out var list))
                {
                    foreach (var item in list)
                    {
                        if (item is not WorldObjectBaseData objData)
                        {
                            continue;
                        }
                        if (!GTools.UnityObjectIsVaild(objData))
                        {
                            continue;
                        }
                        if (!GTools.WorldObjectIsActive(objData))
                        {
                            continue;
                        }
                        tempData.Add(objData);
                    }
                    return true;
                }
            }
            return false;
        }, false, f_Random))
        {
            return false;
        }
        return f_Targets.Count > 0;
    }

    public bool TryGetRandomNearTarget(int f_Index, ELayer f_TargetType, int f_Random, out WorldObjectBaseData f_Target)
    {
        f_Target = null;
        if (!TryGetRandomTargetByWorldObjectType(f_Index, f_TargetType, out var list, f_Random))
        {
            return false;
        }
        var dis = float.MaxValue;
        if (!TryGetChunkData(f_Index, out var chunkData))
        {
            return false;
        }
        foreach (var item in list)
        {
            var tempDis = Vector3.SqrMagnitude(item.WorldPosition - chunkData.WorldPosition);
            if (tempDis >= dis)
            {
                continue;
            }
            dis = tempDis;
            f_Target = item;
        }

        return f_Target != null;
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ������ɫ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<UnityObjectData, Dictionary<int, Color>> m_ChunkColorList = new();
    public void AddChunkColor(UnityObjectData f_Target, int f_Index, Color f_Color)
    {
        if (!m_ChunkColorList.TryGetValue(f_Target, out var list))
        {
            list = new();
            m_ChunkColorList.Add(f_Target, list);
        }
        if (list.ContainsKey(f_Index))
        {
            list[f_Index] = f_Color;
        }
        else
        {
            list.Add(f_Index, f_Color);
        }
        UpdateChunkColor(f_Index, f_Color);
    }
    public void ClearChunkColor(UnityObjectData f_Target, int f_Index)
    {
        if (!m_ChunkColorList.TryGetValue(f_Target, out var list))
        {
            return;
        }
        if (list.Remove(f_Index) && list.Count == 0)
        {
            m_ChunkColorList.Remove(f_Target);
        }
        UpdateChunkColor(f_Index, null);
    }
    private void UpdateChunkColor(int f_Index, Color? f_Color = null)
    {
        if (!TryGetChunkData(f_Index, out var chunkData))
        {
            return;
        }
        chunkData.SetChunkColor(f_Color);
    }
}