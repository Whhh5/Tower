using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWeatherRandomGain : MonoBehaviour, IEventSystem
{
    private class WeatherGainItem
    {
        public WeatherGainItem(Transform f_SourceItem)
        {
            m_SourceItem = f_SourceItem;
        }
        public Transform m_SourceItem = null;
        public Transform Item = null;
        public int Index;

        public void SetActive(bool f_ToActive)
        {
            if (Item != null)
            {
                Item.gameObject.SetActive(f_ToActive);
            }
        }

    }
    private Dictionary<EEventSystemType, string> SubscribeList => new()
    {
        { EEventSystemType.WeatherMgr_ChangeWeather, "" },
        { EEventSystemType.WeatherMgr_ChangeWeatherEvent, "" },
    };

    private void Awake()
    {
        foreach (var item in SubscribeList)
        {
            GTools.EventSystemMgr.Subscribe(item.Key, this, item.Value);
        }
    }

    private List<WeatherGainItem> m_WeatherGainItemList = new();
    public void ReceptionEvent(EEventSystemType f_Event, object f_Params, string f_SendDesc)
    {
        var curWeather = GTools.WeatherMgr.CurWeatherType;
        var curWeatherEvent = GTools.WeatherMgr.CurWeatherEventType;
        switch (f_Event)
        {
            case EEventSystemType.WeatherMgr_ChangeWeather:
                {
                    if (GTools.WeatherMgr.TryGetCurWeatherInfo(out var curWeatherInfo))
                    {
                        var list = GTools.WeatherMgr.TryGetCurWeatherRandomGain();

                        UpdateRandomGainList(list);
                    }
                }
                break;
            case EEventSystemType.WeatherMgr_ChangeWeatherEvent:
                {
                    if (GTools.WeatherMgr.TryGetCurWeatherEventInfo(out var curWeatherEventInfo))
                    {

                    }
                }
                break;
            default:
                break;
        }
    }
    public void UpdateRandomGainList(List<WeatherGainRandomData> f_List)
    {
        foreach (var item in m_WeatherGainItemList)
        {
            item.SetActive(false);
        }
    }
    public void SelectionWeatherGainItem(WeatherGainRandomData f_ItemData)
    {

    }
    public void UpdateWeatherGainItem(WeatherGainRandomData f_ItemData)
    {

    }
}
