using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IWorldObjectType
{
    public EWorldObjectType ObjectType { get; }
}
public class Entity_ChunkMapData : UnityObjectData
{
    public Entity_ChunkMapData() : base(0)
    {

    }
    public int ChunkIndex { get; private set; }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_ChunkMap;

    public override EWorldObjectType ObjectType => EWorldObjectType.None;

    public EWorldObjectType CurChunkObjType { get; private set; }
    public Entity_ChunkMap EntityTarget => GetCom<Entity_ChunkMap>();


    private Dictionary<EWorldObjectType, List<DependChunkData>> m_ObjDataList = new();

    public void InitData(int f_Index)
    {
        ChunkIndex = f_Index;
        CurChunkObjType = EWorldObjectType.Road;
    }
    public bool AddObject(DependChunkData f_Obj, bool f_isForce = false)
    {
        var objType = f_Obj.ObjectType;
        if (f_isForce || !IsExistObj(objType))
        {
            CurChunkObjType |= objType;
            if (!m_ObjDataList.TryGetValue(objType, out var list))
            {
                list = new();
                m_ObjDataList.Add(objType, list);
            }
            list.Add(f_Obj);
            return true;
        }
        return false;
    }
    public bool ClearObject(DependChunkData f_Obj)
    {
        var objType = f_Obj.ObjectType;
        if (!m_ObjDataList.TryGetValue(objType, out var list))
        {
            return false;
        }
        var result = list.Remove(f_Obj);
        if (f_Obj is WorldObjectBaseData objData)
        {
            objData.HideAttackRange();
        }
        if (result && list.Count == 0)
        {
            m_ObjDataList.Remove(objType);
        }
        CurChunkObjType &= ~objType;
        return result;
    }
    public bool IsPass()
    {
        return (CurChunkObjType == EWorldObjectType.Road);
    }
    public bool IsExistObj(EWorldObjectType f_DetectionTarget)
    {
        return (CurChunkObjType & f_DetectionTarget) != EWorldObjectType.None;
    }
    public bool IsExistObj(ELayer f_Layer, out List<DependChunkData> f_Result)
    {
        f_Result = new();
        foreach (var item in m_ObjDataList)
        {
            foreach (var data in item.Value)
            {
                if ((data.LayerMask & f_Layer) == 0)
                {
                    continue;
                }
                f_Result.Add(data);
            }
        }
        return f_Result.Count > 0;
    }
    public bool GetObjectByType(out List<DependChunkData> f_DataList, EWorldObjectType? f_ObjType = null)
    {
        var objType = f_ObjType;
        f_DataList = new();
        foreach (var item in m_ObjDataList)
        {
            if (objType != null)
            {
                if (objType == item.Key)
                {
                    f_DataList = item.Value;
                }
            }
            else
            {
                f_DataList.AddRange(item.Value);
            }
        }
        return f_DataList.Count > 0;
    }
    public bool IsExistObj(DependChunkData f_ObjData)
    {
        if (m_ObjDataList.TryGetValue(f_ObjData.ObjectType, out var list))
        {
            return list.Contains(f_ObjData);
        }
        return false;
    }
    public void OnMouseUp()
    {
        if (GTools.HeroCardPoolMgr.PathPointUpClick(this))
        {
            return;
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_Place);
        }
    }
    public void OnMouseEnter()
    {

    }
    public void OnMouseExit()
    {
        
    }
    public Color BaseColor => Color.white;
    public Color MainColor = Color.white;
    public void SetChunkColor(Color? f_Color = null)
    {
        var color = f_Color ?? BaseColor;
        MainColor = color;
        if (EntityTarget != null)
        {
            EntityTarget.SetChunkColor();
        }
    }
}
public class Entity_ChunkMap : ObjectPoolBase
{
    private string DGID => SaveID.ToString();
    private Entity_ChunkMapData Data => GetData<Entity_ChunkMapData>();
    [SerializeField]
    private List<SpriteRenderer> m_SpriteRenders = new();
    [SerializeField]
    private Color m_EnterToColor = Color.cyan;
    [SerializeField]
    private Color m_OriginalColor;
    [SerializeField]
    private Color m_CurColor;
    [SerializeField]
    private TextMeshPro m_Txt = null;

    private bool m_IsEnter = false;
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        m_CurColor = m_OriginalColor;
        UpdateColor();

        m_Txt.text = Data.ChunkIndex.ToString();
        SetChunkColor();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }


    private void OnMouseEnter()
    {
        m_IsEnter = true;
        StartEnter();

        if (Data.GetObjectByType(out var listData, EWorldObjectType.Preson))
        {
            foreach (var item in listData)
            {
                if (item is not WorldObjectBaseData objData)
                {
                    continue;
                }
                var color = (objData.LayerMask & ELayer.Player) == ELayer.Player ? Color.green : Color.red;
                objData.ShowAttackRange(color);
            }
        }
        Data.OnMouseEnter();
    }
    private void OnMouseExit()
    {
        m_IsEnter = false;
        StopEnter();

        if (Data.GetObjectByType(out var listData, EWorldObjectType.Preson))
        {
            foreach (var item in listData)
            {
                if (item is not WorldObjectBaseData objData)
                {
                    continue;
                }
                objData.HideAttackRange();
            }
        }

        Data.OnMouseExit();
    }
    private void OnMouseUp()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_IsEnter && Input.GetMouseButtonUp(0))
        {
            Data.OnMouseUp();
        }
    }

    private void StartEnter()
    {
        if (false)
        {
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_EnterChunk);
        }
        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_EnterToColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
        {
            m_CurColor = Color.Lerp(curColor, m_EnterToColor, value);
            UpdateColor();

        }, 1.0f, moveTime * 0.1f)
            .SetId(DGID)
            .SetEase(Ease.OutExpo);
    }
    private void StopEnter()
    {
        if (false)
        {
            GTools.AudioMgr.PlayAudio(EAudioType.Scene_ExitChunk);
        }
        DOTween.Kill(DGID);
        var curColor = m_CurColor;
        var interval = curColor - m_OriginalColor;
        var moveTime = Mathf.Abs(interval.r) + Mathf.Abs(interval.g) + Mathf.Abs(interval.b) + Mathf.Abs(interval.a);
        DOTween.To(() => 0.0f, (value) =>
        {
            m_CurColor = Color.Lerp(curColor, m_OriginalColor, value);
            UpdateColor();

        }, 1.0f, moveTime)
            .SetId(DGID);
    }
    private void UpdateColor()
    {
        foreach (var item in m_SpriteRenders)
        {
            item.color = m_CurColor;
        }
    }
    [SerializeField]
    private List<SpriteRenderer> m_SetColorList = new();
    public void SetChunkColor()
    {
        foreach (var item in m_SetColorList)
        {
            item.color = Data.MainColor;
        }
    }
}
