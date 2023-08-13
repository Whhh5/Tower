using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EGameStatus
{
    Begin,
    Playing,
    Pause,
    Finish,
}
public class GameManager
{
    public static EGameStatus CurGameStatus = EGameStatus.Begin;
    [RuntimeInitializeOnLoadMethod]
    public static async void StartGame()
    {
        Application.targetFrameRate = GTools.MaxFrameRate;

        await InitializationManager();
        CurGameStatus = EGameStatus.Playing;

        // ≥ı ºªØ
        GTools.MathfMgr.Initialization();
        HeroCardPoolMgr.Ins.Init();


        await AssetsMgr.Ins.LoadPrefabAsync<GameMgr>(EAssetName.GameManager, null);


        WorldMapManager.Ins.CreateChunkTest();
        WorldMapManager.Ins.InitMonsterSpawnPointData();
        WorldMapManager.Ins.CreateRoadExtend();
        MonsterManager.Ins.CreateEntityTest();


        WeatherMgr.Ins.Initialization();
    }




    public static async UniTask InitializationManager()
    {

    }
}

