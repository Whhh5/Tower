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
public interface IInitialization
{
    void Initialization();
}
public class GameManager
{
    public static List<IInitialization> m_StaticSingleton = new()
    {
        GTools.WeatherMgr,
        GTools.TerrainMgr,
        GTools.PlayerMgr,
        GTools.CameraMgr,
        GTools.HeroCardPoolMgr,
        GTools.PlayerMgr,
    };


    public static EGameStatus CurGameStatus = EGameStatus.Begin;
    [RuntimeInitializeOnLoadMethod]
    public static async void StartGame()
    {
        Application.targetFrameRate = GTools.MaxFrameRate;

        Cursor.lockState = CursorLockMode.Confined;

        await InitializationManager();
        CurGameStatus = EGameStatus.Playing;

        // ≥ı ºªØ
        GTools.MathfMgr.Initialization();


        await AssetsMgr.Ins.LoadPrefabAsync<GameMgr>(EAssetName.GameManager, null);


        WorldMapManager.Ins.CreateChunkTest();
        WorldMapManager.Ins.InitMonsterSpawnPointData();
        WorldMapManager.Ins.CreateRoadExtend();
        MonsterManager.Ins.CreateEntityTest();


        foreach (var item in m_StaticSingleton)
        {
            item.Initialization();
        }
        GTools.TerrainMgr.SetColorTest();
    }




    public static async UniTask InitializationManager()
    {

    }
}

