using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public enum EFormationType
{
    Near,
}
public class FormationMgr : Singleton<FormationMgr>
{
    private Dictionary<Entity_HeroBaseNewData, List<EFormationType>> m_WorldObjDic = new();
    private Dictionary<EFormationType, Dictionary<Entity_HeroBaseNewData, List<Entity_HeroBaseNewData>>> m_FormationDic = new();


    public void FormationDetection(Entity_HeroBaseNewData f_ObjData)
    {
        var formationType = EFormationType.Near;
        if (!f_ObjData.TryGetRandomTeam(out var list, 1))
        {
            return;
        }
        if (!m_FormationDic.TryGetValue(formationType, out var dic))
        {
            dic = new();
            m_FormationDic.Add(formationType, dic);
        }
        if (!dic.TryGetValue(f_ObjData, out var curRangeList))
        {
            curRangeList = new();
            dic.Add(f_ObjData, curRangeList);
        }
        foreach (var item in list)
        {
            if (item is not Entity_HeroBaseNewData heroData)
            {
                continue;
            }
            curRangeList.Add(heroData);
            AddFormationGain(heroData);
            if (!m_WorldObjDic.TryGetValue(heroData, out var typeList))
            {
                typeList = new();
                m_WorldObjDic.Add(heroData, typeList);
            }
            if (!typeList.Contains(formationType))
            {
                typeList.Add(formationType);
            }
            if (!dic.TryGetValue(heroData, out var listRange))
            {
                listRange = new();
                dic.Add(heroData, listRange);
            }
            listRange.Add(f_ObjData);
        }

        if (!m_WorldObjDic.TryGetValue(f_ObjData, out var listType))
        {
            listType = new();
            m_WorldObjDic.Add(f_ObjData, listType);
        }
        listType.Add(formationType);
        AddFormationGain(f_ObjData);
    }
    public void FormationRemove(Entity_HeroBaseNewData f_ObjData)
    {
        if (m_WorldObjDic.TryGetValue(f_ObjData, out var formationTypeList))
        {
            m_WorldObjDic.Remove(f_ObjData);
            foreach (var item in formationTypeList)
            {
                if (!m_WorldObjDic.TryGetValue(f_ObjData, out var _))
                {
                    continue;
                }
                m_WorldObjDic.Remove(f_ObjData);
            }
        }
        List<Entity_HeroBaseNewData> updateList = new();
        foreach (var item in m_FormationDic)
        {
            foreach (var data in item.Value)
            {
                if (data.Value.Contains(f_ObjData))
                {
                    data.Value.Remove(f_ObjData);
                }
                if (data.Value.Count == 0)
                {
                    updateList.Add(data.Key);
                }
            }
        }
        foreach (var item in updateList)
        {
            if (m_WorldObjDic.TryGetValue(item, out var typeList))
            {
                foreach (var type in typeList)
                {
                    if (!m_FormationDic.TryGetValue(type, out var list))
                    {
                        continue;
                    }
                    list.Remove(item);
                    RemoveFormationGain(item);
                }
            }
            FormationDetection(item);
        }
    }

    private void AddFormationGain(Entity_HeroBaseNewData f_ObjData)
    {
        var gainType = EGainType.None;
        switch (f_ObjData.EntityVocationalType)
        {
            case EHeroVocationalType.Warrior:
                gainType = EGainType.AttackHarm1;
                break;
            case EHeroVocationalType.Enchanter:
                gainType = EGainType.AttackRange1;
                break;
            case EHeroVocationalType.Supplementary:
                //gainType = EGainType.AttackHarm1;
                break;
            case EHeroVocationalType.MainTank:
                gainType = EGainType.Deffense1;
                break;
            default:
                break;
        }
        f_ObjData.AddGain(gainType, null);
    }
    private void RemoveFormationGain(Entity_HeroBaseNewData f_ObjData)
    {
        var gainType = EGainType.None;
        switch (f_ObjData.EntityVocationalType)
        {
            case EHeroVocationalType.Warrior:
                gainType = EGainType.AttackHarm1;
                break;
            case EHeroVocationalType.Enchanter:
                gainType = EGainType.AttackRange1;
                break;
            case EHeroVocationalType.Supplementary:
                //gainType = EGainType.AttackHarm1;
                break;
            case EHeroVocationalType.MainTank:
                gainType = EGainType.Deffense1;
                break;
            default:
                break;
        }

        f_ObjData.RemoveGain(gainType);
    }
}
