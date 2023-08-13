using DG.Tweening;
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


    [SerializeField]
    private CanvasGroup m_HintCanvasGroup = null;
    [SerializeField]
    private TextMeshProUGUI m_HintWeatherName = null;
    [SerializeField]
    private TextMeshProUGUI m_HintEventName = null;
    [SerializeField]
    private AnimationCurve m_AlphaCurve = null;
    private int DoKeyID => GetInstanceID();
    public void Initialization()
    {
        m_HintCanvasGroup.alpha = 0;
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
            m_WeatherName.text = $"{curWeather.WeatherInfo.Name}";
        }
    }
    public void UpdateWeatherEvent()
    {
        // 更新名称 + 图片
        var curWeather = WeatherMgr.Ins.CurEventData;
        if (curWeather != null)
        {
            m_EventName.text = $"{curWeather.WeatherEventInfo.Name}";
            StartWeatherHint();
        }
    }

    public void UpdateTime()
    {
        m_WeatherResidueTime.text = $"{WeatherMgr.Ins.CurWeatherResidueTime:0.0}";
        m_EventResidueTime.text = $"{WeatherMgr.Ins.CurWeatherEventResidueTime:0.0}";
    }

    public void StartWeatherHint()
    {
        var funcID = DoKeyID + 1;
        var curWeather = WeatherMgr.Ins.CurWeatherData;
        var curEvent = WeatherMgr.Ins.CurEventData;
        if (curWeather != null && curEvent != null)
        {
            m_HintWeatherName.text = $"{curWeather.WeatherInfo.Name}:{curEvent.WeatherEventInfo.Name}";
            m_HintEventName.text = $"{curEvent.WeatherEventInfo.Describe}";


            DOTween.Kill(funcID);
            var doTime = m_AlphaCurve.keys[^1].time;
            var curAlpha = m_HintEventName.alpha;
            DOTween.To(() => 0, (value) =>
              {
                  var alpha = m_AlphaCurve.Evaluate(value * doTime);
                  m_HintCanvasGroup.alpha = alpha;
              }, 1.0f, doTime)
                .SetId(funcID);
        }
    }
}
