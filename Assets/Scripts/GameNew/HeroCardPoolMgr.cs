using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

public class HeroCardPoolMgr : Singleton<HeroCardPoolMgr>, IUpdateBase
{

    //===============================----------------------========================================
    //=====-----                                                                         -----=====
    //                                catalogue -- 英雄实体
    //=====-----                                                                         -----=====
    //===============================----------------------========================================
    public Entity_HeroBaseNewData CreateHeroCardEntity(
        EHeroCardType f_HeroType,
        EHeroCradStarLevel f_StarLevel = EHeroCradStarLevel.Level1,
        int f_TargetIndex = -1)
    {
        if (!GTools.TableMgr.TryGetHeroBaseNewData(f_HeroType, -1, f_StarLevel, out var heroData))
        {
            LogError($"获取英雄实体失败 type = {f_HeroType}, level = {f_StarLevel}");
            return null;
        }
        heroData.InitData(f_TargetIndex);
        return heroData;
    }
    public void RemoveHeroCardData(Entity_HeroBaseData f_HeroData)
    {
        GTools.CreateMapNew.ClearChunkElement(f_HeroData);
        ILoadPrefabAsync.UnLoad(f_HeroData);
    }
    public Entity_MonsterBaseNewData CreateMonsterEntity(
        EHeroCardType f_MonsterType,
        IncubatorAttributeInfo? f_AttributeOffset = null,
        EHeroCradStarLevel f_StarLevel = EHeroCradStarLevel.Level1,
        int f_TargetIndex = -1)
    {
        if (!GTools.TableMgr.TryGetMonsterBaseNewData(f_MonsterType, -1, f_StarLevel, out var monsterData))
        {
            LogError($"获取怪物实体失败 type = {f_MonsterType}, level = {f_StarLevel}");
            return null;
        }
        if (f_TargetIndex >= 0 && GTools.CreateMapNew.TryGetChunkData(f_TargetIndex, out var _))
        {
            monsterData.UpdateAddAttributes(f_AttributeOffset ?? new());
            monsterData.InitData(f_TargetIndex);
        }
        return monsterData;
    }



    //===============================----------------------========================================
    //=====-----                                                                         -----=====
    //                                catalogue -- 备战席
    //=====-----                                                                         -----=====
    //===============================----------------------========================================
    private int WarSeatCount => GameDataMgr.WarSeatCount;
    private int HeroPoolCount => GameDataMgr.HeroPoolCount;
    private int WarSeatRowCount => GameDataMgr.WarSeatRowCount;
    private float WarSeatLength => GameDataMgr.WarSeatLength;
    public Vector2 WarSeatInterval => GameDataMgr.WarSeatInterval;

    public int UpdateLevelID { get; set; }

    public EUpdateLevel UpdateLevel => EUpdateLevel.Level2;

    public float LasteUpdateTime { get; set; }
    public float UpdateDelta { get; set; }

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
        await GTools.UIWindowManager.LoadWindowAsync<UIGameMonster>(EAssetName.UIGameMonster);
        await GTools.UIWindowManager.LoadWindowAsync<UIGameFinish>(EAssetName.UIGameFinish);
        await GTools.UIWindowManager.LoadWindowAsync<UIDrawWindow>(EAssetName.UIDrawWindow);



        // 开始实例化
        CreateWarSeatEntityAsync();
    }
    private async void CreateWarSeatEntityAsync()
    {
        foreach (var item in m_WarSeatList)
        {
            await ILoadPrefabAsync.LoadAsync(item.Value);
            await UniTask.Delay(10);
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
    private async void AddWarSeatList(EHeroCardType f_HeroCard)
    {
        var heroData = CreateHeroCardEntity(f_HeroCard, EHeroCradStarLevel.Level1, -1);
        if (TryGetWarSeatItem(out var chunkData))
        {
            chunkData.SetCurHeroCard(heroData);
            heroData.SetPosition(chunkData.PointUp);
        }
        await ILoadPrefabAsync.LoadAsync(heroData);
    }
    private void RemoveWarSearList(Entity_ChunkWarSeatData f_WarSeatData)
    {
        f_WarSeatData.ClearHeroCard();
    }
    private bool TryGetWarSeatItem(out Entity_ChunkWarSeatData f_WarSeatData)
    {
        f_WarSeatData = null;
        foreach (var item in m_WarSeatList)
        {
            if (item.Value.TryGetHeroData(out var heroData))
            {
                continue;
            }
            f_WarSeatData = item.Value;
            break;
        }
        return f_WarSeatData != null;
    }

    //===============================----------------------========================================
    //=====-----                                                                         -----=====
    //                                catalogue -- 英雄抽卡
    //=====-----                                                                         -----=====
    //===============================----------------------========================================
    // 全部卡牌数量信息
    private Dictionary<EQualityType, Dictionary<EHeroCardType, HeroCradPoolInfo>> m_HeroCradPool = new();

    // 已经购买的卡牌
    private Dictionary<EHeroCardType, HeroCradPoolInfo> m_CurCradList = new();
    // 当前在刷新列表中，但是未购买的卡牌
    private Dictionary<EHeroCardType, int> m_CurCradViewList = new();
    public int GetHeroCardResidueCount(EHeroCardType f_HeroType)
    {
        var count1 = GetHeroCardPoolCount(f_HeroType);
        var count2 = GetHeroCardListCount(f_HeroType);
        var count = count1 + count2;
        return count;
    }
    public int GetHeroCardPoolCount(EHeroCardType f_HeroType)
    {
        if (!GTools.TableMgr.TryGetHeroCradInfo(f_HeroType, out var heroInfo))
        {
            return 0;
        }
        if (!m_HeroCradPool.TryGetValue(heroInfo.QualityLevel, out var list))
        {
            return 0;
        }
        if (!list.TryGetValue(f_HeroType, out var cardPoolData))
        {
            return 0;
        }
        return cardPoolData.ResidueCount;
    }
    private void AddCurCardViewListData(EHeroCardType f_HeroType, int f_Count = 1)
    {
        if (!m_CurCradViewList.TryGetValue(f_HeroType, out var count))
        {
            count = 0;
            m_CurCradViewList.Add(f_HeroType, count);
        }
        m_CurCradViewList[f_HeroType] = count + f_Count;
    }
    private bool RemoveCurCardViewListData(EHeroCardType f_HeroType, int f_Count = 1)
    {
        if (!m_CurCradViewList.TryGetValue(f_HeroType, out var count))
        {
            return false;
        }
        if (count > f_Count)
        {
            m_CurCradViewList[f_HeroType] = count - f_Count;
        }
        else
        {
            m_CurCradViewList.Remove(f_HeroType);
        }
        return true;
    }
    public int GetHeroCardListCount(EHeroCardType f_HeroType)
    {
        if (!m_CurCradViewList.TryGetValue(f_HeroType, out var count))
        {
            return 0;
        }
        return count;
    }
    public override void Awake()
    {
       //m_HeroCradPool.Clear();
       // for (int i = 0; i < (int)EHeroCardType.EnumCount; i++)
       // {
       //     var type = (EHeroCardType)i;
       //     if (TableMgr.Ins.TryGetHeroCradInfo(type, out var info))
       //     {
       //         if (!m_HeroCradPool.TryGetValue(info.QualityLevel, out var value))
       //         {
       //             value = new();
       //             m_HeroCradPool.Add(info.QualityLevel, value);
       //         }

       //         var poolInfo = new HeroCradPoolInfo(type);
       //         value.Add(type, poolInfo);
       //     }
       // } 


        InitHeroCradList();
    }
    public void InitCardPoolList()
    {
        m_CurCradList.Clear();
        m_CurCradViewList.Clear();
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
    }

    public bool TryGetCurCardInfo(EHeroCardType f_BuyTarget, out HeroCradPoolInfo f_Result)
    {
        return m_CurCradList.TryGetValue(f_BuyTarget, out f_Result);
    }
    public void UpdateAllCurCardInfo()
    {
        foreach (var item in m_CurCradList)
        {
            //m_MainWindow.UpdateHeroInfo(item.Key);
        }
    }
    /// <summary>
    /// 买卡牌
    /// </summary>
    public bool BuyCard(EHeroCardType f_BuyTarget)
    {
        if (!GTools.TableMgr.TryGetHeroCradInfo(f_BuyTarget, out var cardInfo))
        {
            return false;
        }
        if (!GTools.PlayerMgr.TryExpenditure(cardInfo.QualityLevelInfo.Expenditure))
        {
            return false;
        }
        if (!m_CurCradList.TryGetValue(f_BuyTarget, out var value))
        {
            value = new(f_BuyTarget, 0);
            m_CurCradList.Add(f_BuyTarget, value);
        }
        if (!RemoveCurCardViewListData(f_BuyTarget))
        {
            return false;
        }
        AddWarSeatList(f_BuyTarget);
        return true;
    }
    /// <summary>
    /// 卖卡牌
    /// </summary>
    public void SellCard(EHeroCardType f_CellTarget, int f_Count = 1)
    {
        if (m_CurCradList.TryGetValue(f_CellTarget, out var value))
        {
            var recycleCount = Mathf.Min(value.ResidueCount, f_Count);
            RecycleGroupCrad(f_CellTarget, recycleCount);

            value.Pop(recycleCount);
            if (value.ResidueCount <= 0)
            {
                m_CurCradList.Remove(f_CellTarget);
            }

            //m_MainWindow.UpdateCardInfo(f_CellTarget);
            //m_MainWindow.UpdateHeroInfo(f_CellTarget);
        }
    }
    /// <summary>
    /// 获取一组卡牌
    /// </summary>
    /// <returns></returns>
    public bool TryGetGroupCrad(out List<EHeroCardType> f_Result, int? f_Count = null)
    {
        var getCount = f_Count ?? GameDataMgr.HeroPoolCount;
        // 想要获取的卡牌数量
        var playerLevel = EPlayerLevel.Level1;
        f_Result = null;
        if (!GTools.TableMgr.TryGetPlayerLevelInfo(playerLevel, out var playerLevelInfo))
        {
            return false;
        }

        //foreach (var item in m_CurCradList)
        //{
        //    RecycleGroupCrad(item);
        //}

        // 拿取固定数量
        f_Result = new();
        for (int i = 0; i < getCount; i++)
        {
            if (!TryGetRangeCrad(out var target))
            {
                continue;
            }
            f_Result.Add(target);

            if (!m_CurCradViewList.TryGetValue(target, out var count))
            {
                count = 0;
                m_CurCradViewList.Add(target, count);
            }
            m_CurCradViewList[target] = count + 1;
        }
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
        if (!TableMgr.Ins.TryGetHeroCradInfo(f_EHeroCradType, out var heroCardInfo))
        {
            return;
        }
        if (!m_HeroCradPool.TryGetValue(heroCardInfo.QualityLevel, out var poolInfo))
        {
            return;
        }
        if (!poolInfo.TryGetValue(f_EHeroCradType, out var cradPoolInfo))
        {
            return;
        }
        cradPoolInfo.Push(f_Count);
        RemoveCurCardViewListData(f_EHeroCradType, f_Count);
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
    public int TryGetGroupSkill(out List<CardGroupInfos> f_Result)
    {
        f_Result = new();
        if (!ILoadPrefabAsync.TryGetEntityByType<Entity_HeroBaseData>(EWorldObjectType.Preson, out var listData))
        {
            return 0;
        }
        if (HeroMgr.Ins.TryGetSkillCards(out var skillList, GameDataMgr.CardSkillCount))
        {
            foreach (var skillItem in skillList)
            {
                var probaility = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
                if (probaility > GameDataMgr.CardSkillProbability)
                {
                    continue;
                }
                var cardInfo = new CardGroupInfos()
                {
                    CardType = ECardType.Skill,
                    TypeData = skillItem,
                };
                f_Result.Add(cardInfo);
            }
        }
        return f_Result.Count;
    }

    public bool TryGetGroup(out List<CardGroupInfos> f_Result)
    {
        var count = TryGetGroupSkill(out f_Result);
        var heroCardCount = GameDataMgr.HeroPoolCount - count;
        if (TryGetGroupCrad(out var heroCard, heroCardCount))
        {
            foreach (var item in heroCard)
            {
                CardGroupInfos cardGroupInfo = new()
                {
                    CardType = ECardType.HeroPerson,
                    TypeData = new HeroCardInfo()
                    {
                        HeroType = item,
                    },
                };
                f_Result.Add(cardGroupInfo);
            }
        }
        return f_Result.Count > 0;
    }
    public bool TryGetHeroCrad(out CardGroupInfos f_Result)
    {
        f_Result = null;
        if (TryGetGroupCrad(out var heroCard, 1))
        {
            CardGroupInfos cardGroupInfo = new()
            {
                CardType = ECardType.HeroPerson,
                TypeData = new HeroCardInfo()
                {
                    HeroType = heroCard[0],
                },
            };
            f_Result = cardGroupInfo;
        }
        return f_Result != null;
    }

    //===============================----------------------========================================
    //=====-----                                                                         -----=====
    //                                catalogue -- 备战席移动
    //=====-----                                                                         -----=====
    //===============================----------------------========================================
    private Entity_ChunkWarSeatData m_CurEnterWarSeat = null;
    public void EnterWarSeat(Entity_ChunkWarSeatData f_WarSeat)
    {
        if (!f_WarSeat.TryGetHeroData(out var heroData))
        {
            return;
        }
        m_CurEnterWarSeat = f_WarSeat;
    }
    public void ExitWarSeat(Entity_ChunkWarSeatData f_WarSeat)
    {
        if (m_CurEnterWarSeat == f_WarSeat)
        {
            m_CurEnterWarSeat = null;
        }
    }
    public bool PathPointUpClick(Entity_ChunkMapData f_ChunkData)
    {
        var data = m_CurEnterWarSeat;
        if (data == null)
        {
            return false;
        }
        m_CurEnterWarSeat = null;
        if (!data.TryGetHeroData(out var heroData))
        {
            return false;
        }
        if (!f_ChunkData.IsPass())
        {
            data.ResetHeroPosition();
            return false;
        }
        heroData.SetObjBehaviorStatus(true);
        heroData.MoveToChunk(f_ChunkData.ChunkIndex);
        heroData.SetPosition(f_ChunkData.WorldPosition);
        GTools.FormationMgr.FormationDetection(heroData);
        data.ClearHeroCard();
        return true;
    }
    public bool WarSeatUpClick(Entity_ChunkWarSeatData f_ChunkData)
    {
        var data = m_CurEnterWarSeat;
        if (data == null)
        {
            return false;
        }
        m_CurEnterWarSeat = null;
        if (f_ChunkData == data)
        {
            data.ResetHeroPosition();
            return false;
        }
        if (!data.TryGetHeroData(out var targetHerpData))
        {
            return false;
        }
        if (f_ChunkData.TryGetHeroData(out var curHeroData))
        {
            data.SetCurHeroCard(curHeroData);
            curHeroData.SetPosition(data.WorldPosition);
        }
        else
        {
            data.ClearHeroCard();
        }
        f_ChunkData.SetCurHeroCard(targetHerpData);
        targetHerpData.SetPosition(f_ChunkData.WorldPosition);
        return true;
    }
    public void OnUpdate()
    {
        if (m_CurEnterWarSeat == null)
        {
            return;
        }
        if (m_CurEnterWarSeat.TryGetHeroData(out var heroData))
        {

        }
    }
}
public enum EHeroCardWarSeatType
{
    WarSeat,
    PathPoint,
}
public class CardWarSeatData
{
    public EHeroCardWarSeatType ChunkType;
    public UnityObjectData TargetChunk; // Entity_HeroBaseData
    public T GetTargetData<T>()
        where T : UnityObjectData
    {
        return TargetChunk as T;
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