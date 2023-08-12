using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class GameManager
{
    [RuntimeInitializeOnLoadMethod]
    public static async void StartGame()
    {
        Application.targetFrameRate = GTools.MaxFrameRate;

        await InitializationManager();


        // ≥ı ºªØ
        GTools.MathfMgr.Initialization();
        HeroCardPoolMgr.Ins.Init();


        await AssetsMgr.Ins.LoadPrefabAsync<GameMgr>(EAssetName.GameManager, null);


        WorldMapManager.Ins.CreateChunkTest();
        WorldMapManager.Ins.InitMonsterSpawnPointData();
        WorldMapManager.Ins.CreateRoadExtend();
        MonsterManager.Ins.CreateEntityTest();
    }




    public static async UniTask InitializationManager()
    {

    }
}

