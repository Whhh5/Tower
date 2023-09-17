using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEventSystemType : int
{
    None,
    SCENE_LOAD_START,
    SCENE_LOAD_FINISH,
    /// <summary>
    /// 窗口初始化完成之后调用
    /// </summary>
    UI_WINDOW_LOAD_FINISH,
    /// <summary>
    /// 窗口卸载完成之后调用，
    /// 参数是一个枚举 EWindow  
    /// </summary>
    UI_WINDOW_UNLOAD_FINISH,
    UI_WINDOW_SHOW,
    UI_WINDOW_HIDE,
    WeatherMgr_ChangeWeather,
    WeatherMgr_ChangeWeatherEvent,
    WeatherMgr_SelectWeatherEvent,
    WeatherMgr_AddWeatherGain,
    UIOnEnableInfos_Enter,
    UIOnEnableInfos_Exit,
    CreateHero,
    DestroyHero,

    Hero_Skill_Add,
    EnumCount,
}
public interface IEventSystem
{
    void ReceptionEvent(EEventSystemType f_Event, EventSystemParamData f_Params);
}
