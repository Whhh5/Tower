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
public enum EHeroCradType
{

    Hero1,
    Hero2,
    Hero3,
    Hero4,
    EnemyCount,
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
public class WorldRandomEventInfo
{
    public string Describe = "";

}
public enum EWorldRandomEvent
{
    // 台风
    Typhoon,
    // 火山
    Volcano,
    // 洪水
    Flood,
}
public enum AssetKey
{
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
}
public class HeroCradInfo
{
    public EHeroQualityLevel QualityLevel;
    public string Name;
    public AssetKey AssetKet;
    public HeroCradLevelInfo QualityLevelInfo => TableMgr.Ins.TryGetHeroCradLevelInfo(QualityLevel, out var levelInfo) ? levelInfo : null;
    public bool GetWorldObjectData(EHeroCradType f_HeroType, int f_TargetIndex, out WorldObjectBaseData f_Result) => TableMgr.Ins.GetHeroDataByType(f_HeroType, f_TargetIndex, out f_Result);
}
public class HeroCradLevelInfo
{
    public int MaxCount;
    public Color Color;
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

        { AssetKey.EmitterElement_GuidedMissile, "Prefabs/WorldObject/EmitterElement_GuidedMissile" },
        { AssetKey.EmitterElement_SwordLow, "Prefabs/WorldObject/EmitterElement_SwordLow" },

        { AssetKey.Emitter_SwordLow, "Prefabs/WorldObject/Emitter_SwordLow" },
        { AssetKey.Emitter_SwordHeight, "Prefabs/WorldObject/Emitter_SwordHeight" },
        { AssetKey.Emitter_GuidedMissileBaseCommon, "Prefabs/WorldObject/Emitter_GuidedMissileBaseCommon" },



        { AssetKey.TestTimeLine, "Prefabs/TimeLine/TestTimeLine" },
        { AssetKey.Hero3SkillEffect, "Prefabs/TimeLine/Hero3SkillEffect" },
        { AssetKey.WorldUIEntityHint, "Prefabs/WorldUI/WorldUIEntityHint" },

        // 特效
        { AssetKey.Effect_Buff_Poison , "Prefabs/Effects/Effect_Buff_Poison" },
        { AssetKey.Effect_Buff_AddBlood , "Prefabs/Effects/Effect_Buff_AddBlood" },
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
                MaxCount = 30,
                Color = Color.gray,
            }
        },
        {
            EHeroQualityLevel.Level2,
            new()
            {
                MaxCount = 20,
                Color = Color.yellow,
            }
        },
        {
            EHeroQualityLevel.Level3,
            new()
            {
                MaxCount = 10,
                Color = Color.red,
            }
        },
        {
            EHeroQualityLevel.Level4,
            new()
            {
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
    public bool GetHeroDataByType(EHeroCradType f_HeroType, int f_TargetIndex, out WorldObjectBaseData f_Result)
    {
        f_Result = null;
        switch (f_HeroType)
        {
            case EHeroCradType.Hero1:
                f_Result = new Entity_Player_Hero1Data(0, f_TargetIndex, null);
                break;
            case EHeroCradType.Hero2:
                f_Result = new Entity_Player_Hero2Data(0, f_TargetIndex);
                break;
            case EHeroCradType.Hero3:
                f_Result = new Entity_Player_Hero3Data(0, f_TargetIndex);
                break;
            case EHeroCradType.Hero4:
                f_Result = new Entity_Player_Hero4Data(0, f_TargetIndex);
                break;
            case EHeroCradType.EnemyCount:
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
    private Dictionary<EWorldRandomEvent, WorldRandomEventInfo> m_WorldRandomEvent = new()
    {
        {
            EWorldRandomEvent.Typhoon,
            new()
            {
                Describe = "风起云涌",
            }
        },
        {
            EWorldRandomEvent.Volcano,
            new()
            {
                Describe = "烈火燎原",
            }
        },
        {
            EWorldRandomEvent.Flood,
            new()
            {
                Describe = "",
            }
        },
    };
    public bool GetWorldRandomEvent(EWorldRandomEvent f_Type, out WorldRandomEventInfo f_Result)
    {
        return m_WorldRandomEvent.TryGetValue(f_Type, out f_Result);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Buff 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EBuffType, BuffInfo> m_BuffInfo = new()
    {
        {
            EBuffType.AddBlood,
            new BuffInfo()
            {
                BuffType = EBuffType.AddBlood,
                Desc = "持续回血",
                Name = "治疗",
            }
        },
        {

            EBuffType.Poison,
            new BuffInfo()
            {
                BuffType = EBuffType.Poison,
                Desc = "持续减血",
                Name = "中毒",
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
    private Dictionary<EGainView, GainBaseData> m_GainView = new()
    {
        {
            EGainView.Collect1,
            new()
            {
             GainView = Collect1,
             GainType = EGainType.Collect,
             
            }
        },
    };

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
public class GoldMgr : Singleton<GoldMgr>
{
    public int CurGold { get; private set; } = 100;
}