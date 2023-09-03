using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_WeatherGainData : UIItemBaseData<WeatherGainRandomData>
{
    public override AssetKey AssetPrefabID => AssetKey.UIItem_WeatherGain;

    private UIItem_WeatherGain TargetIns => GetCom<UIItem_WeatherGain>();
    public override void UpdateItemData(WeatherGainRandomData f_Data, Transform f_Parent = null)
    {
        base.UpdateItemData(f_Data, f_Parent);
        UpdateThis();
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        UpdateThis();
    }
    public void UpdateThis()
    {
        if (TargetIns != null)
        {
            TargetIns.UpdateThis();
        }
    }
    public void Click_Select()
    {
        GTools.WeatherMgr.SelectWeatherGainItem(Data);
    }
    public void Click_Update()
    {
        if (GTools.WeatherMgr.TryUpdateWeatherGainItem(Data.Index, out var newData))
        {
            UpdateItemData(newData);
        }
    }
}
public class UIItem_WeatherGain : UIItemBase
{
    [SerializeField]
    private Button m_SelectBtn = null;
    [SerializeField]
    private Button m_UpdateBtn = null;

    [SerializeField]
    private Image m_IconImg = null;
    [SerializeField]
    private TextMeshProUGUI m_NameTxt = null;
    [SerializeField]
    private TextMeshProUGUI m_DescribesTxt = null;
    [SerializeField]
    private Image m_MainBackgroundImg = null;

    public UIItem_WeatherGainData WeatherGainData => GetData<UIItem_WeatherGainData>();

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

        m_SelectBtn.onClick.AddListener(Click_Select);
        m_UpdateBtn.onClick.AddListener(Click_Update);
    }
    public override async UniTask OnUnLoadAsync()
    {
        if (m_IconImg.sprite != null)
        {
            ILoadSpriteAsync.UnLoad(m_IconImg.sprite);
            m_IconImg.sprite = null;
        }
        m_SelectBtn.onClick.RemoveAllListeners();
        m_UpdateBtn.onClick.RemoveAllListeners();

        await base.OnUnLoadAsync();
    }
    public async void UpdateThis()
    {
        var data = WeatherGainData.Data;
        m_IconImg.gameObject.SetActive(false);
        if (m_IconImg.sprite != null)
        {
            ILoadSpriteAsync.UnLoad(m_IconImg.sprite);
            m_IconImg.sprite = null;
        }
        if (GTools.TableMgr.TryGetAssetPath(data.WeatherGainIcon, out var path))
        {
            var sprite = await ILoadSpriteAsync.LoadAsync(path);
            m_IconImg.sprite = sprite;
            m_IconImg.gameObject.SetActive(true);
        }
        m_NameTxt.text = data.WeatherGainInfo.Name;
        m_DescribesTxt.text = data.WeatherGainInfo.Describe;
        m_MainBackgroundImg.color = data.WeatherLevelInfo.Color;
    }
    public void Click_Select()
    {
        WeatherGainData.Click_Select();
    }
    public void Click_Update()
    {
        WeatherGainData.Click_Update();
    }
}
