using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Entity_IncubatorBaseData : WorldObjectBaseData
{
    public Entity_IncubatorBaseData() : base(-1, -1)
    {
    }
    public override EEntityType EntityType => EEntityType.Incubator;
    public IncubatorAttributeInfo AddAttribute = new();
    public void Initialization(int f_index, int f_ChunkIndex, EQualityType f_Quality, EHeroCradStarLevel f_HeroStarLevl)
    {
        base.Initialization(f_index, f_ChunkIndex);
        if (WorldMapMgr.Ins.TryGetChunkData(CurrentIndex, out var chunkData))
        {
            SetPosition(chunkData.WorldPosition);
        }
        if (!GTools.HeroIncubatorPoolMgr.TryGetRandomCardByLevel(f_Quality, out var heroType))
        {
            return;
        }
        m_HeroType = heroType;
        m_HeroStarLevel = f_HeroStarLevl;
        if (TableMgr.Ins.TryGetHeroCradInfo(heroType, out var heroInfo)
            && TableMgr.Ins.TryGetIncubatorInfo(heroInfo.QualityLevel, out var incubatorInfo))
        {
            m_QuelityLevel = heroInfo.QualityLevel;
            m_IncubatorTime = incubatorInfo.IncubatorTime;
            m_Size = Vector3.one * 1.2f * (int)m_QuelityLevel;
            m_IsFinish = false;

            StartIncubator();
        }
    }
    public void StartIncubator()
    {
        GTools.WorldMapMgr.MoveChunkElement(this, CurrentIndex);
        SetPersonStatus(EPersonStatusType.Entrance, EAnimatorStatus.Stop);
        m_EndTime = m_IncubatorTime + Time.time;
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));

        GTools.WorldWindowMgr.CreateWorldSlider(this, () =>
        {
            return IncubatorSchedule;
        }, () =>
        {
            return true;
        });
    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_Incubator1;

    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;


    protected EHeroCardType m_HeroType = EHeroCardType.EnumCount;
    private EHeroCradStarLevel m_HeroStarLevel;
    private EQualityType m_QuelityLevel = EQualityType.EnumCount;
    private float m_IncubatorTime = 0;
    private float m_EndTime = 1;
    private float StartTime => m_EndTime - m_IncubatorTime;
    public float IncubatorSchedule => Mathf.Clamp01((Time.time - StartTime) / m_IncubatorTime);
    private Vector3 m_Size = Vector3.one;
    private bool m_IsFinish = false;
    public Entity_IncubatorBase IncubatorEntity => GetCom<Entity_IncubatorBase>();

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
        }
        if (!m_IsFinish && IncubatorSchedule == 1)
        {
            SetPersonStatus(EPersonStatusType.Die);
            m_IsFinish = true;
        }
        else
        {
            UpdateCurAddAttribute();
        }
    }
    public void UpdateCurAddAttribute()
    {
        if (GTools.WeatherMgr.TryGetCurWeatherInfo(out var weatherInfo))
        {
            AddAttribute += weatherInfo.IncubatorRatio * UpdateDelta;
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
        GTools.HeroMgr.CreateHero(m_HeroType, m_HeroStarLevel, CurrentIndex, out var heroData, AddAttribute);
    }
}


public class Entity_IncubatorBase : ObjectPoolBase
{
    public Transform MainBody = null;
    public Entity_IncubatorBaseData DataBase => GetData<Entity_IncubatorBaseData>();
    public void SetIncubatorScale(Vector3 f_Scale)
    {
        MainBody.localScale = f_Scale;
    }
}
