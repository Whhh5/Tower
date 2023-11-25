using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;

public enum EAudioType
{
    // 攻击起手
    Hero_Warrior1_Attack1,
    // 击中目标
    Hero_Warrior1_Attack2,
    // 技能分散
    Hero_Warrior1_Skill1,
    // 向上缓冲
    Hero_Warrior1_Skill2,
    // 向下攻击
    Hero_Warrior1_Skill3,
    // 击中目标
    Hero_Warrior1_Skill4,

    // 攻击起手
    Hero_Enchanter1_Attack1,
    // 创建粒子
    Hero_Enchanter1_Attack2,
    // 击中敌人
    Hero_Enchanter1_Attack3,
    // 技能动画
    Hero_Enchanter1_Skill1,
    // 向下坠落
    Hero_Enchanter1_Skill2,
    // 集中敌人
    Hero_Enchanter1_Skill3,

    // 攻击起手
    Hero_Supplementary1_Attack1,
    // 击中敌人
    Hero_Supplementary1_Attack2,
    // 光圈变大
    Hero_Supplementary1_Skill1,
    // 光圈缩小
    Hero_Supplementary1_Skill2,

    // 怪物1 开始攻击
    Monster_Warrior1_Attack1,
    // 怪物1 击中敌人
    Monster_Warrior1_Attack2,
    // 怪物2 开始攻击
    Monster_Warrior2_Attack1,
    // 怪物2 攻击到敌人
    Monster_Warrior2_Attack2,
    // 怪物2 磁场击中音效
    Monster_Warrior2_Skill1,
    // 怪物2 磁场音效
    Monster_Warrior2_Skill2,
    // 怪物3 开始攻击
    Monster_Warrior3_Attack1,
    // 怪物3 攻击到敌人
    Monster_Warrior3_Attack2,
    // 怪物3 技能集中敌人
    Monster_Warrior3_Skill1,
    // 怪物3 技能起手
    Monster_Warrior3_Skill2,
    // 怪物3 技能到达
    Monster_Warrior3_Skill3,
    // 怪物3 技能返回
    Monster_Warrior3_Skill4,
    // boss 攻击开始
    Monster_Boss1_Attack1,
    // boss 集中敌人
    Monster_Boss1_Attack2,
    // boss 磁场音效
    Monster_Boss1_Skill1,
    // boss 磁场集中敌人音效
    Monster_Boss1_Skill2,
    // boss 技能击中敌人
    Monster_Boss1_Skill3,
    // boss 技能起手
    Monster_Boss1_Skill4,
    // boss 技能到达
    Monster_Boss1_Skill5,
    // boss 技能返回
    Monster_Boss1_Skill6,

    // 游戏入口界面背景音乐
    Scene_GameEntrance,
    // 游戏选择关卡界面背景音乐
    Scene_SelectLevel,
    // 游戏中背景音乐
    Scene_Background,
    // 下一波提示音效
    Scene_NextWave,
    // 最后一搏提示音效
    Scene_LastWave,
    // 购买卡牌音效
    Scene_BuyCard,
    // 卡牌备战席换位音效
    Scene_ChangeWarSeat,
    // 卡牌放置到场景音效
    Scene_Place,
    // 鼠标进入备战席声音
    Scene_EnterWarSeat,
    // 鼠标退出备战席声音
    Scene_ExitWarSeat,
    // 鼠标进入格子声音
    Scene_EnterChunk,
    // 鼠标退出格子声音
    Scene_ExitChunk,
    // 游戏开始音效
    Scene_GameStart,
    EnumCount,
}
public enum EDGWorldID
{
    HeroMove,
    HeroAttack,
    HeroSkill,

    EnergyCrystalEffect,

    MonsterMove,
    MonsterAttack,

    EffectMove,

    Boss1EffectAlpha,

    AttackEffect1Data,

    WorldObjDoMovePosition,

    UIGameHelp,

    Entity_Effect_GoldAddData,
}
public enum EHeroVocationalType
{
    None,
    // 战士
    Warrior,
    // 法师
    Enchanter,
    // 辅助
    Supplementary,
    // 坦克
    MainTank,
}
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

    Resource = 1 << 6,

    EnumCount = 7,
}
public enum EEntityType
{
    // 人
    Person = 1 << 0,
    // 龙
    Dragon = 1 << 1,
    Incubator = 1 << 2,
    Item = 1 << 3,
    EnergyCrystal = 1 << 4,
    God = 1 << 20,
    Max = int.MaxValue,
}
public enum ETowerType
{
    Light,
    Dark,
}
public enum EHeroCardType
{

    Hero1,
    Hero2,
    Hero3,
    Hero4,

    Monster_Default1,
    Monster_Default2,
    Monster_Default3,
    Monster_Boss1,



    EnumCount,
}
public enum EMonsterCardType
{

    Monster_1,
    Monster_2,
    Monster_3,
}
public enum EQualityType
{
    None,
    Quality1,
    Quality2,
    Quality3,
    Quality4,
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
    WeatherSpeed,
    WeatherMaxBlood,
    WeatherAttack,
    WeatherDefense,
}
public enum ECardType
{
    Skill,
    HeroPerson,
    Incubator,
}
public enum ELockStatus
{
    Lock,
    UnLock,
}
public enum EWeatherGainType
{
    Default1,
    Default2,
    Default3,
    Default4,
    Default5,
    Default6,
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
public enum EAssetKey
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
    Entity_Monster_Default3,
    Entity_Monster_Boss1,
    Entity_Player_Default2,
    Entity_Player_Default3,
    TestTimeLine,
    WorldUIEntityHint,
    WorldUISliderInfo,
    UISkillInfo,
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

    // UI Item
    UIItem_WeatherGain,
    UIItem_CurWeatherGain,
    UIItem_Skill,
    UIItem_Equipment,

    // 实体
    Entity_Marrizer,
    Entity_ChunkMap,
    Entity_ChunkWarSeat,
    Entity_EnergyCrystal1,
    Entity_EnergyCrystal1_Effect1,
    Entity_Supplementary1SkillEffect,
    Entity_Formation_Near,
    Entity_Formation_Near_Effect,
    Entity_Formation_Sphere,
    Entity_Formation_Sphere_Effect,
    Entity_Enchanter1SkillItem,
    Entity_Monster_Boss1SkillItem,
    Entity_Monster_Warrior3SkillItem,
    Entity_Effect_AddBlood,
    Entity_GameBackground,
    Entity_Effect_GoldAdd,

    Entity_Hero_Warrior1,
    Entity_Enchanter1,
    Entity_Supplementary1,
    Entity_MainTank1,
    Entity_Monster_Warrior1,
    Entity_Monster_Warrior2,
    Entity_Monster_Warrior3,

    Entity_Default_SkillElement,
    Entity_Hero_Warrior1_SkillElement,
    Entity_Hero_EnchanterEffect,
    Entity_Hero_EnchanterAttackEffect,

    // 特效
    Hero3SkillEffect,
    Effect_Buff_AddBlood,
    Effect_Buff_Poison,
    Effect_Buff_WeatherGainSpeed,
    Effect_Buff_WeatherBuffAttack,
    Effect_Buff_WeatherBuffMaxBlood,
    Effect_Buff_WeatherBuffDefence,
    Entity_Effect_Tower_Light1_Attack,
    Entity_Effect_Attack_Default1,
    Entity_Monster_Boss1Effect,
    Entity_AttackEffect1,
    Entity_Background,
    // buff图标路径
    BuffIcon_Poison,
    BuffIcon_AddBlood,

    // 增益
    Entity_Gain_Laubch1,
    Entity_Gain_Collect1,
    Effect_Gain_Volccano1,
    Effect_Gain_Volccano2,
    Effect_Gain_Volccano3,
    Entity_Gain_AddAttackSpeed1,
    Entity_Gain_AddAttackHarm1,
    Entity_Gain_AddDeffense1,
    Entity_Gain_AddAttackRange1,
    Entity_Effect_HeroToWarSeat,

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
    // 天气轮回增益
    Icon_WeatherGainLevel1Default1,
    Icon_WeatherGainLevel2Default1,
    Icon_WeatherGainLevel3Default1,
    Icon_WeatherGainLevel4Default1,
    Icon_WeatherGainLevelMaxDefault1,
    // 技能体表 英雄1
    Icon_Skill_Hero1_Stage1_Default1,
    Icon_Skill_Hero1_Stage1_Default2,
    Icon_Skill_Hero1_Stage1_Default3,
    Icon_Skill_Hero1_Stage1_Default4,
    Icon_Skill_Hero1_Stage2_Default1_Loss1,
    Icon_Skill_Hero1_Stage2_Default1_Loss2,
    Icon_Skill_Hero1_Stage2_Default1_Loss3,
    Icon_Skill_Hero1_Stage2_Default2_Loss1,
    Icon_Skill_Hero1_Stage2_Default2_Loss2,
    Icon_Skill_Hero1_Stage2_Default3_Loss1,
    Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1,
    Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2,
    Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3,
    Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1,
    Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2,
    Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1,
    Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1,
    Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2,
    Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1,
    Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1,

    Icon_Skill_Hero2_Stage1_Default1,
    Icon_Skill_Hero2_Stage1_Default2,
    Icon_Skill_Hero2_Stage1_Default3,
    Icon_Skill_Hero2_Stage1_Default4,
    Icon_Skill_Hero2_Stage2_Default1_Loss1,
    Icon_Skill_Hero2_Stage2_Default1_Loss2,
    Icon_Skill_Hero2_Stage2_Default1_Loss3,
    Icon_Skill_Hero2_Stage2_Default2_Loss1,
    Icon_Skill_Hero2_Stage2_Default2_Loss2,
    Icon_Skill_Hero2_Stage2_Default3_Loss1,
    Icon_Skill_Hero2_Stage3_Default1_Loss1_Height1,
    Icon_Skill_Hero2_Stage3_Default1_Loss1_Height2,
    Icon_Skill_Hero2_Stage3_Default1_Loss1_Height3,
    Icon_Skill_Hero2_Stage3_Default1_Loss2_Height1,
    Icon_Skill_Hero2_Stage3_Default1_Loss2_Height2,
    Icon_Skill_Hero2_Stage3_Default1_Loss3_Height1,
    Icon_Skill_Hero2_Stage3_Default2_Loss1_Height1,
    Icon_Skill_Hero2_Stage3_Default2_Loss1_Height2,
    Icon_Skill_Hero2_Stage3_Default2_Loss2_Height1,
    Icon_Skill_Hero2_Stage3_Default3_Loss1_Height1,

    Icon_Skill_Hero3_Stage1_Default1,
    Icon_Skill_Hero3_Stage1_Default2,
    Icon_Skill_Hero3_Stage1_Default3,
    Icon_Skill_Hero3_Stage1_Default4,
    Icon_Skill_Hero3_Stage2_Default1_Loss1,
    Icon_Skill_Hero3_Stage2_Default1_Loss2,
    Icon_Skill_Hero3_Stage2_Default1_Loss3,
    Icon_Skill_Hero3_Stage2_Default2_Loss1,
    Icon_Skill_Hero3_Stage2_Default2_Loss2,
    Icon_Skill_Hero3_Stage2_Default3_Loss1,
    Icon_Skill_Hero3_Stage3_Default1_Loss1_Height1,
    Icon_Skill_Hero3_Stage3_Default1_Loss1_Height2,
    Icon_Skill_Hero3_Stage3_Default1_Loss1_Height3,
    Icon_Skill_Hero3_Stage3_Default1_Loss2_Height1,
    Icon_Skill_Hero3_Stage3_Default1_Loss2_Height2,
    Icon_Skill_Hero3_Stage3_Default1_Loss3_Height1,
    Icon_Skill_Hero3_Stage3_Default2_Loss1_Height1,
    Icon_Skill_Hero3_Stage3_Default2_Loss1_Height2,
    Icon_Skill_Hero3_Stage3_Default2_Loss2_Height1,
    Icon_Skill_Hero3_Stage3_Default3_Loss1_Height1,

    Icon_Skill_Hero4_Stage1_Default1,
    Icon_Skill_Hero4_Stage1_Default2,
    Icon_Skill_Hero4_Stage1_Default3,
    Icon_Skill_Hero4_Stage1_Default4,
    Icon_Skill_Hero4_Stage2_Default1_Loss1,
    Icon_Skill_Hero4_Stage2_Default1_Loss2,
    Icon_Skill_Hero4_Stage2_Default1_Loss3,
    Icon_Skill_Hero4_Stage2_Default2_Loss1,
    Icon_Skill_Hero4_Stage2_Default2_Loss2,
    Icon_Skill_Hero4_Stage2_Default3_Loss1,
    Icon_Skill_Hero4_Stage3_Default1_Loss1_Height1,
    Icon_Skill_Hero4_Stage3_Default1_Loss1_Height2,
    Icon_Skill_Hero4_Stage3_Default1_Loss1_Height3,
    Icon_Skill_Hero4_Stage3_Default1_Loss2_Height1,
    Icon_Skill_Hero4_Stage3_Default1_Loss2_Height2,
    Icon_Skill_Hero4_Stage3_Default1_Loss3_Height1,
    Icon_Skill_Hero4_Stage3_Default2_Loss1_Height1,
    Icon_Skill_Hero4_Stage3_Default2_Loss1_Height2,
    Icon_Skill_Hero4_Stage3_Default2_Loss2_Height1,
    Icon_Skill_Hero4_Stage3_Default3_Loss1_Height1,
    // 装备图标
    Icon_Equipment_Default1,
    Icon_Equipment_Default2,
    Icon_Equipment_Default3,
    Icon_Equipment_Default4,
    // 职业图标
    Icon_Warrior,
    Icon_Enchanter,
    Icon_Supplementary,
    Icon_MainTank,
    // 天气增益建筑物
    Entity_WeatherGainView,

    // 技能 skill
    SkillClip_,


    // 角色图标
    Icon_Hero1,
    Icon_Hero2,
    Icon_Hero3,
    Icon_Hero4,
    Icon_Monster1,
    Icon_Monster2,
    Icon_Monster3,
    Icon_Boss1,

    // 地图配置文件
    Cfg_MapLevel1,
    Cfg_MapLevel2,
    Cfg_MapLevel3,
    Cache_GameData,
    // 阵型配置文件
    Cfg_Formation_Near,
    Cfg_Formation_Sphere,

    // 音效部分
    // 英雄音效
    Audio_Hero_Warrior1_Attack1,
    Audio_Hero_Warrior1_Attack2,
    Audio_Hero_Warrior1_Skill1,
    Audio_Hero_Warrior1_Skill2,
    Audio_Hero_Warrior1_Skill3,
    Audio_Hero_Warrior1_Skill4,
    Audio_Hero_Enchanter1_Attack1,
    Audio_Hero_Enchanter1_Attack2,
    Audio_Hero_Enchanter1_Attack3,
    Audio_Hero_Enchanter1_Skill1,
    Audio_Hero_Enchanter1_Skill2,
    Audio_Hero_Enchanter1_Skill3,
    Audio_Hero_Supplementary1_Attack1,
    Audio_Hero_Supplementary1_Attack2,
    Audio_Hero_Supplementary1_Skill1,
    Audio_Hero_Supplementary1_Skill2,
    Audio_Monster_Warrior1_Attack1,
    Audio_Monster_Warrior1_Attack2,
    Audio_Monster_Warrior2_Attack1,
    Audio_Monster_Warrior2_Attack2,
    Audio_Monster_Warrior2_Skill1,
    Audio_Monster_Warrior2_Skill2,
    Audio_Monster_Warrior3_Attack1,
    Audio_Monster_Warrior3_Attack2,
    Audio_Monster_Warrior3_Skill1,
    Audio_Monster_Warrior3_Skill2,
    Audio_Monster_Warrior3_Skill3,
    Audio_Monster_Warrior3_Skill4,
    Audio_Monster_Boss1_Attack1,
    Audio_Monster_Boss1_Attack2,
    Audio_Monster_Boss1_Skill1,
    Audio_Monster_Boss1_Skill2,
    Audio_Monster_Boss1_Skill3,
    Audio_Monster_Boss1_Skill4,
    Audio_Monster_Boss1_Skill5,
    Audio_Monster_Boss1_Skill6,


    // 场景音效
    Audio_Scene_Background,
    Audio_Scene_SelectLevel,
    Audio_Scene_GameEntrance,
    Audio_Scene_NextWave,
    Audio_Scene_LastWave,
    Audio_Scene_BuyCard,
    Audio_Scene_ChangeWarSeat,
    Audio_Scene_Place,
    Audio_Scene_EnterWarSeat,
    Audio_Scene_ExitWarSeat,
    Audio_Scene_EnterChunk,
    Audio_Scene_ExitChunk,
    Audio_Scene_GameStart,

    // 游戏提示
    Img_Help_Common_1,
    Img_Help_Common_2,
    Img_Help_Common_3,
    Img_Help_Common_4,
    Img_Help_Common_5,


    Img_Help_Level0_1,

    Img_Help_Level1_1,

    Img_Help_Level2_1,

    Img_Help_Level3_1,


}
public enum EAttackEffectType
{
    Default1,
    Default2,
}
public enum EWeatherGainLevel
{
    Level1 = 1,
    Level2,
    Level3,
    Level4,
    MaxLevel,
}
public enum EEquipmentType
{
    EquipDefault1,
    EquipDefault2,
    EquipDefault3,
    EquipDefault4,
}
public enum EPersonSkillType
{
    None,
    Stage1_Default1,
    Stage1_Default2,
    Stage1_Default3,
    Stage1_Default4,

    Stage2_Default1_Loss1,
    Stage2_Default1_Loss2,
    Stage2_Default1_Loss3,

    Stage2_Default2_Loss1,
    Stage2_Default2_Loss2,

    Stage2_Default3_Loss1,

    Stage3_Default1_Loss1_Height1,
    Stage3_Default1_Loss1_Height2,
    Stage3_Default1_Loss1_Height3,

    Stage3_Default1_Loss2_Height1,
    Stage3_Default1_Loss2_Height2,

    Stage3_Default1_Loss3_Height1,

    Stage3_Default2_Loss1_Height1,
    Stage3_Default2_Loss1_Height2,

    Stage3_Default2_Loss2_Height1,

    Stage3_Default3_Loss1_Height1,

}

public enum EGameHelpType
{
    None,
    Common,
    Level0,
    Level1,
    Level2,
    Level3,
    Level4,
}
public enum EMapLevelType
{
    Level0,
    Level1,
    Level2,
    Level3,
    Level4,
    EnumCount,
}
public class GameHelpInfo
{
    public int Count => InfoAssetKeyList.Count;
    public List<EAssetKey> InfoAssetKeyList;
}
public class SkillLink
{
    public EPersonSkillType SkillType;
    public List<SkillLink> NextStageSkills;
}
public class PersonSkillInfos
{
    public int Count;
    public List<SkillLink> SkillLink;
}
public class HeroCradInfo
{
    public EQualityType QualityLevel;
    public EHeroVocationalType Vocational;
    public string Name;
    public EAssetKey AssetKet;
    public EAssetKey Icon;
    public PersonSkillInfos SkillLinkInfos;
    public HeroCradLevelInfo QualityLevelInfo => TableMgr.Ins.TryGetHeroCradLevelInfo(QualityLevel, out var levelInfo) ? levelInfo : null;

}
public class HeroVocationalInfo
{
    public EHeroVocationalType VocationalType;
    public EAssetKey IconID;
    public string Name;
}
public class HeroCradLevelInfo : IExpenditure
{
    public string Name = "普通";
    public int MaxCount; // 最大数量
    public Color Color; // 等级颜色

    public int ExpenditureBase;
    public int Expenditure => ExpenditureBase;

}
public class HeroIncubatorInfo : IExpenditure
{
    public string Name;
    public EQualityType QualityLevel;
    public EAssetKey IncubatorPrefab;
    public EAssetKey IncubatorIcon;
    public EAssetKey IncubatorDebrisIcon;
    public float IncubatorTime; // 孵化时间
    public int ExpenditureBase; // 花费

    public int Expenditure => ExpenditureBase;
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
public class QualityInfo
{
    public EQualityType QualityType;
    public Color Color;
}
public class PlayerLevelInfo
{
    // 获取卡牌概率
    public Dictionary<EQualityType, float> DicCradProbability = new();

    // 获得卡牌概率范围
    public (float tMin, float tMAx) GetRangeCradProbability(EQualityType f_CradLevel)
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
    public bool GetQualityRandom(out Dictionary<EQualityType, (float tMin, float tMax)> f_Result)
    {
        // 概率范围
        var curLevelPro = f_Result = new();

        foreach (var item in DicCradProbability)
        {
            var range = GetRangeCradProbability(item.Key);
            curLevelPro.Add(item.Key, range);
        }

        for (int i = 0; i < (int)EQualityType.EnumCount; i++)
        {
            var level = (EQualityType)i;
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
                if (!curLevelPro.TryGetValue((EQualityType)tempI, out var proValue))
                {
                    continue;
                }
                lastLevel = tempI;
                proValue.tMax += halfRange;
                curLevelPro[(EQualityType)tempI] = proValue;
                break;
            }
            tempI = i;
            while (++tempI < (int)EQualityType.EnumCount)
            {
                if (!curLevelPro.TryGetValue((EQualityType)tempI, out var proValue))
                {
                    continue;
                }
                latterLevel = tempI;
                proValue.tMin -= halfRange;
                curLevelPro[(EQualityType)tempI] = proValue;
                break;
            }
            if (lastLevel == null && latterLevel != null)
            {
                var proValue = curLevelPro[(EQualityType)latterLevel];
                proValue.tMin -= halfRange;
                curLevelPro[(EQualityType)latterLevel] = proValue;
            }
            if (latterLevel == null && lastLevel != null)
            {
                var proValue = curLevelPro[(EQualityType)lastLevel];
                proValue.tMax += halfRange;
                curLevelPro[(EQualityType)lastLevel] = proValue;
            }
        }
        return curLevelPro.Count > 0;
    }
}
public class EnergyCrystalInfo
{
    public EQualityType Quality;
    public string Name;
    public int Blood;
    public int BlastValue;
    public int BlastRange;
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
public class WeatherGainInfo
{
    public string Name;
    public EWeatherGainType WeatherGainType;
    public string Describe;
    public Dictionary<EWeatherGainLevel, EAssetKey> IconDic;
    public bool TryGetWeatherGainData(out WeatherGainData f_Result, EWeatherGainLevel f_Level = EWeatherGainLevel.Level1)
    {
        return GTools.TableMgr.TryGetWeatherGainData(WeatherGainType, out f_Result, f_Level);
    }
    public bool TryGetWeatherGainIcon(out EAssetKey f_Result, EWeatherGainLevel f_Level = EWeatherGainLevel.Level1)
    {
        return IconDic.TryGetValue(f_Level, out f_Result);
    }
}
public class WeatherGainLevelInfo
{
    public EWeatherGainLevel Level;
    public Color Color;
}
public abstract class WeatherGainData
{
    public abstract EWeatherGainType WeatherGainType { get; }
    public EWeatherGainLevel Level { get; private set; }
    public void Initialization(EWeatherGainLevel f_Level)
    {
        Level = f_Level;
    }
    public abstract void StartExecute(WorldObjectBaseData f_WorldObj);
    public WeatherGainLevelInfo WeatherLevelInfo => GTools.TableMgr.TryGetWeatherGainLevelInfo(Level, out var result) ? result : null;
}
public class EquipmentInfo
{
    public EEquipmentType EquipmentType;
    public EQualityType QualityType;
    public PersonPropertys Property;
    public string EquipmentName;
    public string Describes;
    public EAssetKey IconKey;
}
public interface IExpenditure
{
    public int Expenditure { get; } // 花费金币
}
public enum EAnimationEvent
{
    Attack,
    Audio,
    Function,
}
public class AnimationItemData
{
    public EAnimationEvent EventType;
    public int Parmas;
}
public class AudioInfo
{
    public string Name;
    public bool IsLoop;
    public EAssetKey AudioAssetKey;
}
public class PersonSkillInfo : IExpenditure
{
    public EPersonSkillType PersonSkillType;
    public string SkillName;
    public string Describes;
    public EQualityType Quality;
    public int ExpenditureBase; // 花费金币
    public int Expenditure => ExpenditureBase; // 花费金币
    public EAssetKey IconKey;

    // 技能剪辑
    //public AssetKey AnimaClip => PersonSkillType;
    // 技能事件数据
    public Dictionary<float, AnimationItemData> AnimaEventDataAttack;

    // 技能
    public Color GetColor()
    {
        if (GTools.TableMgr.TryGetQualityInfo(Quality, out var qualityInfo))
        {
            return qualityInfo.Color;
        }
        return Color.white;
    }
}
public struct IncubatorAttributeInfo
{
    public float BloodRatio;
    public float HarmRatio;
    public float DefenceRatio;
    public float AtkSpeedRatio;

    public static IncubatorAttributeInfo operator +(IncubatorAttributeInfo a, IncubatorAttributeInfo b)
    {
        a.BloodRatio += b.BloodRatio;
        a.HarmRatio += b.HarmRatio;
        a.DefenceRatio += b.DefenceRatio;
        a.AtkSpeedRatio += b.AtkSpeedRatio;
        return a;
    }
    public static IncubatorAttributeInfo operator *(IncubatorAttributeInfo a, float b)
    {
        a.BloodRatio *= b;
        a.HarmRatio *= b;
        a.DefenceRatio *= b;
        a.AtkSpeedRatio *= b;
        return a;
    }
    public static IncubatorAttributeInfo operator *(IncubatorAttributeInfo a, IncubatorAttributeInfo b)
    {
        a.BloodRatio *= b.BloodRatio;
        a.HarmRatio *= b.HarmRatio;
        a.DefenceRatio *= b.DefenceRatio;
        a.AtkSpeedRatio *= b.AtkSpeedRatio;
        return a;
    }
}
public class GainInfo
{
    public EBuffView GainView;
    public EGainType GainType;
    public EntityGainBaseData CreateGain(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
    {
        if (GTools.TableMgr.TryGetGainData(GainType, out var result))
        {
            result.Initialization(f_Initiator, f_Target);
            return result;
        }
        return null;
    }
}
public class BuffInfo
{
    public string Name = "";
    public string Desc = "";
    public EAssetKey IconPath;
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
public struct PersonPropertys
{
    public int Blood;
    public int MaxBlood;
    public int Attack;
    public int Defense;
    public float AtkSpeed;
    public float MoveSpeed;
}
public class WeatherInfo
{
    public string Name = "";
    public string Describe = "";
    public List<WeatherInfoEventInfo> EventList;
    public EWeatherType WeatherType;
    public IncubatorAttributeInfo IncubatorRatio;
    public List<EWeatherGainType> WeatherGainTypeList;
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

public class MapLevelInfo
{
    public Vector2Int MapWH;
    public Vector2 MapChunkSize;
    public Vector2 MapChunkInterval;
    public Dictionary<EBarrierType, List<BarrierData>> BarrierData = new();
    public Dictionary<int, LevelEnergyCrystalData> EnergyCrystalData = new();
    public Dictionary<int, LevelWaveInfo> MonsterData = new();

    public int WarSeatCount;
    public int WarSeatRowCount;
    public int HeroPoolCount;
    public float WarSeatLength;
    public Vector2 WarSeatInterval;
    public EGameHelpType GameNewHelpInfo;

    [Space(10), Header("刷新列表花费")]
    public int LevelUpdateExpenditure;
    [Header("游戏初始金币")]
    public int LevelInitGlod;

    [Space(50), Header("刷出技能数量")]
    public int CardSkillCount;
    [Header("技能出现的概率")]
    public float CardSkillProbability;
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
    static string WeatherGainIconParentPath = "Icons/WeatherGainIcon";
    static string SkillIconParentPath = "Icons/Skills";
    static string EquipmentIconParentPath = "Icons/Equipment";
    static string VoctionalIconParentPath = "Icons/Vocational";
    static string HeroIconParentPath = "Icons/HeroIcon";
    static string MapConfigParentPath = "Config/Map";
    static string FormationConfigParentPath = "Config/Formation";
    static string AudioParentPath = "Audio";
    static string GameHelpParentPath = "Icons/GameHelpInfo";
    private static readonly Dictionary<EAssetKey, string> m_DicIDToPath = new()
    {
        { EAssetKey.Alp1, "Prefabs/WorldObject/Entity_Alt1" },
        { EAssetKey.Alp2, "Prefabs/WorldObject/Entity_Alt1" },
        { EAssetKey.Alp3, "Prefabs/WorldObject/Entity_Alt1" },
        { EAssetKey.Road1, "Prefabs/WorldObject/Entity_Road1" },
        { EAssetKey.Chunk1, "Prefabs/WorldObject/Entity_Chunk1" },
        { EAssetKey.SpawnPointMonster1, "Prefabs/WorldObject/Entity_SpawnPointMonster1" },
        { EAssetKey.SpawnPointPlayer1, "Prefabs/WorldObject/Entity_SpawnPointPlayer1" },
        { EAssetKey.Entity_Monster_Default1, "Prefabs/WorldObject/Entity_Monster_Default1" },
        { EAssetKey.Entity_Monster_Default2, "Prefabs/WorldObject/Entity_Monster_Default2" },
        { EAssetKey.Entity_Monster_Default3, "Prefabs/WorldObject/Entity_Monster_Default3" },
        { EAssetKey.Entity_Monster_Boss1, "Prefabs/PerfabNew/Entity_Monster_Boss1" },
        { EAssetKey.Entity_Player_Default1, "Prefabs/PerfabNew/Entity_Player_Default1" },
        { EAssetKey.Entity_Player_Default2, "Prefabs/PerfabNew/Entity_Player_Default2" },

        { EAssetKey.Entity_Player_Default3, "Prefabs/PerfabNew/Entity_Player_Default3" },
        { EAssetKey.Entity_Player_Hero1, "Prefabs/WorldObject/Entity_Player_Hero1" },
        { EAssetKey.Entity_Player_Hero2, "Prefabs/WorldObject/Entity_Player_Hero2" },
        { EAssetKey.Entity_Player_Hero3, "Prefabs/WorldObject/Entity_Player_Hero3" },
        { EAssetKey.Entity_Player_Hero4, "Prefabs/WorldObject/Entity_Player_Hero4" },

        // UI Item
        { EAssetKey.UIItem_WeatherGain, "Prefabs/UI/Item/UIItem_WeatherGain" },
        { EAssetKey.UIItem_CurWeatherGain, "Prefabs/UI/Item/UIItem_CurWeatherGain" },
        { EAssetKey.UIItem_Skill, "Prefabs/UI/Item/UIItem_Skill" },
        { EAssetKey.UIItem_Equipment, "Prefabs/UI/Item/UIItem_Equipment" },

        // 防御塔
        { EAssetKey.Entity_Tower_Light1, "Prefabs/WorldObject/Entity_Tower_Light1" },
        { EAssetKey.Entity_Tower_Dark1, "Prefabs/WorldObject/Entity_Tower_Dark1" },

        // 武器
        { EAssetKey.EmitterElement_GuidedMissile, "Prefabs/WorldObject/EmitterElement_GuidedMissile" },
        { EAssetKey.EmitterElement_SwordLow, "Prefabs/WorldObject/EmitterElement_SwordLow" },
        { EAssetKey.Emitter_SwordLow, "Prefabs/WorldObject/Emitter_SwordLow" },
        { EAssetKey.Emitter_SwordHeight, "Prefabs/WorldObject/Emitter_SwordHeight" },
        { EAssetKey.Emitter_GuidedMissileBaseCommon, "Prefabs/WorldObject/Emitter_GuidedMissileBaseCommon" },
        { EAssetKey.Entity_Incubator1, "Prefabs/WorldObject/Entity_Incubator1" },
        { EAssetKey.Entity_Incubator2, "Prefabs/WorldObject/Entity_Incubator2" },
        { EAssetKey.Entity_Incubator3, "Prefabs/WorldObject/Entity_Incubator3" },
        { EAssetKey.Entity_Incubator4, "Prefabs/WorldObject/Entity_Incubator4" },



        { EAssetKey.TestTimeLine, "Prefabs/TimeLine/TestTimeLine" },
        { EAssetKey.Hero3SkillEffect, "Prefabs/TimeLine/Hero3SkillEffect" },
        { EAssetKey.WorldUIEntityHint, "Prefabs/WorldUI/WorldUIEntityHint" },
        { EAssetKey.WorldUISliderInfo, "Prefabs/WorldUI/WorldUISliderInfo" },
        { EAssetKey.UISkillInfo, "Prefabs/WorldUI/UISkillInfo" },

        { EAssetKey.Entity_Marrizer, "Prefabs/PerfabNew/Entity_Massif" },
        { EAssetKey.Entity_ChunkMap, "Prefabs/PerfabNew/Entity_ChunkMap" },
        { EAssetKey.Entity_ChunkWarSeat, "Prefabs/PerfabNew/Entity_ChunkWarSeat" },
        { EAssetKey.Entity_EnergyCrystal1, "Prefabs/PerfabNew/Entity_EnergyCrystal1" },
        { EAssetKey.Entity_EnergyCrystal1_Effect1, "Prefabs/PerfabNew/Entity_EnergyCrystal1_Effect1" },
        { EAssetKey.Entity_Supplementary1SkillEffect, "Prefabs/PerfabNew/Entity_Supplementary1SkillEffect" },
        { EAssetKey.Entity_Formation_Near, "Prefabs/PerfabNew/Entity_Formation_Near" },
        { EAssetKey.Entity_Formation_Near_Effect, "Prefabs/PerfabNew/Entity_Formation_Near_Effect" },
        { EAssetKey.Entity_Formation_Sphere, "Prefabs/PerfabNew/Entity_Formation_Sphere" },
        { EAssetKey.Entity_Formation_Sphere_Effect, "Prefabs/PerfabNew/Entity_Formation_Sphere_Effect" },
        { EAssetKey.Entity_Enchanter1SkillItem, "Prefabs/PerfabNew/Entity_Enchanter1SkillItem" },
        { EAssetKey.Entity_Monster_Boss1SkillItem, "Prefabs/PerfabNew/Entity_Monster_Boss1SkillItem" },
        { EAssetKey.Entity_Monster_Warrior3SkillItem, "Prefabs/PerfabNew/Entity_Monster_Warrior3SkillItem" },
        { EAssetKey.Entity_Effect_AddBlood, "Prefabs/PerfabNew/Entity_Effect_AddBlood" },
        { EAssetKey.Entity_GameBackground, "Prefabs/PerfabNew/Entity_GameBackground" },
        { EAssetKey.Entity_Effect_GoldAdd, "Prefabs/PerfabNew/Entity_Effect_GoldAdd" },
        



        { EAssetKey.Entity_Hero_Warrior1, "Prefabs/PerfabNew/Entity_Hero_Warrior1" },
        { EAssetKey.Entity_Enchanter1, "Prefabs/PerfabNew/Entity_Enchanter1" },
        { EAssetKey.Entity_Supplementary1, "Prefabs/PerfabNew/Entity_Supplementary1" },
        { EAssetKey.Entity_MainTank1, "Prefabs/PerfabNew/Entity_MainTank1" },
        { EAssetKey.Entity_Monster_Warrior1, "Prefabs/PerfabNew/Entity_Monster_Warrior1" },
        { EAssetKey.Entity_Hero_Warrior1_SkillElement, "Prefabs/PerfabNew/Entity_Hero_Warrior1_SkillElement" },
        { EAssetKey.Entity_Monster_Warrior2, "Prefabs/PerfabNew/Entity_Monster_Warrior2" },
        { EAssetKey.Entity_Monster_Warrior3, "Prefabs/PerfabNew/Entity_Monster_Warrior3" },
        { EAssetKey.Entity_Hero_EnchanterEffect, "Prefabs/PerfabNew/Entity_Hero_EnchanterEffect" },
        { EAssetKey.Entity_Hero_EnchanterAttackEffect, "Prefabs/PerfabNew/Entity_Hero_EnchanterAttackEffect" },
        { EAssetKey.Entity_Monster_Boss1Effect, "Prefabs/PerfabNew/Entity_Monster_Boss1Effect" },
        { EAssetKey.Entity_AttackEffect1, "Prefabs/PerfabNew/Entity_AttackEffect1" },
        { EAssetKey.Entity_Background, "Prefabs/PerfabNew/Entity_Background" },
        { EAssetKey.Entity_Effect_HeroToWarSeat, "Prefabs/PerfabNew/Entity_Effect_HeroToWarSeat" },





        // 特效
        { EAssetKey.Effect_Buff_Poison, "Prefabs/Effects/Effect_Buff_Poison" },
        { EAssetKey.Effect_Buff_WeatherGainSpeed, "Prefabs/Effects/Effect_Buff_WeatherGainSpeed" },
        { EAssetKey.Effect_Buff_WeatherBuffAttack, "Prefabs/Effects/Effect_Buff_WeatherBuffAttack" },
        { EAssetKey.Effect_Buff_WeatherBuffMaxBlood, "Prefabs/Effects/Effect_Buff_WeatherBuffMaxBlood" },
        { EAssetKey.Effect_Buff_WeatherBuffDefence, "Prefabs/Effects/Effect_Buff_WeatherBuffDefense" },
        { EAssetKey.Entity_Effect_Tower_Light1_Attack, "Prefabs/Effects/Entity_Effect_Tower_Light1_Attack" },
        { EAssetKey.Entity_Effect_Attack_Default1, "Prefabs/Effects/Entity_Effect_Attack_Default1" },
        { EAssetKey.Effect_Buff_AddBlood, "Prefabs/Effects/Effect_Buff_AddBlood" },
        { EAssetKey.Entity_Gain_Laubch1, "Prefabs/Effects/Entity_Gain_Laubch1" },
        { EAssetKey.Entity_Gain_Collect1, "Prefabs/Effects/Entity_Gain_Collect1" },
        { EAssetKey.Effect_Gain_Volccano1, "Prefabs/Effects/Effect_Gain_Volccano1" },
        { EAssetKey.Effect_Gain_Volccano2, "Prefabs/Effects/Effect_Gain_Volccano2" },
        { EAssetKey.Effect_Gain_Volccano3, "Prefabs/Effects/Effect_Gain_Volccano3" },
        { EAssetKey.Entity_Gain_AddAttackSpeed1, "Prefabs/Effects/Entity_Gain_AddAttackSpeed1" },
        { EAssetKey.Entity_Gain_AddAttackHarm1, "Prefabs/Effects/Entity_Gain_AddAttackHarm1" },
        { EAssetKey.Entity_Gain_AddAttackRange1, "Prefabs/Effects/Entity_Gain_AddAttackRange1" },
        { EAssetKey.Entity_Gain_AddDeffense1, "Prefabs/Effects/Entity_Gain_AddDeffense1" },

        // icon 
        { EAssetKey.BuffIcon_Poison, $"{BuffIconParentPath}/Poison" },
        { EAssetKey.BuffIcon_AddBlood, $"{BuffIconParentPath}/AddBlood" },
        { EAssetKey.Icon_Incubator1, $"{IncubatorIconParentPath}/Icon_Incubator1" },
        { EAssetKey.Icon_IncubatorDebris1, $"{IncubatorIconParentPath}/Icon_IncubatorDebris1" },
        { EAssetKey.Icon_Incubator2, $"{IncubatorIconParentPath}/Icon_Incubator2" },
        { EAssetKey.Icon_IncubatorDebris2, $"{IncubatorIconParentPath}/Icon_IncubatorDebris2" },
        { EAssetKey.Icon_Incubator3, $"{IncubatorIconParentPath}/Icon_Incubator3" },
        { EAssetKey.Icon_IncubatorDebris3, $"{IncubatorIconParentPath}/Icon_IncubatorDebris3" },
        { EAssetKey.Icon_Incubator4, $"{IncubatorIconParentPath}/Icon_Incubator4" },
        { EAssetKey.Icon_IncubatorDebris4, $"{IncubatorIconParentPath}/Icon_IncubatorDebris4" },

        // 天气增益icon
        { EAssetKey.Icon_WeatherGainLevel1Default1, $"{WeatherGainIconParentPath}/Icon_WeatherGainLevel1Default1" },
        { EAssetKey.Icon_WeatherGainLevel2Default1, $"{WeatherGainIconParentPath}/Icon_WeatherGainLevel2Default1" },
        { EAssetKey.Icon_WeatherGainLevel3Default1, $"{WeatherGainIconParentPath}/Icon_WeatherGainLevel3Default1" },
        { EAssetKey.Icon_WeatherGainLevel4Default1, $"{WeatherGainIconParentPath}/Icon_WeatherGainLevel4Default1" },
        { EAssetKey.Icon_WeatherGainLevelMaxDefault1, $"{WeatherGainIconParentPath}/Icon_WeatherGainLevelMaxDefault1" },

        // 天气增益建筑物
        { EAssetKey.Entity_WeatherGainView, "Prefabs/WorldObject/Entity_WeatherGainView" },

        // 技能体表
        { EAssetKey.Icon_Skill_Hero1_Stage1_Default1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default1" },
        { EAssetKey.Icon_Skill_Hero1_Stage1_Default2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default2" },
        { EAssetKey.Icon_Skill_Hero1_Stage1_Default3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default3" },
        { EAssetKey.Icon_Skill_Hero1_Stage1_Default4, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default4" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss1" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss2" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss3" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default2_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss1" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default2_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss2" },
        { EAssetKey.Icon_Skill_Hero1_Stage2_Default3_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default3_Loss1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1" },

        { EAssetKey.Icon_Skill_Hero2_Stage1_Default1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default1" },
        { EAssetKey.Icon_Skill_Hero2_Stage1_Default2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default2" },
        { EAssetKey.Icon_Skill_Hero2_Stage1_Default3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default3" },
        { EAssetKey.Icon_Skill_Hero2_Stage1_Default4, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default4" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default1_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss1" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default1_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss2" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default1_Loss3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss3" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default2_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss1" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default2_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss2" },
        { EAssetKey.Icon_Skill_Hero2_Stage2_Default3_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default3_Loss1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss1_Height3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss2_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default1_Loss3_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default2_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default2_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default2_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero2_Stage3_Default3_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1" },

        { EAssetKey.Icon_Skill_Hero3_Stage1_Default1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default1" },
        { EAssetKey.Icon_Skill_Hero3_Stage1_Default2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default2" },
        { EAssetKey.Icon_Skill_Hero3_Stage1_Default3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default3" },
        { EAssetKey.Icon_Skill_Hero3_Stage1_Default4, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default4" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default1_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss1" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default1_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss2" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default1_Loss3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss3" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default2_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss1" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default2_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss2" },
        { EAssetKey.Icon_Skill_Hero3_Stage2_Default3_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default3_Loss1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss1_Height3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss2_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default1_Loss3_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default2_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default2_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default2_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero3_Stage3_Default3_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1" },

        { EAssetKey.Icon_Skill_Hero4_Stage1_Default1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default1" },
        { EAssetKey.Icon_Skill_Hero4_Stage1_Default2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default2" },
        { EAssetKey.Icon_Skill_Hero4_Stage1_Default3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default3" },
        { EAssetKey.Icon_Skill_Hero4_Stage1_Default4, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage1_Default4" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default1_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss1" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default1_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss2" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default1_Loss3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default1_Loss3" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default2_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss1" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default2_Loss2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default2_Loss2" },
        { EAssetKey.Icon_Skill_Hero4_Stage2_Default3_Loss1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage2_Default3_Loss1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss1_Height3, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss2_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default1_Loss3_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default2_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default2_Loss1_Height2, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default2_Loss2_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1" },
        { EAssetKey.Icon_Skill_Hero4_Stage3_Default3_Loss1_Height1, $"{SkillIconParentPath}/Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1" },
        // 装备图标
        { EAssetKey.Icon_Equipment_Default1, $"{EquipmentIconParentPath}/Icon_Equipment_Default1" },
        { EAssetKey.Icon_Equipment_Default2, $"{EquipmentIconParentPath}/Icon_Equipment_Default2" },
        { EAssetKey.Icon_Equipment_Default3, $"{EquipmentIconParentPath}/Icon_Equipment_Default3" },
        { EAssetKey.Icon_Equipment_Default4, $"{EquipmentIconParentPath}/Icon_Equipment_Default4" },
        // 职业图标
        { EAssetKey.Icon_Warrior, $"{VoctionalIconParentPath}/Icon_Warrior" },
        { EAssetKey.Icon_Enchanter, $"{VoctionalIconParentPath}/Icon_Enchanter" },
        { EAssetKey.Icon_Supplementary, $"{VoctionalIconParentPath}/Icon_Supplementary" },
        { EAssetKey.Icon_MainTank, $"{VoctionalIconParentPath}/Icon_MainTank" },


        // 角色图标
        { EAssetKey.Icon_Hero1, $"{HeroIconParentPath}/Icon_Hero1" },
        { EAssetKey.Icon_Hero2, $"{HeroIconParentPath}/Icon_Hero2" },
        { EAssetKey.Icon_Hero3, $"{HeroIconParentPath}/Icon_Hero3" },
        { EAssetKey.Icon_Hero4, $"{HeroIconParentPath}/Icon_Hero4" },
        { EAssetKey.Icon_Monster1, $"{HeroIconParentPath}/Icon_Monster1" },
        { EAssetKey.Icon_Monster2, $"{HeroIconParentPath}/Icon_Monster2" },
        { EAssetKey.Icon_Monster3, $"{HeroIconParentPath}/Icon_Monster3" },
        { EAssetKey.Icon_Boss1, $"{HeroIconParentPath}/Icon_Boss1" },

        // 地图配置文件
        { EAssetKey.Cfg_MapLevel1, $"{MapConfigParentPath}/Cfg_MapLevel1" },
        { EAssetKey.Cfg_MapLevel2, $"{MapConfigParentPath}/Cfg_MapLevel2" },
        { EAssetKey.Cfg_MapLevel3, $"{MapConfigParentPath}/Cfg_MapLevel3" },
        { EAssetKey.Cache_GameData, $"{MapConfigParentPath}/Cache_GameData" },
        // 阵型配置文件
        { EAssetKey.Cfg_Formation_Near, $"{FormationConfigParentPath}/Cfg_Formation_Near" },
        { EAssetKey.Cfg_Formation_Sphere, $"{FormationConfigParentPath}/Cfg_Formation_Sphere" },
        // 音效部分
        { EAssetKey.Audio_Hero_Warrior1_Attack1, $"{AudioParentPath}/Audio_Hero_Warrior1_Attack1" },
        { EAssetKey.Audio_Hero_Warrior1_Attack2, $"{AudioParentPath}/Audio_Hero_Warrior1_Attack2" },
        { EAssetKey.Audio_Hero_Warrior1_Skill1, $"{AudioParentPath}/Audio_Hero_Warrior1_Skill1" },
        { EAssetKey.Audio_Hero_Warrior1_Skill2, $"{AudioParentPath}/Audio_Hero_Warrior1_Skill2" },
        { EAssetKey.Audio_Hero_Warrior1_Skill3, $"{AudioParentPath}/Audio_Hero_Warrior1_Skill3" },
        { EAssetKey.Audio_Hero_Warrior1_Skill4, $"{AudioParentPath}/Audio_Hero_Warrior1_Skill4" },
        { EAssetKey.Audio_Hero_Enchanter1_Attack1, $"{AudioParentPath}/Audio_Hero_Enchanter1_Attack1" },
        { EAssetKey.Audio_Hero_Enchanter1_Attack2, $"{AudioParentPath}/Audio_Hero_Enchanter1_Attack2" },
        { EAssetKey.Audio_Hero_Enchanter1_Attack3, $"{AudioParentPath}/Audio_Hero_Enchanter1_Attack3" },
        { EAssetKey.Audio_Hero_Enchanter1_Skill1, $"{AudioParentPath}/Audio_Hero_Enchanter1_Skill1" },
        { EAssetKey.Audio_Hero_Enchanter1_Skill2, $"{AudioParentPath}/Audio_Hero_Enchanter1_Skill2" },
        { EAssetKey.Audio_Hero_Enchanter1_Skill3, $"{AudioParentPath}/Audio_Hero_Enchanter1_Skill3" },
        { EAssetKey.Audio_Hero_Supplementary1_Attack1, $"{AudioParentPath}/Audio_Hero_Supplementary1_Attack1" },
        { EAssetKey.Audio_Hero_Supplementary1_Attack2, $"{AudioParentPath}/Audio_Hero_Supplementary1_Attack2" },
        { EAssetKey.Audio_Hero_Supplementary1_Skill1, $"{AudioParentPath}/Audio_Hero_Supplementary1_Skill1" },
        { EAssetKey.Audio_Hero_Supplementary1_Skill2, $"{AudioParentPath}/Audio_Hero_Supplementary1_Skill2" },
        { EAssetKey.Audio_Monster_Warrior1_Attack1, $"{AudioParentPath}/Audio_Monster_Warrior1_Attack1" },
        { EAssetKey.Audio_Monster_Warrior1_Attack2, $"{AudioParentPath}/Audio_Monster_Warrior1_Attack2" },
        { EAssetKey.Audio_Monster_Warrior2_Attack1, $"{AudioParentPath}/Audio_Monster_Warrior2_Attack1" },
        { EAssetKey.Audio_Monster_Warrior2_Attack2, $"{AudioParentPath}/Audio_Monster_Warrior2_Attack2" },
        { EAssetKey.Audio_Monster_Warrior2_Skill1, $"{AudioParentPath}/Audio_Monster_Warrior2_Skill1" },
        { EAssetKey.Audio_Monster_Warrior3_Attack1, $"{AudioParentPath}/Audio_Monster_Warrior3_Attack1" },
        { EAssetKey.Audio_Monster_Warrior3_Attack2, $"{AudioParentPath}/Audio_Monster_Warrior3_Attack2" },
        { EAssetKey.Audio_Monster_Warrior3_Skill1, $"{AudioParentPath}/Audio_Monster_Warrior3_Skill1" },
        { EAssetKey.Audio_Monster_Warrior3_Skill2, $"{AudioParentPath}/Audio_Monster_Warrior3_Skill2" },
        { EAssetKey.Audio_Monster_Warrior3_Skill3, $"{AudioParentPath}/Audio_Monster_Warrior3_Skill3" },
        { EAssetKey.Audio_Monster_Warrior3_Skill4, $"{AudioParentPath}/Audio_Monster_Warrior3_Skill4" },
        { EAssetKey.Audio_Monster_Boss1_Attack1, $"{AudioParentPath}/Audio_Monster_Boss1_Attack" },
        { EAssetKey.Audio_Monster_Boss1_Attack2, $"{AudioParentPath}/Audio_Monster_Boss1_Attack" },
        { EAssetKey.Audio_Monster_Boss1_Skill1, $"{AudioParentPath}/Audio_Monster_Boss1_Skill1" },
        { EAssetKey.Audio_Monster_Boss1_Skill2, $"{AudioParentPath}/Audio_Monster_Boss1_Skill2" },
        { EAssetKey.Audio_Monster_Boss1_Skill3, $"{AudioParentPath}/Audio_Monster_Boss1_Skill3" },
        { EAssetKey.Audio_Monster_Boss1_Skill4, $"{AudioParentPath}/Audio_Monster_Boss1_Skill4" },
        { EAssetKey.Audio_Monster_Boss1_Skill5, $"{AudioParentPath}/Audio_Monster_Boss1_Skill5" },
        { EAssetKey.Audio_Monster_Boss1_Skill6, $"{AudioParentPath}/Audio_Monster_Boss1_Skill6" },
        { EAssetKey.Audio_Scene_GameEntrance, $"{AudioParentPath}/Audio_Scene_GameEntrance" },
        { EAssetKey.Audio_Scene_Background, $"{AudioParentPath}/Audio_Scene_Background" },
        { EAssetKey.Audio_Scene_SelectLevel, $"{AudioParentPath}/Audio_Scene_SelectLevel" },
        { EAssetKey.Audio_Scene_NextWave, $"{AudioParentPath}/Audio_Scene_NextWave" },
        { EAssetKey.Audio_Scene_LastWave, $"{AudioParentPath}/Audio_Scene_LastWave" },

        { EAssetKey.Audio_Scene_BuyCard, $"{AudioParentPath}/Audio_Scene_BuyCard" },
        { EAssetKey.Audio_Scene_ChangeWarSeat, $"{AudioParentPath}/Audio_Scene_ChangeWarSeat" },
        { EAssetKey.Audio_Scene_Place, $"{AudioParentPath}/Audio_Scene_Place" },
        { EAssetKey.Audio_Scene_EnterWarSeat, $"{AudioParentPath}/Audio_Scene_EnterWarSeat" },
        { EAssetKey.Audio_Scene_ExitWarSeat, $"{AudioParentPath}/Audio_Scene_ExitWarSeat" },
        { EAssetKey.Audio_Scene_EnterChunk, $"{AudioParentPath}/Audio_Scene_EnterChunk" },
        { EAssetKey.Audio_Scene_ExitChunk, $"{AudioParentPath}/Audio_Scene_ExitChunk" },
        { EAssetKey.Audio_Scene_GameStart, $"{AudioParentPath}/Audio_Scene_GameStart" },

        { EAssetKey.Img_Help_Common_1, $"{GameHelpParentPath}/Img_Help_Common_1" },
        { EAssetKey.Img_Help_Common_2, $"{GameHelpParentPath}/Img_Help_Common_2" },
        { EAssetKey.Img_Help_Common_3, $"{GameHelpParentPath}/Img_Help_Common_3" },
        { EAssetKey.Img_Help_Common_4, $"{GameHelpParentPath}/Img_Help_Common_4" },
        { EAssetKey.Img_Help_Common_5, $"{GameHelpParentPath}/Img_Help_Common_5" },
        { EAssetKey.Img_Help_Level0_1, $"{GameHelpParentPath}/Img_Help_Level0_1" },
        { EAssetKey.Img_Help_Level1_1, $"{GameHelpParentPath}/Img_Help_Level1_1" },
        { EAssetKey.Img_Help_Level2_1, $"{GameHelpParentPath}/Img_Help_Level2_1" },
        { EAssetKey.Img_Help_Level3_1, $"{GameHelpParentPath}/Img_Help_Level3_1" },

    };
    public bool TryGetAssetPath(EAssetKey f_Key, out string f_Result)
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

    private Dictionary<EHeroCardType, HeroCradInfo> m_HeroCradInfo = new()
    {
        {
            EHeroCardType.Hero1,
            new()
            {
                QualityLevel = EQualityType.Quality1,
                Vocational = EHeroVocationalType.Warrior,
                Name = "战士",
                AssetKet = EAssetKey.Entity_Player_Hero1,
                Icon = EAssetKey.Icon_Hero1,
                SkillLinkInfos = new()
                {
                    Count = 4,
                    SkillLink = new()
                    {
                        new()
                        {
                            SkillType = EPersonSkillType.Stage1_Default1,
                            NextStageSkills = new()
                            {
                                new()
                                {
                                    SkillType = EPersonSkillType.Stage2_Default1_Loss1,
                                    NextStageSkills = new()
                                    {
                                        new()
                                        {
                                            SkillType = EPersonSkillType.Stage3_Default1_Loss1_Height1,
                                        },
                                        //new()
                                        //{
                                        //    SkillType = EPersonSkillType.Stage3_Default1_Loss1_Height2,
                                        //},
                                        //new()
                                        //{
                                        //    SkillType = EPersonSkillType.Stage3_Default1_Loss1_Height3,
                                        //}
                                    }
                                },
                                //new()
                                //{
                                //    SkillType = EPersonSkillType.Stage2_Default1_Loss2,
                                //    NextStageSkills = new()
                                //    {
                                //        new()
                                //        {
                                //            SkillType = EPersonSkillType.Stage3_Default1_Loss2_Height1,
                                //        },
                                //        new()
                                //        {
                                //            SkillType = EPersonSkillType.Stage3_Default1_Loss2_Height2,
                                //        }
                                //    }
                                //},
                                //new()
                                //{
                                //    SkillType = EPersonSkillType.Stage2_Default1_Loss3,
                                //    NextStageSkills = new()
                                //    {
                                //        new()
                                //        {
                                //            SkillType = EPersonSkillType.Stage3_Default1_Loss3_Height1,
                                //        },
                                //    }
                                //},
                            }
                        },
                        //new()
                        //{
                        //    SkillType = EPersonSkillType.Stage1_Default2,
                        //    NextStageSkills = new()
                        //    {
                        //        new()
                        //        {
                        //            SkillType = EPersonSkillType.Stage2_Default2_Loss1,
                        //            NextStageSkills = new()
                        //            {
                        //                new()
                        //                {
                        //                    SkillType = EPersonSkillType.Stage3_Default2_Loss1_Height1,
                        //                },
                        //                new()
                        //                {
                        //                    SkillType = EPersonSkillType.Stage3_Default2_Loss1_Height2,
                        //                },
                        //            }
                        //        },
                        //        new()
                        //        {
                        //            SkillType = EPersonSkillType.Stage2_Default1_Loss2,
                        //            NextStageSkills = new()
                        //            {
                        //                new()
                        //                {
                        //                    SkillType = EPersonSkillType.Stage3_Default2_Loss2_Height1,
                        //                },
                        //            }
                        //        },
                        //    }
                        //},
                        //new()
                        //{
                        //    SkillType = EPersonSkillType.Stage1_Default3,
                        //    NextStageSkills = new()
                        //    {
                        //        new()
                        //        {
                        //            SkillType = EPersonSkillType.Stage2_Default3_Loss1,
                        //            NextStageSkills = new()
                        //            {
                        //                new()
                        //                {
                        //                    SkillType = EPersonSkillType.Stage3_Default3_Loss1_Height1,
                        //                },
                        //            }
                        //        },
                        //    }
                        //},
                        //new()
                        //{
                        //    SkillType = EPersonSkillType.Stage1_Default4,
                        //    NextStageSkills = new(),
                        //}
                    }
                }
            }
        },
        {
            EHeroCardType.Hero2,
            new()
            {
                QualityLevel = EQualityType.Quality2,
                Vocational = EHeroVocationalType.Enchanter,
                Name = "法师",
                AssetKet = EAssetKey.Entity_Player_Hero2,
                Icon = EAssetKey.Icon_Hero2,
                SkillLinkInfos = new()
                {
                    Count = 3,
                }
            }
        },
        {
            EHeroCardType.Hero3,
            new()
            {
                QualityLevel = EQualityType.Quality3,
                Vocational = EHeroVocationalType.Supplementary,
                Name = "牧师",
                AssetKet = EAssetKey.Entity_Player_Hero3,
                Icon = EAssetKey.Icon_Hero3,
                SkillLinkInfos = new()
                {
                    Count = 3,
                }
            }
        },
        {
            EHeroCardType.Hero4,
            new()
            {
                QualityLevel = EQualityType.Quality4,
                Vocational = EHeroVocationalType.MainTank,
                Name = "坦克",
                AssetKet = EAssetKey.Entity_Player_Hero4,
                Icon = EAssetKey.Icon_Hero4,
                SkillLinkInfos = new()
                {
                    Count = 3,
                }
            }
        },
        {
            EHeroCardType.Monster_Default1,
            new()
            {
                QualityLevel = EQualityType.None,
                Name = "monster 1",
                AssetKet = EAssetKey.Entity_Monster_Default1,
                Icon = EAssetKey.Icon_Monster1,
                SkillLinkInfos = new()
                {
                    Count = 0,
                }
            }
        },
        {
            EHeroCardType.Monster_Default2,
            new()
            {
                QualityLevel = EQualityType.None,
                Name = "monster 2",
                AssetKet = EAssetKey.Entity_Monster_Default2,
                Icon = EAssetKey.Icon_Monster2,
                SkillLinkInfos = new()
                {
                    Count = 1,
                }
            }
        },
        {
            EHeroCardType.Monster_Default3,
            new()
            {
                QualityLevel = EQualityType.None,
                Name = "monster 2",
                AssetKet = EAssetKey.Entity_Monster_Default3,
                Icon = EAssetKey.Icon_Monster3,
                SkillLinkInfos = new()
                {
                    Count = 1,
                }
            }
        },
        {
            EHeroCardType.Monster_Boss1,
            new()
            {
                QualityLevel = EQualityType.None,
                Name = "Boss 1",
                AssetKet = EAssetKey.Entity_Monster_Boss1,
                Icon = EAssetKey.Icon_Boss1,
                SkillLinkInfos = new()
                {
                    Count = 1,
                }
            }
        },
    };
    public bool TryGetHeroCradInfo(EHeroCardType f_EHeroCradType, out HeroCradInfo f_HeroCradInfo)
    {
        return m_HeroCradInfo.TryGetValue(f_EHeroCradType, out f_HeroCradInfo);
    }
    public void LoopHeroCradInfo(Action<EHeroCardType, HeroCradInfo> f_LoopCallback)
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
    private Dictionary<EQualityType, HeroCradLevelInfo> m_HeroCradLevelInfo = new()
    {
        {
            EQualityType.None,
            new()
            {
                Name = "普通",
                MaxCount = 0,
                Color = Color.gray,
                ExpenditureBase = 1,
            }
        },
        {
            EQualityType.Quality1,
            new()
            {
                Name = "普通",
                MaxCount = 50,
                Color = Color.gray,
                ExpenditureBase = 3,
            }
        },
        {
            EQualityType.Quality2,
            new()
            {
                Name = "罕见",
                MaxCount = 50,
                Color = Color.yellow,
                ExpenditureBase = 4,
            }
        },
        {
            EQualityType.Quality3,
            new()
            {
                Name = "传说",
                MaxCount = 50,
                Color = Color.red,
                ExpenditureBase = 5,
            }
        },
        {
            EQualityType.Quality4,
            new()
            {
                Name = "史诗",
                MaxCount = 50,
                Color = Color.cyan,
                ExpenditureBase = 6,
            }
        },
    };
    public bool TryGetHeroCradLevelInfo(EQualityType f_EHeroCradLevel, out HeroCradLevelInfo f_HeroCradLevelInfo)
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
                        EQualityType.Quality1,
                        0.5f
                    },
                    {
                        EQualityType.Quality2,
                        0.25f
                    },
                    {
                        EQualityType.Quality3,
                        0.5f
                    },
                    {
                        EQualityType.Quality4,
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
    private Dictionary<EQualityType, HeroIncubatorInfo> m_IncubatorInfo = new()
    {
        {
            EQualityType.Quality1,
            new()
            {
                Name = "普通孵化器",
                QualityLevel = EQualityType.Quality1,
                IncubatorTime = 10,
                ExpenditureBase = 2,
                IncubatorPrefab = EAssetKey.Entity_Incubator1,
                IncubatorIcon = EAssetKey.Icon_Incubator1,
                IncubatorDebrisIcon = EAssetKey.Icon_IncubatorDebris1,
            }
        },
        {
            EQualityType.Quality2,
            new()
            {
                Name = "罕见孵化器",
                QualityLevel = EQualityType.Quality2,
                IncubatorTime = 20,
                ExpenditureBase = 4,
                IncubatorPrefab = EAssetKey.Entity_Incubator2,
                IncubatorIcon = EAssetKey.Icon_Incubator2,
                IncubatorDebrisIcon = EAssetKey.Icon_IncubatorDebris2,
            }
        },
        {
            EQualityType.Quality3,
            new()
            {
                Name = "传说孵化器",
                QualityLevel = EQualityType.Quality3,
                IncubatorTime = 30,
                ExpenditureBase = 6,
                IncubatorPrefab = EAssetKey.Entity_Incubator3,
                IncubatorIcon = EAssetKey.Icon_Incubator3,
                IncubatorDebrisIcon = EAssetKey.Icon_IncubatorDebris3,
            }
        },
        {
            EQualityType.Quality4,
            new()
            {
                Name = "史诗孵化器",
                QualityLevel = EQualityType.Quality4,
                IncubatorTime = 40,
                ExpenditureBase = 10,
                IncubatorPrefab = EAssetKey.Entity_Incubator4,
                IncubatorIcon = EAssetKey.Icon_Incubator4,
                IncubatorDebrisIcon = EAssetKey.Icon_IncubatorDebris4,
            }
        },
    };
    public bool TryGetIncubatorInfo(EQualityType f_QualityLevle, out HeroIncubatorInfo f_IncubatorInfo)
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
    private Dictionary<EHeroCardType, WorldObjectBaseData> m_HeroData = new()
    {

    };
    public bool GetHeroDataByType(EHeroCardType f_HeroType, int f_TargetIndex, EHeroCradStarLevel f_StarLevel, out Entity_HeroBaseData f_Result)
    {
        f_Result = null;
        if (TryGetCardDataByType(f_HeroType, f_StarLevel, out var result))
        {
            if (result is Entity_HeroBaseData heroData)
            {
                f_Result = heroData;
                heroData.MoveToChunk(f_TargetIndex);
            }
        }
        return f_Result != null;
    }
    public bool TryGetMonsterBaseNewData(EHeroCardType f_HeroType, int f_TargetIndex, EHeroCradStarLevel f_StarLevel, out Entity_MonsterBaseNewData f_Result)
    {
        if (TryGetMonsterEntityData(f_HeroType, out f_Result))
        {
            f_Result.MoveToChunk(f_TargetIndex);
        }
        return f_Result != null;
    }
    public bool TryGetHeroBaseNewData(EHeroCardType f_HeroType, int f_TargetIndex, EHeroCradStarLevel f_StarLevel, out Entity_HeroBaseNewData f_Result)
    {
        f_Result = null;
        if (TryGetHeroEntityData(f_HeroType, out var result))
        {
            if (result is Entity_HeroBaseNewData heroData)
            {
                f_Result = heroData;
                heroData.MoveToChunk(f_TargetIndex);
            }
        }
        return f_Result != null;
    }

    public bool TryGetCardDataByType(EHeroCardType f_HeroType, EHeroCradStarLevel f_StarLevel, out WorldObjectBaseData f_Result)
    {
        f_Result = null;
        switch (f_HeroType)
        {
            case EHeroCardType.Hero1:
                f_Result = new Entity_Player_Hero1Data(f_StarLevel);
                break;
            case EHeroCardType.Hero2:
                f_Result = new Entity_Player_Hero2Data(f_StarLevel);
                break;
            case EHeroCardType.Hero3:
                f_Result = new Entity_Player_Hero3Data(f_StarLevel);
                break;
            case EHeroCardType.Hero4:
                f_Result = new Entity_Player_Hero4Data(f_StarLevel);
                break;
            case EHeroCardType.EnumCount:
                break;
            default:
                break;
        }
        return f_Result != null;
    }

    public bool TryGetHeroEntityData(EHeroCardType f_HeroType, out WorldObjectBaseData f_Result)
    {
        f_Result = null;
        switch (f_HeroType)
        {
            case EHeroCardType.Hero1:
                f_Result = new Entity_Hero_Warrior1Data();
                break;
            case EHeroCardType.Hero2:
                f_Result = new Entity_Enchanter1Data();
                break;
            case EHeroCardType.Hero3:
                f_Result = new Entity_Supplementary1Data();
                break;
            case EHeroCardType.Hero4:
                f_Result = new Entity_MainTank1Data();
                break;
            case EHeroCardType.EnumCount:
                break;
            default:
                break;
        }
        return f_Result != null;
    }
    public bool TryGetMonsterEntityData(EHeroCardType f_HeroType, out Entity_MonsterBaseNewData f_Result)
    {
        f_Result = null;
        switch (f_HeroType)
        {
            case EHeroCardType.Monster_Default1:
                f_Result = new Entity_Monster_Warrior1Data();
                break;
            case EHeroCardType.Monster_Default2:
                f_Result = new Entity_Monster_Warrior2Data();
                break;
            case EHeroCardType.Monster_Default3:
                f_Result = new Entity_Monster_Warrior3Data();
                break;
            case EHeroCardType.Monster_Boss1:
                f_Result = new Entity_Monster_Boss1Data();
                break;
            case EHeroCardType.EnumCount:
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
                IncubatorRatio = new()
                {
                    BloodRatio = 0.1f,
                    AtkSpeedRatio = 0.1f,
                    HarmRatio = 0.2f,
                    DefenceRatio = 0.2f,
                },
                WeatherGainTypeList = new()
                {
                    EWeatherGainType.Default1,
                    EWeatherGainType.Default2,
                    EWeatherGainType.Default3,
                    EWeatherGainType.Default4,
                    EWeatherGainType.Default5,
                    EWeatherGainType.Default6,
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
                IncubatorRatio = new()
                {
                    BloodRatio = 0.1f,
                    AtkSpeedRatio = 0.1f,
                    HarmRatio = 0.2f,
                    DefenceRatio = 0.2f,
                },
                WeatherGainTypeList = new()
                {
                    EWeatherGainType.Default1,
                    EWeatherGainType.Default2,
                    EWeatherGainType.Default3,
                    EWeatherGainType.Default4,
                    EWeatherGainType.Default5,
                    EWeatherGainType.Default6,
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
                IncubatorRatio = new()
                {
                    BloodRatio = 0.1f,
                    AtkSpeedRatio = 0.1f,
                    HarmRatio = 0.2f,
                    DefenceRatio = 0.2f,
                },
                WeatherGainTypeList = new()
                {
                    EWeatherGainType.Default1,
                    EWeatherGainType.Default2,
                    EWeatherGainType.Default3,
                    EWeatherGainType.Default4,
                    EWeatherGainType.Default5,
                    EWeatherGainType.Default6,
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
    //                                catalogue -- 天气随机增益 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EWeatherGainType, WeatherGainInfo> m_WeatherGainList = new()
    {
        {
            EWeatherGainType.Default1,
            new()
            {
                Name = "Default1",
                WeatherGainType = EWeatherGainType.Default1,
                Describe = "增加攻速 × 3",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
        {
            EWeatherGainType.Default2,
            new()
            {
                Name = "Default2",
                WeatherGainType = EWeatherGainType.Default2,
                Describe = "增级攻击 × 5",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
        {
            EWeatherGainType.Default3,
            new()
            {
                Name = "Default3",
                WeatherGainType = EWeatherGainType.Default3,
                Describe = "增加最大血量 × 0.5",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
        {
            EWeatherGainType.Default4,
            new()
            {
                Name = "Default4",
                WeatherGainType = EWeatherGainType.Default4,
                Describe = "增加防御 × 0.5",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
        {
            EWeatherGainType.Default5,
            new()
            {
                Name = "Default5",
                WeatherGainType = EWeatherGainType.Default5,
                Describe = "为开发功能",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
        {
            EWeatherGainType.Default6,
            new()
            {
                Name = "Default6",
                WeatherGainType = EWeatherGainType.Default6,
                Describe = "未开发功能",
                IconDic = new()
                {
                    {
                        EWeatherGainLevel.Level1,
                        EAssetKey.Icon_WeatherGainLevel1Default1
                    },
                    {
                        EWeatherGainLevel.Level2,
                        EAssetKey.Icon_WeatherGainLevel2Default1
                    },
                    {
                        EWeatherGainLevel.Level3,
                        EAssetKey.Icon_WeatherGainLevel3Default1
                    },
                    {
                        EWeatherGainLevel.Level4,
                        EAssetKey.Icon_WeatherGainLevel4Default1
                    },
                    {
                        EWeatherGainLevel.MaxLevel,
                        EAssetKey.Icon_WeatherGainLevelMaxDefault1
                    },
                }
            }
        },
    };
    public bool TryGetWeatherGainInfo(EWeatherGainType f_WeatherGainType, out WeatherGainInfo f_Result)
    {
        return m_WeatherGainList.TryGetValue(f_WeatherGainType, out f_Result);
    }
    public bool TryGetWeatherGainData(EWeatherGainType f_WeatherGainType, out WeatherGainData f_Result, EWeatherGainLevel f_Level)
    {
        f_Result = null;
        switch (f_WeatherGainType)
        {
            case EWeatherGainType.Default1:
                f_Result = new Weather_GainDefault1();
                break;
            case EWeatherGainType.Default2:
                f_Result = new Weather_GainDefault2();
                break;
            case EWeatherGainType.Default3:
                f_Result = new Weather_GainDefault3();
                break;
            case EWeatherGainType.Default4:
                f_Result = new Weather_GainDefault4();
                break;
            case EWeatherGainType.Default5:
                f_Result = new Weather_GainDefault5();
                break;
            case EWeatherGainType.Default6:
                f_Result = new Weather_GainDefault6();
                break;
            default:
                break;
        }
        f_Result.Initialization(f_Level);
        return f_Result != null;
    }
    private Dictionary<EWeatherGainLevel, WeatherGainLevelInfo> m_WeatherGainLevelInfo = new()
    {
        {
            EWeatherGainLevel.Level1,
            new()
            {
                Level = EWeatherGainLevel.Level1,
                Color = Color.gray,
            }
        },
        {
            EWeatherGainLevel.Level2,
            new()
            {
                Level = EWeatherGainLevel.Level2,
                Color = Color.green,
            }
        },
        {
            EWeatherGainLevel.Level3,
            new()
            {
                Level = EWeatherGainLevel.Level3,
                Color = Color.yellow,
            }
        },
        {
            EWeatherGainLevel.Level4,
            new()
            {
                Level = EWeatherGainLevel.Level4,
                Color = Color.red,
            }
        },
        {
            EWeatherGainLevel.MaxLevel,
            new()
            {
                Level = EWeatherGainLevel.MaxLevel,
                Color = Color.cyan,
            }
        },
    };
    public bool TryGetWeatherGainLevelInfo(EWeatherGainLevel f_Level, out WeatherGainLevelInfo f_Result)
    {
        return m_WeatherGainLevelInfo.TryGetValue(f_Level, out f_Result);
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
                IconPath = EAssetKey.BuffIcon_AddBlood
            }
        },
        {

            EBuffType.Poison,
            new BuffInfo()
            {
                BuffType = EBuffType.Poison,
                Desc = "持续减血",
                Name = "中毒",
                IconPath = EAssetKey.BuffIcon_Poison
            }
        },
        {

            EBuffType.WeatherSpeed,
            new BuffInfo()
            {
                BuffType = EBuffType.WeatherSpeed,
                Desc = "增加速度",
                Name = "速度",
                IconPath = EAssetKey.BuffIcon_Poison
            }
        },
        {

            EBuffType.WeatherAttack,
            new BuffInfo()
            {
                BuffType = EBuffType.WeatherAttack,
                Desc = "增加攻击",
                Name = "攻击",
                IconPath = EAssetKey.BuffIcon_Poison
            }
        },
        {

            EBuffType.WeatherMaxBlood,
            new BuffInfo()
            {
                BuffType = EBuffType.WeatherMaxBlood,
                Desc = "增加最大生命值",
                Name = "最大生命值",
                IconPath = EAssetKey.BuffIcon_Poison
            }
        },
        {

            EBuffType.WeatherDefense,
            new BuffInfo()
            {
                BuffType = EBuffType.WeatherDefense,
                Desc = "增加防御力",
                Name = "防御力",
                IconPath = EAssetKey.BuffIcon_Poison
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
            case EBuffType.WeatherSpeed:
                f_Result = IBuffUtil.CreateBuffData<Effect_Buff_WeatherGainSpeedData>(f_Initiator, f_Target);
                break;
            case EBuffType.WeatherAttack:
                f_Result = IBuffUtil.CreateBuffData<Effect_Buff_WeatherBuffAttackData>(f_Initiator, f_Target);
                break;
            case EBuffType.WeatherMaxBlood:
                f_Result = IBuffUtil.CreateBuffData<Effect_Buff_WeatherBuffMaxBloodData>(f_Initiator, f_Target);
                break;
            case EBuffType.WeatherDefense:
                f_Result = IBuffUtil.CreateBuffData<Effect_Buff_WeatherBuffDefenceData>(f_Initiator, f_Target);
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
    private Dictionary<EGainType, GainInfo> m_GainInfo = new()
    {
        {
            EGainType.Launch1,
            new()
            {
                GainType = EGainType.Launch1,
                GainView = EBuffView.Launch,
            }
        },
        {
            EGainType.Volccano1,
            new()
            {
                GainType = EGainType.Volccano1,
                GainView = EBuffView.Interval,
            }
        },
        {
            EGainType.Volccano2,
            new()
            {
                GainType = EGainType.Volccano2,
                GainView = EBuffView.Interval,
            }
        },
        {
            EGainType.Volccano3,
            new()
            {
                GainType = EGainType.Volccano3,
                GainView = EBuffView.Interval,
            }
        },
        {
            EGainType.AttackSpeed1,
            new()
            {
                GainType = EGainType.AttackSpeed1,
                GainView = EBuffView.perpetual,
            }
        },
        {
            EGainType.AttackHarm1,
            new()
            {
                GainType = EGainType.AttackHarm1,
                GainView = EBuffView.perpetual,
            }
        },
        {
            EGainType.AttackRange1,
            new()
            {
                GainType = EGainType.AttackRange1,
                GainView = EBuffView.perpetual,
            }
        },
        {
            EGainType.Deffense1,
            new()
            {
                GainType = EGainType.Deffense1,
                GainView = EBuffView.perpetual,
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
            case EGainType.AttackSpeed1:
                f_Result = new Entity_Gain_AddAttackSpeed1Data();
                break;
            case EGainType.AttackHarm1:
                f_Result = new Entity_Gain_AddAttackHarm1Data();
                break;
            case EGainType.AttackRange1:
                f_Result = new Entity_Gain_AddAttackRange1Data();
                break;
            case EGainType.Deffense1:
                f_Result = new Entity_Gain_AddDeffense1Data();
                break;
            case EGainType.EnumCount:
                break;
            default:
                break;
        }
        return f_Result != null;
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击特效 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public bool TryGetAttactEffectBaseData(EAttackEffectType f_EffectType, out EntityEffectBaseData f_Result, Vector3 f_StartPos)
    {
        f_Result = null;
        switch (f_EffectType)
        {
            case EAttackEffectType.Default1:
                var eff = new Entity_Effect_Attack_Default1Data();
                eff.Initialization(f_StartPos, null);
                f_Result = eff;
                break;
            case EAttackEffectType.Default2:
                break;
            default:
                break;
        }

        return f_Result != null;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 品质 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EQualityType, QualityInfo> m_QualityInfos = new()
    {
        {
            EQualityType.Quality1,
            new()
            {
                QualityType = EQualityType.Quality1,
                Color = new Color32(128, 128, 128, 255),
            }
        },
        {
            EQualityType.Quality2,
            new()
            {
                QualityType = EQualityType.Quality2,
                Color = new Color32(80, 231, 182, 255),
            }
        },
        {
            EQualityType.Quality3,
            new()
            {
                QualityType = EQualityType.Quality3,
                Color = new Color32(231, 152, 80, 255),
            }
        },
        {
            EQualityType.Quality4,
            new()
            {
                QualityType = EQualityType.Quality4,
                Color = new Color32(243, 89, 49, 255),
            }
        },
    };
    public bool TryGetQualityInfo(EQualityType f_QualityType, out QualityInfo f_Result)
    {
        return m_QualityInfos.TryGetValue(f_QualityType, out f_Result);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 技能 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EPersonSkillType, PersonSkillInfo> m_PersonSkillInfos = new()
    {
        {
            EPersonSkillType.Stage1_Default1,
            new()
            {
                SkillName = "Hero1_Stage1_Default1",
                Describes = "Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1Hero1_Stage1_Default1",
                Quality = EQualityType.Quality1,
                PersonSkillType = EPersonSkillType.Stage1_Default1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage1_Default1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage1_Default2,
            new()
            {
                SkillName = "Hero1_Stage1_Default2",
                Describes = "Hero1_Stage1_Default2Hero1_Stage1_Default2Hero1_Stage1_Default2Hero1_Stage1_Default2Hero1_Stage1_Default2Hero1_Stage1_Default2",
                Quality = EQualityType.Quality2,
                PersonSkillType = EPersonSkillType.Stage1_Default2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage1_Default2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage1_Default3,
            new()
            {
                SkillName = "Hero1_Stage1_Default3",
                Describes = "Hero1_Stage1_Default3Hero1_Stage1_Default3Hero1_Stage1_Default3",
                Quality = EQualityType.Quality3,
                PersonSkillType = EPersonSkillType.Stage1_Default3,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage1_Default3,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage1_Default4,
            new()
            {
                SkillName = "Hero1_Stage1_Default4",
                Describes = "Hero1_Stage1_Default4Hero1_Stage1_Default4Hero1_Stage1_Default4Hero1_Stage1_Default4Hero1_Stage1_Default4Hero1_Stage1_Default4Hero1_Stage1_Default4",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage1_Default4,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage1_Default4,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default1_Loss1,
            new()
            {
                SkillName = "Hero1_Stage2_Default1_Loss1",
                Describes = "Hero1_Stage2_Default1_Loss1",
                Quality = EQualityType.Quality1,
                PersonSkillType = EPersonSkillType.Stage2_Default1_Loss1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default1_Loss2,
            new()
            {
                SkillName = "Hero1_Stage2_Default1_Loss2",
                Describes = "Hero1_Stage2_Default1_Loss2Hero1_Stage2_Default1_Loss2",
                Quality = EQualityType.Quality2,
                PersonSkillType = EPersonSkillType.Stage2_Default1_Loss2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default1_Loss3,
            new()
            {
                SkillName = "Hero1_Stage2_Default1_Loss3",
                Describes = "Hero1_Stage2_Default1_Loss3Hero1_Stage2_Default1_Loss3Hero1_Stage2_Default1_Loss3",
                Quality = EQualityType.Quality2,
                PersonSkillType = EPersonSkillType.Stage2_Default1_Loss3,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default1_Loss3,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss1_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss1_Height1",
                Describes = "Hero1_Stage3_Default1_Loss1_Height1",
                Quality = EQualityType.Quality2,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss1_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss1_Height2,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss1_Height2",
                Describes = "Hero1_Stage3_Default1_Loss1_Height2Hero1_Stage3_Default1_Loss1_Height2",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss1_Height2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default2_Loss1_Height2,
            new()
            {
                SkillName = "Hero1_Stage3_Default2_Loss1_Height2",
                Describes = "Hero1_Stage3_Default2_Loss1_Height2Hero1_Stage3_Default2_Loss1_Height2",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default2_Loss1_Height2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss1_Height2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default2_Loss1_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default2_Loss1_Height1",
                Describes = "Hero1_Stage3_Default2_Loss1_Height1Hero1_Stage3_Default2_Loss1_Height1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default2_Loss1_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss1_Height1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default2_Loss2_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default2_Loss2_Height1",
                Describes = "Hero1_Stage3_Default2_Loss2_Height1Hero1_Stage3_Default2_Loss2_Height1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default2_Loss2_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default2_Loss2_Height1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default3_Loss1_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default3_Loss1_Height1",
                Describes = "Hero1_Stage3_Default3_Loss1_Height1Hero1_Stage3_Default3_Loss1_Height1Hero1_Stage3_Default3_Loss1_Height1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default3_Loss1_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default3_Loss1_Height1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default2_Loss1,
            new()
            {
                SkillName = "Hero1_Stage2_Default2_Loss1",
                Describes = "Hero1_Stage2_Default2_Loss1Hero1_Stage2_Default2_Loss1Hero1_Stage2_Default2_Loss1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage2_Default2_Loss1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default2_Loss1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default2_Loss2,
            new()
            {
                SkillName = "Hero1_Stage2_Default2_Loss2",
                Describes = "Hero1_Stage2_Default2_Loss2Hero1_Stage2_Default2_Loss2",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage2_Default2_Loss2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default2_Loss2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage2_Default3_Loss1,
            new()
            {
                SkillName = "Hero1_Stage2_Default3_Loss1",
                Describes = "Hero1_Stage2_Default3_Loss1Hero1_Stage2_Default3_Loss1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage2_Default3_Loss1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage2_Default3_Loss1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss1_Height3,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss1_Height3",
                Describes = "Hero1_Stage3_Default1_Loss1_Height3Hero1_Stage3_Default1_Loss1_Height3",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss1_Height3,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss1_Height3,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss2_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss2_Height1",
                Describes = "Hero1_Stage3_Default1_Loss2_Height1Hero1_Stage3_Default1_Loss2_Height1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss2_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss2_Height1,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss2_Height2,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss2_Height2",
                Describes = "Hero1_Stage3_Default1_Loss2_Height2Hero1_Stage3_Default1_Loss2_Height2",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss2_Height2,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss2_Height2,
                ExpenditureBase = 0,
            }
        },
        {
            EPersonSkillType.Stage3_Default1_Loss3_Height1,
            new()
            {
                SkillName = "Hero1_Stage3_Default1_Loss3_Height1",
                Describes = "Hero1_Stage3_Default1_Loss3_Height1Hero1_Stage3_Default1_Loss3_Height1",
                Quality = EQualityType.Quality4,
                PersonSkillType = EPersonSkillType.Stage3_Default1_Loss3_Height1,
                IconKey = EAssetKey.Icon_Skill_Hero1_Stage3_Default1_Loss3_Height1,
                ExpenditureBase = 0,
            }
        },
    };
    public bool TryGetPersonSkillInfo(EPersonSkillType f_SkillType, out PersonSkillInfo f_Result)
    {
        return m_PersonSkillInfos.TryGetValue(f_SkillType, out f_Result);
    }
    public bool TryGetPersonSkillData(EPersonSkillType f_SkillType, out PersonSkillBaseData f_DataBase)
    {
        f_DataBase = null;
        switch (f_SkillType)
        {
            case EPersonSkillType.Stage1_Default1:
                f_DataBase = new PersonSkillData_Hero1_Stage1_Default1();
                break;
            case EPersonSkillType.Stage1_Default2:
                f_DataBase = new PersonSkillData_Hero1_Stage1_Default2();
                break;
            case EPersonSkillType.Stage1_Default3:
                f_DataBase = new PersonSkillData_Hero1_Stage1_Default3();
                break;
            case EPersonSkillType.Stage1_Default4:
                f_DataBase = new PersonSkillData_Hero1_Stage1_Default4();
                break;
            case EPersonSkillType.Stage2_Default1_Loss1:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default1_Loss1();
                break;
            case EPersonSkillType.Stage2_Default1_Loss2:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default1_Loss2();
                break;
            case EPersonSkillType.Stage2_Default1_Loss3:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default1_Loss3();
                break;
            case EPersonSkillType.Stage2_Default2_Loss1:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default2_Loss1();
                break;
            case EPersonSkillType.Stage2_Default2_Loss2:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default2_Loss2();
                break;
            case EPersonSkillType.Stage2_Default3_Loss1:
                f_DataBase = new PersonSkillData_Hero1_Stage2_Default3_Loss1();
                break;
            case EPersonSkillType.Stage3_Default1_Loss1_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss1_Height1();
                break;
            case EPersonSkillType.Stage3_Default1_Loss1_Height2:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss1_Height2();
                break;
            case EPersonSkillType.Stage3_Default1_Loss1_Height3:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss1_Height3();
                break;
            case EPersonSkillType.Stage3_Default1_Loss2_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss2_Height1();
                break;
            case EPersonSkillType.Stage3_Default1_Loss2_Height2:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss2_Height2();
                break;
            case EPersonSkillType.Stage3_Default1_Loss3_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default1_Loss3_Height1();
                break;
            case EPersonSkillType.Stage3_Default2_Loss1_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default2_Loss1_Height1();
                break;
            case EPersonSkillType.Stage3_Default2_Loss1_Height2:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default2_Loss1_Height2();
                break;
            case EPersonSkillType.Stage3_Default2_Loss2_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default2_Loss2_Height1();
                break;
            case EPersonSkillType.Stage3_Default3_Loss1_Height1:
                f_DataBase = new PersonSkillData_Hero1_Stage3_Default3_Loss1_Height1();
                break;
            default:
                break;
        }
        return f_DataBase != null;
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 装备 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EEquipmentType, EquipmentInfo> m_EquipmentInfos = new()
    {
        {
            EEquipmentType.EquipDefault1,
            new()
            {
                EquipmentType = EEquipmentType.EquipDefault1,
                EquipmentName = "EquipDefault1",
                Describes = "EquipDefault1EquipDefault1",
                Property = new()
                {
                    Blood = 100,
                    AtkSpeed = 1.0f,
                    Attack = 10,
                    Defense = 12,
                    MaxBlood = 600,
                    MoveSpeed = 1.0f,
                },
                QualityType = EQualityType.Quality1,
                IconKey = EAssetKey.Icon_Equipment_Default1,
            }
        },
        {
            EEquipmentType.EquipDefault2,
            new()
            {
                EquipmentType = EEquipmentType.EquipDefault2,
                EquipmentName = "EquipDefault2",
                Describes = "EquipDefault2EquipDefault2EquipDefault2",
                Property = new()
                {
                    Blood = 100,
                    AtkSpeed = 1.0f,
                    Attack = 10,
                    Defense = 12,
                    MaxBlood = 600,
                    MoveSpeed = 1.0f,
                },
                QualityType = EQualityType.Quality2,
                IconKey = EAssetKey.Icon_Equipment_Default2,
            }
        },
        {
            EEquipmentType.EquipDefault3,
            new()
            {
                EquipmentType = EEquipmentType.EquipDefault3,
                EquipmentName = "EquipDefault3",
                Describes = "EquipDefault3EquipDefault3EquipDefault3EquipDefault3",
                Property = new()
                {
                    Blood = 100,
                    AtkSpeed = 1.0f,
                    Attack = 10,
                    Defense = 12,
                    MaxBlood = 600,
                    MoveSpeed = 1.0f,
                },
                QualityType = EQualityType.Quality3,
                IconKey = EAssetKey.Icon_Equipment_Default3,
            }
        },
        {
            EEquipmentType.EquipDefault4,
            new()
            {
                EquipmentType = EEquipmentType.EquipDefault4,
                EquipmentName = "EquipDefault4",
                Describes = "EquipDefault4EquipDefault4EquipDefault4EquipDefault4EquipDefault4EquipDefault4",
                Property = new()
                {
                    Blood = 100,
                    AtkSpeed = 1.0f,
                    Attack = 10,
                    Defense = 12,
                    MaxBlood = 600,
                    MoveSpeed = 1.0f,
                },
                QualityType = EQualityType.Quality4,
                IconKey = EAssetKey.Icon_Equipment_Default4,
            }
        },
    };
    public bool TryGetEquipmentInfo(EEquipmentType f_EquipType, out EquipmentInfo f_Result)
    {
        return m_EquipmentInfos.TryGetValue(f_EquipType, out f_Result);
    }
    public bool TryGetEquipData(EEquipmentType f_EquipType, out EquipmentBaseData f_DataBase)
    {
        f_DataBase = null;
        switch (f_EquipType)
        {
            case EEquipmentType.EquipDefault1:
                f_DataBase = new EquipmentData_Default1();
                break;
            case EEquipmentType.EquipDefault2:
                f_DataBase = new EquipmentData_Default2();
                break;
            case EEquipmentType.EquipDefault3:
                f_DataBase = new EquipmentData_Default3();
                break;
            case EEquipmentType.EquipDefault4:
                f_DataBase = new EquipmentData_Default4();
                break;
            default:
                break;
        }
        return f_DataBase != null;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 能量水晶 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EQualityType, EnergyCrystalInfo> m_EnergyCrystalInfos = new()
    {
        {
            EQualityType.Quality1,
            new()
            {
                Quality = EQualityType.Quality1,
                Name = "下级水晶",
                Blood = 600,
                BlastValue = 300,
                BlastRange = 2,
            }
        },
    };
    public bool TryGetEnergyCrystalInfo(EQualityType f_Quality, out EnergyCrystalInfo f_Result)
    {
        return m_EnergyCrystalInfos.TryGetValue(f_Quality, out f_Result);
    }
    public bool TryGetEnergyCrystalData(EQualityType f_Quality, out Entity_EnergyCrystalBaseData f_Result)
    {
        f_Result = null;
        switch (f_Quality)
        {
            case EQualityType.None:
                break;
            case EQualityType.Quality1:
                f_Result = new Entity_EnergyCrystal1Data();
                break;
            case EQualityType.Quality2:
                break;
            case EQualityType.Quality3:
                break;
            case EQualityType.Quality4:
                break;
            case EQualityType.EnumCount:
                break;
            default:
                break;
        }
        return f_Result != null;
    }

    public class MapConfigInfo
    {
        public EAssetKey AssetPath;
    }
    private Dictionary<EMapLevelType, MapConfigInfo> m_MapConfigDic = new()
    {
        {
            EMapLevelType.Level1,
            new()
            {
                AssetPath = EAssetKey.Cfg_MapLevel1,
            }
        },
        {
            EMapLevelType.Level2,
            new()
            {
                AssetPath = EAssetKey.Cfg_MapLevel2,
            }
        },
        {
            EMapLevelType.Level3,
            new()
            {
                AssetPath = EAssetKey.Cfg_MapLevel3,
            }
        },
    };
    public bool TryGetMapConfigInfo(EMapLevelType f_LevelType, out MapConfigInfo f_LevelInfo)
    {
        return m_MapConfigDic.TryGetValue(f_LevelType, out f_LevelInfo);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 阵型实例 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--

    public bool TryGetFormationData(EFormationType f_Type, out Entity_FormationBaseData f_Data)
    {
        f_Data = null;
        switch (f_Type)
        {
            case EFormationType.Near:
                f_Data = new Entity_Formation_NearData();
                break;
            case EFormationType.Sphere:
                f_Data = new Entity_Formation_SphereData();
                break;
            default:
                break;
        }
        return f_Data != null;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 职业 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EHeroVocationalType, HeroVocationalInfo> m_HeroVocationalInfo = new()
    {
        {
            EHeroVocationalType.Warrior,
            new()
            {
                VocationalType = EHeroVocationalType.Warrior,
                IconID = EAssetKey.Icon_Warrior,
                Name = "战士",
            }
        },
        {
            EHeroVocationalType.Enchanter,
            new()
            {
                VocationalType = EHeroVocationalType.Enchanter,
                IconID = EAssetKey.Icon_Enchanter,
                Name = "法师",
            }
        },
        {
            EHeroVocationalType.Supplementary,
            new()
            {
                VocationalType = EHeroVocationalType.Supplementary,
                IconID = EAssetKey.Icon_Supplementary,
                Name = "辅助",
            }
        },
        {
            EHeroVocationalType.MainTank,
            new()
            {
                VocationalType = EHeroVocationalType.MainTank,
                IconID = EAssetKey.Icon_MainTank,
                Name = "肉盾",
            }
        },
    };
    public bool TryGetHeroVocationalInfo(EHeroVocationalType f_HeroVocation, out HeroVocationalInfo f_HeroVocationalInfo)
    {
        return m_HeroVocationalInfo.TryGetValue(f_HeroVocation, out f_HeroVocationalInfo);
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 关卡信息
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    //法师
    public static int EnchanterAtk = 120; //攻击
    public static int EnchanterBlood = 500; //血量
    //坦克
    public static int MainTankBlood = 2000; //血量
    public static int MainTankDefence = 60; //防御
    //辅助
    public static int SupplementaryAtk = 50;
    public static int SupplementaryBlood = 500;
    public static int SupplementaryDefence = 20;
    //战士
    public static int WarriorAtk = 100;
    public static int WarriorBlood = 1000; 

    //怪物基本属性 {攻击，生命，防御}
    public static int[,] monsterBaseInfo =new int[4,3] {
        {30,600,0}, //monster1
        {50,3000,80}, //monster2
        {120,2000,40}, //monster3
        {120,20000,50}, //boss1
    }; 
    private Dictionary<EMapLevelType, MapLevelInfo> m_LevelWaveInfos = new()
    {
        {
            EMapLevelType.Level0,
            new()
            {
                GameNewHelpInfo = EGameHelpType.Level0,
                MapWH = new(9, 24),
                MapChunkSize = new Vector2(Mathf.Sqrt(1 - 0.5f * 0.5f) * 2, 2),
                MapChunkInterval = new(0, 0),
                BarrierData = new()
                {
                    {
                        EBarrierType.Massif,
                        new()
                        {
                            new()
                            {
                                Index = new(0, 8)
                            },
                            new()
                            {
                                Index = new(0, 5)
                            },
                            new()
                            {
                                Index = new(1, 7)
                            },
                            new()
                            {
                                Index = new(2, 7)
                            },
                            new()
                            {
                                Index = new(1, 5)
                            },
                            new()
                            {
                                Index = new(2, 6)
                            },
                            new()
                            {
                                Index = new(6, 7)
                            },
                            new()
                            {
                                Index = new(7, 6)
                            },
                            new()
                            {
                                Index = new(8, 6)
                            },
                            new()
                            {
                                Index = new(7, 7)
                            },
                            new()
                            {
                                Index = new(8, 8)
                            },
                        }
                    }
                },
                EnergyCrystalData = new()
                {
                    {
                        0,
                        new()
                        {
                            StartIndex = 144,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        1,
                        new()
                        {
                            StartIndex = 120,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        2,
                        new()
                        {
                            StartIndex = 96,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        3,
                        new()
                        {
                            StartIndex = 72,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        4,
                        new()
                        {
                            StartIndex = 48,
                            Quality = EQualityType.Quality1,
                        }
                    },
                },
                MonsterData = new()
                {
                    {
                        0,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 111,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 1.5f,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },

                            }
                        }
                    },
                    {
                        1,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 190,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 142,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 94,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 46,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 44,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 92,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 140,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 188,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 186,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 138,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 90,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 42,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                            }
                        }
                    },
                },

                WarSeatCount = 12,
                WarSeatRowCount = 6,
                HeroPoolCount = 10,
                WarSeatLength = 1.8f,
                WarSeatInterval = new(0.1f, 0.1f),

                LevelUpdateExpenditure = 2,
                LevelInitGlod = 20,
            }
        },
        {
            EMapLevelType.Level1,
            new()
            {
                GameNewHelpInfo = EGameHelpType.Level1,
                MapWH = new(9, 24),
                MapChunkSize = new Vector2(Mathf.Sqrt(1 - 0.5f * 0.5f) * 2, 2),
                MapChunkInterval = new(0, 0),
                BarrierData = new()
                {
                    {
                        EBarrierType.Massif,
                        new()
                        {
                            new()
                            {
                                Index = new(0, 8)
                            },
                            new()
                            {
                                Index = new(0, 5)
                            },
                            new()
                            {
                                Index = new(1, 7)
                            },
                            new()
                            {
                                Index = new(2, 7)
                            },
                            new()
                            {
                                Index = new(1, 5)
                            },
                            new()
                            {
                                Index = new(2, 6)
                            },
                            new()
                            {
                                Index = new(6, 7)
                            },
                            new()
                            {
                                Index = new(7, 6)
                            },
                            new()
                            {
                                Index = new(8, 6)
                            },
                            new()
                            {
                                Index = new(7, 7)
                            },
                            new()
                            {
                                Index = new(8, 8)
                            },
                        }
                    }
                },
                EnergyCrystalData = new()
                {
                    {
                        0,
                        new()
                        {
                            StartIndex = 144,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        1,
                        new()
                        {
                            StartIndex = 120,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        2,
                        new()
                        {
                            StartIndex = 96,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        3,
                        new()
                        {
                            StartIndex = 72,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        4,
                        new()
                        {
                            StartIndex = 48,
                            Quality = EQualityType.Quality1,
                        }
                    },
                },
                MonsterData = new()
                {
                    {
                        0,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 110,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 108,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                }
                            }
                        }
                    },
                    {
                        1,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 190,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 142,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 94,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 46,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 188,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 140,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 92,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 44,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 186,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 138,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 90,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 42,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                            }
                        }
                    },
                },

                WarSeatCount = 12,
                WarSeatRowCount = 6,
                HeroPoolCount = 10,
                WarSeatLength = 1.8f,
                WarSeatInterval = new(0.1f, 0.1f),

                LevelUpdateExpenditure = 2,
                LevelInitGlod = 35,
            }
        },
        {
            EMapLevelType.Level2,
            new()
            {
                GameNewHelpInfo = EGameHelpType.Level2,
                MapWH = new(9, 24),
                MapChunkSize = new Vector2(Mathf.Sqrt(1 - 0.5f * 0.5f) * 2, 2),
                MapChunkInterval = new(0, 0),
                BarrierData = new()
                {
                    {
                        EBarrierType.Massif,
                        new()
                        {
                            new()
                            {
                                Index = new(1, 1)
                            },
                            new()
                            {
                                Index = new(7, 5)
                            },
                            new()
                            {
                                Index = new(7, 6)
                            },
                            new()
                            {
                                Index = new(3, 15)
                            },
                            new()
                            {
                                Index = new(1, 6)
                            },
                            new()
                            {
                                Index = new(2, 6)
                            },
                            new()
                            {
                                Index = new(3, 8)
                            },
                            new()
                            {
                                Index = new(3, 5)
                            },
                        }
                    }
                },
                EnergyCrystalData = new()
                {
                    {
                        0,
                        new()
                        {
                            StartIndex = 144,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        1,
                        new()
                        {
                            StartIndex = 120,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        2,
                        new()
                        {
                            StartIndex = 96,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        3,
                        new()
                        {
                            StartIndex = 72,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        4,
                        new()
                        {
                            StartIndex = 48,
                            Quality = EQualityType.Quality1,
                        }
                    },
                },
                MonsterData = new()
                {
                    {
                        0,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 153,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 57,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 105,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                            }
                        }
                    },
                    {
                        1,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 156,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 108,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 60,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 158,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 110,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 62,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 160,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 112,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 64,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                            }
                        }
                    },
                    {
                        2,
                        new()
                        {
                            ActiveTime = 30f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 186,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 138,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 90,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 42,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 188,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 140,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 92,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 44,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 190,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 142,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 94,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0.5f,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 46,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 215,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 167,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 119,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 71,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 23,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                            }
                        }
                    },
                },

                WarSeatCount = 12,
                WarSeatRowCount = 6,
                HeroPoolCount = 10,
                WarSeatLength = 1.8f,
                WarSeatInterval = new(0.1f, 0.1f),

                LevelUpdateExpenditure = 2,
                LevelInitGlod = 30,
            }
        },
        {
            EMapLevelType.Level3,
            new()
            {
                GameNewHelpInfo = EGameHelpType.Level3,
                MapWH = new(9, 24),
                MapChunkSize = new Vector2(Mathf.Sqrt(1 - 0.5f * 0.5f) * 2, 2),
                MapChunkInterval = new(0, 0),
                BarrierData = new()
                {
                    {
                        EBarrierType.Massif,
                        new()
                        {
                            new()
                            {
                                Index = new(1, 1)
                            },
                            new()
                            {
                                Index = new(7, 5)
                            },
                            new()
                            {
                                Index = new(7, 6)
                            },
                            new()
                            {
                                Index = new(3, 15)
                            },
                            new()
                            {
                                Index = new(1, 6)
                            },
                            new()
                            {
                                Index = new(2, 6)
                            },
                            new()
                            {
                                Index = new(3, 8)
                            },
                            new()
                            {
                                Index = new(3, 5)
                            },
                        }
                    }
                },
                EnergyCrystalData = new()
                {
                    {
                        0,
                        new()
                        {
                            StartIndex = 144,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        1,
                        new()
                        {
                            StartIndex = 120,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        2,
                        new()
                        {
                            StartIndex = 96,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        3,
                        new()
                        {
                            StartIndex = 72,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        4,
                        new()
                        {
                            StartIndex = 48,
                            Quality = EQualityType.Quality1,
                        }
                    },
                },
                MonsterData = new()
                {
                    {
                        0,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 201,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 153,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 105,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 57,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 9,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                }
                            }
                        }
                    },
                    {
                        1,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 179,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 131,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 83,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 35,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 180,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 132,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 84,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 36,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                            }
                        }
                    },
                    {
                        2,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 182,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 134,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 86,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 38,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 184,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 136,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 88,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 40,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 186,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 138,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 90,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 42,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                            }
                        }
                    },
                    {
                        3,
                        new()
                        {
                            ActiveTime = 30.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 166,
                                    MonsterType = EHeroCardType.Monster_Boss1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = -0.4f,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = -0.3f,
                                    },
                                },
                                new()
                                {
                                    StartIndex = 70,
                                    MonsterType = EHeroCardType.Monster_Boss1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = -0.4f,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = -0.3f,
                                    },
                                },
                            }
                        }
                    },
                },

                WarSeatCount = 12,
                WarSeatRowCount = 6,
                HeroPoolCount = 10,
                WarSeatLength = 1.8f,
                WarSeatInterval = new(0.1f, 0.1f),

                LevelUpdateExpenditure = 2,
                LevelInitGlod = 30,
            }
        },
        {
            EMapLevelType.Level4,
            new()
            {
                GameNewHelpInfo = EGameHelpType.Level4,
                MapWH = new(9, 24),
                MapChunkSize = new Vector2(Mathf.Sqrt(1 - 0.5f * 0.5f) * 2, 2),
                MapChunkInterval = new(0, 0),
                BarrierData = new()
                {
                    {
                        EBarrierType.Massif,
                        new()
                        {
                            new()
                            {
                                Index = new(1, 0)
                            },
                            new()
                            {
                                Index = new(2, 1)
                            },
                            new()
                            {
                                Index = new(7, 0)
                            },
                            new()
                            {
                                Index = new(6, 1)
                            },
                        }
                    }
                },
                EnergyCrystalData = new()
                {
                    {
                        0,
                        new()
                        {
                            StartIndex = 144,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        1,
                        new()
                        {
                            StartIndex = 120,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        2,
                        new()
                        {
                            StartIndex = 96,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        3,
                        new()
                        {
                            StartIndex = 72,
                            Quality = EQualityType.Quality1,
                        }
                    },
                    {
                        4,
                        new()
                        {
                            StartIndex = 48,
                            Quality = EQualityType.Quality1,
                        }
                    },
                },
                MonsterData = new()
                {
                    {
                        0,
                        new()
                        {
                            ActiveTime = 50.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 179,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 131,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 83,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 35,
                                    MonsterType = EHeroCardType.Monster_Default1,
                                },
                                new()
                                {
                                    StartIndex = 180,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 132,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 84,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 36,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                            }
                        }
                    },
                    {
                        1,
                        new()
                        {
                            ActiveTime = 50.0f,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 112,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 136,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 161,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 185,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 88,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 65,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                                new()
                                {
                                    StartIndex = 41,
                                    MonsterType = EHeroCardType.Monster_Default2,
                                },
                            }
                        }
                    },
                    {
                        2,
                        new()
                        {
                            ActiveTime = 50,
                            MonsterList = new()
                            {
                                new()
                                {
                                    StartIndex = 114,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 138,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 163,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 164,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 165,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 141,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 118,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 93,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 69,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 68,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 67,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 90,
                                    MonsterType = EHeroCardType.Monster_Default3,
                                },
                                new()
                                {
                                    StartIndex = 116,
                                    MonsterType = EHeroCardType.Monster_Boss1,
                                    AttributeInfoOffset = new()
                                    {
                                        BloodRatio = 0,
                                        HarmRatio = 0.2f,
                                        AtkSpeedRatio = 0,
                                        DefenceRatio = 0,
                                    },
                                },
                            }
                        }
                    },
                },

                WarSeatCount = 12,
                WarSeatRowCount = 6,
                HeroPoolCount = 10,
                WarSeatLength = 1.8f,
                WarSeatInterval = new(0.1f, 0.1f),

                LevelUpdateExpenditure = 2,
                LevelInitGlod = 70,
            }
        },
    };
    public bool TryGetLevelWaveInfo(EMapLevelType f_MapLevel, out MapLevelInfo f_MapLevelInfo)
    {
        return m_LevelWaveInfos.TryGetValue(f_MapLevel, out f_MapLevelInfo);
    }

    private Dictionary<EAudioType, AudioInfo> m_AudioInfoList = new()
    {
        {
            EAudioType.Hero_Warrior1_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Warrior1_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Warrior1_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Warrior1_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Warrior1_Skill3,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Skill3,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Warrior1_Skill4,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Warrior1_Skill4,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Attack3,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Attack3,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Enchanter1_Skill3,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Enchanter1_Skill3,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Supplementary1_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Supplementary1_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Supplementary1_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Supplementary1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Supplementary1_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Supplementary1_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Hero_Supplementary1_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Hero_Supplementary1_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior1_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior1_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior1_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior2_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior2_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior2_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior2_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior2_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior2_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior2_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior2_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Skill3,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Skill3,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Warrior3_Skill4,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Warrior3_Skill4,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Attack1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Attack1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Attack2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Attack2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill1,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill1,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill2,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill2,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill3,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill3,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill4,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill4,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill5,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill5,
                IsLoop = false,
            }
        },
        {
            EAudioType.Monster_Boss1_Skill6,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Monster_Boss1_Skill6,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_Background,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_Background,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_SelectLevel,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_SelectLevel,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_GameEntrance,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_GameEntrance,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_NextWave,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_NextWave,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_LastWave,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_LastWave,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_BuyCard,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_BuyCard,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_ChangeWarSeat,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_ChangeWarSeat,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_Place,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_Place,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_EnterWarSeat,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_EnterWarSeat,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_ExitWarSeat,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_ExitWarSeat,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_EnterChunk,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_EnterChunk,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_ExitChunk,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_ExitChunk,
                IsLoop = false,
            }
        },
        {
            EAudioType.Scene_GameStart,
            new()
            {
                Name = "",
                AudioAssetKey = EAssetKey.Audio_Scene_GameStart,
                IsLoop = false,
            }
        },

    };
    public bool TryGetAudioInfo(EAudioType f_AudioType, out AudioInfo f_AudioInfo)
    {
        return m_AudioInfoList.TryGetValue(f_AudioType, out f_AudioInfo);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 游戏帮助提示 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EGameHelpType, GameHelpInfo> m_GameHelpInfos = new()
    {
        {
            EGameHelpType.Common,
            new()
            {
                InfoAssetKeyList = new()
                {
                    EAssetKey.Img_Help_Common_1,
                    EAssetKey.Img_Help_Common_2,
                    EAssetKey.Img_Help_Common_3,
                    EAssetKey.Img_Help_Common_4,
                    EAssetKey.Img_Help_Common_5,
                },
            }
        },
        {
            EGameHelpType.Level0,
            new()
            {
                InfoAssetKeyList = new()
                {
                    EAssetKey.Img_Help_Level0_1,
                },
            }
        },
        {
            EGameHelpType.Level1,
            new()
            {
                InfoAssetKeyList = new()
                {
                    EAssetKey.Img_Help_Level1_1,
                },
            }
        },
        {
            EGameHelpType.Level2,
            new()
            {
                InfoAssetKeyList = new()
                {
                    EAssetKey.Img_Help_Level2_1,
                },
            }
        },
        {
            EGameHelpType.Level3,
            new()
            {
                InfoAssetKeyList = new()
                {
                    EAssetKey.Img_Help_Level3_1,
                },
            }
        },
    };
    public bool TryGetGameHelpInfo(EGameHelpType f_GameHelpType, out GameHelpInfo f_GameHelpInfo)
    {
        return m_GameHelpInfos.TryGetValue(f_GameHelpType, out f_GameHelpInfo);
    }
}
