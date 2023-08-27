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
                    var entity = new Entity_Monster_Default1Data(MonsterKey++, targetIndex, spawnPoint);
                    CreateEntity(index, entity);
                });

                m_Queue.Add(2, () =>
                {
                    var entity2 = new Entity_Monster_Default2Data(MonsterKey++, targetIndex, spawnPoint);
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
                    var entity = new Entity_Player_Default1Data(MonsterKey++, targetIndex, spawnPoint);
                    CreateEntity(index, entity);
                });

                m_Queue.Add(2, () =>
                {
                    var entity2 = new Entity_Player_Default2Data(MonsterKey++, targetIndex, spawnPoint);
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
        PathCondition.GetCondition(AssetKey.Person_Enemy, out var condition);
        ListStack<int> list = null;

        await GTools.ParallelTaskAsync(m_DicMonster, async (key, value) =>
        {
            foreach (var data in value)
            {
                while (!WorldMapMgr.Ins.TryGetRangeChunkByIndex(data.Value.SpawnPointData.CurrentIndex, out list, condition, true, 2))
                {
                    if (!m_IsPlay)
                    {
                        return;
                    }
                    await UniTask.Delay(1000);
                }

                data.Value.SetStartIndex(list[0]);
                await ILoadPrefabAsync.LoadAsync(data.Value);
                await UniTask.Delay(1000);
            }
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
}


public class GodEntityData : WorldObjectBaseData
{
    public GodEntityData() : base(0, -1)
    {

    }

    public override string ObjectName => "上帝";

    public override ELayer LayerMask => ELayer.All;

    public override ELayer AttackLayerMask => ELayer.All;

    public override AssetKey AssetPrefabID => AssetKey.None;

    public override EWorldObjectType ObjectType => EWorldObjectType.EnumCount;
}