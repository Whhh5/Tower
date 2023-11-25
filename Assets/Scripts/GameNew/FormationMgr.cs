using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

public interface ISOAssetUtil
{
    public static async UniTask<TSOCom> LoadAsync<TSOCom>(EAssetKey f_AsseTKey)
        where TSOCom : ScriptableObject
    {
        if (GTools.TableMgr.TryGetAssetPath(f_AsseTKey, out var path))
        {
            var asset = await Resources.LoadAsync<ScriptableObject>(path);
            return asset as TSOCom;
        }
        return null;
    }
    public static void UnLoad(ScriptableObject f_Obj)
    {
        Resources.UnloadAsset(f_Obj);
    }
}
public enum EFormationType
{
    Near,
    Sphere,
    EnumCount,
}
public class FormationData
{
    public int CentreIndex => CentreHero.CurrentIndex;
    public Entity_HeroBaseNewData CentreHero = null;
    public Dictionary<int, Entity_HeroBaseNewData> Map = new();
}
public class FormationMgr : Singleton<FormationMgr>, IUpdateBase
{
    private Dictionary<Entity_HeroBaseNewData, List<EFormationType>> m_WorldObjDic = new();
    private Dictionary<EFormationType, Dictionary<Entity_HeroBaseNewData, List<Entity_HeroBaseNewData>>> m_FormationDic = new();

    private List<EAssetKey> m_ConfigList = new()
    {
        EAssetKey.Cfg_Formation_Near,
        EAssetKey.Cfg_Formation_Sphere,
    };
    public override async void Awake()
    {
        base.Awake();

        GTools.LifecycleMgr.AddUpdate(this);

        foreach (var item in m_ConfigList)
        {
            var asset = await ISOAssetUtil.LoadAsync<SOHeroFormation>(item);
            m_FormationInfo.Add(asset.FormationType, asset);
        }
    }
    public override void Destroy()
    {
        GTools.LifecycleMgr.RemoveUpdate(this);

        base.Destroy();
    }


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
        switch (f_ObjData.GetVocationalType())
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
        switch (f_ObjData.GetVocationalType())
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


    private Dictionary<EFormationType, SOHeroFormation> m_FormationInfo = new();
    public bool TryGetFormationInfo(EFormationType f_FormationType, out SOHeroFormation f_FormationInfo)
    {
        return m_FormationInfo.TryGetValue(f_FormationType, out f_FormationInfo);
    }

    private Vector2Int MapSize => GameDataMgr.MapWH;

    public int UpdateLevelID { get; set; }

    public EUpdateLevel UpdateLevel => EUpdateLevel.Level3;

    public float LasteUpdateTime { get; set; }
    public float UpdateDelta { get; set; }

    private Dictionary<EFormationType, Dictionary<Entity_HeroBaseNewData, Entity_FormationBaseData>> m_CurFormationDic = new();

    public void TestFormation()
    {
        if (!ILoadPrefabAsync.TryGetEntityByType<Entity_HeroBaseNewData>(EWorldObjectType.Preson, out var allHero))
        {
            return;
        }

        Dictionary<EFormationType, List<FormationData>> forData = new();
        foreach (var dataKey in m_FormationInfo)
        {
            var data = dataKey.Value;
            var centreHeroType = data.GetCentreHeroVocationalType();
            var formationRowCol = data.GetCentreRowCol();
            Dictionary<int, Entity_HeroBaseNewData> heroDic = new();
            List<Entity_HeroBaseNewData> list = new();
            foreach (var item in allHero)
            {
                if (!GTools.UnityObjectIsVaild(item.Value) || !GTools.WorldObjectIsActive(item.Value))
                {
                    continue;
                }
                heroDic.Add(item.Value.CurrentIndex, item.Value);
                if (item.Value.GetVocationalType() != centreHeroType)
                {
                    continue;
                }
                list.Add(item.Value);
            }

            List<FormationData> formatsList = new();
            foreach (var hero in list)
            {
                FormationData dataData = new();
                dataData.CentreHero = hero;
                var curHeroRowCol = GTools.CreateMapNew.IndexToRowCol(hero.CurrentIndex);
                var startRowCol = curHeroRowCol + formationRowCol * new Vector2Int(1, -1);
                var isEven = startRowCol.x % 2 == 0;
                startRowCol.y -= isEven ? 0 : 1;
                bool isNext = false;
                for (int i = 0; i < data.ArraySize.x; i++)
                {
                    var row = startRowCol.x - i;
                    if (row < 0 || row >= MapSize.x)
                    {
                        continue;
                    }
                    for (int j = 0; j < data.ArraySize.y; j++)
                    {
                        var col = startRowCol.y + j + (isEven ? 0 : (i % 2 == 0 ? 0 : 1));
                        var value = data.ArrayIndex[i, j];
                        var index = GTools.CreateMapNew.RowColToIndex(new Vector2Int(row, col));

                        if (value == 0)
                        {
                            continue;
                        }
                        else if (false
                            || col < 0
                            || col >= MapSize.y
                            || !heroDic.TryGetValue(index, out var heroData)
                            || heroData.GetVocationalType() != data.GetHeroVocationalType(value))
                        {
                            isNext = true;
                            break;
                        }
                        else
                        {
                            var formationIndex = i * data.ArraySize.y + j;
                            dataData.Map.Add(formationIndex, heroData);
                        }
                    }
                    if (isNext)
                    {
                        break;
                    }
                }
                if (isNext)
                {
                    continue;
                }
                formatsList.Add(dataData);
            }
            forData.Add(dataKey.Key, formatsList);
        }
        UpdateCurrentFormation(forData);

        string logStr = "ÕóÐÍ¼ì²â£¬½á¹û: ";
        foreach (var item in forData)
        {
            logStr += $"\t{item.Key}:";
            foreach (var data in item.Value)
            {
                logStr += $"\n\t\t";
                foreach (var heroData in data.Map)
                {
                    logStr += $" -> {heroData.Value.CurrentIndex}";
                }
            }
        }
        Log(logStr);


    }
    private void UpdateCurrentFormation(Dictionary<EFormationType, List<FormationData>> f_List)
    {
        var tempData = m_CurFormationDic;
        tempData = new();
        foreach (var item in f_List)
        {
            tempData.Add(item.Key, new());
            var list = tempData[item.Key];
            foreach (var data in item.Value)
            {
                if (!m_CurFormationDic.TryGetValue(item.Key, out var listdata) || !listdata.TryGetValue(data.CentreHero, out var forData))
                {
                    if (GTools.TableMgr.TryGetFormationData(item.Key, out forData))
                    {
                        forData.SetFormationData(data);
                        forData.SetPosition(data.CentreHero.GetCurChunkPos());
                        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(forData));
                    }
                }
                list.Add(data.CentreHero, forData);
            }
        }

        foreach (var item in m_CurFormationDic)
        {
            if (!tempData.TryGetValue(item.Key, out var list))
            {
                list = new();
            }
            foreach (var data in item.Value)
            {
                if (!list.ContainsKey(data.Key))
                {
                    ILoadPrefabAsync.UnLoad(data.Value);
                }
            }
        }

        m_CurFormationDic = tempData;
    }

    private float m_LastTime = 0.0f;
    public void OnUpdate()
    {
        if (Time.time - m_LastTime < 1.0f)
        {
            return;
        }
        m_LastTime = Time.time;
        TestFormation();
    }
}
