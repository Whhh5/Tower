using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItem_CurWeatherGainData : UIItemBaseData<WeatherGainRandomData>
{
    public override AssetKey AssetPrefabID => AssetKey.UIItem_CurWeatherGain;

    public UIItem_CurWeatherGain TargetIns => GetCom<UIItem_CurWeatherGain>();
    public override void UpdateItemData(WeatherGainRandomData f_Data, Transform f_Parent)
    {
        base.UpdateItemData(f_Data, f_Parent);
        UpdateInfos();
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        UpdateInfos();
    }
    public void UpdateInfos()
    {
        if (TargetIns != null)
        {
            TargetIns.UpdateInfos();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Log($"enter weather gain name => {Data.WeatherGainInfo.Name}");
        var hintInfos = new UIOnEnableInfosShowData()
        {
            IconKey = Data.WeatherGainIcon,
            Description = Data.WeatherGainInfo.Describe,
            Name = Data.WeatherGainInfo.Name,
        };
        GTools.EventSystemMgr.SendEvent(EEventSystemType.UIOnEnableInfos_Enter, hintInfos);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Log($"exit weather gain name => {Data.WeatherGainInfo.Name}");
        GTools.EventSystemMgr.SendEvent(EEventSystemType.UIOnEnableInfos_Exit);
    }
}
public class UIItem_CurWeatherGain : UIItemBase, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image m_IconImg = null;
    [SerializeField]
    private Image m_MainBackground = null;
    public UIItem_CurWeatherGainData WeatherGainData => GetData<UIItem_CurWeatherGainData>();
    public void OnPointerEnter(PointerEventData eventData)
    {
        WeatherGainData.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WeatherGainData.OnPointerExit(eventData);
    }
    public async void UpdateInfos()
    {
        var data = WeatherGainData.Data.WeatherGainInfo;

        m_MainBackground.color = WeatherGainData.Data.WeatherLevelInfo.Color;
        if (m_IconImg.sprite != null)
        {
            ILoadSpriteAsync.UnLoad(m_IconImg.sprite);
            m_IconImg.sprite = null;
        }
        if (GTools.TableMgr.TryGetAssetPath(WeatherGainData.Data.WeatherGainIcon, out var path))
        {
            m_IconImg.sprite = await ILoadSpriteAsync.LoadAsync(path);
        }
    }
}
