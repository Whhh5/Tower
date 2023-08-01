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
public class MonsterManager : MonoSingleton<MonsterManager>
{
    private bool m_IsPlay = false;
    private int MonsterKey = int.MinValue;
    private Dictionary<int, Dictionary<int, Person_EnemyData>> m_DicMonster = new();

    private Dictionary<int, Entity_SpawnPointMonsterData> MonsterSpawnPoint => WorldMapManager.Ins.MonsterSpawnPoint;
    private Dictionary<int, Entity_SpawnPointPlayerData> PlayerSpawnPoint => WorldMapManager.Ins.PlayerSpawnPoint;
    private int TargetIndex => WorldMapManager.Ins.HomeIndex;
    // 初始化怪物生成点
    public void InitMonsterSpawnPoint()
    {

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
        WorldMapManager.Ins.AddChunkElement(f_SpwawnPointData, true);

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
                while (!WorldMapManager.Ins.TryGetRangeChunkByIndex(data.Value.SpawnPointData.CurrentIndex, out list, condition, true, 2))
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



    public async void CreateHero()
    {
        var originalSpawnPoint = PlayerSpawnPoint[0];
        var targetSpawnPoint = MonsterSpawnPoint[1];
        var hero1 = new Entity_Player_Hero1Data(0, originalSpawnPoint.CurrentIndex, targetSpawnPoint);

        var weapon = new Emitter_SwordHeightData(0, hero1); 
        hero1.SetWeapon(weapon);

        await ILoadPrefabAsync.LoadAsync(hero1);
        await ILoadPrefabAsync.LoadAsync(weapon);
    }
}