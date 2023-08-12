using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Sirenix.OdinInspector;

public class CreateTestManager : MonoBase
{
    public Vector3 m_EnemyPosition;
    [Button]
    public async void CreateEnemy()
    {
        await GTools.AssetsMgr.LoadPrefabPoolAsync<Person_Enemy>(EAssetName.Person_Enemy_Crab, null, m_EnemyPosition);
    }

    [Button]
    public async void CreatePlayer()
    {
        await GTools.AssetsMgr.LoadPrefabPoolAsync<Person_Player>(EAssetName.Person_Player, null, m_EnemyPosition);
    }
}
