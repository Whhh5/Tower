using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWeatherSystem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_WeatherName = null;
    [SerializeField]
    private TextMeshProUGUI m_WeatherResidueTime = null;

    [SerializeField]
    private TextMeshProUGUI m_EventName = null;
    [SerializeField]
    private TextMeshProUGUI m_EventResidueTime = null;

    public void Initialization()
    {
        UpdateWeather();
        UpdateWeatherEvent();
        UpdateTime();
    }
    public void UpdateWeather()
    {
        // 更新名称 + 图片
        var curWeather = WeatherMgr.Ins.CurWeatherData;
        if (curWeather != null)
        {
            m_WeatherName.text = $"{curWeather.WeatherType}";
        }
    }
    public void UpdateWeatherEvent()
    {
        // 更新名称 + 图片
        var curWeather = WeatherMgr.Ins.CurEventData;
        if (curWeather != null)
        {
            m_EventName.text = $"{curWeather.WeatherEventType}";
        }
    }

    public void UpdateTime()
    {
        m_WeatherResidueTime.text = $"{WeatherMgr.Ins.CurWeatherResidueTime}";
        m_EventResidueTime.text = $"{WeatherMgr.Ins.CurWeatherEventResidueTime}";
    }
}
