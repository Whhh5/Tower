using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Entity_IncubatorData : WorldObjectBaseData
{
    public Entity_IncubatorData() : base(-1, -1)
    {
    }

    public bool Initialization(int f_index, int f_ChunkIndex, EHeroCradType f_HeroInfo)
    {
        SetCurrentChunkIndex(f_ChunkIndex);
        if (WorldMapManager.Ins.TryGetChunkData(CurrentIndex, out var chunkData))
        {
            SetPosition(chunkData.WorldPosition);
        }
        m_HeroType = f_HeroInfo;
        if (TableMgr.Ins.TryGetHeroCradInfo(f_HeroInfo, out var heroInfo))
        {
            m_QuelityLevel = heroInfo.QualityLevel;
            m_IncubatorTime = heroInfo.QualityLevelInfo.IncubatorTime;
            m_Size = Vector3.one * 1.2f * (int)m_QuelityLevel;
            m_IsFinish = false;

            StartIncubator();
            return true;
        }
        return false;
    }
    public void StartIncubator()
    {
        WorldMapManager.Ins.MoveChunkElement(this, CurrentIndex);
        SetPersonStatus(EPersonStatusType.Entrance, EAnimatorStatus.Stop);
        m_EndTime = m_IncubatorTime + Time.time;
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));
    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_Incubator;

    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;


    protected EHeroCradType m_HeroType = EHeroCradType.EnemyCount;
    private EHeroQualityLevel m_QuelityLevel = EHeroQualityLevel.EnumCount;
    private float m_IncubatorTime = 0;
    private float m_EndTime = 1;
    private float StartTime => m_EndTime - m_IncubatorTime;
    public float IncubatorSchedule => Mathf.Clamp01((Time.time - StartTime) / m_IncubatorTime);
    private Vector3 m_Size = Vector3.one;
    private bool m_IsFinish = false;
    public Entity_Incubator IncubatorEntity => GetCom<Entity_Incubator>();

    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (CurStatus)
        {
            case EPersonStatusType.Entrance:
                break;
            case EPersonStatusType.Idle:
                break;
            case EPersonStatusType.Die:
                break;
            default:
                break;
        }

        var size = Vector3.Lerp(Vector3.one, m_Size, IncubatorSchedule);
        if (IncubatorEntity != null)
        {
            IncubatorEntity.SetIncubatorScale(size);
            IncubatorEntity.UpdateResidueTime();
        }
        if (!m_IsFinish && IncubatorSchedule == 1)
        {
            SetPersonStatus(EPersonStatusType.Die);
            m_IsFinish = true;
        }
    }
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();
        switch (CurStatus)
        {
            case EPersonStatusType.Entrance:
                {
                    SetPersonStatus(EPersonStatusType.Idle);
                }
                break;
            case EPersonStatusType.Die:
                {
                    ILoadPrefabAsync.UnLoad(this);
                }
                break;
            default:
                break;
        }
    }
    public override void AnimatorCallback000()
    {
        base.AnimatorCallback000();
        switch (CurStatus)
        {
            case EPersonStatusType.Die:
                {
                    if (IncubatorSchedule == 1)
                    {
                        // ´´½¨Ó¢ÐÛ
                        IncubatorFinish();
                    }
                }
                break;
            default:
                break;
        }
    }
    public virtual void IncubatorFinish()
    {
        if (TableMgr.Ins.GetHeroDataByType(m_HeroType, CurrentIndex, out var worldObjData))
        {
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(worldObjData));
        }
    }
}


public class Entity_Incubator : ObjectPoolBase
{
    public Transform MainBody = null;
    public TextMeshPro ResidueTime = null;
    public void SetIncubatorScale(Vector3 f_Scale)
    {
        MainBody.localScale = f_Scale;
    }
    public void UpdateResidueTime()
    {
        var time = GetData<Entity_IncubatorData>();
        ResidueTime.text = $"{time.IncubatorSchedule}";
    }
}
