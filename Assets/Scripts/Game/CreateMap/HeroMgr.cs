using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class EventData_CreateHero : EventSystemParamData
{
    public Entity_HeroBaseData HeroData = null;

}
public class EventData_DestroyHero : EventSystemParamData
{
    public Entity_HeroBaseData HeroData = null;

}
public class HeroAddSKillEventData : EventSystemParamData
{
    public Entity_HeroBaseData HeroData;
    public EPersonSkillType SkillType;
}
public class SkillCardInfo : ICardGroupData
{
    public EPersonSkillType SkillType;
    public Entity_HeroBaseData HeroData;
}
public class HeroMgr : Singleton<HeroMgr>
{
    private Dictionary<EEntityType, Dictionary<int, Entity_HeroBaseData>> m_CurScenesHeroList = new();

    public bool CreateHero(EHeroCardType f_HeroType, EHeroCradStarLevel f_HeroLevel, int f_ChunkIndex, out Entity_HeroBaseData f_HeroBaseData, IncubatorAttributeInfo f_AddAttribute)
    {
        if (TableMgr.Ins.GetHeroDataByType(f_HeroType, f_ChunkIndex, f_HeroLevel, out f_HeroBaseData))
        {
            f_HeroBaseData.UpdateAddAttributes(f_AddAttribute);
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(f_HeroBaseData));
            EventData_CreateHero eventData = new()
            {
                HeroData = f_HeroBaseData,
            };
            AddSceneHero(f_HeroBaseData);
            GTools.EventSystemMgr.SendEvent(EEventSystemType.CreateHero, eventData);
            return true;
        }
        return false;
    }
    public void DestroyHero(Entity_HeroBaseData f_HeroBaseData)
    {
        var eventData = new EventData_DestroyHero()
        {
            HeroData = f_HeroBaseData,
        };
        RemoveSceneHero(f_HeroBaseData);
        GTools.EventSystemMgr.SendEvent(EEventSystemType.DestroyHero, eventData);
    }
    public void AddSceneHero(Entity_HeroBaseData f_HeroData)
    {
        if (!m_CurScenesHeroList.TryGetValue(f_HeroData.EntityType, out var list))
        {
            list = new();
            m_CurScenesHeroList.Add(f_HeroData.EntityType, list);
        }
        list.Add(f_HeroData.LoadKey, f_HeroData);
    }
    public void RemoveSceneHero(Entity_HeroBaseData f_HeroData)
    {
        if (m_CurScenesHeroList.TryGetValue(f_HeroData.EntityType, out var list))
        {
            if (list.TryGetValue(f_HeroData.LoadKey, out var value))
            {
                list.Remove(f_HeroData.LoadKey);
                if (list.Count == 0)
                {
                    m_CurScenesHeroList.Remove(f_HeroData.EntityType);
                }
            }
        }
    }
    public ResultData<string> AddSkillHero(Entity_HeroBaseData f_HeroData, EPersonSkillType f_SKillType)
    {
        var result = new ResultData<string>();
        if (GTools.TableMgr.TryGetHeroCradInfo(f_HeroData.HeroCradType, out var heroInfo))
        {
            var curSkillCount = f_HeroData.GetCurSkillCount();
            if (curSkillCount < heroInfo.SkillLinkInfos.Count)
            {
                f_HeroData.AddNextSkill(f_SKillType);
                Log($"技能 添加成功 heroType = {f_HeroData.HeroCradType}, skill = {f_SKillType}");
                result.SetData($"添加成功 heroType = {f_HeroData.HeroCradType}, skill = {f_SKillType}");
                HeroAddSKillEventData eventData = new()
                {
                    HeroData = f_HeroData,
                    SkillType = f_SKillType,
                };
                GTools.EventSystemMgr.SendEvent(EEventSystemType.Hero_Skill_Add, eventData);
            }
            else
            {
                result.SetData("技能数量达到上限", EResult.Defeated);
            }
        }
        else
        {
            result.SetData($"未获取到英雄配置信息 heroType = {f_HeroData.HeroCradType}", EResult.Defeated);
        }
        return result;
    }
    public bool TryGetSkillCards(out List<SkillCardInfo> f_Result, int f_Count = 5/*GTools.CardSkillGroupCount*/)
    {
        // 可以刷出技能的英雄
        f_Result = new();

        if (!m_CurScenesHeroList.TryGetValue(EEntityType.Person, out var heroList))
        {
            return false;
        }

        List<SkillCardInfo> list = new();
        foreach (var item in heroList)
        {
            var data = item.Value;
            if (!GTools.TableMgr.TryGetHeroCradInfo(data.HeroCradType, out var heroInfo))
            {
                continue;
            }
            var curSkillLink = data.GetCurSkillLink();
            var curSkillCount = curSkillLink.Count;
            if (curSkillCount >= heroInfo.SkillLinkInfos.Count)
            {
                continue;
            }
            var skillOriginal = heroInfo.SkillLinkInfos.SkillLink;
            List<SkillLink> targetList = null;
            int loopIndex = -1;
            LoopContext(skillOriginal);
            void LoopContext(List<SkillLink> list)
            {
                if (++loopIndex == curSkillCount)
                {
                    targetList = list;
                    return;
                }
                if (targetList != null)
                {
                    return;
                }
                while (list != null)
                {
                    foreach (var skill in list)
                    {
                        if (skill.SkillType != curSkillLink[loopIndex])
                        {
                            continue;
                        }
                        LoopContext(skill.NextStageSkills);
                        return;
                    }
                }
            }
            if (targetList != null)
            {
                var targetIndex = GTools.MathfMgr.GetRandomValue(0, targetList.Count - 1);
                var skill = targetList[targetIndex];

                var index = GTools.MathfMgr.GetRandomValue(0, list.Count - 1);
                list.Insert(index, new()
                {
                    HeroData = data,
                    SkillType = skill.SkillType,
                });
            }
        }

        while (f_Count-- > 0 && list.Count > 0)
        {
            var index = GTools.MathfMgr.GetRandomValue(0, list.Count - 1);
            var skillItem = list[index];
            f_Result.Add(skillItem);
            list.RemoveAt(index);
        }
        return f_Result.Count > 0;
    }
}
