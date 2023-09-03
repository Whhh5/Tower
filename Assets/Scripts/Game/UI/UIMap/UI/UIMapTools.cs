using System;
using UnityEngine;

#region ö������
public enum EUIMapFlagPointType
{
    None,
    /// <summary>
    /// ��ͨ NPC
    /// </summary>
    NPC,
    /// <summary>
    /// ����
    /// </summary>
    Monster,
    /// <summary>
    /// ���� NPC
    /// </summary>
    TransferNPC,
    /// <summary>
    /// �ɼ���
    /// </summary>
    Collection,
    /// <summary>
    /// ������
    /// </summary>
    TransferPortal,
    /// <summary>
    /// ����
    /// </summary>
    CarriagePoint,
    /// <summary>
    /// �ɽ�����
    /// </summary>
    AvailableTask,
    /// <summary>
    /// �ɽ�������
    /// </summary>
    DeliverableTask,
    /// <summary>
    /// �¼�
    /// </summary>
    Incident,
    /// <summary>
    /// Ұ��ê��
    /// </summary>
    FieldAnchor,
    /// <summary>
    /// ����ê��
    /// </summary>
    CitiesAndTownsAnchor,
    /// <summary>
    /// ����̨
    /// </summary>
    ManufacturingStation,
    /// <summary>
    /// ��ǵ�
    /// </summary>
    FlagPoint,
    /// <summary>
    /// ����
    /// </summary>
    Teammate,
    /// <summary>
    /// Ӫ��
    /// </summary>
    Campsite,
    /// <summary>
    /// ����������
    /// </summary>
    UnderwayTask,
    /// <summary>
    /// �¼���Χ
    /// </summary>
    IncidentRange,
    /// <summary>
    /// Ӣ��
    /// </summary>
    Hero,
    /// <summary>
    /// �������
    /// </summary>
    CopyEntrance,
    /// <summary>
    /// BOSS
    /// </summary>
    Boss,
    /// <summary>
    /// С�����ı�
    /// </summary>
    MinAreaText,
    /// <summary>
    /// �������ı�
    /// </summary>
    MaxAreaText,
    /// <summary>
    /// �������
    /// </summary>
    TownBoard,
    /// <summary>
    /// ��������
    /// </summary>
    EnumCount,
}

public enum EUIMapTracePointType
{
    None,
    Type1,
    Type2,
    Type3,
    EnumCount,
}

public enum EUIMapFlagPointInfoType
{
    None,
    Type1,
    Type2,
    Type3,
    Type4,
    Type5,
    Type6,
    Type7,
    Type8,
    Type9,
    Type10,
    Type11,
    Type12,
    Type13,
    Type14,
    EnumCount,
}
public enum EUIMapFlagMoveType
{
    Static,
    Move,
    EnumCount,
}

public enum EUIMapStatus
{
    BigMap = 1 << 0,
    MiniMap = 1 << 1,
    All = int.MaxValue,
}

public enum EUIMapAssetType : ushort
{
    None,
    UIMapChunk,
    UIMapFlagPoint,
    EnumCount,
}


public enum EUIMapDGIDType
{
    SetUIMapState,
    MoveMapCentreToAnchored2D,
    ShowFlagPointInfoTypeWindow,
    ChangMapScaleTo,
    UpdateFlagStateByUIMapSatus,
    Button3DOnMouseEnter,
    UIWeatherRandomGain_Animation,
    UIOnEnableInfos_Animation,
    EnumCount,
}

public enum EUIMapClick : ushort
{
    ChangeMapStatus,
    ChangeMapChunkStatus,
    EnumCount,
}
#endregion


#region �ӿ�

#endregion


public class UIMapPath
{
    public const string m_PrefabPath = "Prefabs/UIMap/";
    public const string m_MapChunkPath = "Art/UIMap/";
}


#region ��ǵ�����
[Serializable]
public class UIMapFlagPointBase
{
    /// <summary>
    /// ������ ��ǵ� ʵ��� ����λ��
    /// </summary>
    [SerializeField]
    public Vector3 m_WorldPos = default(Vector3);
    /// <summary>
    /// ���
    /// </summary>
    [SerializeField]
    public string m_Name = default(string);
    /// <summary>
    /// ��ǰ��ǵ�����
    /// </summary>
    public EUIMapFlagPointType m_FlagType = EUIMapFlagPointType.None;
    /// <summary>
    /// ��ǰ��ǵ�����
    /// </summary>
    public dynamic m_Dynamic = null;
    /// <summary>
    /// ��ǰ�� �ƶ�����
    /// </summary>
    [SerializeField]
    public EUIMapFlagMoveType m_MovementType = EUIMapFlagMoveType.Static;
    /// <summary>
    /// ��ǰ��ǵ���ʾ��ͼ
    /// </summary>
    [SerializeField]
    public EUIMapStatus m_ShowMapType = EUIMapStatus.All;
    /// <summary>
    /// update �¼� 0.5s / 1
    /// </summary>
    private Func<Vector3> m_OnClickUpdate05 = null;
    public Func<Vector3> OnClickUpdate05
    {
        private get => m_OnClickUpdate05;
        set => m_OnClickUpdate05 = value;
    }
    /// <summary>
    /// ��ȡ��ǰ��ǵ��� ui ��ͼ���� ��������Ϣ
    /// </summary>
    public (int index, int row, int col, Vector2 uimapPos) m_MapPositionInfo =>
        UIMapManager.Instance.GetChunkIndexByWorldPos(m_WorldPos);
    /// <summary>
    /// ��ǰ���� ui ��ͼ�����λ��
    /// </summary>
    public Vector2 m_LocalPosition => UIMapManager.Instance.WorldToUIMapPos(m_WorldPos);

    public UIMapFlagPointBase(EUIMapFlagMoveType f_MovementType = EUIMapFlagMoveType.Static)
    {
        m_MovementType = f_MovementType;
        if (f_MovementType == EUIMapFlagMoveType.Move)
            UIMapManager.Instance.m_LiftCycle.UpdateFrameRate05 += Update05;
    }

    ~UIMapFlagPointBase()
    {
        if (m_MovementType == EUIMapFlagMoveType.Move)
            UIMapManager.Instance.m_LiftCycle.UpdateFrameRate05 -= Update05;
    }

    void Update05()
    {
        m_WorldPos = m_OnClickUpdate05?.Invoke() ?? m_WorldPos;
    }
}
[Serializable]
public sealed class FlagPointInfo : UIMapFlagPointBase
{
    public FlagPointInfo() : base(EUIMapFlagMoveType.Static) { }
    public EUIMapFlagPointInfoType? m_Type => m_Dynamic as EUIMapFlagPointInfoType?;
    //public EUIMapFlagPointInfoType m_Type = EUIMapFlagPointInfoType.Type1;
}
public sealed class FlagPointMonsterInfo : UIMapFlagPointBase
{
    private FlagPointMonsterInfo() : base(EUIMapFlagMoveType.Move) { }
    public Transform m_Target => m_Dynamic as Transform;
}


public enum EFlagPointInfo
{
    m_Type,
}
#endregion

