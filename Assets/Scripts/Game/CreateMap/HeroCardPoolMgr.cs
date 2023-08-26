using B1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HeroCradPoolInfo
{
    public HeroCradPoolInfo(EHeroCradType f_EHeroCradType, int? f_Count = null)
    {
        Type = f_EHeroCradType;
        ResidueCount = f_Count ?? LevelInfo.MaxCount;
    }
    public EHeroCradType Type { get; private set; }
    public int ResidueCount { get; private set; }

    public EHeroQualityLevel Level => CradInfo.QualityLevel;
    public HeroCradLevelInfo LevelInfo => TableMgr.Ins.TryGetHeroQualityInfo(Level, out var levelInfo) ? levelInfo : null;
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
public class HeroCardPoolMgr : Singleton<HeroCardPoolMgr>
{
    private Dictionary<EHeroQualityLevel, Dictionary<EHeroCradType, HeroCradPoolInfo>> m_HeroCradPool = new();

    private Dictionary<EHeroCradType, HeroCradPoolInfo> m_CurCrad = new();
    public int CardGroupCount => 5;
    private List<EHeroCradType> m_CurCradList = new();
    public override void Initialization()
    {
        m_HeroCradPool.Clear();
        for (int i = 0; i < (int)EHeroCradType.EnemyCount; i++)
        {
            var type = (EHeroCradType)i;
            if (TableMgr.Ins.TryGetHeroCradInfo(type, out var info))
            {
                if (!m_HeroCradPool.TryGetValue(info.QualityLevel, out var value))
                {
                    value = new();
                    m_HeroCradPool.Add(info.QualityLevel, value);
                }

                var poolInfo = new HeroCradPoolInfo(type);
                value.Add(type, poolInfo);
            }
        }

        m_MainWindow = GameObject.FindObjectOfType<UIBattleMainWindow>();

        InitHeroCradList();
    }

    private UIBattleMainWindow m_MainWindow = null;
    public bool TryGetCurCardInfo(EHeroCradType f_BuyTarget, out HeroCradPoolInfo f_Result)
    {
        return m_CurCrad.TryGetValue(f_BuyTarget, out f_Result);
    }
    public void UpdateAllCurCardInfo()
    {
        foreach (var item in m_CurCrad)
        {
            m_MainWindow.UpdateCardInfo(item.Key);
            m_MainWindow.UpdateHeroInfo(item.Key);
        }
    }
    /// <summary>
    /// 买卡牌
    /// </summary>
    public bool BuyCard(EHeroCradType f_BuyTarget)
    {
        if (GTools.TableMgr.TryGetHeroCradInfo(f_BuyTarget, out var cardInfo))
        {
            if (GTools.PlayerMgr.TryExpenditure(cardInfo.QualityLevelInfo.Expenditure))
            {
                if (m_CurCradList.Contains(f_BuyTarget))
                {
                    if (!m_CurCrad.TryGetValue(f_BuyTarget, out var value))
                    {
                        value = new(f_BuyTarget, 0);
                        m_CurCrad.Add(f_BuyTarget, value);
                    }
                    value.Push();

                    m_CurCradList.Remove(f_BuyTarget);
                    m_MainWindow.UpdateCardInfo(f_BuyTarget);
                    m_MainWindow.UpdateHeroInfo(f_BuyTarget);
                    return true;
                }
            }
            else
            {
                GTools.FloaterHint("<color=#FF0000FF>金币不足!!!</color>");
            }
        }
        return false;
    }
    /// <summary>
    /// 卖卡牌
    /// </summary>
    public void SellCard(EHeroCradType f_CellTarget, int f_Count = 1)
    {
        if (m_CurCrad.TryGetValue(f_CellTarget, out var value))
        {
            var recycleCount = Mathf.Min(value.ResidueCount, f_Count);
            RecycleGroupCrad(f_CellTarget, recycleCount);

            value.Pop(recycleCount);
            if (value.ResidueCount <= 0)
            {
                m_CurCrad.Remove(f_CellTarget);
            }

            m_MainWindow.UpdateCardInfo(f_CellTarget);
            m_MainWindow.UpdateHeroInfo(f_CellTarget);
        }
    }
    /// <summary>
    /// 获取一组卡牌
    /// </summary>
    /// <returns></returns>
    public bool TryGetGroupCrad(out List<EHeroCradType> f_Result)
    {
        // 想要获取的卡牌数量
        var playerLevel = EPlayerLevel.Level1;
        f_Result = null;
        if (!GTools.TableMgr.TryGetPlayerLevelInfo(playerLevel, out var playerLevelInfo))
        {
            return false;
        }

        foreach (var item in m_CurCradList)
        {
            RecycleGroupCrad(item);
        }

        // 拿取固定数量
        f_Result = new();
        for (int i = 0; i < CardGroupCount; i++)
        {
            if (TryGetRangeCrad(out var target))
            {
                f_Result.Add(target);
            }
        }
        m_CurCradList = f_Result;
        return f_Result.Count > 0;



        bool TryGetRangeCrad(out EHeroCradType f_HeroCradType)
        {
            // 筛选当前可用
            Dictionary<EHeroQualityLevel, List<EHeroCradType>> curCradDic = new();
            foreach (var item in m_HeroCradPool)
            {
                List<EHeroCradType> result = new();
                foreach (var cradPool in item.Value)
                {
                    for (int i = 0; i < cradPool.Value.ResidueCount; i++)
                    {
                        result.Add(cradPool.Key);
                    }
                }
                if (result.Count > 0)
                {
                    curCradDic.Add(item.Key, result);
                }
            }




            // 概率范围
            Dictionary<EHeroQualityLevel, (float tMin, float tMax)> curLevelPro = new();

            foreach (var item in curCradDic)
            {
                var range = playerLevelInfo.GetRangeCradProbability(item.Key);
                curLevelPro.Add(item.Key, range);
            }

            for (int i = 0; i < (int)EHeroQualityLevel.EnumCount; i++)
            {
                var level = (EHeroQualityLevel)i;
                if (curLevelPro.ContainsKey(level))
                {
                    continue;
                }
                var range = playerLevelInfo.GetRangeCradProbability(level);
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

            // 根据当前等级筛选
            var curValue = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
            EHeroCradType? resultOne = null;
            foreach (var item in curLevelPro)
            {
                if (curValue >= item.Value.tMin && curValue <= item.Value.tMax)
                {
                    var range = GTools.MathfMgr.GetRandomValue(0, curCradDic[item.Key].Count - 1);
                    var value = curCradDic[item.Key][range];
                    resultOne = value;
                    m_HeroCradPool[item.Key][value].Pop();
                    break;
                }
            }
            f_HeroCradType = resultOne ?? EHeroCradType.EnemyCount;
            return resultOne != null;
        }

    }
    /// <summary>
    /// 回收卡牌
    /// </summary>
    public void RecycleGroupCrad(EHeroCradType f_EHeroCradType, int f_Count = 1)
    {
        if (TableMgr.Ins.TryGetHeroCradInfo(f_EHeroCradType, out var heroCardInfo))
        {
            if (m_HeroCradPool.TryGetValue(heroCardInfo.QualityLevel, out var poolInfo))
            {
                if (poolInfo.TryGetValue(f_EHeroCradType, out var cradPoolInfo))
                {
                    cradPoolInfo.Push(f_Count);
                }
            }
        }
    }


    private Dictionary<EHeroQualityLevel, ListStack<EHeroCradType>> m_HeroCardList = new();
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

    public bool TryGetRandomCardByLevel(EHeroQualityLevel f_Level, out EHeroCradType f_HerpType)
    {
        f_HerpType = EHeroCradType.EnemyCount;
        if (m_HeroCardList.TryGetValue(f_Level, out var list))
        {
            var index = GTools.MathfMgr.GetRandomValue(0, list.Count - 1);
            var heroType = list[index];
            f_HerpType = heroType;
            return true;
        }

        return false;
    }

    public bool TryGetCardGroup(out List<EHeroCradType> f_Result, int f_Count = 5, EPlayerLevel? f_PlayerLevel = null)
    {
        var result = f_Result = new();
        var playerLevel = f_PlayerLevel ?? GTools.PlayerMgr.Level;

        if (GTools.TableMgr.TryGetPlayerLevelInfo(playerLevel, out var playerInfo))
        {
            if (playerInfo.GetQualityRandom(out var qualityInfo))
            {
                for (int i = 0; i < f_Count; i++)
                {
                    // 获取品质等级
                    var qualityPro = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
                    var qualityLevel = EHeroQualityLevel.Level1;
                    foreach (var item in qualityInfo)
                    {
                        if (qualityPro < item.Value.tMin || qualityPro > item.Value.tMax)
                        {
                            continue;
                        }
                        qualityLevel = item.Key;
                        break;
                    }

                    // 获取随机英雄碎片
                    if (TryGetRandomCardByLevel(qualityLevel, out var heroType))
                    {
                        result.Add(heroType);
                    }

                }

            }
        }
        return result.Count > 0;
    }





}