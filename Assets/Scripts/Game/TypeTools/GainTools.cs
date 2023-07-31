using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GainMgr : Singleton<GainMgr>
{
    public Dictionary<ushort, (EGainType tGainType, Type tClassType)> m_GainMap = null;

    public GainMgr()
    {
        ushort index = 0;
        m_GainMap = new()
        {
            { ++index, (EGainType.Launch, typeof(Gain_Launch1)) },
            { ++index, (EGainType.Collect, typeof(Gain_Collect)) },
        };
    }

    public EGainType GetGainType(ushort f_ID)
    {
        if (m_GainMap.TryGetValue(f_ID, out var value))
        {
            return value.tGainType;
        }
        return EGainType.None;
    }
    public GainBase GetGain(ushort f_ID, Person f_TargetPerson)
    {
        if (m_GainMap.TryGetValue(f_ID, out var result))
        {
            var ins = GTools.GetInsttance(result.tClassType);
            var target = ins as GainBase;
            target.Initialization(f_ID, result.tGainType, f_TargetPerson);
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
public abstract class GainBase : Base, IExecute
{
    public ushort ID { get; private set; }
    public EGainType GainType { get; private set; }
    protected ushort m_Level;
    protected ushort m_TierCount;
    protected Person m_TargetPerson = null;
    protected int m_Probability = 0;
    public virtual void Initialization(ushort f_ID, EGainType f_GainType, Person f_TargetPerson)
    {
        ID = f_ID;
        GainType = f_GainType;
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

public class Gain_Launch1 : GainBase
{
    public override void Initialization(ushort f_ID, EGainType f_GainType, Person f_TargetPerson)
    {
        base.Initialization(f_ID, f_GainType, f_TargetPerson);

        m_Probability = 30;
    }

    public override async UniTask Execute(int f_CurProbability)
    {
        var wepon = await GTools.WeaponMgr.GetSetWeaponAsync(EAssetName.Emitter_GuidedMissileBaseCommon, m_TargetPerson);
        await wepon.StartExecute();
        await wepon.StopExecute();
        await GTools.WeaponMgr.DestroyWeaponAsync(wepon);
    }


}
public class Gain_Collect : GainBase
{
    public override void Initialization(ushort f_ID, EGainType f_GainType, Person f_TargetPerson)
    {
        base.Initialization(f_ID, f_GainType, f_TargetPerson);

        m_Probability = 100;
    }

    public override async UniTask Execute(int f_CurProbability)
    {

    }
}


