using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GainMgr : Singleton<GainMgr>
{
}

public enum EBuffView : ushort
{
    None,
    Launch, // ¹¥»÷
    Collect, // 
    Hit, // ¹¥»÷
    BeHit, // ÊÜ»÷
    Interval, // ¼ä¸ô
    perpetual, // ÓÀ¾Ã
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

    AttackHarm1,
    AttackSpeed1,
    AttackRange1,
    Deffense1,

    EnumCount,
}
public interface IGainUtil
{
    public static void InflictionGain(EGainType f_GainType, WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        f_Recipient.AddGain(f_GainType, f_Initiator ?? MonsterMgr.Ins.GodEntityData);
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
    public abstract EBuffView GainView { get; }
    protected WorldObjectBaseData m_Recipient = null;
    protected WorldObjectBaseData m_Initiator = null;
    protected int m_Probability = 0;
    protected virtual float DurationTime { get; } = 0;
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
        m_IsExecute = false;
        GTools.LifecycleMgr.RemoveUpdate(this);
        ILoadPrefabAsync.UnLoad(this);
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
        //if (randomNum <= m_Probability)
        //{

        //}
        ExecuteContext(randomNum);
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
            m_Recipient.RemoveGain(GainType);
        }
        switch (GainView)
        {
            case EBuffView.Interval:
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

public abstract class EntityGainBase : ObjectPoolBase
{

}


