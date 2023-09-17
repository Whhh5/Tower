using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeatherRandomGain : MonoBehaviour, IEventSystem
{
    CanvasGroup CanvasGroup => m_WeatherGainSelectVieww.GetComponent<CanvasGroup>();
    EUIMapDGIDType DGAnimaID => EUIMapDGIDType.UIWeatherRandomGain_Animation;

    private Dictionary<EEventSystemType, string> SubscribeList => new()
    {
        { EEventSystemType.WeatherMgr_ChangeWeather, "" },

        { EEventSystemType.WeatherMgr_ChangeWeatherEvent, "" },
        { EEventSystemType.WeatherMgr_SelectWeatherEvent, "" },
    };

    private void Awake()
    {
        PlayDestoryAnimation();
        foreach (var item in SubscribeList)
        {
            GTools.EventSystemMgr.Subscribe(item.Key, this);
        }
        m_UpdateBtn.onClick.AddListener(Click_UpdateWeatherGainList);
        m_MainBackgroudOriginalColor = m_MainBackground.color;
    }
    private Dictionary<int, UIItem_WeatherGainData> m_WeatherGainItemList = new();
    [SerializeField]
    private CanvasGroup m_WeatherGainSelectVieww = null;
    [SerializeField]
    private RectTransform m_ScrollListRoot = null;
    [SerializeField]
    private Image m_MainBackground = null;
    Color m_MainBackgroudOriginalColor;
    [SerializeField]
    private Button m_UpdateBtn = null;
    [SerializeField]
    private RectTransform m_CurWeatherGainListRoot = null;


    private void SetMainBackgroundColor(Color f_ToColor)
    {
        m_MainBackground.color = f_ToColor;
    }
    public void ReceptionEvent(EEventSystemType f_Event, EventSystemParamData f_Params)
    {
        switch (f_Event)
        {
            case EEventSystemType.WeatherMgr_ChangeWeather:
                {
                    Click_UpdateWeatherGainList();
                }
                break;
            case EEventSystemType.WeatherMgr_ChangeWeatherEvent:
                {
                    if (GTools.WeatherMgr.TryGetCurWeatherEventInfo(out var curWeatherEventInfo))
                    {
                    }
                }
                break;
            case EEventSystemType.WeatherMgr_SelectWeatherEvent:
                {
                    var data = f_Params as WeatherSelectGainEventData;
                    SetMainBackgroundColor(data.WeatherGainData.WeatherLevelInfo.Color);
                    PlayDestoryAnimation();


                    AddCurWeatherGainItem(data.WeatherGainData);
                }
                break;
            case EEventSystemType.WeatherMgr_AddWeatherGain:
                {
                    var data = f_Params as WeatherAddGainEventData;
                }
                break;
            default:
                break;
        }
    }
    public void Click_UpdateWeatherGainList()
    {
        if (GTools.WeatherMgr.TryGetCurWeatherInfo(out var curWeatherInfo))
        {
            if (GTools.WeatherMgr.UpdateCurWeatherRandomGain())
            {
                var list = GTools.WeatherMgr.GetCurUpdateWeatherGainList();


                UpdateRandomGainList(list);
            }
        }
    }
    public void UpdateRandomGainList(List<WeatherGainRandomData> f_List)
    {
        ClearWeatherGainItems();
        SetMainBackgroundColor(m_MainBackgroudOriginalColor);
        foreach (var data in f_List)
        {
            var targetData = new UIItem_WeatherGainData();
            targetData.UpdateItemData(data, m_ScrollListRoot);
            m_WeatherGainItemList.Add(data.Index, targetData);
        }
        PlayEnableAnimation();
    }
    public void ClearWeatherGainItems()
    {
        foreach (var item in m_WeatherGainItemList)
        {
            ILoadPrefabAsync.UnLoad(item.Value);
        }
        m_WeatherGainItemList.Clear();
    }
    public void PlayEnableAnimation()
    {
        DOTween.Kill(DGAnimaID);
        var curAlpha = CanvasGroup.alpha;
        var toAlpha = 1.0f;
        DOTween.To(() => 0.0f, value =>
          {
              var updateAlpha = (toAlpha - curAlpha) * value + curAlpha;
              CanvasGroup.alpha = updateAlpha;

          }, 1.0f, toAlpha - curAlpha)
            .SetId(DGAnimaID)
            .OnStart(() =>
            {
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            });
    }
    public void PlayDestoryAnimation()
    {
        DOTween.Kill(DGAnimaID);
        var curAlpha = CanvasGroup.alpha;
        var toAlpha = 0.0f;
        DOTween.To(() => 0.0f, value =>
        {
            var updateAlpha = (toAlpha - curAlpha) * value + curAlpha;
            CanvasGroup.alpha = updateAlpha;

        }, 1.0f, curAlpha - toAlpha)
            .SetId(DGAnimaID)
            .OnStart(()=>
            {
                CanvasGroup.interactable = false;
            })
            .OnComplete(() =>
            {
                CanvasGroup.blocksRaycasts = false;
            });
    }

    public void AddCurWeatherGainItem(WeatherGainRandomData f_Data)
    {
        var item = new UIItem_CurWeatherGainData();
        item.UpdateItemData(f_Data, m_CurWeatherGainListRoot);
    }
}
