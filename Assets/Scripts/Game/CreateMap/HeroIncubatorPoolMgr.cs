using B1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IncubatorCardInfo : ICardGroupData
{
    public EHeroQualityLevel QualityLevel;
}
public class HeroCardInfo : ICardGroupData
{
    public EHeroCardType HeroType;
}
public class HeroCradPoolInfo
{
    public HeroCradPoolInfo(EHeroCardType f_EHeroCradType, int? f_Count = null)
    {
        Type = f_EHeroCradType;
        ResidueCount = f_Count ?? LevelInfo.MaxCount;
    }
    public EHeroCardType Type { get; private set; }
    public int ResidueCount { get; private set; }

    public EHeroQualityLevel Level => CradInfo.QualityLevel;
    public HeroCradLevelInfo LevelInfo => TableMgr.Ins.TryGetHeroCradLevelInfo(Level, out var levelInfo) ? levelInfo : null;
    public HeroCradInfo CradInfo => TableMgr.Ins.TryGetHeroCradInfo(Type, out var cradInfo) ? cradInfo : null;


    public void Push(int f_Count = 1)
    {
        ResidueCount = Mathf.Clamp(ResidueCount + f_Count, 0, LevelInfo.MaxCount);
    }
    public void Pop(int f_Count = 1)
    {
        ResidueCount = Mathf.Clamp(ResidueCount - f_Count, 0, LevelInfo.MaxCount);
    }
    public EHeroCradStarLevel StarLevel()
    {
        EHeroCradStarLevel result = EHeroCradStarLevel.Level1;
        var max = (int)EHeroCradStarLevel.EnumCount;
        for (int i = max - 1; i > 0; i--)
        {
            if (ResidueCount >= Mathf.Pow(3, i - 1))
            {
                result = (EHeroCradStarLevel)i;
                break;
            }
        }
        return result;
    }
}
public class HeroIncubatorPoolMgr : Singleton<HeroIncubatorPoolMgr>
{
    private Dictionary<EHeroQualityLevel, ListStack<EHeroCardType>> m_HeroCardList = new();
    public override void Awake()
    {
        InitHeroCradList();
    }


    private void InitHeroCradList()
    {
        GTools.TableMgr.LoopHeroCradInfo((type, info) =>
        {
            if (!m_HeroCardList.TryGetValue(info.QualityLevel, out var list))
            {
                list = new("", 20);
                m_HeroCardList.Add(info.QualityLevel, list);
            }
            list.Push(type);
        });
    }
    /// <summary>
    /// 买孵化器
    /// </summary>
    public bool BuyIncubator<T>(T f_Quality)
        where T : IExpenditure
    {
        if (GTools.PlayerMgr.TryExpenditure(f_Quality.Expenditure))
        {
            return true;
        }
        else
        {
            GTools.FloaterHint("<color=#FF0000FF>金币不足!!!</color>");
        }
        return false;
    }

    /// <summary>
    /// 回收孵化器
    /// </summary>
    public void RecycleGroupCrad(EHeroQualityLevel f_Quality, int f_Count = 1)
    {

    }


    /// <summary>
    /// 根据品质随机获取一个英雄
    /// </summary>
    /// <returns></returns>
    public bool TryGetRandomCardByLevel(EHeroQualityLevel f_Level, out EHeroCardType f_HerpType)
    {
        f_HerpType = EHeroCardType.EnumCount;
        if (m_HeroCardList.TryGetValue(f_Level, out var list))
        {
            var index = GTools.MathfMgr.GetRandomValue(0, list.Count - 1);
            var heroType = list[index];
            f_HerpType = heroType;
            return true;
        }

        return false;
    }


    /// <summary>
    /// 根据玩家等级随机获取一组孵化器
    /// </summary>
    /// <returns></returns>
    public bool TryGetIncybatorGroup(out List<ICardGroupData> f_Result, int f_Count = GTools.CardGroupCount, EPlayerLevel? f_PlayerLevel = null)
    {
        var count = f_Count;
        var result = f_Result = new();
        var playerLevel = f_PlayerLevel ?? GTools.PlayerMgr.Level;

        if (GTools.TableMgr.TryGetPlayerLevelInfo(playerLevel, out var playerInfo))
        {
            if (playerInfo.GetQualityRandom(out var qualityInfo))
            {
                for (int i = 0; i < count; i++)
                {
                    // 获取品质等级
                    var qualityPro = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
                    foreach (var item in qualityInfo)
                    {
                        if (qualityPro < item.Value.tMin || qualityPro > item.Value.tMax)
                        {
                            continue;
                        }
                        var type = GTools.MathfMgr.GetRandomValue(0, 1);
                        if (type > 0.5)
                        {
                            result.Add(new IncubatorCardInfo()
                            {
                                QualityLevel = item.Key,
                            });
                        }
                        else if(TryGetRandomCardByLevel(item.Key, out var heroType))
                        {
                            result.Add(new HeroCardInfo()
                            {
                                QualityLevel = heroType,
                            });
                        }
                        break;
                    }

                }

            }
        }
        return result.Count > 0;
    }





}