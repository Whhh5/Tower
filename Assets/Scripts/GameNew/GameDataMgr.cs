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

        if (GTools.TableMgr.TryGetAssetPath(EAssetKey.Cache_GameData, out var path))
        {
            m_GameData = await GTools.LoadAssetManager.LoadAsync<GameData>(path);
        }
    }

    public bool TryGetLevelData(EMapLevelType f_MapLevel, out LevelData f_LevelData)
    {
        return m_GameData.TryGetLevelData(f_MapLevel, out f_LevelData);
    }
    public void PassCurLevel()
    {
        PassLevel(CurMapLevel);
    }
    public void PassLevel(EMapLevelType f_MapLevel)
    {
        if (!TryGetLevelData(f_MapLevel, out var mapInfo))
        {
            return;
        }
        mapInfo.IsPass = true;
    }

    public EMapLevelType CurMapLevel { get; private set; }
    private static MapLevelInfo m_MapConfig = null;
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
    public static int LevelInitGlod => m_MapConfig.LevelInitGlod; 
    public static int CardSkillCount => m_MapConfig.CardSkillCount; 
    public static float CardSkillProbability => m_MapConfig.CardSkillProbability;
    public static Dictionary<int, LevelEnergyCrystalData> EnergyCrystalData => m_MapConfig.EnergyCrystalData;
    public static Dictionary<int, LevelWaveInfo> MonsterData => m_MapConfig.MonsterData;
    private void UpdateMapCfg(MapLevelInfo f_MapCfg)
    {
        m_MapConfig = f_MapCfg;
    }
    public async void SetMapData(EMapLevelType f_LevelType)
    {
        if (false)
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
        }
        else
        {

        }
        if (!GTools.TableMgr.TryGetLevelWaveInfo(f_LevelType, out var mapInfo))
        {
            return;
        }
        CurMapLevel = f_LevelType;
        ExitCurLevel();
        UpdateMapCfg(mapInfo);
        EnterNextLevel();

        GTools.CreateMapNew.CreateMapData();
        GTools.HeroCardPoolMgr.CreateHeroCardPoolConfig();
        GTools.PlayerMgr.SetGoldCount(LevelInitGlod);
        GTools.HeroCardPoolMgr.InitCardPoolList();
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
