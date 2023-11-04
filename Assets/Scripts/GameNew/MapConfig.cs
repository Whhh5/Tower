using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// 障碍物类型
/// </summary>
[Serializable]
public enum EBarrierType
{
    /// <summary>
    /// 山体
    /// </summary>
    Massif,
}
[Serializable]
public class BarrierData
{
    public Vector2Int Index;
}
public class LevelEnergyCrystalData
{
    public int StartIndex;
    public EQualityType Quality;
}
public class LevelMonsterData
{
    public int StartIndex;
    public EHeroCardType MonsterType;
}
[CreateAssetMenu(fileName = "NewMapData", menuName = "ScriptableObject/MapData", order = 0)]
public class MapConfig : SerializedScriptableObject
{
    public Vector2Int MapWH;
    public float MapChunkLength;
    public Vector2 MapChunkInterval;
    public Dictionary<EBarrierType, List<BarrierData>> BarrierData = new();
    public Dictionary<int, LevelEnergyCrystalData> EnergyCrystalData = new();
    public Dictionary<int, List<LevelMonsterData>> MonsterData = new();

    public int WarSeatCount;
    public int WarSeatRowCount;
    public int HeroPoolCount;
    public float WarSeatLength;
    public Vector2 WarSeatInterval;

    [Space(10), Header("刷新列表花费")]
    public int LevelUpdateExpenditure;
    [Header("游戏初始金币")]
    public int LevelInitGlod;

    [Space(50), Header("刷出技能数量")]
    public int CardSkillCount;
    [Header("技能出现的概率")]
    public float CardSkillProbability;
}
