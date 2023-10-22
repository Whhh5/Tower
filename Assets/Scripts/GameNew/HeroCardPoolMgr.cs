using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

public class HeroCardPoolMgr : Singleton<HeroCardPoolMgr>
{
    private int WarSeatCount => GameDataMgr.WarSeatCount;
    private int HeroPoolCount => GameDataMgr.HeroPoolCount;
    private int WarSeatRowCount => GameDataMgr.WarSeatRowCount;
    private float WarSeatLength => GameDataMgr.WarSeatLength;
    public Vector2 WarSeatInterval => GameDataMgr.WarSeatInterval;

    private Transform m_Root = null;
    private Dictionary<int, Entity_ChunkWarSeatData> m_WarSeatList = new();

    private UIHeroCardSelect m_MainWindow = null;
    /// <summary>
    /// 创建卡牌池列表
    /// </summary>
    public async void CreateHeroCardPoolConfig()
    {
        ClearHeroCardPoolConfig();
        // 创建根节点
        if (m_Root == null)
        {
            var root = new GameObject("Root_WarSeat");
            var tran = root.transform;
            tran.SetPositionAndRotation(Vector3.left * 2, Quaternion.Euler(Vector3.zero));
            m_Root = tran;
        }
        // 初始化备战席列表
        for (int i = 0; i < WarSeatCount; i++)
        {
            var row = i % WarSeatRowCount;
            var col = i / WarSeatRowCount;

            var posY = WarSeatLength * 0.5f + row * (WarSeatLength + WarSeatInterval.y);
            var posX = WarSeatLength * 0.5f + col * (WarSeatLength + WarSeatInterval.x);

            var seatData = new Entity_ChunkWarSeatData();
            seatData.SetParent(m_Root);
            seatData.SetPosition(new Vector3(-posX, posY, 0) + m_Root.position);

            m_WarSeatList.Add(i, seatData);
        }

        // 创建 ui 卡池列表
        m_MainWindow = await GTools.UIWindowManager.LoadWindowAsync<UIHeroCardSelect>(EAssetName.UIHeroCardSelect);



        // 开始实例化
        CreateWarSeatEntityAsync();
    }
    private async void CreateWarSeatEntityAsync()
    {
        foreach (var item in m_WarSeatList)
        {
            await ILoadPrefabAsync.LoadAsync(item.Value);
            await UniTask.Delay(100);
        }
    }
    private void ClearWarSeatEntityAsync()
    {
        foreach (var item in m_WarSeatList)
        {
            ILoadPrefabAsync.UnLoad(item.Value);
        }
    }
    public void ClearHeroCardPoolConfig()
    {
        // 销毁 ui 卡池列表

        // 销毁备战席列表
        ClearWarSeatEntityAsync();
        m_WarSeatList.Clear();
    }


    private Dictionary<EQualityType, Dictionary<EHeroCardType, HeroCradPoolInfo>> m_HeroCradPool = new();

    private Dictionary<EHeroCardType, HeroCradPoolInfo> m_CurCrad = new();
    public int CardGroupCount => 5;
    private List<EHeroCardType> m_CurCradList = new();
    public override void Awake()
    {
        m_HeroCradPool.Clear();
        for (int i = 0; i < (int)EHeroCardType.EnumCount; i++)
        {
            var type = (EHeroCardType)i;
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


        InitHeroCradList();
    }

    public bool TryGetCurCardInfo(EHeroCardType f_BuyTarget, out HeroCradPoolInfo f_Result)
    {
        return m_CurCrad.TryGetValue(f_BuyTarget, out f_Result);
    }
    public void UpdateAllCurCardInfo()
    {
        foreach (var item in m_CurCrad)
        {
            //m_MainWindow.UpdateHeroInfo(item.Key);
        }
    }
    /// <summary>
    /// 买卡牌
    /// </summary>
    public bool BuyCard(EHeroCardType f_BuyTarget)
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
                    //m_MainWindow.ClearCardItem(f_BuyTarget);
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
    public void SellCard(EHeroCardType f_CellTarget, int f_Count = 1)
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

            //m_MainWindow.UpdateCardInfo(f_CellTarget);
            //m_MainWindow.UpdateHeroInfo(f_CellTarget);
        }
    }
    /// <summary>
    /// 获取一组卡牌
    /// </summary>
    /// <returns></returns>
    public bool TryGetGroupCrad(out List<EHeroCardType> f_Result)
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



        bool TryGetRangeCrad(out EHeroCardType f_HeroCradType)
        {
            // 筛选当前可用
            Dictionary<EQualityType, List<EHeroCardType>> curCradDic = new();
            foreach (var item in m_HeroCradPool)
            {
                List<EHeroCardType> result = new();
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
            Dictionary<EQualityType, (float tMin, float tMax)> curLevelPro = new();

            foreach (var item in curCradDic)
            {
                var range = playerLevelInfo.GetRangeCradProbability(item.Key);
                curLevelPro.Add(item.Key, range);
            }

            for (int i = 0; i < (int)EQualityType.EnumCount; i++)
            {
                var level = (EQualityType)i;
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

            // 根据当前等级筛选
            var curValue = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
            EHeroCardType? resultOne = null;
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
            f_HeroCradType = resultOne ?? EHeroCardType.EnumCount;
            return resultOne != null;
        }

    }
    /// <summary>
    /// 回收卡牌
    /// </summary>
    public void RecycleGroupCrad(EHeroCardType f_EHeroCradType, int f_Count = 1)
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


    private Dictionary<EQualityType, ListStack<EHeroCardType>> m_HeroCardList = new();
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

    public bool TryGetRandomCardByLevel(EQualityType f_Level, out EHeroCardType f_HerpType)
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

    public bool TryGetCardGroup(out List<EHeroCardType> f_Result, int f_Count = 5, EPlayerLevel? f_PlayerLevel = null)
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
                    var qualityLevel = EQualityType.Quality1;
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
public class HeroCradPoolInfo
{
    public HeroCradPoolInfo(EHeroCardType f_EHeroCradType, int? f_Count = null)
    {
        Type = f_EHeroCradType;
        ResidueCount = f_Count ?? LevelInfo.MaxCount;
    }
    public EHeroCardType Type { get; private set; }
    public int ResidueCount { get; private set; }

    public EQualityType Level => CradInfo.QualityLevel;
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