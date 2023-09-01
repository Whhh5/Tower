using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GainMgr : Singleton<GainMgr>
{
}

public enum EGainView : ushort
{
    None,
    Launch,
    Collect,
    Hit,
    BeHit,
    Interval,
    EnumCount,
}
public enum EGainType : ushort
{
    None,

    Launch1,
    Collect1,

    Volccano1,
    Volccano2,
    Volccano3,


    Weather_Default1,
    Weather_Default2,
    Weather_Default3,
    Weather_Default4,
    Weather_Default5,
    Weather_Default6,


    EnumCount,
}
public interface IGainUtil
{
    public static void InflictionGain(EGainType f_GainView, WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        f_Recipient.AddGain(f_GainView, f_Initiator ?? MonsterMgr.Ins.GodEntityData);
    }
    public static void RemoteGain(EGainType f_GainView, WorldObjectBaseData f_Recipient)
    {
        f_Recipient.RemoveGain(f_GainView);
    }
}
public abstract class EntityGainBaseData : UnityObjectData
{
    protected EntityGainBaseData() : base(0)
    {
    }
    public abstract EGainType GainType { get; }
    public abstract EGainView GainView { get; }
    protected WorldObjectBaseData m_Recipient = null;
    protected WorldObjectBaseData m_Initiator = null;
    protected int m_Probability = 0;
    protected abstract float DurationTime { get; }
    protected virtual float IntervalTime { get; } = 1.0f;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    protected float m_CurResidueTime = 0;
    protected float m_EndTime = 0;

    private bool m_IsExecute = false;
    private float m_LastExecuteTime = 0;
    public virtual void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        m_Initiator = f_Initiator;
        m_Recipient = f_Recipient;
        m_Probability = 30;
        Reset();
    }
    public virtual void StartExecute()
    {
        m_IsExecute = true;
        GTools.LifecycleMgr.AddUpdate(this);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));
    }

    public virtual void StopExecute()
    {
        ILoadPrefabAsync.UnLoad(this);
        m_IsExecute = false;
        GTools.LifecycleMgr.RemoveUpdate(this);
    }

    public void Reset()
    {
        m_CurResidueTime = DurationTime;
        m_EndTime = Time.time + DurationTime;
    }

    public void Execute()
    {
        if (!m_IsExecute)
        {
            Log("ÔöÒæÎ´Ö´ÐÐ");
            return;
        }
        var randomNum = GTools.MathfMgr.GetRandomValue(0, 100);
        if (randomNum <= m_Probability)
        {
            ExecuteContext(randomNum);
        }
    }
    public abstract void ExecuteContext(int f_CurProbability);
    public abstract Vector3 GetPosiion();

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Update Æª
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (DurationTime > 0 && Time.time > m_EndTime)
        {
            StopExecute();
        }
        switch (GainView)
        {
            case EGainView.Interval:
                {
                    if (Time.time - m_LastExecuteTime > IntervalTime)
                    {
                        Execute();
                        m_LastExecuteTime = Time.time;
                    }
                }
                break;
            default:
                break;
        }
        SetPosition(GetPosiion());
    }
}

public abstract class EntityGainBase: ObjectPoolBase
{

}


