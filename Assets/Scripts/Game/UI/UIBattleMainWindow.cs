using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBattleMainWindow : MonoBehaviour, IUpdateBase
{
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ��������
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--


    private void Awake()
    {
        m_CardViewItem.gameObject.SetActive(false);
        m_ItemCard.gameObject.SetActive(false);
        m_ItemHero.gameObject.SetActive(false);
        m_TargetIcon.gameObject.SetActive(false);
        m_TargetArrow.gameObject.SetActive(false);
        for (int i = 0; i < GTools.CardGroupCount; i++)
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
    //                                catalogue -- ѡ�����б�
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
        if (GTools.CardMgr.TryGetCardGroup(out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                switch (data.CardType)
                {
                    case ECardType.Skill:
                        var skillData = data.GetTargetType<SkillCardInfo>();
                        AddCardSkill(i, skillData);
                        break;
                    case ECardType.Incubator:
                        var incubatorData = data.GetTargetType<IncubatorCardInfo>();
                        AddCardIncubator(i, incubatorData);
                        break;
                    case ECardType.HeroPerson:
                        var heroData = data.GetTargetType<HeroCardInfo>();
                        AddCardHero(i, heroData);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    private void AddCardIncubator(int f_Index, IncubatorCardInfo f_IncubatorData)
    {
        var quality = f_IncubatorData.QualityLevel;
        var item = m_ItemList[f_Index];
        if (GTools.TableMgr.TryGetHeroCradLevelInfo(quality, out var qualityInfo) && GTools.TableMgr.TryGetIncubatorInfo(quality, out var incubatorInfo))
        {
            CreateSelectCardItem(f_Index, incubatorInfo.Name, qualityInfo.Color, incubatorInfo.Expenditure, incubatorInfo.IncubatorDebrisIcon, () =>
            {
                if (HeroIncubatorPoolMgr.Ins.BuyIncubator(incubatorInfo))
                {
                    item.gameObject.SetActive(false);
                    AddIncubaorDebris(quality);
                }
            });
        }
    }
    private void AddCardHero(int f_Index, HeroCardInfo f_IncubatorData)
    {
        var heroType = f_IncubatorData.HeroType;
        var item = m_ItemList[f_Index];
        if (GTools.TableMgr.TryGetHeroCradInfo(heroType, out var heroInfo)
            && GTools.TableMgr.TryGetHeroCradLevelInfo(heroInfo.QualityLevel, out var heroLevelInfo))
        {
            CreateSelectCardItem(f_Index, heroInfo.Name, heroLevelInfo.Color, heroLevelInfo.Expenditure, heroInfo.Icon, () =>
            {
                if (HeroIncubatorPoolMgr.Ins.BuyIncubator(heroLevelInfo))
                {
                    item.gameObject.SetActive(false);
                    AddHeroDebris(heroType);
                }
            });
        }
    }
    private void CreateSelectCardItem(int f_Index, string f_Name, Color f_QuaColor, int f_Expenditure, EAssetKey f_Icon, Action f_ClickEvent)
    {
        var item = m_ItemList[f_Index];
        m_Content.GetComponent<HorizontalLayoutGroup>().enabled = true;
        m_Content.GetComponent<ContentSizeFitter>().enabled = true;
        item.gameObject.SetActive(true);
        item.Find("Txt_Name").GetComponent<TextMeshProUGUI>().text = $"{f_Name}";
        item.Find("Img_Quality").GetComponent<Image>().color = f_QuaColor;
        item.Find("Tex_Expenditure").GetComponent<TextMeshProUGUI>().text = $"{f_Expenditure}";
        GTools.RunUniTask(async () =>
        {
            if (GTools.TableMgr.TryGetAssetPath(f_Icon, out var path))
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
            f_ClickEvent.Invoke();
        });
    }
    private void AddCardSkill(int f_Index, SkillCardInfo f_SkillData)
    {
        var skillType = f_SkillData.SkillType;
        var heroData = f_SkillData.HeroData;

        var item = m_ItemList[f_Index];
        if (GTools.TableMgr.TryGetPersonSkillInfo(skillType, out var skillInfo))
        {
            m_Content.GetComponent<HorizontalLayoutGroup>().enabled = true;
            m_Content.GetComponent<ContentSizeFitter>().enabled = true;
            item.gameObject.SetActive(true);
            item.Find("Txt_Name").GetComponent<TextMeshProUGUI>().text = $"{skillInfo.SkillName}";
            item.Find("Img_Quality").GetComponent<Image>().color = skillInfo.GetColor();
            item.Find("Tex_Expenditure").GetComponent<TextMeshProUGUI>().text = $"{skillInfo.Expenditure}";
            GTools.RunUniTask(async () =>
            {
                if (GTools.TableMgr.TryGetAssetPath(skillInfo.IconKey, out var path))
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
                if (HeroIncubatorPoolMgr.Ins.BuyIncubator(skillInfo))
                {
                    var addSkillResult = GTools.HeroMgr.AddSkillHero(heroData, skillType);
                    if (addSkillResult.Result == EResult.Defeated)
                    {
                        Debug.Log(addSkillResult.Value);
                        return;
                    }
                    item.gameObject.SetActive(false);
                }
            });
        }
    }
    public void UpLevel()
    {

    }




    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- �ѹ������б�
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    class IncubatorDebrisData
    {
        public IncubatorDebrisData(EQualityType f_QualityLevel, Transform f_Item)
        {
            QualityLevel = f_QualityLevel;
            Load(f_Item);
            SetQuality();
        }
        public Transform Item = null;
        public EQualityType QualityLevel;

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
        public async void SetIcon(EAssetKey f_AssetKey)
        {
            if (GTools.TableMgr.TryGetAssetPath(f_AssetKey, out var path))
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
            target.text = $"��{f_Name}";
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

    private Dictionary<EQualityType, List<IncubatorDebrisData>> m_CurCardInsList = new();
    private int ResultantQuanatity => GTools.ResultantQuanatity;

    /// <summary>
    /// ���Ӣ����Ƭ
    /// </summary>
    public void AddIncubaorDebris(EQualityType f_Quality, int f_Count = 1)
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
    public void UpdateIncubatorInfo(EQualityType f_Quality)
    {
        if (m_CurCardInsList.TryGetValue(f_Quality, out var list))
        {
            var count = (false ? ResultantQuanatity : 1);
            while (list.Count >= count)
            {
                AddIncubator(f_Quality);

                for (int i = 0; i < count; i++)
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
    //                                catalogue -- ���Ӣ�ۿ���
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<EHeroCardType, List<IncubatorDebrisData>> m_CurHeroCardInsList = new();
    public void AddHeroDebris(EHeroCardType f_HeroType, int f_Count = 1)
    {
        if (!m_CurHeroCardInsList.TryGetValue(f_HeroType, out var info))
        {
            info = new();
            m_CurHeroCardInsList.Add(f_HeroType, info);
        }
        if (!GTools.TableMgr.TryGetHeroCradInfo(f_HeroType, out var heroInfo))
        {
            return;
        }
        var item = new IncubatorDebrisData(heroInfo.QualityLevel, m_ItemCard);

        item.SetName(heroInfo.Name);
        item.SetIcon(heroInfo.Icon);

        info.Add(item);
        UpdateHeroInfo(f_HeroType);
    }
    public void UpdateHeroInfo(EHeroCardType f_HeroType)
    {
        if (m_CurHeroCardInsList.TryGetValue(f_HeroType, out var list))
        {
            var count = (false ? ResultantQuanatity : 1);
            while (list.Count >= count)
            {
                AddHero(f_HeroType);

                for (int i = 0; i < count; i++)
                {
                    var item = list[0];
                    item.UnLoad();
                    list.Remove(item);
                }
                if (list.Count == 0)
                {
                    m_CurHeroCardInsList.Remove(f_HeroType);
                    break;
                }
            }
        }
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ���ٻ�Ӣ���б��б�
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    [SerializeField]
    private RectTransform m_CurHeroList = null;
    [SerializeField]
    private Button m_BtnHeroList = null;

    [SerializeField]
    private Transform m_ItemHero = null;

    private Dictionary<EQualityType, Dictionary<EHeroCradStarLevel, List<IncubatorDebrisData>>> m_IncubetorInsList = new();
    private Dictionary<EHeroCardType, Dictionary<EHeroCradStarLevel, List<IncubatorDebrisData>>> m_HeroInsList = new();

    public void AddIncubator(EQualityType f_Quality, EHeroCradStarLevel f_StarLevel = EHeroCradStarLevel.Level1)
    {
        if (!m_IncubetorInsList.TryGetValue(f_Quality, out var list))
        {
            list = new();
            m_IncubetorInsList.Add(f_Quality, list);
        }
        var starLevel = f_StarLevel;
        var item = GetIncubatorItem(f_Quality, starLevel);
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
    private void RemoveIncubator(IncubatorDebrisData f_Item, EHeroCradStarLevel f_StarLevel)
    {
        if (m_IncubetorInsList.TryGetValue(f_Item.QualityLevel, out var list))
        {
            if (list.TryGetValue(f_StarLevel, out var datas))
            {
                f_Item.UnLoad();
                datas.Remove(f_Item);
            }
        }
    }
    private void AddHero(EHeroCardType f_HeroType, EHeroCradStarLevel f_StarLevel = EHeroCradStarLevel.Level1)
    {
        if (!m_HeroInsList.TryGetValue(f_HeroType, out var list))
        {
            list = new();
            m_HeroInsList.Add(f_HeroType, list);
        }
        var starLevel = f_StarLevel;
        var item = GetHeroItem(f_HeroType, starLevel);
        item.SetStarLevel((int)starLevel);
        if (GTools.TableMgr.TryGetHeroCradInfo(f_HeroType, out var heroInfo))
        {
            item.SetName(heroInfo.Name);
            item.SetIcon(heroInfo.Icon);
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
            AddHero(f_HeroType, f_StarLevel + 1);
        }

    }
    private IncubatorDebrisData GetIncubatorItem(EQualityType f_Quality, EHeroCradStarLevel f_StarLevel)
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
            m_CurIncubatorItem = item;
            SetTargetIcon(image.sprite);
            SetCurSelectIncubator(f_Quality, f_StarLevel);
        }
        return item;
    }
    private IncubatorDebrisData GetHeroItem(EHeroCardType f_HeroType, EHeroCradStarLevel f_StarLevel)
    {
        if (!GTools.TableMgr.TryGetHeroCradInfo(f_HeroType, out var heroInfo))
        {
            return null;
        }
        var item = new IncubatorDebrisData(heroInfo.QualityLevel, m_ItemHero);

        var triggerDown = new EventTrigger.Entry();
        var eventTrigger = item.Item.Find("Btn_Button").GetComponent<EventTrigger>();
        triggerDown.eventID = EventTriggerType.PointerDown;
        triggerDown.callback = new EventTrigger.TriggerEvent();
        triggerDown.callback.AddListener(DownClick);
        eventTrigger.triggers.Add(triggerDown);

        void DownClick(BaseEventData eventData)
        {
            var image = item.Item.Find("Img_Icon").GetComponent<Image>();
            m_CurIncubatorItem = item;
            SetTargetIcon(image.sprite);
            SetCurSelectHero(f_HeroType, f_StarLevel);
        }
        return item;
    }

    private EQualityType m_CurSelectIncubator = EQualityType.EnumCount;
    private EHeroCardType m_CurSelectHero = EHeroCardType.EnumCount;
    private EHeroCradStarLevel m_StarLevel = EHeroCradStarLevel.None;
    private IncubatorDebrisData m_CurIncubatorItem = null;
    public int UpdateLevelID { get; set; }
    [SerializeField]
    private RectTransform m_TargetIcon = null;
    [SerializeField]
    private RectTransform m_TargetArrow = null;


    [SerializeField]
    private Entity_HeroBaseData m_CurSelectHeroEntity = null;
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    private Vector2 m_MouseClickDownPos = Vector2.zero;
    public void SetCurSelectIncubator(EQualityType f_Quality, EHeroCradStarLevel f_StarLevel)
    {
        if (f_Quality != m_CurSelectIncubator)
        {
            var pos = IUIUtil.GetMouseUGUIPosition();
            m_MouseClickDownPos = pos;
            m_TargetArrow.anchoredPosition = pos;

            m_CurSelectIncubator = f_Quality;
            m_StarLevel = f_StarLevel;
            GTools.LifecycleMgr.AddUpdate(this);
            m_TargetIcon.gameObject.SetActive(true);
            m_TargetArrow.gameObject.SetActive(true);
        }
    }
    public void SetCurSelectHero(EHeroCardType f_HeroType, EHeroCradStarLevel f_StarLevel)
    {
        if (f_HeroType != m_CurSelectHero)
        {
            var pos = IUIUtil.GetMouseUGUIPosition();
            m_MouseClickDownPos = pos;
            m_TargetArrow.anchoredPosition = pos;

            m_CurSelectHero = f_HeroType;
            m_StarLevel = f_StarLevel;
            GTools.LifecycleMgr.AddUpdate(this);
            m_TargetIcon.gameObject.SetActive(true);
            m_TargetArrow.gameObject.SetActive(true);
        }
    }
    private void EndCurSelectHero()
    {
        m_TargetIcon.gameObject.SetActive(false);
        m_TargetArrow.gameObject.SetActive(false);
        m_CurSelectHeroEntity = null;
        var curSelectChunk = WorldMapMgr.Ins.GetCurMouseEnable();
        if (WorldMapMgr.Ins.TryGetChunkData(curSelectChunk, out var chunkData))
        {
            if (chunkData.CurObjectType == EWorldObjectType.Road)
            {
                if (m_CurSelectIncubator != EQualityType.EnumCount)
                {
                    PlaceIncubator(m_CurSelectIncubator, chunkData.Index);
                }
                if (m_CurSelectHero != EHeroCardType.EnumCount)
                {
                    GTools.HeroMgr.CreateHero(m_CurSelectHero, m_StarLevel, chunkData.Index, out var heroData, new());
                }
            }
        }
        m_CurSelectIncubator = EQualityType.EnumCount;
        m_CurSelectHero = EHeroCardType.EnumCount;
        m_StarLevel = EHeroCradStarLevel.None;
        m_CurIncubatorItem = null;
    }
    public void SetTargetIcon(Sprite f_Sprite)
    {
        m_TargetIcon.GetComponent<Image>().sprite = f_Sprite;
    }

    public void OnUpdate()
    {
        var pos = IUIUtil.GetMouseUGUIPosition();
        if (m_CurSelectIncubator != EQualityType.EnumCount
            || m_CurSelectHero != EHeroCardType.EnumCount)
        {
            m_TargetIcon.anchoredPosition = pos;
        }
        if (m_CurSelectHeroEntity != null)
        {

        }
        var forward = pos - m_MouseClickDownPos;
        m_TargetArrow.up = forward.normalized;
        var size = m_TargetArrow.sizeDelta;
        size.y = Vector2.Distance(forward, Vector2.zero);
        m_TargetArrow.sizeDelta = size;

        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurSelectHeroEntity != null && PathManager.Ins.TryGetAStarPath(m_CurSelectHeroEntity.CurrentIndex, WorldMapMgr.Ins.GetCurMouseEnable(), out var path))
            {
                m_CurSelectHeroEntity.SetPath(path);
            }
            EndCurSelectHero();
            GTools.LifecycleMgr.RemoveUpdate(this);
        }
    }
    private void PlaceIncubator(EQualityType f_IncubatorLevel, int f_ChunkIndex)
    {
        var incubator = new Entity_Incubator1Data();
        incubator.Initialization(0, f_ChunkIndex, f_IncubatorLevel, m_StarLevel);
        RemoveIncubator(m_CurIncubatorItem, m_StarLevel);
    }
}
