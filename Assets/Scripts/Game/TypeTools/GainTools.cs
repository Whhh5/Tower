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

    EnumCount,
}
public interface IGainUtil
{
    public static void InflictionGain(EGainType f_GainView, WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
    {
        f_Target.AddGainAsync(f_GainView, f_Initiator ?? MonsterManager.Ins.GodEntityData);
    }
}
public abstract class GainBaseData : Base
{
    public abstract EGainType GainView { get; }
    public abstract EGainView GainType { get; }
    protected ushort m_Level;
    protected ushort m_TierCount;
    protected WorldObjectBaseData m_Recipient = null;
    protected WorldObjectBaseData m_Initiator = null;
    protected int m_Probability = 0;

    public virtual void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        m_Initiator = f_Initiator;
        m_Recipient = f_Recipient;
        m_Level = 1;
        m_TierCount = 1;
        m_Probability = 30;
    }
    public virtual void StartExecute()
    {
        var randomNum = GTools.MathfMgr.GetRandomValue(0, 100);
        if (randomNum <= m_Probability)
        {
            Execute(randomNum);
        }
    }

    public virtual void StopExecute()
    {
        Log("StopExecute");
    }

    public void AddTier()
    {
        m_TierCount++;
    }
    public void RediusTier()
    {
        m_TierCount--;
    }
    public void SetLevel(ushort f_Level)
    {
        m_Level = f_Level;
    }
    public void Reset()
    {

    }

    public abstract void Execute(int f_CurProbability);
}

public class Gain_Launch1 : GainBaseData
{
    public override EGainType GainView => EGainType.Launch1;
    public override EGainView GainType => EGainView.Launch;

    public override void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        base.Initialization(f_Initiator, f_Recipient);
        m_Probability = 30;
    }

    public override async void Execute(int f_CurProbability)
    {
        var wepon = new Emitter_GuidedMissileBaseCommonData(0, m_Recipient);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(wepon));
        await wepon.StartExecute();
        await wepon.StopExecute();
        ILoadPrefabAsync.UnLoad(wepon);
    }


}
public class Gain_Collect1 : GainBaseData
{
    public override EGainType GainView => EGainType.Collect1;
    public override EGainView GainType => EGainView.Collect;

    public override void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        base.Initialization(f_Initiator, f_Recipient);
        m_Probability = 100;
    }

    public override void Execute(int f_CurProbability)
    {

    }
}


