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
public class UIHeroCardSelect : UIWindow, IPointerEnterHandler, IPointerExitHandler
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
    private Vector3 MainListStartPos => new Vector3(0, -200, 0);
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
    private Vector3 GameInfostartPos => new Vector3(-230, 200, 0);

    [SerializeField, Header("设置界面"), Space(30)]
    private GameObject m_SettingWindow = null;
    [SerializeField]
    private Button m_SettingBtn = null;
    [SerializeField]
    private Button m_SettingCloseBtn = null;
    [SerializeField]
    private RectTransform m_SettingBtnItem = null;

    [SerializeField, Header("阵型界面"), Space(30)]
    private Button m_FormationBtn = null;

    [SerializeField, Header("帮助界面"), Space(30)]
    private Button m_GameHelpBtn = null;

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
    private Dictionary<int, SettingData> m_SettingFuncList = new()
    {
        {
            1,
            new()
            {
                name = "返回",
                click = ()=>
                {
                    GameManager.ReturnSelectWindow();
                    GameManager.SetGameScale(1);
                },
                item = null,
            }
        },

    };

    public override async UniTask AwakeAsync()
    {
        InitShowElement();
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
        // 初始化设置列表
        m_SettingBtnItem.gameObject.SetActive(false);
        foreach (var item in m_SettingFuncList)
        {
            item.Value.InitData(m_SettingBtnItem);
        }
        m_SettingBtn.onClick.RemoveAllListeners();
        m_SettingBtn.onClick.AddListener(() =>
        {
            m_SettingWindow.SetActive(true);
            GameManager.SetGameScale(0);
        });
        m_FormationBtn.onClick.RemoveAllListeners();
        m_FormationBtn.onClick.AddListener(async () =>
        {
            await UIWindowManager.Ins.LoadWindowAsync<UIFormationInfo>(EAssetName.UIFormationInfo);
        });
        m_SettingCloseBtn.onClick.RemoveAllListeners();
        m_SettingCloseBtn.onClick.AddListener(() =>
        {
            m_SettingWindow.SetActive(false);
            GameManager.SetGameScale(1);
        });
        m_SettingWindow.SetActive(false);
    }

    private float m_CurTime = 0.0f;
    protected override void Update()
    {
        base.Update();

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

        var mapNewMgr = GTools.CreateMapNew;
        // 刷新金币数量
        m_CurGlodCount.text = $"{GTools.PlayerMgr.GetGoldCount()}";

        // 刷新当前怪物波数
        m_CurMonsterWave.text = $"{mapNewMgr.GetCurWaveCount() + 1}/{mapNewMgr.GetMaxWaveCount()}";

        // 刷新当前怪物数量
        m_CurWaveMonsterCount.text = $"{mapNewMgr.GetCurWaveMonsterActiveCount()}/{mapNewMgr.GetCurWaveMonsterCount()}";

        // 刷新当前剩余能量站数量
        m_CurEnergyCount.text = $"{mapNewMgr.GetCurActiveEnergyCount()}/{mapNewMgr.GetCurMaxExergyCount()}";
    }

    public override async UniTask OnShowAsync()
    {

    }

    public override async UniTask OnUnLoadAsync()
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
        foreach (var item in m_SettingFuncList)
        {
            item.Value.Destroy();
        }
        await base.OnUnLoadAsync();
    }

    private string DG_ID => $"asdasdasdasd";
    private CanvasGroup HeroResidueRootGroup => m_HeroCardResidueRoot.GetComponent<CanvasGroup>();
    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(DG_ID);
        var curAlpha = HeroResidueRootGroup.alpha;
        var time = 1 - curAlpha;
        DOTween.To(() => 0.0f, slider =>
          {
              var alpha = Mathf.Lerp(curAlpha, 1, slider);
              HeroResidueRootGroup.alpha = alpha;
              //m_BtnGroup.alpha = alpha;

          }, 1.0f, time * 0.5f)
            .SetId(DG_ID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(DG_ID);
        var curAlpha = HeroResidueRootGroup.alpha;
        var time = curAlpha;
        DOTween.To(() => 0.0f, slider =>
        {
            var alpha = Mathf.Lerp(curAlpha, 0, slider);
            HeroResidueRootGroup.alpha = alpha;
            //m_BtnGroup.alpha = alpha;

        }, 1.0f, time * 0.2f)
            .SetId(DG_ID);
    }

    private void InitShowElement()
    {
        BtnGroupRect.anchoredPosition3D = BtnGroupStartPos;
        MainListRect.anchoredPosition3D = MainListStartPos;
        m_GameInfo.anchoredPosition3D = GameInfostartPos;
        HeroResidueRootGroup.alpha = 0;
    }
    public void StartShowElement()
    {
        GTools.AudioMgr.PlayAudio(EAudioType.Scene_GameStart);
        DOTween.To(() => 0.0f, slider =>
          {
              var pos1 = Vector3.Lerp(BtnGroupStartPos, Vector3.zero, slider);
              var pos2 = Vector3.Lerp(MainListStartPos, Vector3.zero, slider);
              var pos3 = Vector3.Lerp(GameInfostartPos, Vector3.up * 200f, slider);

              BtnGroupRect.anchoredPosition3D = pos1;
              MainListRect.anchoredPosition3D = pos2;
              m_GameInfo.anchoredPosition3D = pos3;

          }, 1.0f, 0.5f);
    }

    private void ReturnSelectWindow()
    {

    }
}
