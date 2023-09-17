using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using UnityEngine.EventSystems;

public class WeatherEventData : EventSystemParamData
{
    public WeatherBaseData LastWeatherData;
}
public class WeatherEventEventData : EventSystemParamData
{
    public WeatherEventBaseData LastWeathereventData;
}
public class WeatherSelectGainEventData : EventSystemParamData
{
    public WeatherGainRandomData WeatherGainData;
}
public class WeatherAddGainEventData : EventSystemParamData
{
    public WeatherGainData WeatherGainData;
}
public class WeatherGainRandomData
{
    public int Index;
    public int EventIndex;
    public EWeatherGainType WeatherGainType;
    public EWeatherGainLevel Level;

    public WeatherGainLevelInfo WeatherLevelInfo => GTools.TableMgr.TryGetWeatherGainLevelInfo(Level, out var result) ? result : null;
    public WeatherGainInfo WeatherGainInfo => GTools.TableMgr.TryGetWeatherGainInfo(WeatherGainType, out var WeatherGainInfo) ? WeatherGainInfo : null;
    public AssetKey WeatherGainIcon => WeatherGainInfo.TryGetWeatherGainIcon(out var iconId, Level) ? iconId : AssetKey.None;
}
public abstract class WeatherEventBaseData
{
    public abstract EWeatherEventType WeatherEventType { get; }
    public abstract EGainType GainType { get; }
    public float DurationTime { get; set; }

    public WeatherEventInfo WeatherEventInfo;


    public void Initialization(float f_DurationTime)
    {
        DurationTime = f_DurationTime;
    }
}
public class WeatherEvent_Volcano1Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano1;

    public override EGainType GainType => EGainType.Volccano1;
}
public class WeatherEvent_Volcano2Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano2;

    public override EGainType GainType => EGainType.Volccano2;
}
public class WeatherEvent_Volcano3Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano3;

    public override EGainType GainType => EGainType.Volccano3;
}
public class WeatherEvent_Typhoon1Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Typhoon1;

    public override EGainType GainType => throw new System.NotImplementedException();
}
public class WeatherEvent_Typhoon2Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Typhoon2;

    public override EGainType GainType => throw new System.NotImplementedException();
}
public class WeatherEvent_Typhoon3Data : WeatherEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Typhoon3;

    public override EGainType GainType => throw new System.NotImplementedException();
}
public interface IWeatherUtil
{
    public static void ChangeWeatherSyatem(EWeatherType f_WeatherType)
    {

    }
    public static void ChangeWeatherEvent(EWeatherEventType f_WeatherEventType)
    {

    }
}
public abstract class WeatherBaseData
{
    public abstract EWeatherType WeatherType { get; }
    public float DurationTime { get; private set; }
    public WeatherInfo WeatherInfo;
    public void Initialization(float f_DurationTime)
    {
        DurationTime = f_DurationTime;
    }
    public virtual void StartExecute()
    {

    }
    public virtual void StopExecute()
    {

    }
}
public class Weather_VolcanoData : WeatherBaseData
{
    public override EWeatherType WeatherType => EWeatherType.Volcano;
}
public class Weather_TyphoonData : WeatherBaseData
{
    public override EWeatherType WeatherType => EWeatherType.Typhoon;
}
public class Weather_GainDefault1 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default1;
    public EBuffType BuffType = EBuffType.WeatherSpeed;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {
        f_WorldObj.AddBuffAsync(EBuffType.WeatherSpeed, GTools.MonsterMgr.GodEntityData);
    }
}
public class Weather_GainDefault2 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default2;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {
        f_WorldObj.AddBuffAsync(EBuffType.WeatherAttack, GTools.MonsterMgr.GodEntityData);
    }
}
public class Weather_GainDefault3 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default3;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {
        f_WorldObj.AddBuffAsync(EBuffType.WeatherMaxBlood, GTools.MonsterMgr.GodEntityData);
    }
}
public class Weather_GainDefault4 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default4;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {
        f_WorldObj.AddBuffAsync(EBuffType.WeatherDefense, GTools.MonsterMgr.GodEntityData);
    }
}
public class Weather_GainDefault5 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default5;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {

    }
}
public class Weather_GainDefault6 : WeatherGainData
{
    public override EWeatherGainType WeatherGainType => EWeatherGainType.Default6;

    public override void StartExecute(WorldObjectBaseData f_WorldObj)
    {

    }
}
public class WeatherUpdateEventInfo
{
    public EWeatherEventType EventType;
    public float DurationTime;
    public WeatherEventInfo WeatherEventInfo => TableMgr.Ins.TryGetWeatherEventInfo(EventType, out var value) ? value : null;
}
public class WeatherUpdateInfo
{
    public EWeatherType WeatherType;
    public float DurationTime;
    public ListStack<WeatherUpdateEventInfo> EventList = new("天气事件", 10);
    public WeatherInfo WeatherInfo => TableMgr.Ins.TryGetWeatherInfo(WeatherType, out var weatherInfo) ? weatherInfo : null;
}
public class WeatherMgr : Singleton<WeatherMgr>, IUpdateBase
{
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
    private ListStack<WeatherUpdateInfo> m_CurWeatherSystemList = new("", 10);

    public EWeatherType CurWeatherType => CurWeatherUpdateInfo.WeatherType;
    public EWeatherEventType CurWeatherEventType => CurEventInfo.EventType;

    private int WeatherRandomGainCount => GTools.WeatherRandomGainCount;


    // 开始运行第一个天气系统间隔时间
    private float m_StartIntervalTime = 0;
    // 开始运行第一个天气事件间隔时间
    private float m_StartEventIntervalTime = 0;
    // 天气系统运行开始事件
    private float m_StartTime = 0;




    // update 相关
    public int UpdateLevelID { get; set; }
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level2;
    private float m_LasteUpdateTime = 0;
    private float m_TimeDelta = 0;

    // ui 窗口
    private UIWeatherSystem m_CurWeatherWindow = null;
    public void SetCurWeatherWindow(UIWeatherSystem f_Window)
    {
        m_CurWeatherWindow = f_Window;
        f_Window.Initialization();
    }
    public override void Start()
    {
        m_StartIntervalTime = 10;
        m_StartEventIntervalTime = 5;
        m_StartTime = Time.time;

        m_CurWeatherResidueTime = 0;
        m_WeatherTime = 0;
        m_WeatherEndTime = 0;

        var window = GameObject.FindObjectOfType<UIWeatherSystem>();
        SetCurWeatherWindow(window);


        m_CurWeatherSystemList = new("", 10)
        {

        };
        var updateInfo = new WeatherUpdateInfo()
        {
            WeatherType = EWeatherType.Volcano,
            DurationTime = 180.0f,
            EventList = new(""),
        };
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano1, DurationTime = 30.0f });
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano3, DurationTime = 30.0f });
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano2, DurationTime = 30.0f });
        m_CurWeatherSystemList.Push(updateInfo);
        var updateInfo2 = new WeatherUpdateInfo()
        {
            WeatherType = EWeatherType.Typhoon,
            DurationTime = 22,
            EventList = new(""),
        };
        updateInfo2.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano1, DurationTime = 30.0f });
        updateInfo2.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano3, DurationTime = 30.0f });
        updateInfo2.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano2, DurationTime = 30.0f });
        m_CurWeatherSystemList.Push(updateInfo2);
        var updateInfo3 = new WeatherUpdateInfo()
        {
            WeatherType = EWeatherType.Volcano,
            DurationTime = 92.0f,
            EventList = new(""),
        };
        updateInfo3.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano1, DurationTime = 30.0f });
        updateInfo3.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano3, DurationTime = 30.0f });
        updateInfo3.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano2, DurationTime = 30.0f });
        m_CurWeatherSystemList.Push(updateInfo3);
        var updateInfo4 = new WeatherUpdateInfo()
        {
            WeatherType = EWeatherType.Volcano,
            DurationTime = 92.0f,
            EventList = new(""),
        };
        updateInfo4.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano1, DurationTime = 30.0f });
        updateInfo4.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano3, DurationTime = 30.0f });
        updateInfo4.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano2, DurationTime = 30.0f });
        m_CurWeatherSystemList.Push(updateInfo4);

        GTools.LifecycleMgr.AddUpdate(this);
    }
    public void OnUpdate()
    {
        m_LasteUpdateTime = m_LasteUpdateTime == 0 ? Time.time : m_LasteUpdateTime;
        m_TimeDelta = Time.time - m_LasteUpdateTime;

        // 天气刷新
        if (m_WeatherEndTime == 0 || m_WeatherEndTime < Time.time)
        {
            m_CurWeatherResidueTime = 0;
            if (ChangeWeatherSystem(out var weatherInfo))
            {
                var lastWeather = CurWeatherData;
                WeatherEventData eventdata = new()
                {
                    Description = "天气事件 -> 切换天气, 参数是上一个天气实例数据",
                    LastWeatherData = lastWeather,
                };
                GTools.EventSystemMgr.SendEvent(EEventSystemType.WeatherMgr_ChangeWeather, eventdata);
                if (lastWeather != null)
                {
                    lastWeather.StopExecute();
                }
                weatherInfo.StartExecute();
                CurWeatherData = weatherInfo;
                m_CurWeatherWindow.UpdateWeather();

                InitCurEventData();
            }
        }
        else
        {
            m_CurWeatherResidueTime -= m_TimeDelta;
        }
        // 事件刷新
        if (m_WeatherEventEndTime == 0 || m_WeatherEventEndTime < Time.time)
        {
            m_CurWeatherEventResidueTime = 0;
            if (ChangeWeatherEvent(out var value))
            {
                var lastWeatherEvent = CurEventData;
                WeatherEventEventData eventData = new()
                {
                    LastWeathereventData = lastWeatherEvent,
                    Description = "天气事件 -> 切换天气事件, 参数是上一个天气事件实例数据",
                };
                GTools.EventSystemMgr.SendEvent(EEventSystemType.WeatherMgr_ChangeWeatherEvent, eventData);
                if (lastWeatherEvent != null)
                {
                    StopExecute();
                }
                CurEventData = value;
                StartExecute();

                m_CurWeatherWindow.UpdateWeatherEvent();
            }
        }
        else
        {
            m_CurWeatherEventResidueTime -= m_TimeDelta;
        }

        if (m_CurWeatherWindow != null)
        {
            m_CurWeatherWindow.UpdateTime();
        }

        m_LasteUpdateTime = Time.time;
    }
    public bool TryGetCurWeatherInfo(out WeatherInfo f_WeatherInfo)
    {
        if (GTools.TableMgr.TryGetWeatherInfo(CurWeatherType, out f_WeatherInfo))
        {
            return true;
        }
        return false;
    }
    public bool TryGetCurWeatherEventInfo(out WeatherEventInfo f_WeatherEventInfo)
    {
        if (GTools.TableMgr.TryGetWeatherEventInfo(CurWeatherEventType, out f_WeatherEventInfo))
        {
            return true;
        }
        return false;
    }
    // 天气切换数据
    // 切换到下一个天气剩余时间
    private float m_CurWeatherResidueTime = 0;
    public float CurWeatherResidueTime => Mathf.Max(0, m_CurWeatherResidueTime);
    // 当前天气系统运行时间
    private float m_WeatherTime = 0;
    private float m_WeatherEndTime = 0;
    public WeatherBaseData CurWeatherData = null;
    public WeatherUpdateInfo CurWeatherUpdateInfo = null;
    public bool ChangeWeatherSystem(out WeatherBaseData f_WeatherInfo)
    {
        f_WeatherInfo = null;
        if (m_CurWeatherSystemList.TryPop(out var weatherInfo) && weatherInfo.WeatherInfo != null)
        {
            CurWeatherUpdateInfo = weatherInfo;
            f_WeatherInfo = weatherInfo.WeatherInfo.CreateWeatherData(weatherInfo.DurationTime);
            m_WeatherTime = m_CurWeatherResidueTime = weatherInfo.DurationTime;
            m_WeatherEndTime = Time.time + weatherInfo.DurationTime;
            return true;
        }
        return false;
    }

    // 天气事件切换数据
    // 切换到下一个事件剩余时间
    private float m_CurWeatherEventResidueTime = 0;
    public float CurWeatherEventResidueTime => Mathf.Max(0, m_CurWeatherEventResidueTime);
    // 当前天气事件运行时间
    private float m_WeatherEventTime = 0;
    private float m_WeatherEventEndTime = 0;
    private ListStack<WeatherUpdateEventInfo> m_EventList = new("天气事件", 10);
    public WeatherEventBaseData CurEventData = null;
    public WeatherUpdateEventInfo CurEventInfo = null;
    private void InitCurEventData()
    {
        m_EventList = CurWeatherUpdateInfo.EventList;
        m_WeatherEventTime = 0;
        m_WeatherEventEndTime = 0;
        m_CurWeatherEventResidueTime = 0;
    }
    public bool ChangeWeatherEvent(out WeatherEventBaseData f_EventInfo)
    {
        f_EventInfo = null;
        if (m_EventList.TryPop(out var eventInfo) && eventInfo.WeatherEventInfo != null)
        {
            CurEventInfo = eventInfo;
            f_EventInfo = eventInfo.WeatherEventInfo.CreateWeatherData(eventInfo.DurationTime);
            m_CurWeatherEventResidueTime = eventInfo.DurationTime;
            m_WeatherEventTime = eventInfo.DurationTime;
            m_WeatherEventEndTime = Time.time + f_EventInfo.DurationTime;
            return true;
        }
        return false;
    }


    // 对外
    /// <summary>
    /// 对一个对象应用当前天气效果
    /// </summary>
    /// <param name="f_Recipient"></param>
    public void InflictionGain(WorldObjectBaseData f_Recipient)
    {
        if (CurEventData != null)
        {
            IGainUtil.InflictionGain(CurEventData.GainType, null, f_Recipient);
        }
    }
    /// <summary>
    /// 开始运行天气事件效果
    /// </summary>
    public virtual void StartExecute()
    {
        if (ILoadPrefabAsync.TryGetEntityByType<WorldObjectBaseData>(EWorldObjectType.Preson, out var dic))
        {
            foreach (var item in dic)
            {
                InflictionGain(item.Value);
            }
        }
    }
    /// <summary>
    /// 停止运行天气事件效果
    /// </summary>
    public virtual void StopExecute()
    {
        if (ILoadPrefabAsync.TryGetEntityByType<WorldObjectBaseData>(EWorldObjectType.Preson, out var dic))
        {
            foreach (var item in dic)
            {
                IGainUtil.RemoteGain(CurEventData.GainType, item.Value);
            }
        }
    }



    //===============================----------------------========================================
    //=====-----                                                                         -----=====
    //                                catalogue -- 天气随机buff
    //=====-----                                                                         -----=====
    //===============================----------------------========================================
    private List<WeatherGainData> m_CurWeatherGainList = new();
    private List<WeatherGainRandomData> m_CurUpdateGainList = new();
    public void AddWeatherGain(EWeatherGainType f_WeatherGainType, EWeatherGainLevel f_Level)
    {
        if (GTools.TableMgr.TryGetWeatherGainInfo(f_WeatherGainType, out var f_Info))
        {
            if (f_Info.TryGetWeatherGainData(out var data, f_Level))
            {
                m_CurWeatherGainList.Add(data);
                WeatherAddGainEventData eventData = new()
                {
                    WeatherGainData = data,
                    Description = "添加天气增益  WeatherGainData",
                };
                GTools.EventSystemMgr.SendEvent(EEventSystemType.WeatherMgr_AddWeatherGain, eventData);
            }
        }
    }
    public bool UpdateCurWeatherRandomGain()
    {
        ClearCurUpdateWeatherGainList();

        while (m_CurUpdateGainList.Count < WeatherRandomGainCount)
        {
            TryUpdateWeatherGainItem(m_CurUpdateGainList.Count, out var itemData);
        }

        return m_CurUpdateGainList.Count == WeatherRandomGainCount;
    }
    public void ExecuteCurWeatherGain(WorldObjectBaseData f_WorldObj)
    {
        foreach (var item in m_CurWeatherGainList)
        {
            item.StartExecute(f_WorldObj);
        }
    }
    public List<WeatherGainRandomData> GetCurUpdateWeatherGainList()
    {
        return m_CurUpdateGainList;
    }
    public void ClearCurUpdateWeatherGainList()
    {
        m_CurUpdateGainList.Clear();
    }
    public void SelectWeatherGainItem(WeatherGainRandomData f_WeatherItem)
    {
        if (m_CurUpdateGainList.Contains(f_WeatherItem))
        {
            AddWeatherGain(f_WeatherItem.WeatherGainType, f_WeatherItem.Level);
            WeatherSelectGainEventData eventData = new()
            {
                WeatherGainData = f_WeatherItem,
                Description = "选择天气增益  WeatherGainRandomData",
            };
            GTools.EventSystemMgr.SendEvent(EEventSystemType.WeatherMgr_SelectWeatherEvent, eventData);
        }
        ClearCurUpdateWeatherGainList();
    }
    public bool TryUpdateWeatherGainItem(int f_Index, out WeatherGainRandomData f_NewData)
    {
        f_NewData = null;
        if (TryGetCurWeatherInfo(out var weatherInfo))
        {
            while (m_CurUpdateGainList.Count - f_Index <= 0)
            {
                m_CurUpdateGainList.Add(null);
            }
            var gainList = weatherInfo.WeatherGainTypeList;
            var eventIndex = GTools.MathfMgr.GetRandomValue(0, gainList.Count);
            var level = (EWeatherGainLevel)GTools.MathfMgr.GetRandomValue((int)EWeatherGainLevel.Level1, (int)EWeatherGainLevel.MaxLevel);

            foreach (var item in m_CurUpdateGainList)
            {
                if (item != null && item.EventIndex == eventIndex && gainList.Count >= WeatherRandomGainCount)
                {
                    return TryUpdateWeatherGainItem(f_Index, out f_NewData);
                }
            }
            var itemData = new WeatherGainRandomData()
            {
                Index = f_Index,
                EventIndex = eventIndex,
                WeatherGainType = gainList[eventIndex],
                Level = level
            };
            m_CurUpdateGainList[f_Index] = itemData;
            f_NewData = itemData;
        }
        return f_NewData != null;
    }
}