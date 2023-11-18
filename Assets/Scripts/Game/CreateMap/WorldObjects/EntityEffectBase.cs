using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using B1;

public abstract class EntityEffectBaseData : UnityObjectData
{
    public EntityEffectBaseData() : base(0)
    {
    }
    public EntityEffectBase Target => GetCom<EntityEffectBase>();
    public override bool IsUpdateEnable => true;
    public double CurPlayTime => CurPlaySchedule * Duration;
    public float CurPlaySchedule = 0; // 0 - 1
    public float PlaySpeed = 1;
    public bool m_IsPlay = false;
    public WorldObjectBaseData Initiator;
    public float Duration => Target != null ? (float)Target.Duration : 1.0f;
    public DirectorWrapMode DirectorUpdateMode;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public virtual void Initialization(Vector3 f_StartPos, WorldObjectBaseData f_Initiator, DirectorWrapMode f_WrapMode = DirectorWrapMode.Loop)
    {
        DirectorUpdateMode = f_WrapMode;
        SetPosition(f_StartPos);
        Initiator = f_Initiator;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_IsPlay)
        {
            var value = CurPlaySchedule + (UpdateDelta * PlaySpeed) / Duration;
            if (value > 1)
            {
                switch (DirectorUpdateMode)
                {
                    case DirectorWrapMode.Hold:
                        break;
                    case DirectorWrapMode.Loop:
                        value %= 1;
                        break;
                    case DirectorWrapMode.None:
                        ILoadPrefabAsync.UnLoad(this);
                        break;
                    default:
                        break;
                }
            }
            CurPlaySchedule = Mathf.Clamp01(value);
            UpdateScheme();
        }
    }
    public override void AfterLoad()
    {
        base.AfterLoad();

        CurPlaySchedule = 0;
        m_IsPlay = true;
        UpdateScheme();
    }
    public override void UnLoad()
    {
        m_IsPlay = false;
        base.UnLoad();
    }
    public void UpdateScheme()
    {
        if (Target != null)
        {
            Target.UpdateScheme();
        }
    }
    public virtual void ExecuteEffect(WorldObjectBaseData m_TargetEnemy)
    {

    }
}
public abstract class EntityEffectBase : ObjectPoolBase
{
    public PlayableDirector TimeLine => GetComponent<PlayableDirector>();
    public EntityEffectBaseData EffData => GetData<EntityEffectBaseData>();

    public double Duration => TimeLine != null ? TimeLine.duration : 0;

    public void UpdateScheme()
    {
        if (TimeLine != null)
        {
            TimeLine.time = EffData.CurPlayTime;
            TimeLine.Evaluate();
        }
    }
}
