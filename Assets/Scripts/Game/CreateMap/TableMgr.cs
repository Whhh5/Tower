using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;

[Flags]
public enum EWorldObjectType : int
{
    None = 0,

    // 道路
    Road = 1 << 1,

    // 建筑物 -- 可破坏的静态物体
    Construction = 1 << 2,

    // 墙体 -- 不可破坏的静态物体
    Wall = 1 << 3,
    // 实体人
    Preson = 1 << 4,

    // 特效
    Effect = 1 << 5,

    EnumCount = 6,
}
public enum ETowerType
{
    Light,
    Dark,
}
public enum EHeroCradType
{

    Hero1,
    Hero2,
    Hero3,
    Hero4,
    EnumCount,
}
public enum EHeroQualityLevel
{
    Level1,
    Level2,
    Level3,
    Level4,
    EnumCount,
}
public enum EHeroCradStarLevel
{
    None = 0,
    Level1,
    Level2,
    Level3,
    Level4,
    EnumCount,
}
public enum EHeroFetterType
{
    // 水
    Weater,
    // 火
    Flame,
    // 电
    Electricity,

    EnumCount,
    None,
}
public enum EBuffType
{
    AddBlood, // 加血
    Poison, // 中毒
}
public enum EWeatherType
{
    // 台风
    Typhoon,
    // 火山
    Volcano,
    // 洪水
    Flood,
    EnumCount,
}
public enum EWeatherEventType
{
    // 台风
    Typhoon1,
    Typhoon2,
    Typhoon3,
    // 火山
    Volcano1,
    Volcano2,
    Volcano3,
    // 洪水
    Flood1,
    Flood2,
    Flood3,
    // 冰
    Ice1,
    Ice2,
    Ice3,
    EnumCount,
}
public enum AssetKey
{
    None,
    Alp1,
    Alp2,
    Alp3,
    Road1,
    Chunk1,
    SpawnPointMonster1,
    SpawnPointPlayer1,
    Person_Enemy,
    Entity_Monster_Default1,
    Entity_Player_Default1,
    Entity_Monster_Default2,
    Entity_Player_Default2,
    Entity_Player_Default3,
    TestTimeLine,
    WorldUIEntityHint,
    UISliderInfo,
    EmitterElement_GuidedMissile,
    EmitterElement_SwordLow,

    Emitter_SwordLow,
    Emitter_SwordHeight,
    Emitter_GuidedMissileBaseCommon,
    // 英雄
    Entity_Player_Hero1,
    Entity_Player_Hero2,
    Entity_Player_Hero3,
    Entity_Player_Hero4,


    // 特效
    Hero3SkillEffect,
    Effect_Buff_AddBlood,
    Effect_Buff_Poison,
    Entity_Effect_Tower_Light1_Attack,

    // buff图标路径
    BuffIcon_Poison,
    BuffIcon_AddBlood,

    // 增益
    Entity_Gain_Laubch1,
    Entity_Gain_Collect1,
    Effect_Gain_Volccano1,
    Effect_Gain_Volccano2,
    Effect_Gain_Volccano3,

    // 防御塔
    Entity_Tower_Light1,
    Entity_Tower_Dark1,

    // 孵化蛋
    Entity_Incubator1,
    Entity_Incubator2,
    Entity_Incubator3,
    Entity_Incubator4,
    Icon_Incubator1,
    Icon_IncubatorDebris1,
    Icon_Incubator2,
    Icon_IncubatorDebris2,
    Icon_Incubator3,
    Icon_IncubatorDebris3,
    Icon_Incubator4,
    Icon_IncubatorDebris4,

}
public class HeroCradInfo
{
    public EHeroQualityLevel QualityLevel;
    public string Name;
    public AssetKey AssetKet;
    public HeroCradLevelInfo QualityLevelInfo => TableMgr.Ins.TryGetHeroCradLevelInfo(QualityLevel, out var levelInfo) ? levelInfo : null;

}
public class HeroCradLevelInfo
{
    public string Name = "普通";
    public int MaxCount; // 最大数量
    public Color Color; // 等级颜色
}
public class HeroIncubatorInfo
{
    public string Name;
    public EHeroQualityLevel QualityLevel;
    public AssetKey IncubatorPrefab;
    public AssetKey IncubatorIcon;
    public AssetKey IncubatorDebrisIcon;
    public float IncubatorTime; // 孵化时间
    public int Expenditure; // 花费
}
public abstract class HeroFetterInfo : Base
{
    // 等级阶段buff
    public abstract int[] LevelStage { get; }
    public int CurStage { get; private set; } = -1;
    public abstract EHeroFetterType HeroFetterType { get; }
    public int CurCount { get; protected set; } = 0;
    public int MaxCount => LevelStage[^1];
    public int MinCount => 0;




    public void ChangeCount(int f_ChangeValue)
    {
        var toValue = CurCount + f_ChangeValue;
        var toStage = 0;
        for (int i = LevelStage.Length - 1; i >= 0; i++)
        {
            var item = LevelStage[i];
            if (toValue >= item)
            {
                toStage = i + 1;
                break;
            }
        }
        if (CurStage != toStage)
        {
            OnChangeStageClick(toStage);
            CurStage = toStage;
        }
        CurCount = Mathf.Clamp(toValue, MinCount, MaxCount);
    }
    /// <summary>
    /// 羁绊阶段等级改变调用
    /// </summary>
    /// <param name="f_ToStage"></param>
    public abstract void OnChangeStageClick(int f_ToStage);
}
public class PlayerLevelInfo
{
    // 获取卡牌概率
    public Dictionary<EHeroQualityLevel, float> DicCradProbability = new();

    // 获得卡牌概率范围
    public (float tMin, float tMAx) GetRangeCradProbability(EHeroQualityLevel f_CradLevel)
    {
        Vector2 result = Vector2.zero;
        var curResidue = 1.0f;
        foreach (var item in DicCradProbability)
        {
            var min = 1 - curResidue;
            var max = min + (1 - min) * item.Value;
            if (item.Key == f_CradLevel)
            {
                result = new(min, max);
                break;
            }
            curResidue = 1 - max;
        }
        return (result.x, result.y);
    }
    public bool GetQualityRandom(out Dictionary<EHeroQualityLevel, (float tMin, float tMax)> f_Result)
    {
        // 概率范围
        var curLevelPro = f_Result = new();

        foreach (var item in DicCradProbability)
        {
            var range = GetRangeCradProbability(item.Key);
            curLevelPro.Add(item.Key, range);
        }

        for (int i = 0; i < (int)EHeroQualityLevel.EnumCount; i++)
        {
            var level = (EHeroQualityLevel)i;
            if (curLevelPro.ContainsKey(level))
            {
                continue;
            }
            var range = GetRangeCradProbability(level);
            var halfRange = (range.tMAx - range.tMin) * 0.5f;
            int? lastLevel = null;
            int? latterLevel = null;
            var tempI = i;
            while (--tempI >= 0)
            {
                if (!curLevelPro.TryGetValue((EHeroQualityLevel)tempI, out var proValue))
                {
                    continue;
                }
                lastLevel = tempI;
                proValue.tMax += halfRange;
                curLevelPro[(EHeroQualityLevel)tempI] = proValue;
                break;
            }
            tempI = i;
            while (++tempI < (int)EHeroQualityLevel.EnumCount)
            {
                if (!curLevelPro.TryGetValue((EHeroQualityLevel)tempI, out var proValue))
                {
                    continue;
                }
                latterLevel = tempI;
                proValue.tMin -= halfRange;
                curLevelPro[(EHeroQualityLevel)tempI] = proValue;
                break;
            }
            if (lastLevel == null && latterLevel != null)
            {
                var proValue = curLevelPro[(EHeroQualityLevel)latterLevel];
                proValue.tMin -= halfRange;
                curLevelPro[(EHeroQualityLevel)latterLevel] = proValue;
            }
            if (latterLevel == null && lastLevel != null)
            {
                var proValue = curLevelPro[(EHeroQualityLevel)lastLevel];
                proValue.tMax += halfRange;
                curLevelPro[(EHeroQualityLevel)lastLevel] = proValue;
            }
        }
        return curLevelPro.Count > 0;
    }
}
public enum EPlayerLevel
{
    Level1,
    Level2,
    EnumCount,
}
public class HeroCardStarLevelInfo
{
    public Color Color;
}
public class WeatherInfoEventInfo
{
    public EWeatherEventType WeatherEventType;
    // 触发概率
    public float Probability;
}
public class WeatherInfo
{
    public string Name = "";
    public string Describe = "";
    public List<WeatherInfoEventInfo> EventList;
    public EWeatherType WeatherType;
    public WeatherBaseData CreateWeatherData(float f_DurationTime)
    {
        if (TableMgr.Ins.TryGetWeatherData(WeatherType, out var result))
        {
            result.WeatherInfo = this;
            result.Initialization(f_DurationTime);
            return result;
        }
        return null;
    }
}
public class WeatherEventInfo
{
    public string Name = "";
    public string Describe = "";
    public EWeatherEventType WeatherEventType;
    public WeatherEventBaseData CreateWeatherData(float f_DurationTime)
    {
        if (TableMgr.Ins.TryGetWeatherEventData(WeatherEventType, out var result))
        {
            result.WeatherEventInfo = this;
            result.Initialization(f_DurationTime);
            return result;
        }
        return null;
    }
}
public class TableMgr : Singleton<TableMgr>
{

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 类型颜色篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private readonly Dictionary<EWorldObjectType, Color> DicMapColor = new()
    {
        { EWorldObjectType.None, Color.black },
        { EWorldObjectType.Road, Color.yellow },
        { EWorldObjectType.Construction, Color.gray },
        { EWorldObjectType.Wall, Color.cyan },
        { EWorldObjectType.Preson, Color.green },
    };

    public bool TryGetColorByObjectType(EWorldObjectType f_ObjectType, out Color f_Result)
    {
        return DicMapColor.TryGetValue(f_ObjectType, out f_Result);
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 资源路径篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    static string BuffIconParentPath = "Icons/BuffIcon";
    static string IncubatorIconParentPath = "Icons/IncubatorIcon";
    private static readonly Dictionary<AssetKey, string> m_DicIDToPath = new()
    {
        { AssetKey.Alp1, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Alp2, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Alp3, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Road1, "Prefabs/WorldObject/Entity_Road1" },
        { AssetKey.Chunk1, "Prefabs/WorldObject/Entity_Chunk1" },
        { AssetKey.SpawnPointMonster1, "Prefabs/WorldObject/Entity_SpawnPointMonster1" },
        { AssetKey.SpawnPointPlayer1, "Prefabs/WorldObject/Entity_SpawnPointPlayer1" },
        { AssetKey.Entity_Monster_Default1, "Prefabs/WorldObject/Entity_Monster_Default1" },
        { AssetKey.Entity_Monster_Default2, "Prefabs/WorldObject/Entity_Monster_Default2" },
        { AssetKey.Entity_Player_Default1, "Prefabs/WorldObject/Entity_Player_Default1" },
        { AssetKey.Entity_Player_Default2, "Prefabs/WorldObject/Entity_Player_Default2" },

        { AssetKey.Entity_Player_Default3, "Prefabs/WorldObject/Entity_Player_Default3" },
        { AssetKey.Entity_Player_Hero1, "Prefabs/WorldObject/Entity_Player_Hero1" },
        { AssetKey.Entity_Player_Hero2, "Prefabs/WorldObject/Entity_Player_Hero2" },
        { AssetKey.Entity_Player_Hero3, "Prefabs/WorldObject/Entity_Player_Hero3" },
        { AssetKey.Entity_Player_Hero4, "Prefabs/WorldObject/Entity_Player_Hero4" },


        // 防御塔
        { AssetKey.Entity_Tower_Light1, "Prefabs/WorldObject/Entity_Tower_Light1" },
        { AssetKey.Entity_Tower_Dark1, "Prefabs/WorldObject/Entity_Tower_Dark1" },

        // 武器
        { AssetKey.EmitterElement_GuidedMissile, "Prefabs/WorldObject/EmitterElement_GuidedMissile" },
        { AssetKey.EmitterElement_SwordLow, "Prefabs/WorldObject/EmitterElement_SwordLow" },
        { AssetKey.Emitter_SwordLow, "Prefabs/WorldObject/Emitter_SwordLow" },
        { AssetKey.Emitter_SwordHeight, "Prefabs/WorldObject/Emitter_SwordHeight" },
        { AssetKey.Emitter_GuidedMissileBaseCommon, "Prefabs/WorldObject/Emitter_GuidedMissileBaseCommon" },
        { AssetKey.Entity_Incubator1, "Prefabs/WorldObject/Entity_Incubator1" },
        { AssetKey.Entity_Incubator2, "Prefabs/WorldObject/Entity_Incubator2" },
        { AssetKey.Entity_Incubator3, "Prefabs/WorldObject/Entity_Incubator3" },
        { AssetKey.Entity_Incubator4, "Prefabs/WorldObject/Entity_Incubator4" },



        { AssetKey.TestTimeLine, "Prefabs/TimeLine/TestTimeLine" },
        { AssetKey.Hero3SkillEffect, "Prefabs/TimeLine/Hero3SkillEffect" },
        { AssetKey.WorldUIEntityHint, "Prefabs/WorldUI/WorldUIEntityHint" },
        { AssetKey.UISliderInfo, "Prefabs/WorldUI/UISliderInfo" },

        // 特效
        { AssetKey.Effect_Buff_Poison, "Prefabs/Effects/Effect_Buff_Poison" },
        { AssetKey.Entity_Effect_Tower_Light1_Attack, "Prefabs/Effects/Entity_Effect_Tower_Light1_Attack" },
        { AssetKey.Effect_Buff_AddBlood, "Prefabs/Effects/Effect_Buff_AddBlood" },
        { AssetKey.Entity_Gain_Laubch1, "Prefabs/Effects/Entity_Gain_Laubch1" },
        { AssetKey.Entity_Gain_Collect1, "Prefabs/Effects/Entity_Gain_Collect1" },
        { AssetKey.Effect_Gain_Volccano1, "Prefabs/Effects/Effect_Gain_Volccano1" },
        { AssetKey.Effect_Gain_Volccano2, "Prefabs/Effects/Effect_Gain_Volccano2" },
        { AssetKey.Effect_Gain_Volccano3, "Prefabs/Effects/Effect_Gain_Volccano3" },

        // icon 
        { AssetKey.BuffIcon_Poison, $"{BuffIconParentPath}/Poison" },
        { AssetKey.BuffIcon_AddBlood, $"{BuffIconParentPath}/AddBlood" },
        { AssetKey.Icon_Incubator1, $"{IncubatorIconParentPath}/Icon_Incubator1" },
        { AssetKey.Icon_IncubatorDebris1, $"{IncubatorIconParentPath}/Icon_IncubatorDebris1" },
        { AssetKey.Icon_Incubator2, $"{IncubatorIconParentPath}/Icon_Incubator2" },
        { AssetKey.Icon_IncubatorDebris2, $"{IncubatorIconParentPath}/Icon_IncubatorDebris2" },
        { AssetKey.Icon_Incubator3, $"{IncubatorIconParentPath}/Icon_Incubator3" },
        { AssetKey.Icon_IncubatorDebris3, $"{IncubatorIconParentPath}/Icon_IncubatorDebris3" },
        { AssetKey.Icon_Incubator4, $"{IncubatorIconParentPath}/Icon_Incubator4" },
        { AssetKey.Icon_IncubatorDebris4, $"{IncubatorIconParentPath}/Icon_IncubatorDebris4" },
    };
    public bool GetAssetPath(AssetKey f_Key, out string f_Result)
    {
        return m_DicIDToPath.TryGetValue(f_Key, out f_Result);
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 英雄卡牌篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--

    private Dictionary<EHeroCradType, HeroCradInfo> m_HeroCradInfo = new()
    {
        {
            EHeroCradType.Hero1,
            new()
            {
                QualityLevel = EHeroQualityLevel.Level1,
                Name = "Hero1",
                AssetKet = AssetKey.Entity_Player_Hero1,
            }
        },
        {
            EHeroCradType.Hero2,
            new()
            {
                QualityLevel = EHeroQualityLevel.Level2,
                Name = "Hero2",
                AssetKet = AssetKey.Entity_Player_Hero2,
            }
        },
        {
            EHeroCradType.Hero3,
            new()
            {
                QualityLevel = EHeroQualityLevel.Level3,
                Name = "Hero3",
                AssetKet = AssetKey.Entity_Player_Hero3,
            }
        },
        {
            EHeroCradType.Hero4,
            new()
            {
                QualityLevel = EHeroQualityLevel.Level4,
                Name = "Hero4",
                AssetKet = AssetKey.Entity_Player_Hero4,
            }
        },
    };
    public bool TryGetHeroCradInfo(EHeroCradType f_EHeroCradType, out HeroCradInfo f_HeroCradInfo)
    {
        return m_HeroCradInfo.TryGetValue(f_EHeroCradType, out f_HeroCradInfo);
    }
    public void LoopHeroCradInfo(Action<EHeroCradType, HeroCradInfo> f_LoopCallback)
    {
        foreach (var item in m_HeroCradInfo)
        {
            f_LoopCallback.Invoke(item.Key, item.Value);
        }
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 卡牌等级对照篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EHeroQualityLevel, HeroCradLevelInfo> m_HeroCradLevelInfo = new()
    {
        {
            EHeroQualityLevel.Level1,
            new()
            {
                Name = "普通",
                MaxCount = 30,
                Color = Color.gray,
            }
        },
        {
            EHeroQualityLevel.Level2,
            new()
            {
                Name = "罕见",
                MaxCount = 20,
                Color = Color.yellow,
            }
        },
        {
            EHeroQualityLevel.Level3,
            new()
            {
                Name = "传说",
                MaxCount = 10,
                Color = Color.red,
            }
        },
        {
            EHeroQualityLevel.Level4,
            new()
            {
                Name = "史诗",
                MaxCount = 5,
                Color = Color.cyan,
            }
        },
    };
    public bool TryGetHeroCradLevelInfo(EHeroQualityLevel f_EHeroCradLevel, out HeroCradLevelInfo f_HeroCradLevelInfo)
    {
        return m_HeroCradLevelInfo.TryGetValue(f_EHeroCradLevel, out f_HeroCradLevelInfo);
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 玩家等级获取卡牌概率篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EPlayerLevel, PlayerLevelInfo> m_DicPlayLevel = new()
    {
        {
            EPlayerLevel.Level1,
            new()
            {
                DicCradProbability = new()
                {
                    {
                        EHeroQualityLevel.Level1,
                        0.5f
                    },
                    {
                        EHeroQualityLevel.Level2,
                        0.25f
                    },
                    {
                        EHeroQualityLevel.Level3,
                        0.5f
                    },
                    {
                        EHeroQualityLevel.Level4,
                        1f
                    },
                }
            }
        },
    };
    public bool TryGetPlayerLevelInfo(EPlayerLevel f_Level, out PlayerLevelInfo f_PlayerLevelInfo)
    {
        return m_DicPlayLevel.TryGetValue(f_Level, out f_PlayerLevelInfo);
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 星级
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EHeroCradStarLevel, HeroCardStarLevelInfo> m_HeroCardStarLevelInfo = new()
    {
        {
            EHeroCradStarLevel.Level1,
            new()
            {
                Color = Color.gray,
            }
        },
        {
            EHeroCradStarLevel.Level2,
            new()
            {
                Color = Color.white,
            }
        },
        {
            EHeroCradStarLevel.Level3,
            new()
            {
                Color = Color.yellow,
            }
        },
        {
            EHeroCradStarLevel.Level4,
            new()
            {
                Color = Color.cyan,
            }
        },
    };
    public bool TryGetGeroCardStarLevelInfo(EHeroCradStarLevel f_StarLevel, out HeroCardStarLevelInfo f_Info)
    {
        return m_HeroCardStarLevelInfo.TryGetValue(f_StarLevel, out f_Info);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 孵化器信息篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EHeroQualityLevel, HeroIncubatorInfo> m_IncubatorInfo = new()
    {
        {
            EHeroQualityLevel.Level1,
            new()
            {
                Name = "普通孵化器",
                QualityLevel = EHeroQualityLevel.Level1,
                IncubatorTime = 10,
                Expenditure = 2,
                IncubatorPrefab = AssetKey.Entity_Incubator1,
                IncubatorIcon = AssetKey.Icon_Incubator1,
                IncubatorDebrisIcon = AssetKey.Icon_IncubatorDebris1,
            }
        },
        {
            EHeroQualityLevel.Level2,
            new()
            {
                Name = "罕见孵化器",
                QualityLevel = EHeroQualityLevel.Level2,
                IncubatorTime = 20,
                Expenditure = 4,
                IncubatorPrefab = AssetKey.Entity_Incubator2,
                IncubatorIcon = AssetKey.Icon_Incubator2,
                IncubatorDebrisIcon = AssetKey.Icon_IncubatorDebris2,
            }
        },
        {
            EHeroQualityLevel.Level3,
            new()
            {
                Name = "传说孵化器",
                QualityLevel = EHeroQualityLevel.Level3,
                IncubatorTime = 30,
                Expenditure = 6,
                IncubatorPrefab = AssetKey.Entity_Incubator3,
                IncubatorIcon = AssetKey.Icon_Incubator3,
                IncubatorDebrisIcon = AssetKey.Icon_IncubatorDebris3,
            }
        },
        {
            EHeroQualityLevel.Level4,
            new()
            {
                Name = "史诗孵化器",
                QualityLevel = EHeroQualityLevel.Level4,
                IncubatorTime = 40,
                Expenditure = 10,
                IncubatorPrefab = AssetKey.Entity_Incubator4,
                IncubatorIcon = AssetKey.Icon_Incubator4,
                IncubatorDebrisIcon = AssetKey.Icon_IncubatorDebris4,
            }
        },
    };
public bool TryGetIncubatorInfo(EHeroQualityLevel f_QualityLevle, out HeroIncubatorInfo f_IncubatorInfo)
{
    return m_IncubatorInfo.TryGetValue(f_QualityLevle, out f_IncubatorInfo);
}

//--
//===============================----------------------========================================
//-----------------------------                          --------------------------------------
//                                catalogue -- 羁绊
//-----------------------------                          --------------------------------------
//===============================----------------------========================================
//--

public Dictionary<EHeroFetterType, HeroFetterInfo> m_HeroFetterType = new();

//--
//===============================----------------------========================================
//-----------------------------                          --------------------------------------
//                                catalogue -- 英雄实例篇
//-----------------------------                          --------------------------------------
//===============================----------------------========================================
//--
private Dictionary<EHeroCradType, WorldObjectBaseData> m_HeroData = new()
{

};
public bool GetHeroDataByType(EHeroCradType f_HeroType, int f_TargetIndex, EHeroCradStarLevel f_StarLevel, out WorldObjectBaseData f_Result)
{
    f_Result = null;
    switch (f_HeroType)
    {
        case EHeroCradType.Hero1:
            f_Result = new Entity_Player_Hero1Data(0, f_TargetIndex, f_StarLevel);
            break;
        case EHeroCradType.Hero2:
            f_Result = new Entity_Player_Hero2Data(0, f_TargetIndex, f_StarLevel);
            break;
        case EHeroCradType.Hero3:
            f_Result = new Entity_Player_Hero3Data(0, f_TargetIndex, f_StarLevel);
            break;
        case EHeroCradType.Hero4:
            f_Result = new Entity_Player_Hero4Data(0, f_TargetIndex, f_StarLevel);
            break;
        case EHeroCradType.EnumCount:
            break;
        default:
            break;
    }
    return f_Result != null;
}
//--
//===============================----------------------========================================
//-----------------------------                          --------------------------------------
//                                catalogue -- 随机事件篇
//-----------------------------                          --------------------------------------
//===============================----------------------========================================
//--
private Dictionary<EWeatherType, WeatherInfo> m_WeatherInfo = new()
{
    {
        EWeatherType.Typhoon,
        new()
        {
            Name = "风起云涌",
            Describe = "顺风而行",
            WeatherType = EWeatherType.Typhoon,
            EventList = new()
            {
                new()
                {
                    WeatherEventType = EWeatherEventType.Typhoon1,
                    Probability = 0.3f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Typhoon2,
                    Probability = 0.5f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Typhoon3,
                    Probability = 1f,
                },
            },
        }
    },
    {
        EWeatherType.Volcano,
        new()
        {
            Name = "烈火燎原",
            Describe = "燃烧大地上的生物",
            WeatherType = EWeatherType.Volcano,
            EventList = new()
            {
                new()
                {
                    WeatherEventType = EWeatherEventType.Volcano1,
                    Probability = 0.3f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Volcano2,
                    Probability = 0.5f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Volcano3,
                    Probability = 1f,
                },
            },
        }
    },
    {
        EWeatherType.Flood,
        new()
        {
            Name = "",
            WeatherType = EWeatherType.Flood,
            EventList = new()
            {
                new()
                {
                    WeatherEventType = EWeatherEventType.Flood1,
                    Probability = 0.3f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Flood2,
                    Probability = 0.5f,
                },
                new()
                {
                    WeatherEventType = EWeatherEventType.Flood3,
                    Probability = 1f,
                },
            },
        }
    },
};
public bool TryGetWeatherInfo(EWeatherType f_Type, out WeatherInfo f_Result)
{
    return m_WeatherInfo.TryGetValue(f_Type, out f_Result);
}

public bool TryGetWeatherData(EWeatherType f_Type, out WeatherBaseData f_Result)
{
    f_Result = null;
    switch (f_Type)
    {
        case EWeatherType.Typhoon:
            f_Result = new Weather_TyphoonData();
            break;
        case EWeatherType.Volcano:
            f_Result = new Weather_VolcanoData();
            break;
        case EWeatherType.Flood:
            break;
        default:
            break;
    }
    return f_Result != null;
}
private Dictionary<EWeatherEventType, WeatherEventInfo> m_WeatherEventInfo = new()
{
    {
        EWeatherEventType.Typhoon1,
        new()
        {
            Name = "小风",
            Describe = "微风",
            WeatherEventType = EWeatherEventType.Typhoon1,
        }
    },
    {
        EWeatherEventType.Typhoon2,
        new()
        {
            Name = "中风",
            Describe = "是风",
            WeatherEventType = EWeatherEventType.Typhoon2,
        }
    },
    {
        EWeatherEventType.Typhoon3,
        new()
        {
            Name = "大风",
            Describe = "狂风",
            WeatherEventType = EWeatherEventType.Typhoon3,
        }
    },
    {
        EWeatherEventType.Volcano1,
        new()
        {
            Name = "小火",
            Describe = "微热",
            WeatherEventType = EWeatherEventType.Volcano1,
        }
    },
    {
        EWeatherEventType.Volcano2,
        new()
        {
            Name = "中火",
            Describe = "狂热",
            WeatherEventType = EWeatherEventType.Volcano2,
        }
    },
    {
        EWeatherEventType.Volcano3,
        new()
        {
            Name = "大火",
            Describe = "燃烧",
            WeatherEventType = EWeatherEventType.Volcano3,
        }
    },
};
public bool TryGetWeatherEventInfo(EWeatherEventType f_Type, out WeatherEventInfo f_Result)
{
    return m_WeatherEventInfo.TryGetValue(f_Type, out f_Result);
}
public bool TryGetWeatherEventData(EWeatherEventType f_Type, out WeatherEventBaseData f_Result)
{
    f_Result = null;
    switch (f_Type)
    {
        case EWeatherEventType.Typhoon1:
            f_Result = new WeatherEvent_Typhoon1Data();
            break;
        case EWeatherEventType.Typhoon2:
            f_Result = new WeatherEvent_Typhoon2Data();
            break;
        case EWeatherEventType.Typhoon3:
            f_Result = new WeatherEvent_Typhoon3Data();
            break;
        case EWeatherEventType.Volcano1:
            f_Result = new WeatherEvent_Volcano1Data();
            break;
        case EWeatherEventType.Volcano2:
            f_Result = new WeatherEvent_Volcano2Data();
            break;
        case EWeatherEventType.Volcano3:
            f_Result = new WeatherEvent_Volcano3Data();
            break;
        case EWeatherEventType.Flood1:
            break;
        case EWeatherEventType.Flood2:
            break;
        case EWeatherEventType.Flood3:
            break;
        default:
            break;
    }
    return f_Result != null;
}
//--
//===============================----------------------========================================
//-----------------------------                          --------------------------------------
//                                catalogue -- Buff 篇
//-----------------------------                          --------------------------------------
//===============================----------------------========================================
//--
public class BuffInfo
{
    public string Name = "";
    public string Desc = "";
    public AssetKey IconPath;
    public EBuffType BuffType;
    public Effect_BuffBaseData CreateBuffData(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
    {
        if (TableMgr.Ins.TryGetBuffData(BuffType, f_Initiator, f_Target, out var result))
        {
            result.Initialization(f_Initiator, f_Target);
        }
        return result;
    }
}
private Dictionary<EBuffType, BuffInfo> m_BuffInfo = new()
{
    {
        EBuffType.AddBlood,
        new BuffInfo()
        {
            BuffType = EBuffType.AddBlood,
            Desc = "持续回血",
            Name = "治疗",
            IconPath = AssetKey.BuffIcon_AddBlood
        }
    },
    {

        EBuffType.Poison,
        new BuffInfo()
        {
            BuffType = EBuffType.Poison,
            Desc = "持续减血",
            Name = "中毒",
            IconPath = AssetKey.BuffIcon_Poison
        }
    },
};
public bool TryGetBuffInfo(EBuffType f_BuffType, out BuffInfo f_Result)
{
    return m_BuffInfo.TryGetValue(f_BuffType, out f_Result);
}
public bool TryGetBuffData(EBuffType f_BuffType, WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target, out Effect_BuffBaseData f_Result)
{
    f_Result = null;
    switch (f_BuffType)
    {
        case EBuffType.AddBlood:
            f_Result = IBuffUtil.CreateBuffData<Effect_Buff_AddBloodData>(f_Initiator, f_Target);
            break;
        case EBuffType.Poison:
            f_Result = IBuffUtil.CreateBuffData<Effect_Buff_PoisonData>(f_Initiator, f_Target);
            break;
        default:
            break;
    }
    return f_Result != null;
}






//--
//===============================----------------------========================================
//-----------------------------                          --------------------------------------
//                                catalogue -- 增益 篇
//-----------------------------                          --------------------------------------
//===============================----------------------========================================
//--
public class GainInfo
{
    public EGainView GainView;
    public EGainType GainType;
    public EntityGainBaseData CreateGain(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
    {
        if (Ins.TryGetGainData(GainType, out var result))
        {
            result.Initialization(f_Initiator, f_Target);
            return result;
        }
        return null;
    }
}
private Dictionary<EGainType, GainInfo> m_GainInfo = new()
{
    {
        EGainType.Launch1,
        new()
        {
            GainType = EGainType.Launch1,
            GainView = EGainView.Launch,
        }
    },
    {
        EGainType.Volccano1,
        new()
        {
            GainType = EGainType.Volccano1,
            GainView = EGainView.Interval,
        }
    },
    {
        EGainType.Volccano2,
        new()
        {
            GainType = EGainType.Volccano2,
            GainView = EGainView.Interval,
        }
    },
    {
        EGainType.Volccano3,
        new()
        {
            GainType = EGainType.Volccano3,
            GainView = EGainView.Interval,
        }
    },
};
public bool TryGetGainInfo(EGainType f_GainType, out GainInfo f_GainInfo)
{
    return m_GainInfo.TryGetValue(f_GainType, out f_GainInfo);
}
public bool TryGetGainData(EGainType f_GainType, out EntityGainBaseData f_Result)
{
    f_Result = null;
    switch (f_GainType)
    {
        case EGainType.None:
            break;
        case EGainType.Launch1:
            f_Result = new Entity_Gain_Laubch1Data();
            break;
        case EGainType.Collect1:
            f_Result = new Entity_Gain_Collect1Data();
            break;
        case EGainType.Volccano1:
            f_Result = new Effect_Gain_Volccano1Data();
            break;
        case EGainType.Volccano2:
            f_Result = new Effect_Gain_Volccano2Data();
            break;
        case EGainType.Volccano3:
            f_Result = new Effect_Gain_Volccano3Data();
            break;
        case EGainType.EnumCount:
            break;
        default:
            break;
    }
    return f_Result != null;
}

}







public class HeroCradFetterInfo
{
    public EHeroFetterType Type;
    public int Count;
    public HeroFetterInfo CradInfo;
}

public class HeroFetterInfo_Electricity : HeroFetterInfo
{
    public override EHeroFetterType HeroFetterType => EHeroFetterType.Electricity;
    public override int[] LevelStage => new int[3] { 2, 4, 6 };

    public override void OnChangeStageClick(int f_ToStage)
    {
        Log($"HeroFetterInfo_Electricity fetter stage change {CurStage} => {f_ToStage}");
    }
}
public class HeroFetterMgr : Singleton<HeroFetterMgr>
{
    private Dictionary<EHeroFetterType, HeroFetterInfo> m_FetterInfo = new()
    {
        {
            EHeroFetterType.Electricity,
            new HeroFetterInfo_Electricity()
        },
    };

    // 当前玩家羁绊
    private Dictionary<EHeroFetterType, HeroCradFetterInfo> m_HeroFetter = new();

    public void Initialization()
    {
    }

    private void AddHeroFetter(EHeroFetterType f_FetterType)
    {
        if (m_FetterInfo.TryGetValue(f_FetterType, out var value))
        {
            value.ChangeCount(1);
        }
    }
}