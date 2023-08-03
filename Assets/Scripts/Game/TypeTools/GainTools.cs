using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GainMgr : Singleton<GainMgr>
{
    public Dictionary<EGainView, (EGainType tGainType, Func<WorldObjectBaseData, GainBaseData> tGetData)> m_GainMap = null;

    public GainMgr()
    {
        var index = 0;
        m_GainMap = new()
        {
            { (EGainView)(++index), (EGainType.Launch, (value) => new Gain_Launch1(value)) },
            { (EGainView)(++index), (EGainType.Collect, (value) => new Gain_Collect1(value)) },
        };
    }

    public EGainType GetGainType(EGainView f_GainView)
    {
        if (m_GainMap.TryGetValue(f_GainView, out var value))
        {
            return value.tGainType;
        }
        return EGainType.None;
    }
    public GainBaseData GetGain(EGainView f_GainView, WorldObjectBaseData f_TargetPerson)
    {
        if (m_GainMap.TryGetValue(f_GainView, out var result))
        {
            var target = result.tGetData(f_TargetPerson);
            return target;
        }
        return null;
    }
}

public enum EGainType : ushort
{
    None,
    Launch,
    Collect,
    Hit,
    BeHit,
    EnumCount,
}
public enum EGainView : ushort
{
    None,

    Launch1,
    Collect1,

    EnumCount,
}
public abstract class GainBaseData : Base, IExecute
{
    public abstract EGainView GainView { get; }
    public abstract EGainType GainType { get; }
    protected ushort m_Level;
    protected ushort m_TierCount;
    protected WorldObjectBaseData m_TargetPerson = null;
    protected int m_Probability = 0;

    public GainBaseData(WorldObjectBaseData f_TargetPerson)
    {
        m_TargetPerson = f_TargetPerson;
        m_Level = 1;
        m_TierCount = 1;
        m_Probability = 30;
    }
    public async UniTask StartExecute()
    {
        var randomNum = GTools.MathfMgr.GetRandomValue(0, 100);
        if (randomNum <= m_Probability)
        {
            await Execute(randomNum);
        }
    }

    public virtual async UniTask StopExecute()
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

    public abstract UniTask Execute(int f_CurProbability);
}

public class Gain_Launch1 : GainBaseData
{
    public override EGainView GainView => EGainView.Launch1;
    public override EGainType GainType => EGainType.Launch;


    public Gain_Launch1(WorldObjectBaseData f_TargetPerson) : base(f_TargetPerson)
    {
        m_Probability = 30;
    }

    public override async UniTask Execute(int f_CurProbability)
    {
        var wepon = new Emitter_GuidedMissileBaseCommonData(0, m_TargetPerson);
        await ILoadPrefabAsync.LoadAsync(wepon);
        await wepon.StartExecute();
        await wepon.StopExecute();
        ILoadPrefabAsync.UnLoad(wepon);
    }


}
public class Gain_Collect1 : GainBaseData
{
    public override EGainView GainView => EGainView.Collect1;
    public override EGainType GainType => EGainType.Collect;
    public Gain_Collect1(WorldObjectBaseData f_TargetPerson) : base(f_TargetPerson)
    {
        m_Probability = 100;
    }

    public override async UniTask Execute(int f_CurProbability)
    {

    }
}


