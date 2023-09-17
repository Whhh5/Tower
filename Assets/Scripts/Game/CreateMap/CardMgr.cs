using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;


public interface ICardGroupData
{

}
public class CardGroupInfos
{
    public ECardType CardType;
    public ICardGroupData TypeData;
    public T GetTargetType<T>()
        where T : ICardGroupData
    {
        return (T)TypeData;
    }
}
public class CardMgr : Singleton<CardMgr>
{

    public bool TryGetCardGroup(out List<CardGroupInfos> f_Result)
    {
        f_Result = new();
        if (HeroMgr.Ins.TryGetSkillCards(out var skillList))
        {
            foreach (var skillItem in skillList)
            {
                var probaility = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f);
                if (probaility > GTools.AppearSkillCardProbability)
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
        var incubatorCount = GTools.CardGroupCount - f_Result.Count;
        if (GTools.HeroIncubatorPoolMgr.TryGetIncybatorGroup(out var incubatorList, incubatorCount))
        {
            foreach (var incubatorItem in incubatorList)
            {
                var cardInfo = new CardGroupInfos()
                {
                    CardType = ECardType.Incubator,
                    TypeData = incubatorItem,
                };
                f_Result.Add(cardInfo);
            }
        }
        return f_Result.Count > 0;
    }
}
