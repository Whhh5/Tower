using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMapFlagPoint : MonoBehaviour
{
    public bool m_IsEnable = false;
    /// <summary>
    /// �Ƿ����ڱ�׷��
    /// </summary>
    [SerializeField]
    private bool m_IsTracing;
    /// <summary>
    /// ��ǰ��ǵ�����
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI m_Name;
    /// <summary>
    /// ���ͼ��
    /// </summary>
    [SerializeField]
    private Image m_Icon = null;
    /// <summary>
    /// ׷����Ч
    /// </summary>
    [SerializeField]
    private GameObject m_TraceEffect = null;
    /// <summary>
    /// �� ui ��ͼ����� ʵ��λ��
    /// </summary>
    [SerializeField]
    public UIMapFlagPointBase m_Base;
    /// <summary>
    /// ����Ļ�����λ��
    /// </summary>
    [SerializeField]
    private Vector2 m_InViewPos;
    /// <summary>
    /// ��ǰ��ǵ� �����ť
    /// </summary>
    [SerializeField]
    private Button m_Button;
    /// <summary>
    /// MapPointStrategy ��Ӧ������
    /// </summary>
    public MapPointStrategy m_TableData;
    /// <summary>
    /// ׷��ʱ������Ļ�ľ���
    /// </summary>
    [SerializeField, Range(0, 200)]
    private float m_ToDageDistance;
    /// <summary>
    /// ��ǰ��ǵ㱻��¼�Ŀ�
    /// </summary>
    [SerializeField]
    public int m_ChunkIndex;
    /// <summary>
    /// ��ǰ��ǵ� ׷������
    /// </summary>
    [SerializeField]
    public EUIMapTracePointType m_TraceType;
    /// <summary>
    /// ͼƬ��Դ
    /// </summary>
    [SerializeField]
    private List<(string layer, string assetPath)> m_SpriteList = new List<(string layer, string assetPath)>();

    private RectTransform m_Rect => GetComponent<RectTransform>();
    /// <summary>
    /// ��ť����¼�
    /// </summary>
    private Action m_OnClickEvent = null;

    #region Tools
    public void Log(object message)
    {
        Debug.Log($"�� {GetType()} ��:\n {message}");
    }
    #endregion

    #region ���������
    public static bool operator >(UIMapFlagPoint item1, UIMapFlagPoint item2)
    {
        return item1.m_TableData.nPriority > item2.m_TableData.nPriority;
    }
    public static bool operator <(UIMapFlagPoint item1, UIMapFlagPoint item2)
    {
        return item1.m_TableData.nPriority < item2.m_TableData.nPriority;
    }
    #endregion

    #region ���� ״̬
    /// <summary>
    /// �ı䵱ǰ��ǵ��׷��״̬
    /// </summary>
    /// <returns></returns>
    public bool ChangeTrace(bool? f_IsTrace = null)
    {
        m_IsTracing = f_IsTrace ?? !m_IsTracing;

        #region ����Ч������ʾ����
        m_TraceEffect?.SetActive(m_IsTracing);
        #endregion

        #region ǿ��ˢ��һ��λ��
        var pos = UIMapFlagPointManager.Instance.GetFlagPointPosInView(m_Base.m_LocalPosition);
        m_InViewPos = pos.canvasPos;
        m_Rect.anchoredPosition3D = m_IsTracing ? pos.canvasPos : m_Base.m_LocalPosition;
        #endregion

        return m_IsTracing;
    }
    public void SetParameters<T>(T m_BaseData) where T : UIMapFlagPointBase
    {
        OnDestroy();

        m_IsEnable = true;
        m_Base = m_BaseData;
        m_ChunkIndex = m_BaseData.m_MapPositionInfo.index;

        m_OnClickEvent += () => Log($"{m_BaseData.m_FlagType}");
        m_Name.text = m_Base.m_Name;

        #region ����ͼ��
        var layer = $"{m_BaseData.m_FlagType}_{gameObject.GetInstanceID()}";
        var assetPath = $"Art/UIMap/FlagPointIcon/FlagPointIcon_{(int)m_BaseData.m_FlagType}";
        UIMapAssetsManager.Instance.LoadSpriteAssets(m_Icon, layer, assetPath);
        m_SpriteList.Add((layer, assetPath));
        #endregion

        switch (m_BaseData.m_FlagType)
        {
            case EUIMapFlagPointType.None:
                None();
                break;
            case EUIMapFlagPointType.NPC:
                NPC();
                break;
            case EUIMapFlagPointType.Monster:
                Monster(m_BaseData as FlagPointMonsterInfo);
                break;
            case EUIMapFlagPointType.TransferNPC:
                TransferNPC();
                break;
            case EUIMapFlagPointType.Collection:
                Collection();
                break;
            case EUIMapFlagPointType.TransferPortal:
                TransferPortal();
                break;
            case EUIMapFlagPointType.CarriagePoint:
                CarriagePoint();
                break;
            case EUIMapFlagPointType.AvailableTask:
                AvailableTask();
                break;
            case EUIMapFlagPointType.DeliverableTask:
                DeliverableTask();
                break;
            case EUIMapFlagPointType.Incident:
                Incident();
                break;
            case EUIMapFlagPointType.FieldAnchor:
                FieldAnchor();
                break;
            case EUIMapFlagPointType.CitiesAndTownsAnchor:
                CitiesAndTownsAnchor();
                break;
            case EUIMapFlagPointType.ManufacturingStation:
                ManufacturingStation();
                break;
            case EUIMapFlagPointType.FlagPoint:
                FlagPoint(m_BaseData as FlagPointInfo);
                break;
            case EUIMapFlagPointType.Teammate:
                Teammate();
                break;
            case EUIMapFlagPointType.Campsite:
                Campsite();
                break;
            case EUIMapFlagPointType.UnderwayTask:
                UnderwayTask();
                break;
            case EUIMapFlagPointType.IncidentRange:
                IncidentRange();
                break;
            case EUIMapFlagPointType.Hero:
                Hero();
                break;
            case EUIMapFlagPointType.CopyEntrance:
                CopyEntrance();
                break;
            case EUIMapFlagPointType.Boss:
                Boss();
                break;
            case EUIMapFlagPointType.MinAreaText:
                MinAreaText();
                break;
            case EUIMapFlagPointType.MaxAreaText:
                MaxAreaText();
                break;
            case EUIMapFlagPointType.TownBoard:
                TownBoard();
                break;
            case EUIMapFlagPointType.EnumCount:
                EnumCount();
                break;
            default:
                break;
        }

    }
    #endregion

    #region ���ں���
    public void Awake()
    {

    }
    public void Start()
    {

    }
    public void OnEnable()
    {
        m_Button.onClick.AddListener(OnButtonClick);
        UIMapManager.Instance.m_LiftCycle.UpdateFrameRate00 += Update00;

    }
    private void OnDisable()
    {
        m_Button.onClick.RemoveAllListeners();
        UIMapManager.Instance.m_LiftCycle.UpdateFrameRate00 -= Update00;
    }
    private void Update00()
    {
        // ���õ�ǰ��ǵ�λ��
        if (m_IsTracing || m_Base.m_MovementType == EUIMapFlagMoveType.Move)
        {
            var isViewShow = UIMapFlagPointManager.Instance.GetFlagPointPosInView(m_Base.m_LocalPosition);
            m_InViewPos = isViewShow.canvasPos;
            m_Rect.anchoredPosition3D = m_IsTracing ?
                m_InViewPos :
                m_Base.m_LocalPosition;
        }
    }
    public void OnDestroy()
    {
        #region �ͷ�ͼƬ��Դ
        foreach (var sprite in m_SpriteList)
        {
            UIMapAssetsManager.Instance.UnLoadSpriteAssets(sprite.layer, sprite.assetPath);
        }
        m_SpriteList = new List<(string layer, string assetPath)>();
        #endregion

        m_OnClickEvent = null;
        m_Base = null;
        m_IsEnable = false;
    }
    #endregion

    private void OnButtonClick()
    {
        var isViewShow = UIMapFlagPointManager.Instance.GetFlagPointPosInView(m_Base.m_LocalPosition);
        if (isViewShow.isViewShow)
        {
            var rangePoint = UIMapFlagPointManager.Instance.GetRangePoint(this);
            if (rangePoint.Count > 1)
            {
                UIMapFlagPointManager.Instance.ShowFlagPointSelectWindow(rangePoint);
            }
            else
            {
                OnClickEventInvoke();
            }
        }
        else
        {
            UIMapManager.Instance.MoveMapCentreToAnchored2D(m_Base.m_LocalPosition);
        }
    }
    public void OnClickEventInvoke()
    {
        m_OnClickEvent?.Invoke();
    }


    #region ����ӿ�
    public bool GetTraceState()
    {
        return m_IsTracing;
    }
    #endregion



    #region Function
    private void None()
    {

    }
    private void NPC()
    {

    }
    private void Monster(FlagPointMonsterInfo f_MonsterInfo)
    {
        m_Base.OnClickUpdate05 = () =>
              {
                  Log("Monster Update");
                  return m_Base.m_WorldPos;
              };
    }
    private void TransferNPC()
    {

    }
    private void Collection()
    {

    }
    private void TransferPortal()
    {

    }
    private void CarriagePoint()
    {

    }
    private void AvailableTask()
    {

    }
    private void DeliverableTask()
    {

    }
    private void Incident()
    {

    }
    private void FieldAnchor()
    {

    }
    private void CitiesAndTownsAnchor()
    {

    }
    private void ManufacturingStation()
    {

    }
    private void FlagPoint(FlagPointInfo f_FLagPointInfo)
    {
        Log($"pointType = {f_FLagPointInfo.m_FlagType}     worldPos = {f_FLagPointInfo.m_WorldPos}    type = {f_FLagPointInfo.m_Type}");
    }
    private void Teammate()
    {

    }
    private void Campsite()
    {

    }
    private void UnderwayTask()
    {

    }
    private void IncidentRange()
    {

    }
    private void Hero()
    {

    }
    private void CopyEntrance()
    {

    }
    private void Boss()
    {

    }
    private void MinAreaText()
    {

    }
    private void MaxAreaText()
    {

    }
    private void TownBoard()
    {

    }
    private void EnumCount()
    {

    }
    #endregion
}