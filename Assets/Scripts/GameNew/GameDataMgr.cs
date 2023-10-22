using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GameDataMgr : Singleton<GameDataMgr>
{
    private GameData m_GameData = null;
    public override async void Awake()
    {
        base.Awake();

        if (GTools.TableMgr.TryGetAssetPath(AssetKey.Cache_GameData, out var path))
        {
            m_GameData = await GTools.LoadAssetManager.LoadAsync<GameData>(path);
        }
    }

    public bool TryGetLevelData(EMapLevelType f_MapLevel, out LevelData f_LevelData)
    {
        return m_GameData.TryGetLevelData(f_MapLevel, out f_LevelData);
    }

    private static MapConfig m_MapConfig = null;
    public static Vector2Int MapWH => m_MapConfig.MapWH;
    public static Dictionary<EBarrierType, List<BarrierData>> BarrierData => m_MapConfig.BarrierData;
    public static float MapChunkLength => m_MapConfig.MapChunkLength;
    public static Vector2 MapChunkInterval => m_MapConfig.MapChunkInterval;
    public static int WarSeatCount => m_MapConfig.WarSeatCount;
    public static int WarSeatRowCount => m_MapConfig.WarSeatRowCount;
    public static int HeroPoolCount => m_MapConfig.HeroPoolCount;
    public static float WarSeatLength => m_MapConfig.WarSeatLength;
    public static Vector2 WarSeatInterval => m_MapConfig.WarSeatInterval;
    public static int LevelUpdateExpenditure => m_MapConfig.LevelUpdateExpenditure;
    public int LevelInitGlod => m_MapConfig.LevelInitGlod;
    private void UpdateMapCfg(MapConfig f_MapCfg)
    {
        m_MapConfig = f_MapCfg;
    }
    public async void SetMapData(EMapLevelType f_LevelType)
    {
        if (!GTools.TableMgr.TryGetMapConfigInfo(f_LevelType, out var levelInfo))
        {
            LogError("加载场景数据失败，找不到相应配置");
            return;
        }

        if (!GTools.TableMgr.TryGetAssetPath(levelInfo.AssetPath, out var assetPath))
        {
            LogError("加载场景数据失败，未找到配置路径");
            return;
        }

        var asset = await LoadAssetManager.Ins.LoadAsync<MapConfig>(assetPath);
        if (asset == null)
        {
            LogError($"加载场景数据失败，加载资源失败 {assetPath}");
            return;
        }
        ExitCurLevel();
        UpdateMapCfg(asset);
        EnterNextLevel();

        GTools.CreateMapNew.CreateMap();
        GTools.HeroCardPoolMgr.CreateHeroCardPoolConfig();
    }
    private void EnterNextLevel()
    {

    }
    private void ExitCurLevel()
    {
        if (m_MapConfig == null)
        {
            return;
        }
        m_MapConfig = null;
    }

}
