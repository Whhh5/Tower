using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using UnityEngine.EventSystems;


public abstract class WeaterEventBaseData
{
    public abstract EWeatherEventType WeatherEventType { get; }
    public float DurationTime { get; set; }

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
public class WeatherEvent_Volcano1Data : WeaterEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano1;
}
public class WeatherEvent_Volcano2Data : WeaterEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano2;
}
public class WeatherEvent_Volcano3Data : WeaterEventBaseData
{
    public override EWeatherEventType WeatherEventType => EWeatherEventType.Volcano3;
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
    private ListStack<WeatherUpdateInfo> m_CurWeatherSystemList = new("", 10);

    public EWeatherType CurWeatherType = EWeatherType.EnumCount;
    public EWeatherEventType CurWeatherEvent = EWeatherEventType.EnumCount;

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
    public void Initialization()
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
            DurationTime = 60.0f,
            EventList = new(""),
        };
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano1, DurationTime = 10.0f });
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano3, DurationTime = 6.0f });
        updateInfo.EventList.Push(new WeatherUpdateEventInfo() { EventType = EWeatherEventType.Volcano2, DurationTime = 20.0f });
        m_CurWeatherSystemList.Push(updateInfo);

        GTools.LifecycleMgr.AddUpdate(this);
    }
    public void OnUpdate()
    {
        m_LasteUpdateTime = m_LasteUpdateTime == 0 ? Time.time : m_LasteUpdateTime;
        m_TimeDelta = Time.time - m_LasteUpdateTime;

        // 天气刷新
        if (m_WeatherEndTime == 0 || m_WeatherEndTime > Time.time)
        {
            if (ChangeWeatherSystem(out var weatherInfo))
            {
                if (CurWeatherData != null)
                {
                    CurWeatherData.StopExecute();
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
        if (m_WeatherEventEndTime == 0 || m_WeatherEventEndTime > Time.time)
        {
            if (ChangeWeatherEvent(out var value))
            {
                if (CurEventData != null)
                {
                    CurEventData.StopExecute();
                }
                value.StartExecute();
                CurEventData = value;

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
    public WeaterEventBaseData CurEventData = null;
    public WeatherUpdateEventInfo CurEventInfo = null;
    private void InitCurEventData()
    {
        m_EventList = CurWeatherUpdateInfo.EventList;
        m_WeatherEventTime = 0;
        m_WeatherEventEndTime = 0;
        m_CurWeatherEventResidueTime = 0;
    }
    public bool ChangeWeatherEvent(out WeaterEventBaseData f_EventInfo)
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

}
