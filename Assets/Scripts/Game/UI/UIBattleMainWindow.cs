using B1.Event;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBattleMainWindow : MonoBehaviour
{
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--


    private void Awake()
    {
        m_CardViewItem.gameObject.SetActive(false);
        m_ItemCard.gameObject.SetActive(false);
        m_ItemHero.gameObject.SetActive(false);
        for (int i = 0; i < HeroIncubatorPoolMgr.Ins.CardGroupCount; i++)
        {
            var obj = GameObject.Instantiate(m_CardViewItem, m_CardViewItem.parent);
            m_ItemList.Add(obj);
        }

        m_Content.GetComponent<HorizontalLayoutGroup>().enabled = false;
        m_Content.GetComponent<ContentSizeFitter>().enabled = false;

        m_BtnUpdate.onClick.AddListener(UpdateCurCardList);
        m_BtnUpLevel.onClick.AddListener(UpLevel);
        var m_CurStatus = true;
        m_BtnShowList.onClick.AddListener(() =>
        {
            DOTween.Kill(1000);
            m_CurStatus = !m_CurStatus;

            var curWidth = m_RectScrollView.sizeDelta.x;
            var toWidth = m_CurStatus ? 1320 : 0;

            var curSize = m_CurCradList.sizeDelta;
            var toSize = m_CurStatus ? new Vector2(1260, 615) : Vector2.zero;
            DOTween.To(() => 0.0f, value =>
            {
                var curSizeX = curWidth + (toWidth - curWidth) * value;
                m_RectScrollView.sizeDelta = new Vector2(curSizeX, m_RectScrollView.sizeDelta.y);

                m_CurCradList.sizeDelta = curSize + new Vector2(toSize.x - curSize.x, toSize.y - curSize.y) * value;

            }, 1.0f, 0.5f)
            .SetId(1000)
            .OnStart(() =>
            {
                m_BtnUpdate.gameObject.SetActive(m_CurStatus);
                m_BtnUpLevel.gameObject.SetActive(m_CurStatus);
                if (m_CurStatus)
                {
                    m_RectScrollView.gameObject.SetActive(m_CurStatus);
                    m_CurCradList.gameObject.SetActive(m_CurStatus);
                }
            })
            .OnComplete(() =>
            {
                if (!m_CurStatus)
                {
                    m_RectScrollView.gameObject.SetActive(m_CurStatus);
                    m_CurCradList.gameObject.SetActive(m_CurStatus);
                }
            });

        });


        bool heroListStatus = true;
        m_BtnHeroList.onClick.AddListener(() =>
        {
            DOTween.Kill(1001);
            heroListStatus = !heroListStatus;


            var curPos = m_CurHeroList.anchoredPosition.x;
            var toPos = heroListStatus ? 20 : -m_CurHeroList.sizeDelta.x;
            DOTween.To(() => 0.0f, value =>
            {
                var curSizeX = curPos + (toPos - curPos) * value;
                m_CurHeroList.anchoredPosition = new Vector2(curSizeX, m_CurHeroList.anchoredPosition.y);

            }, 1.0f, 0.5f)
            .SetId(1001);
        });


        m_BtnShowList.onClick.Invoke();
        m_BtnHeroList.onClick.Invoke();
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 选择卡牌列表
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    [SerializeField]
    private Button m_BtnUpdate = null;
    [SerializeField]
    private Button m_BtnUpLevel = null;
    [SerializeField]
    private Button m_BtnShowList = null;
    [SerializeField]
    private RectTransform m_RectScrollView = null;

    [SerializeField]
    private Transform m_CardViewItem = null;

    private List<Transform> m_ItemList = new();

    [SerializeField]
    private Transform m_Content = null;





    public void UpdateCurCardList()
    {
        foreach (var item in m_ItemList)
        {
            item.gameObject.SetActive(false);
        }
        if (HeroIncubatorPoolMgr.Ins.TryGetIncybatorGroup(out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                var item = m_ItemList[i];
                if (GTools.TableMgr.TryGetHeroQualityInfo(data, out var info))
                {
                    m_Content.GetComponent<HorizontalLayoutGroup>().enabled = true;
                    m_Content.GetComponent<ContentSizeFitter>().enabled = true;
                    item.gameObject.SetActive(true);
                    item.Find("Txt_Name").GetComponent<TextMeshProUGUI>().text = $"{(int)data}级孵化器";
                    item.Find("Img_Quality").GetComponent<Image>().color = info.Color;
                    item.Find("Tex_Expenditure").GetComponent<TextMeshProUGUI>().text = info.Expenditure.ToString();
                    var btn = item.Find("Btn_Button").GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        m_Content.GetComponent<HorizontalLayoutGroup>().enabled = false;
                        m_Content.GetComponent<ContentSizeFitter>().enabled = false;
                        if (HeroIncubatorPoolMgr.Ins.BuyIncubator(data))
                        {
                            item.gameObject.SetActive(false);
                            AddIncubaorDebris(data);
                        }
                        Debug.Log(data);
                    });
                }
            }
        }
    }
    public void UpLevel()
    {

    }




    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 已购买卡牌列表
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    class IncubatorDebrisData
    {
        public IncubatorDebrisData(EHeroQualityLevel f_QualityLevel, Transform f_Item)
        {
            QualityLevel = f_QualityLevel;
            Load(f_Item);
        }
        private Transform m_Item = null;
        public EHeroQualityLevel QualityLevel;

        private void Load(Transform f_Item)
        {
            UnLoad();
            m_Item = GameObject.Instantiate(f_Item, f_Item.parent);
            m_Item.gameObject.SetActive(true);
        }
        public void UnLoad()
        {
            if (m_Item != null)
            {
                GameObject.Destroy(m_Item.gameObject);
                m_Item = null;
            }
        }
        public void SetName(string f_Name)
        {
            var target = m_Item.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
            target.text = $"×{f_Name}";
        }
        public void SetStarLevel(int f_Star)
        {
            if (m_Item != null)
            {
                var target = m_Item.Find("Txt_StarLevel").GetComponent<TextMeshProUGUI>();
                target.text = $"{f_Star} Star";
            }
        }
        public void SetQuality()
        {
            if (m_Item != null)
            {
                var target = m_Item.Find("Img_Quality").GetComponent<Image>();
                target.color = GTools.TableMgr.TryGetHeroQualityInfo(QualityLevel, out var info) ? info.Color : Color.white;
            }
        }
    }
    [SerializeField]
    private RectTransform m_CurCradList = null;
    [SerializeField]
    private Transform m_ItemCard = null;

    private Dictionary<EHeroQualityLevel, List<IncubatorDebrisData>> m_CurCardInsList = new();


    /// <summary>
    /// 添加英雄碎片
    /// </summary>
    public void AddIncubaorDebris(EHeroQualityLevel f_Quality, int f_Count = 1)
    {
        if (!m_CurCardInsList.TryGetValue(f_Quality, out var info))
        {
            info = new();
            m_CurCardInsList.Add(f_Quality, info);
        }
        var item = new IncubatorDebrisData(f_Quality, m_ItemCard);
        info.Add(item);
        UpdateIncubatorInfo(f_Quality);
    }
    public void UpdateIncubatorInfo(EHeroQualityLevel f_Quality)
    {
        if (m_CurCardInsList.TryGetValue(f_Quality, out var list))
        {
            while (list.Count >= GTools.HeroIncubatorPoolMgr.ResultantQuanatity)
            {
                AddIncubator(f_Quality);

                for (int i = 0; i < GTools.HeroIncubatorPoolMgr.ResultantQuanatity; i++)
                {
                    var item = list[0];
                    item.UnLoad();
                    list.Remove(item);
                }
                if (list.Count == 0)
                {
                    m_CurCardInsList.Remove(f_Quality);
                }
                else
                {
                    UpdateIncubatorInfo(f_Quality);
                }
            }
        }
    }
    public void UpdateCardInfo(EHeroCradType f_HeroCardType)
    {
        if (HeroIncubatorPoolMgr.Ins.TryGetCurCardInfo(f_HeroCardType, out var value))
        {
            if (!m_CurCardInsList.TryGetValue(f_HeroCardType, out var info))
            {
                info = new(f_HeroCardType);
                info.SetCount(value.ResidueCount);
                info.Load(m_ItemCard);
                m_CurCardInsList.Add(f_HeroCardType, info);
            }
            else if (info.Count != value.ResidueCount)
            {
                info.SetCount(value.ResidueCount);
            }
        }
        else if (m_CurCardInsList.TryGetValue(f_HeroCardType, out var info))
        {
            info.UnLoad();
            m_CurCardInsList.Remove(f_HeroCardType);
        }
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 可召唤英雄列表列表
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    class HeroInfo
    {
        public HeroInfo(EHeroCradType f_EHeroCradType)
        {
            HeroCradType = f_EHeroCradType;
        }
        public EHeroCradType HeroCradType;
        public EHeroCradStarLevel CradStarLevel;
        public HeroCradInfo HeroCradInfo => GTools.TableMgr.TryGetHeroCradInfo(HeroCradType, out var info) ? info : null;
        public HeroCardStarLevelInfo HeroCardStarLevelInfo => TableMgr.Ins.TryGetGeroCardStarLevelInfo(CradStarLevel, out var info) ? info : null;


        private Transform m_Item = null;
        public void Load(Transform f_Item)
        {
            UnLoad();
            m_Item = GameObject.Instantiate(f_Item, f_Item.parent);
            m_Item.gameObject.SetActive(true);

            SetQuality();
            SetStarLevel();
            var triggerDown = new EventTrigger.Entry();
            var eventTrigger = m_Item.Find("Btn_Button").GetComponent<EventTrigger>();
            triggerDown.eventID = EventTriggerType.PointerDown;
            triggerDown.callback = new EventTrigger.TriggerEvent();
            triggerDown.callback.AddListener(DownClick);
            eventTrigger.triggers.Add(triggerDown);
        }
        public void DownClick(BaseEventData eventData)
        {
            var image = m_Item.Find("Img_Icon").GetComponent<Image>();
            MoveCardMgr.Ins.SetTargetIcon(image.sprite);
            MoveCardMgr.Ins.SetCurSelectHero(HeroCradType);
        }
        public void UnLoad()
        {
            if (m_Item != null)
            {
                GameObject.Destroy(m_Item.gameObject);
                m_Item = null;
            }
        }
        public void SetQuality()
        {
            if (m_Item != null)
            {
                var target = m_Item.Find("Img_Quality").GetComponent<Image>();
                target.color = HeroCradInfo.QualityLevelInfo.Color;
            }
        }
        public void SetStarLevel(EHeroCradStarLevel? f_CradStarLevel = null)
        {
            CradStarLevel = f_CradStarLevel ?? CradStarLevel;
            if (m_Item != null)
            {
                var target = m_Item.Find("Txt_Star").GetComponent<TextMeshProUGUI>();
                target.text = $"{(int)CradStarLevel}*";
                target.color = HeroCardStarLevelInfo.Color;
            }
        }
    }
    [SerializeField]
    private RectTransform m_CurHeroList = null;
    [SerializeField]
    private Button m_BtnHeroList = null;

    [SerializeField]
    private Transform m_ItemHero = null;

    private Dictionary<EHeroCradType, HeroInfo> m_HeroInsList = new();

    public void AddIncubator(EHeroQualityLevel f_Quality)
    {

    }

    public void UpdateHeroInfo(EHeroCradType f_HeroCardType)
    {
        if (HeroIncubatorPoolMgr.Ins.TryGetCurCardInfo(f_HeroCardType, out var value))
        {
            if (!m_HeroInsList.TryGetValue(f_HeroCardType, out var info))
            {
                info = new(f_HeroCardType);
                info.SetStarLevel(value.StarLevel());
                info.Load(m_ItemHero);
                m_HeroInsList.Add(f_HeroCardType, info);
            }
            else if (value.StarLevel() != info.CradStarLevel)
            {
                info.SetStarLevel(value.StarLevel());
            }
        }
        else if (m_CurCardInsList.TryGetValue(f_HeroCardType, out var info))
        {
            info.UnLoad();
            m_HeroInsList.Remove(f_HeroCardType);
        }
    }
}
