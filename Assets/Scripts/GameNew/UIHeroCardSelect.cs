using B1.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.Jobs;

public class UICardSelectData
{
    public EHeroCardType HeroType;
    public int Expenditure;
    public string Name;
    public Func<bool> OnClick;
    public EAssetKey Icon;
    public EAssetKey VocationalIcon;
    public EQualityType Quality;
}
public class UICardInfo
{
    private RectTransform m_Item;
    private Vector3 m_AnchPos;
    public void InitData(RectTransform f_Original)
    {
        OnDestroy();
        m_Item = GameObject.Instantiate(f_Original, f_Original.parent);
        SetPosX();
    }
    public void OnDestroy()
    {
        if (m_Item != null)
        {
            GameObject.Destroy(m_Item.gameObject);
            m_Item = null;
        }
    }
    public void SetPosX(float? f_LocalPosX = null)
    {
        var localPos = f_LocalPosX != null ? new Vector3(f_LocalPosX ?? 0, 0, 0) : m_AnchPos;
        m_AnchPos = localPos;
        if (m_Item != null)
        {
            m_Item.anchoredPosition = localPos;
        }
    }
    private UICardSelectData m_CurCardData = null;
    public async void UpdateItemInfo()
    {
        if (m_CurCardData != null)
        {
            GTools.HeroCardPoolMgr.RecycleGroupCrad(m_CurCardData.HeroType);
        }
        if (GTools.HeroCardPoolMgr.TryGetHeroCrad(out CardGroupInfos list))
        {
            var data = list.GetTargetType<HeroCardInfo>();
            if (!TryGetHeroCardInfo(data.HeroType, out var cardData))
            {
                return;
            }
            m_CurCardData = cardData;

            if (GTools.TableMgr.TryGetQualityInfo(cardData.Quality, out var qualityInfo))
            {
                m_Item.GetChildCom<Image>(EChildName.Img_Quality).color = qualityInfo.Color;
            }

            var spriteIcon = await ILoadSpriteAsync.LoadAsync(cardData.Icon);
            m_Item.GetChildCom<Image>(EChildName.Img_Icon).gameObject.SetActive(false);
            if (spriteIcon != null)
            {
                m_Item.GetChildCom<Image>(EChildName.Img_Icon).sprite = spriteIcon;
                m_Item.GetChildCom<Image>(EChildName.Img_Icon).gameObject.SetActive(true);
            }

            m_Item.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Name).text = cardData.Name;
            m_Item.GetChildCom<TextMeshProUGUI>(EChildName.Txt_GoldCount).text = cardData.Expenditure.ToString();

            var sprite2 = await ILoadSpriteAsync.LoadAsync(cardData.VocationalIcon);
            m_Item.GetChildCom<Image>(EChildName.Img_Vocational).gameObject.SetActive(false);
            if (sprite2 != null)
            {
                m_Item.GetChildCom<Image>(EChildName.Img_Vocational).sprite = sprite2;
                m_Item.GetChildCom<Image>(EChildName.Img_Vocational).gameObject.SetActive(true);
            }


            m_Item.GetChildCom<Button>(EChildName.Btn_Click).onClick.RemoveAllListeners();
            m_Item.GetChildCom<Button>(EChildName.Btn_Click).onClick.AddListener(() =>
            {
                if (!cardData.OnClick())
                {
                    return;
                }
                m_Item.gameObject.SetActive(false);
                m_CurCardData = null;
            });

            m_Item.gameObject.SetActive(true);
            UpdateStatus(true);
        }
        else
        {
            m_Item.gameObject.SetActive(false);
        }
    }
    private bool TryGetHeroCardInfo(EHeroCardType f_HeroType, out UICardSelectData f_HeroData)
    {
        f_HeroData = null;
        var heroType = f_HeroType;
        if (GTools.TableMgr.TryGetHeroCradInfo(heroType, out var heroInfo)
            && GTools.TableMgr.TryGetHeroCradLevelInfo(heroInfo.QualityLevel, out var levelInfo)
            && GTools.TableMgr.TryGetHeroVocationalInfo(heroInfo.Vocational, out var vocationalInfo))
        {
            f_HeroData = new UICardSelectData();
            f_HeroData.HeroType = heroType;
            f_HeroData.Name = heroInfo.Name;
            f_HeroData.Icon = heroInfo.Icon;
            f_HeroData.Quality = heroInfo.QualityLevel;
            f_HeroData.Expenditure = levelInfo.Expenditure;
            f_HeroData.VocationalIcon = vocationalInfo.IconID;
            f_HeroData.OnClick = () =>
            {
                // 添加到备战席
                GTools.AudioMgr.PlayAudio(EAudioType.Scene_BuyCard);
                var result = GTools.HeroCardPoolMgr.BuyCard(heroType);
                return result;
            };
        }
        return f_HeroData != null;
    }
    private bool m_CurStatus = true;
    public void UpdateStatus(bool? f_Staus = null)
    {
        if (m_CurCardData == null)
        {
            return;
        }
        var curAcive = f_Staus ?? m_CurCardData.Expenditure <= GTools.PlayerMgr.GetGoldCount();
        if (f_Staus == null && m_CurStatus == curAcive)
        {
            return;
        }
        m_CurStatus = curAcive;
        m_Item.GetChildCom<Button>(EChildName.Btn_Click).interactable = curAcive;
        var qualityImg = m_Item.GetChildCom<Image>(EChildName.Img_Quality);
        var color = qualityImg.color;
        color.a = curAcive ? 1 : 0.5f;
        qualityImg.color = color;
    }
}
[Serializable]
public class HeroResidueCountData
{
    public EHeroCardType HeroType;
    public RectTransform Item = null;
    public int MaxCount = 0;
    public async void InitData(EHeroCardType f_HeroType, RectTransform f_OriginalItem)
    {
        HeroType = f_HeroType;
        if (!GTools.TableMgr.TryGetHeroCradInfo(f_HeroType, out var heroInfo))
        {
            return;
        }
        Item = GameObject.Instantiate(f_OriginalItem, f_OriginalItem.parent);
        Item.gameObject.SetActive(true);
        var sprite = await ILoadSpriteAsync.LoadAsync(heroInfo.Icon);
        Item.GetChildCom<Image>(EChildName.Img_Icon).sprite = sprite;
        MaxCount = heroInfo.QualityLevelInfo.MaxCount;
    }
    public void OnDestroy()
    {
        if (Item != null)
        {
            GameObject.Destroy(Item.gameObject);
        }
    }
    public void UpdateItemCount()
    {
        var residueCount = GTools.HeroCardPoolMgr.GetHeroCardResidueCount(HeroType);
        Item.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Count).text = $"{residueCount}/{MaxCount}";
    }
}
public class UIHeroCardSelect : UIWindow
{
    class LoopData
    {
        public int CurCount = -1;
        public UICardInfo CardInfo = null;
    }
    [SerializeField]
    private RectTransform m_CardItem = null;
    [SerializeField]
    private Dictionary<int, LoopData> m_CardList = new();
    [SerializeField]
    private Button m_UpdateList = null;
    [SerializeField]
    private HorizontalLayoutGroup m_MainLayoutGroup = null;
    private RectTransform MainListRect => m_MainLayoutGroup.GetComponent<RectTransform>();
    private Vector3 MainListStartPos => new(0, -230, 0);
    private int HeroPoolCount => GameDataMgr.HeroPoolCount;

    [SerializeField]
    private float m_TempSliderValue = 0;
    [SerializeField]
    private float m_CardInterval = 15.0f;

    private float MaxWidth => m_CardItem.parent.GetComponent<RectTransform>().rect.width + ItemWidth + m_CardInterval;
    private float ItemWidth => m_CardItem.rect.width;

    [SerializeField]
    private RectTransform m_HeroCardResidueRoot = null;
    [SerializeField]
    private RectTransform m_ItemHeroCardResidueCount = null;
    [SerializeField]
    private List<HeroResidueCountData> m_HeroCardResidueList = new();
    [SerializeField]
    private TextMeshProUGUI m_CurGlodCount = null;
    [SerializeField]
    private TextMeshProUGUI m_CurMonsterWave = null;
    [SerializeField]
    private TextMeshProUGUI m_CurWaveMonsterCount = null;
    [SerializeField]
    private TextMeshProUGUI m_CurEnergyCount = null;

    [SerializeField, Header("按钮根节点"), Space(30)]
    private CanvasGroup m_BtnGroup = null;
    private RectTransform BtnGroupRect => m_BtnGroup.GetComponent<RectTransform>();
    private Vector3 BtnGroupStartPos => new Vector3(0, 100, 0);
    [SerializeField]
    private RectTransform m_GameInfo = null;
    private Vector3 GameInfostartPos => new Vector3(-250, 150, 0);

    [SerializeField, Header("设置界面"), Space(30)]
    private Button m_SettingBtn = null;

    [SerializeField, Header("阵型界面"), Space(30)]
    private Button m_FormationBtn = null;

    [SerializeField, Header("帮助界面"), Space(30)]
    private Button m_GameHelpBtn = null;

    private CreateMapNew CreateMapNew => GTools.CreateMapNew;

    public override async UniTask AwakeAsync()
    {
        InitShowElement();
        AwakeMonsterInfo();
    }
    public override async UniTask OnShowAsync()
    {

    }
    protected override void Update()
    {
        base.Update();
        UpdateGameView();
        UpdateMonsterCountList();
    }
    public override async UniTask OnUnLoadAsync()
    {
        UnLoadGameView();
        UnLoadMonsterInfo();
        await base.OnUnLoadAsync();
    }
    #region GameView
    private class SettingData
    {
        public string name;
        public Action click;
        public RectTransform item;
        public void InitData(RectTransform f_TargetItem)
        {
            var itemObj = GameObject.Instantiate(f_TargetItem, f_TargetItem.parent);
            var btn = itemObj.GetChildCom<Button>(EChildName.Btn_Click);
            var title = itemObj.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Title);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => click());
            title.text = name;
            itemObj.gameObject.SetActive(true);
            item = itemObj;
        }
        public void Destroy()
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private float m_CurTime = 0.0f;

    private void UnLoadGameView()
    {
        foreach (var item in m_CardList)
        {
            item.Value.CardInfo.OnDestroy();
        }
        m_CardList.Clear();
        foreach (var item in m_HeroCardResidueList)
        {
            item.OnDestroy();
        }
        m_HeroCardResidueList.Clear();
    }

    private void InitShowElement()
    {
        BtnGroupRect.anchoredPosition3D = BtnGroupStartPos;
        MainListRect.anchoredPosition3D = MainListStartPos;
        m_GameInfo.anchoredPosition3D = GameInfostartPos;


        GTools.AudioMgr.PlayBackground(EAudioType.Scene_Background);
        m_CardItem.gameObject.SetActive(false);
        m_ItemHeroCardResidueCount.gameObject.SetActive(false);
        m_UpdateList.onClick.RemoveAllListeners();
        m_UpdateList.onClick.AddListener(() =>
        {
            //UpdateCardList(GameDataMgr.LevelUpdateExpenditure);

        });

        m_GameHelpBtn.onClick.RemoveAllListeners();
        m_GameHelpBtn.onClick.AddListener(() =>
        {
            GTools.ShowGameHelpWindow(EGameHelpType.Common);
        });

        m_CurTime = 0;

        // 初始化列表
        var count = Mathf.CeilToInt(MaxWidth / (m_CardInterval + ItemWidth));
        m_CardInterval = (MaxWidth - ItemWidth * count) / count;
        for (int i = 0; i < count; i++)
        {
            if (m_CardList.TryGetValue(i, out var item))
            {
                continue;
            }
            item = new();
            UICardInfo cardInfo = new();
            cardInfo.InitData(m_CardItem);
            item.CurCount = -1;
            item.CardInfo = cardInfo;
            m_CardList.Add(i, item);
        }

        // 初始化剩余数量列表
        for (int i = 0; i < (int)EHeroCardType.EnumCount; i++)
        {
            var heroType = (EHeroCardType)i;
            if (!GTools.TableMgr.TryGetHeroCradInfo(heroType, out var _))
            {
                continue;
            }
            if (!GTools.TableMgr.TryGetHeroCradInfo(heroType, out var heroInfo))
            {
                continue;
            }
            if (heroInfo.QualityLevelInfo.MaxCount <= 0)
            {
                continue;
            }
            HeroResidueCountData data = new();
            data.InitData(heroType, m_ItemHeroCardResidueCount);
            m_HeroCardResidueList.Add(data);
        }

        m_SettingBtn.onClick.RemoveAllListeners();
        m_SettingBtn.onClick.AddListener(async () =>
        {
            await GTools.UIWindowManager.LoadWindowAsync<UISettingWindow>(EAssetName.UISettingWindow);

        });
        m_FormationBtn.onClick.RemoveAllListeners();
        m_FormationBtn.onClick.AddListener(async () =>
        {
            await UIWindowManager.Ins.LoadWindowAsync<UIFormationInfo>(EAssetName.UIFormationInfo);
        });
    }
    public void StartShowElement()
    {
        GTools.AudioMgr.PlayAudio(EAudioType.Scene_GameStart);
        DOTween.To(() => 0.0f, slider =>
        {
            var pos1 = Vector3.Lerp(BtnGroupStartPos, Vector3.zero, slider);
            var pos2 = Vector3.Lerp(MainListStartPos, Vector3.zero, slider);
            var pos3 = Vector3.Lerp(GameInfostartPos, Vector3.up * 150, slider);

            BtnGroupRect.anchoredPosition3D = pos1;
            MainListRect.anchoredPosition3D = pos2;
            m_GameInfo.anchoredPosition3D = pos3;

        }, 1.0f, 0.5f);
    }
    private void UpdateGameView()
    {
        m_CurTime += Time.deltaTime;
        for (int i = 0; i < m_CardList.Count; i++)
        {
            var cardLoopData = m_CardList[i];
            var posEndX = i * (ItemWidth + m_CardInterval) + m_CurTime * 50;
            var loopCount = Mathf.FloorToInt(posEndX / MaxWidth);
            var posX = posEndX % MaxWidth;
            cardLoopData.CardInfo.SetPosX(posX - (ItemWidth + m_CardInterval) * 0.5f);
            cardLoopData.CardInfo.UpdateStatus();
            if (cardLoopData.CurCount != loopCount)
            {
                cardLoopData.CurCount = loopCount;
                cardLoopData.CardInfo.UpdateItemInfo();
            }
        }

        foreach (var item in m_HeroCardResidueList)
        {
            item.UpdateItemCount();
        }

        // 刷新金币数量
        m_CurGlodCount.text = $"{GTools.PlayerMgr.GetGoldCount()}";

        // 刷新当前怪物波数
        m_CurMonsterWave.text = $"{CreateMapNew.GetCurWaveCount() + 1}/{CreateMapNew.GetMaxWaveCount()}";

        // 刷新当前怪物数量
        m_CurWaveMonsterCount.text = $"{CreateMapNew.GetCurWaveMonsterActiveCount()}/{CreateMapNew.GetCurWaveMonsterCount()}";

        // 刷新当前剩余能量站数量
        m_CurEnergyCount.text = $"{CreateMapNew.GetCurActiveEnergyCount()}/{CreateMapNew.GetCurMaxExergyCount()}";
    }
    private void ReturnSelectWindow()
    {

    }
    #endregion


    #region MonsterInfo

    private class MonsterWaveItem
    {
        private RectTransform m_Item = null;

        public void Init(RectTransform f_Item)
        {
            m_Item = GameObject.Instantiate(f_Item, f_Item.parent);
            m_Item.gameObject.SetActive(true);
        }
        public void SetActive(bool f_Active)
        {
            m_Item.gameObject.SetActive(f_Active);
        }
        public void SetItemSlider(float f_Value)
        {
            var slider = m_Item.GetChildCom<Image>(EChildName.Img_Slider);
            slider.fillAmount = f_Value;
        }
        public void SetStatusColor(Color f_Color)
        {
            var point = m_Item.GetChildCom<RectTransform>(EChildName.Tran_Point);
            var icon1 = point.GetChildCom<Image>(EChildName.Img_Icon1);
            var icon2 = point.GetChildCom<Image>(EChildName.Img_Icon2);
            icon1.color = icon2.color = f_Color;
        }
        public void UnLoad()
        {
            GameObject.Destroy(m_Item.gameObject);
            m_Item = null;
        }
    }
    private class MonsterIconItem
    {
        private RectTransform m_Item = null;

        public void Init(RectTransform f_Item)
        {
            m_Item = GameObject.Instantiate(f_Item, f_Item.parent);
            m_Item.gameObject.SetActive(true);
        }
        public void SetActive(bool f_Active)
        {
            m_Item.gameObject.SetActive(f_Active);
        }
        public void SetIconStatus(bool f_Status)
        {
            var slider = m_Item.GetChildCom<Image>(EChildName.Img_Icon);
            slider.color = f_Status ? Color.white : Color.gray;
        }
        public void UnLoad()
        {
            GameObject.Destroy(m_Item.gameObject);
            m_Item = null;
        }
    }

    [SerializeField, Header("怪物信息顶部界面"), Space(30)]
    private RectTransform m_UITopTipsRoot = null;
    [SerializeField]
    private RectTransform m_MonsterWaveInfoRoot = null;
    [SerializeField]
    private RectTransform m_MonsterWaveItem = null;
    private Dictionary<int, MonsterWaveItem> m_MonsterWaveList = new();
    [SerializeField]
    private RectTransform m_MonsterCountRoot = null;
    [SerializeField]
    private RectTransform m_MonsterIconItem = null;
    private Dictionary<int, MonsterIconItem> m_MonsterIconList = new();
    private int m_LastWaveResidueMonsterCount = 0;
    [SerializeField]
    private TextMeshProUGUI m_Title = null;


    private async void AwakeMonsterInfo()
    {
        m_LastWave = -1;
        m_MonsterWaveItem.gameObject.SetActive(false);
        m_MonsterIconItem.gameObject.SetActive(false);
        //UpdateMonsterWaveList();
        //UpdateMonsterIconList();
        m_Title.text = GameDataMgr.Subject;

        SetMonsterInfoAlpha(0);
        var titleStartPos = new Vector3(0, 80, 0);
        var titleToPos = Vector3.zero;
        m_Title.GetComponent<RectTransform>().anchoredPosition3D = titleStartPos;


        var startPos = new Vector3(0, 100, 0);
        var toPos = Vector3.zero;
        await UniTask.Delay(2000);
        await DOTween.To(() => 0.0f, slider =>
          {
              var pos = Vector3.Lerp(startPos, toPos, slider);
              m_UITopTipsRoot.anchoredPosition3D = pos;

          }, 1.0f, 1.0f);

       
       
        await DOTween.To(() => 0.0f, slider =>
        {
            var pos = Vector3.Lerp(titleStartPos, titleToPos, slider);
            m_Title.GetComponent<RectTransform>().anchoredPosition3D = pos;

        }, 1.0f, 1.0f);

        await UniTask.Delay(500);

        await DOTween.To(() => 0.0f, slider =>
        {
            SetMonsterInfoAlpha(slider);

        }, 1.0f, 1.0f);

        void SetMonsterInfoAlpha(float f_Alpha)
        {
            m_MonsterCountRoot.GetComponent<CanvasGroup>().alpha = f_Alpha;
            m_MonsterWaveInfoRoot.GetComponent<CanvasGroup>().alpha = f_Alpha;
        }
    }
    private void UpdateMonsterCountList()
    {
        //var monsterMaxWave = CreateMapNew.GetMaxWaveCount();
        //var monsterCurWave = CreateMapNew.GetCurWaveCount() + 1;
        UpdateMonsterWaveList();

        //var monsterMaxCount = CreateMapNew.GetCurWaveMonsterCount();
        //var monsterCurCount = CreateMapNew.GetCurWaveMonsterActiveCount();
        UpdateMonsterIconList();


        var energyMaxCount = CreateMapNew.GetCurMaxExergyCount();
        var energyCurCount = CreateMapNew.GetCurActiveEnergyCount();

    }
    private void UpdateMonsterIconList()
    {
        var maxCount = CreateMapNew.GetMonsterCount();
        var curCount = m_MonsterIconList.Count;
        for (int i = curCount; i < maxCount; i++)
        {
            var data = new MonsterIconItem();
            data.Init(m_MonsterIconItem);

            m_MonsterIconList.Add(i, data);
        }

        for (int i = curCount; i < maxCount; i++)
        {
            var item = m_MonsterIconList[i];
            item.SetActive(false);
        }

        var monsterCurCount = CreateMapNew.GetMonsterActiveCount();
        for (int i = 0; i < maxCount; i++)
        {
            var item = m_MonsterIconList[i];
            item.SetActive(true);
            item.SetIconStatus(i < monsterCurCount);
        }
    }
    private float m_TempSlider = 0.0f;
    private float m_CurSliderValue = 0.0f;
    private int m_LastWave = -1;
    private void UpdateMonsterWaveList()
    {
        var maxCount = CreateMapNew.GetMaxWaveCount();
        var curCount = m_MonsterWaveList.Count;
        for (int i = curCount; i < maxCount; i++)
        {
            var data = new MonsterWaveItem();
            data.Init(m_MonsterWaveItem);

            m_MonsterWaveList.Add(i, data);
        }

        for (int i = curCount; i < maxCount; i++)
        {
            var item = m_MonsterWaveList[i];
            item.SetActive(false);
        }


        var monsterCurWave = CreateMapNew.GetCurWaveCount();
        for (int i = 0; i < maxCount; i++)
        {
            var item = m_MonsterWaveList[i];
            var isPass = i < monsterCurWave + 1;
            var slider = isPass ? 1.0f : 0.0f;
            if (i == monsterCurWave + 1 && GTools.CreateMapNew.TryGetNextWaveTime(out var value))
            {
                var residueTime = value - Time.time;
                var curWaveTime = GTools.CreateMapNew.GetCurWaveTime();
                m_TempSlider = 1 - residueTime / curWaveTime;
                if (m_LastWave != i)
                {
                    m_CurSliderValue = m_TempSlider;
                    m_LastWave = i;
                }
                slider = Mathf.Lerp(m_CurSliderValue, m_TempSlider, Time.deltaTime * 2.0f);
                m_CurSliderValue = slider;
            }
            item.SetItemSlider(slider);

            var color = i < monsterCurWave ? Color.green : Color.red;
            if (i == monsterCurWave)
            {
                color = Color.yellow;
                color.a = Mathf.Sin(Mathf.PI * (Time.time % 1));
            }
            item.SetStatusColor(color);
            item.SetActive(true);
        }
    }

    private void UnLoadMonsterInfo()
    {
        foreach (var item in m_MonsterIconList)
        {
            item.Value.UnLoad();
        }
        m_MonsterIconList.Clear();
        foreach (var item in m_MonsterWaveList)
        {
            item.Value.UnLoad();
        }
        m_MonsterWaveList.Clear();
    }


    #endregion

}
