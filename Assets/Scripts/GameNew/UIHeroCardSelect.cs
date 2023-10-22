using B1.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroCardSelectData
{
    public int ExpenditureBase;
    public string Name;
    public Action OnClick;
    public AssetKey Icon;
    public EQualityType Quality;
}
public class UIHeroCardSelect : UIWindow
{
    [SerializeField]
    private Transform m_CardItem = null;
    [SerializeField]
    private Dictionary<int, Transform> m_CardList = new();

    public override async UniTask AwakeAsync()
    {
        
    }

    public override async UniTask OnShowAsync()
    {
        
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }

    public void CreatCardItem(int f_Index, UIHeroCardSelectData f_CardData)
    {

    }
    public void ClearCardItem(int f_Index)
    {

    }
    private async void SetCardItem(int f_Index, UIHeroCardSelectData f_CardData)
    {
        if (!m_CardList.TryGetValue(f_Index, out var item))
        {
            LogError($"Ë¢ÐÂÊ§°Ü Î´ÕÒµ½Ë÷Òý {f_Index}");
            return;
        }
        if (GTools.TableMgr.TryGetQualityInfo(f_CardData.Quality, out var qualityInfo))
        {
            item.GetChildCom<Image>(EChildName.Img_Quality).color = qualityInfo.Color;
        }

        var spriteIcon = await ILoadSpriteAsync.LoadAsync(f_CardData.Icon);
        if (spriteIcon != null)
        {
            item.GetChildCom<Image>(EChildName.Img_Icon).sprite = spriteIcon;
        }

    }
    public void UpdateCardList(int? f_GoldCount = null)
    {
        var count = f_GoldCount ?? GameDataMgr.LevelUpdateExpenditure;

        //if (GTools.HeroIncubatorPoolMgr.TryGetRandomCardByLevel)
        //{

        //}
    }
}
