using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GlobalEventMgr : Singleton<GlobalEventMgr>, IEventSystem
{
    public override void Awake()
    {
        base.Awake();

        GTools.EventSystemMgr.Subscribe(EEventSystemType.CreateHero, this);
        GTools.EventSystemMgr.Subscribe(EEventSystemType.Hero_Skill_Add, this);
    }

    public void ReceptionEvent(EEventSystemType f_Event, EventSystemParamData f_Params)
    {
        switch (f_Event)
        {
            case EEventSystemType.CreateHero:
                {
                    var heroData = f_Params as EventData_CreateHero;
                    Event_CreateHero(heroData);
                }
                break;
            case EEventSystemType.Hero_Skill_Add:
                var eventData = f_Params as HeroAddSKillEventData;
                Event_Hero_Skill_Add(eventData);
                break;
            default:
                break;
        }
    }
    public void Event_CreateHero(EventData_CreateHero f_HeroData)
    {
        var heroData = f_HeroData.HeroData;
    }
    public void Event_Hero_Skill_Add(HeroAddSKillEventData f_SkillEventData)
    {
        WorldWindowMgr.Ins.AddHeroSkill(f_SkillEventData);
        Log($"{f_SkillEventData.HeroData.HeroCradType} + skill {f_SkillEventData.SkillType} , skill count = {f_SkillEventData.HeroData.GetCurSkillCount()}");
    }
}
