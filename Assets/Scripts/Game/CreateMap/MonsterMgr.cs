using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;
using Cysharp.Threading.Tasks;

public class CommandLevelQueue
{
    private Dictionary<int, ListQueue<Action>> m_Queue = new();

    public void Initialization()
    {
        m_Queue.Clear();
    }
    public void Add(int f_Level, Action f_Func)
    {
        if (!m_Queue.TryGetValue(f_Level, out var list))
        {
            list = new("command queue", 30);
            m_Queue.Add(f_Level, list);
        }
        list.Push(f_Func);
    }
    public void Invok()
    {
        foreach (var item in m_Queue)
        {
            while (item.Value.TryPop(out var func))
            {
                func?.Invoke();
            }
        }
    }
}
public class MonsterMgr : Singleton<MonsterMgr>
{
    private bool m_IsPlay = false;
    private int MonsterKey = int.MinValue;
    private Dictionary<int, Dictionary<int, Person_EnemyData>> m_DicMonster = new();

    private Dictionary<int, Entity_SpawnPointMonsterData> MonsterSpawnPoint => WorldMapMgr.Ins.MonsterSpawnPoint;
    private Dictionary<int, Entity_SpawnPointPlayerData> PlayerSpawnPoint => WorldMapMgr.Ins.PlayerSpawnPoint;
    private int TargetIndex => WorldMapMgr.Ins.HomeIndex;

    public GodEntityData GodEntityData = new();
    // 初始化怪物生成点
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        CreateEntityTest();
    }

    CommandLevelQueue m_Queue = new();
    public async void CreateEntityTest()
    {
        ClearEntity();
        m_Queue.Initialization();
        foreach (var item in MonsterSpawnPoint)
        {
            var spawnPoint = item.Value;
            var index = item.Key;
            foreach (var playerSpawnPoint in PlayerSpawnPoint)
            {
                var targetIndex = playerSpawnPoint.Value.CurrentIndex;
                m_Queue.Add(1, () =>
                {
                    var entity = new Entity_Monster_Default1Data();
                    CreateEntity(index, entity);
                });

                m_Queue.Add(2, () =>
                {
                    var entity2 = new Entity_Monster_Default2Data();
                    CreateEntity(index, entity2);
                });

            }
        }

        foreach (var item in PlayerSpawnPoint)
        {
            var spawnPoint = item.Value;
            var index = item.Key;
            foreach (var monsterSpawnPoint in MonsterSpawnPoint)
            {
                var targetIndex = monsterSpawnPoint.Value.CurrentIndex;
                m_Queue.Add(1, () =>
                {
                    var entity = new Entity_Player_Default1Data();
                    CreateEntity(index, entity);
                });

                m_Queue.Add(2, () =>
                {
                    var entity2 = new Entity_Player_Default2Data();
                    CreateEntity(index, entity2);
                });
            }
        }
        m_Queue.Invok();
        m_IsPlay = true;
        await CreateEntityAsync();
    }
    public async void ClearEntity()
    {
        foreach (var item in m_DicMonster)
        {
            foreach (var monster in item.Value)
            {
                monster.Value.SetPersonStatus(EPersonStatusType.Idle);
            }
        }

        var tempDic = m_DicMonster;
        m_DicMonster = new();
        m_IsPlay = false;
        foreach (var item in tempDic)
        {
            foreach (var monster in item.Value)
            {
                ILoadPrefabAsync.UnLoad(monster.Value);
                await UniTask.Delay(1000);
            }
        }
    }
    public void CreateEntity(int f_SpawnPointIndex, Person_EnemyData f_SpwawnPointData)
    {
        if (!m_DicMonster.TryGetValue(f_SpawnPointIndex, out var list))
        {
            list = new();
            m_DicMonster.Add(f_SpawnPointIndex, list);
        }

        var entityKey = list.Count;

        list.Add(entityKey, f_SpwawnPointData);


    }
    public async UniTask CreateEntityAsync()
    {
        PathCondition.GetCondition(EAssetKey.Person_Enemy, out var condition);
        ListStack<int> list = null;

        await GTools.ParallelTaskAsync(m_DicMonster, async (key, value) =>
        {
            //foreach (var data in value)
            //{
            //    while (!WorldMapMgr.Ins.TryGetRangeChunkByIndex(data.Value.index, out list, condition, true, 2))
            //    {
            //        if (!m_IsPlay)
            //        {
            //            return;
            //        }
            //        await UniTask.Delay(1000);
            //    }

            //    data.Value.SetStartIndex(list[0]);
            //    await ILoadPrefabAsync.LoadAsync(data.Value);
            //    await UniTask.Delay(1000);
            //}
        });
    }



    public bool TryGetMonsterSpawnPoints(out List<Entity_SpawnPointMonsterData> f_Result)
    {
        f_Result = new();
        foreach (var item in MonsterSpawnPoint)
        {
            f_Result.Add(item.Value);
        }
        return f_Result.Count > 0;
    }
    public bool TryGetPlayerSpawnPoint(out Entity_SpawnPointPlayerData f_Result)
    {
        f_Result = PlayerSpawnPoint[0];
        return f_Result != null;
    }
    public async void CreateHero()
    {

    }






    public class CreateMonsterData
    {
        public EHeroCardType CardType;
        public int Level;
    }
    public class WaveData
    {
        public List<CreateMonsterData> MonsterData;
        public float CreateInterval;
        public float DelayTime;
    }
    List<WaveData> WaveDataList = new()
    {
        new()
        {
            MonsterData = new()
            {
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
            },
            CreateInterval = 1,
            DelayTime = 5,
        },
        new()
        {
            MonsterData = new()
            {
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default2,
                    Level = 1,
                },
            },
            CreateInterval = 2,
            DelayTime = 10,
        },
        new()
        {
            MonsterData = new()
            {
                new()
                {
                    CardType = EHeroCardType.Monster_Default1,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default2,
                    Level = 1,
                },
                new()
                {
                    CardType = EHeroCardType.Monster_Default2,
                    Level = 1,
                },
            },
            CreateInterval = 3,
            DelayTime = 15,
        },
    };
    private int m_CurWaveNum = 0;
    public float NextWaveTime { get; private set; } = 0;
    private List<int> m_RangePoints = new();
    public void InitPoints()
    {
        m_RangePoints.Clear();
        foreach (var item in MonsterSpawnPoint)
        {
            if (!GTools.WorldMapMgr.TryGetRangeChunkByIndex(item.Value.CurrentIndex, out var list, GetMonsterPointCondition, true, 1))
            {
                continue;
            }
            foreach (var index in list.GetEnumerator())
            {
                m_RangePoints.Add(index.Value);
            }
        }
    }
    public async void CreateMonsterDataNew()
    {
        m_CurWaveNum = Mathf.Min(WaveDataList.Count, m_CurWaveNum + 1);
        var curWaveIndex = m_CurWaveNum - 1;
        var curWaveData = WaveDataList[curWaveIndex];
        var nextTime = Time.time + curWaveData.DelayTime;
        NextWaveTime = nextTime;


        var residueTime = nextTime - Time.time;
        await UniTask.Delay((int)residueTime);

        var curMonsterList = curWaveData.MonsterData;
        foreach (var item in curMonsterList)
        {

        }
    }
    private bool GetMonsterPointCondition(int f_Index)
    {
        if (GTools.WorldMapMgr.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.CurObjectType == EWorldObjectType.Road)
            {
                return true;
            }
        }
        return false;
    }
}


public class GodEntityData : WorldObjectBaseData
{
    public GodEntityData() : base()
    {

    }
    public override EEntityType EntityType => EEntityType.God;

    public override string ObjectName => "上帝";

    public override ELayer LayerMask => ELayer.All;

    public override ELayer AttackLayerMask => ELayer.All;

    public override EAssetKey AssetPrefabID => EAssetKey.None;

    public override EWorldObjectType ObjectType => EWorldObjectType.EnumCount;
}