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
    void Awake();
    void Start();
    void Destroy();
}
public class GameManager
{
    public static List<IInitialization> m_StaticSingleton = new()
    {
        GTools.MathfMgr,
        GTools.EventSystemMgr,
        GTools.GlobalEventMgr,
        //GTools.WeatherMgr,
        //GTools.WorldMapMgr,
        //GTools.TerrainMgr,
        //GTools.MonsterMgr,
        //GTools.CameraMgr,
        GTools.HeroIncubatorPoolMgr,
        GTools.PlayerMgr,
        GTools.UIMgr,
        GTools.WorldWindowMgr,
        GTools.GameDataMgr,
        GTools.CreateMapNew,
        GTools.HeroCardPoolMgr,
        GTools.AttackMgr,
    };


    public static EGameStatus CurGameStatus = EGameStatus.Begin;
    [RuntimeInitializeOnLoadMethod]
    public static async void StartGame()
    {
        Application.targetFrameRate = GTools.MaxFrameRate;

        // Cursor.lockState = CursorLockMode.Confined;

        CurGameStatus = EGameStatus.Playing;

        Time.timeScale = 1;


        await AssetsMgr.Ins.LoadPrefabAsync<GameMgr>(EAssetName.GameManager, null);



        foreach (var item in m_StaticSingleton)
        {
            item.Awake();
        }


        foreach (var item in m_StaticSingleton)
        {
            item.Start();
        }
    }

    public static async void ReturnSelectWindow()
    {
        //var cts = new CancellationTokenSource();
        //CancellationToken ctk = cts.Token;
        //cts.Cancel();
        ILoadPrefabAsync.UnLoadAll();
        GTools.CreateMapNew.ClearMapAssets();
        GTools.UIWindowManager.CloseAllWindow();
        GTools.HeroCardPoolMgr.ClearHeroCardPoolConfig();
        await GTools.UIWindowManager.LoadWindowAsync<UIGameLevel>(EAssetName.UIGameLevel);
    }

}

