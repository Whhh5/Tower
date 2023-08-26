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
                if (GTools.TableMgr.TryGetHeroCradLevelInfo(data, out var qualityInfo) && GTools.TableMgr.TryGetIncubatorInfo(data, out var incubatorInfo))
                {
                    m_Content.GetComponent<HorizontalLayoutGroup>().enabled = true;
                    m_Content.GetComponent<ContentSizeFitter>().enabled = true;
                    item.gameObject.SetActive(true);
                    item.Find("Txt_Name").GetComponent<TextMeshProUGUI>().text = $"{incubatorInfo.Name}";
                    item.Find("Img_Quality").GetComponent<Image>().color = qualityInfo.Color;
                    item.Find("Tex_Expenditure").GetComponent<TextMeshProUGUI>().text = $"{incubatorInfo.Expenditure}";
                    GTools.RunUniTask(async () =>
                    {
                        if (GTools.TableMgr.GetAssetPath(incubatorInfo.IncubatorDebrisIcon, out var path))
                        {
                            var sprite = await ILoadSpriteAsync.LoadAsync(path);
                            item.Find("Img_Icon").GetComponent<Image>().sprite = sprite;
                        }
                    });
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
            SetQuality();
        }
        public Transform Item = null;
        public EHeroQualityLevel QualityLevel;

        private void Load(Transform f_Item)
        {
            UnLoad();
            Item = GameObject.Instantiate(f_Item, f_Item.parent);
            Item.gameObject.SetActive(true);
        }
        public void UnLoad()
        {
            if (Item != null)
            {
                ILoadSpriteAsync.UnLoad(Item.Find("Img_Icon").GetComponent<Image>().sprite);
                GameObject.Destroy(Item.gameObject);
                Item = null;
            }
        }
        public async void SetIcon(AssetKey f_AssetKey)
        {
            if (GTools.TableMgr.GetAssetPath(f_AssetKey, out var path))
            {
                var sprite = await ILoadSpriteAsync.LoadAsync(path);
                if (Item == null)
                {
                    ILoadSpriteAsync.UnLoad(sprite);
                    return;
                }
                var img = Item.Find("Img_Icon").GetComponent<Image>();
                img.sprite = sprite;
            }
        }
        public void SetName(string f_Name)
        {
            var target = Item.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
            target.text = $"×{f_Name}";
        }
        public void SetStarLevel(int f_Star)
        {
            if (Item != null)
            {
                var target = Item.Find("Txt_StarLevel").GetComponent<TextMeshProUGUI>();
                target.text = $"{f_Star} Star";
            }
        }
        public void SetQuality()
        {
            if (Item != null)
            {
                var target = Item.Find("Img_Quality").GetComponent<Image>();
                target.color = GTools.TableMgr.TryGetHeroCradLevelInfo(QualityLevel, out var info) ? info.Color : Color.white;
            }
        }
    }
    [SerializeField]
    private RectTransform m_CurCradList = null;
    [SerializeField]
    private Transform m_ItemCard = null;

    private Dictionary<EHeroQualityLevel, List<IncubatorDebrisData>> m_CurCardInsList = new();
    private int ResultantQuanatity => GTools.HeroIncubatorPoolMgr.ResultantQuanatity;

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
        if (GTools.TableMgr.TryGetIncubatorInfo(f_Quality, out var incubationInfo))
        {
            item.SetName(incubationInfo.Name);
            item.SetIcon(incubationInfo.IncubatorDebrisIcon);
        }
        info.Add(item);
        UpdateIncubatorInfo(f_Quality);
    }
    public void UpdateIncubatorInfo(EHeroQualityLevel f_Quality)
    {
        if (m_CurCardInsList.TryGetValue(f_Quality, out var list))
        {
            while (list.Count >= ResultantQuanatity)
            {
                AddIncubator(f_Quality);

                for (int i = 0; i < ResultantQuanatity; i++)
                {
                    var item = list[0];
                    item.UnLoad();
                    list.Remove(item);
                }
                if (list.Count == 0)
                {
                    m_CurCardInsList.Remove(f_Quality);
                    break;
                }
            }
        }
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 可召唤英雄列表列表
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    [SerializeField]
    private RectTransform m_CurHeroList = null;
    [SerializeField]
    private Button m_BtnHeroList = null;

    [SerializeField]
    private Transform m_ItemHero = null;

    private Dictionary<EHeroQualityLevel, Dictionary<EHeroCradStarLevel, List<IncubatorDebrisData>>> m_HeroInsList = new();

    public void AddIncubator(EHeroQualityLevel f_Quality, EHeroCradStarLevel f_StarLevel = EHeroCradStarLevel.Level1)
    {
        if (!m_HeroInsList.TryGetValue(f_Quality, out var list))
        {
            list = new();
            m_HeroInsList.Add(f_Quality, list);
        }
        var item = GetIncubatorItem(f_Quality);
        var starLevel = f_StarLevel;
        item.SetStarLevel((int)starLevel);
        if (GTools.TableMgr.TryGetIncubatorInfo(f_Quality, out var incubatorInfo))
        {
            item.SetName(incubatorInfo.Name);
            item.SetIcon(incubatorInfo.IncubatorIcon);
        }

        if (!list.TryGetValue(starLevel, out var datas))
        {
            datas = new();
            list.Add(starLevel, datas);
        }
        datas.Add(item);
        if (datas.Count >= ResultantQuanatity)
        {
            for (int i = 0; i < ResultantQuanatity; i++)
            {
                var target = datas[0];
                target.UnLoad();
                datas.Remove(target);
            }
            AddIncubator(f_Quality, f_StarLevel + 1);
        }
    }
    private IncubatorDebrisData GetIncubatorItem(EHeroQualityLevel f_Quality)
    {
        var item = new IncubatorDebrisData(f_Quality, m_ItemHero);

        var triggerDown = new EventTrigger.Entry();
        var eventTrigger = item.Item.Find("Btn_Button").GetComponent<EventTrigger>();
        triggerDown.eventID = EventTriggerType.PointerDown;
        triggerDown.callback = new EventTrigger.TriggerEvent();
        triggerDown.callback.AddListener(DownClick);
        eventTrigger.triggers.Add(triggerDown);

        void DownClick(BaseEventData eventData)
        {
            var image = item.Item.Find("Img_Icon").GetComponent<Image>();
            MoveCardMgr.Ins.SetTargetIcon(image.sprite);
            MoveCardMgr.Ins.SetCurSelectHero(f_Quality);
        }
        return item;
    }
}
