using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameTest : MonoBehaviour
{
    public int TargetChunkIndex = 0;
    [Button]
    public void CreateEnemy(int f_TargetIndex)
    {
        if (!GTools.CreateMapNew.TryGetChunkData(f_TargetIndex, out var chunkData))
        {
            return;
        }
        if (!chunkData.IsPass())
        {
            return;
        }
        var monsterData = new Entity_Monster_Warrior1Data();
        monsterData.InitData(-1);
        monsterData.MoveToChunk(f_TargetIndex);
        monsterData.SetPosition(chunkData.WorldPosition);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(monsterData));
        WorldWindowMgr.Ins.UpdateBloodHint(monsterData);
    }
    [Button, PropertySpace(50)]
    public void ClearEnemy(Entity_MonsterBaseNew f_EnemyEntity)
    {
        if (f_EnemyEntity == null)
        {
            return;
        }
        var data = f_EnemyEntity.MonsterBaseData;
        if (data == null)
        {
            return;
        }
        WorldWindowMgr.Ins.RemoveBloodHint(data);
        ILoadPrefabAsync.UnLoad(data);
        GTools.CreateMapNew.ClearChunkElement(data);
    }
    [Button, PropertySpace(50)]
    public void MonterEnterEnergyCrystal(Entity_EnergyCrystal1 f_EnemyEntity)
    {
        if (f_EnemyEntity == null)
        {
            return;
        }
        f_EnemyEntity.Data.MonsterEnter();
    }
}
