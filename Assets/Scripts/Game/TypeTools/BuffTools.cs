using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;
using System.Reflection;


[Serializable]
public class BuffInfo
{
    public ushort ID = 0;
    public string Name = "";
    public Type BuffType = null;
    public string Desc = "";
}
public class BuffMgr : Singleton<BuffMgr>
{
    private Dictionary<ushort, BuffInfo> m_BuffMap = null;
    public Dictionary<ushort, BuffInfo> BuffMap => m_BuffMap;
    public BuffMgr()
    {
        ushort index = 0;
        m_BuffMap = new()
        {
            {
                ++index,
                new()
                {
                    ID = index,
                    Name = "持续回血",
                    BuffType = typeof(Buff_AddBlood),
                    Desc = "持续回血 + 持续回血"
                }
            },
            {
                ++index,
                new()
                {
                    ID = index,
                    Name = "中毒",
                    BuffType = typeof(Buff_Poison),
                    Desc = "中毒 + 持续中毒"
                }
            }
        };
    }
    public ResultData<BuffInfo> GetBuffInfo(ushort f_BuffID)
    {
        var result = new ResultData<BuffInfo>();
        if (m_BuffMap.TryGetValue(f_BuffID, out var value))
        {
            result.SetData(value);
        }
        return result;
    }
    public ResultData<BuffBase> GetBuff(ushort f_BuffID, Entity f_Target)
    {
        ResultData<BuffBase> result = new();
        if (m_BuffMap.TryGetValue(f_BuffID, out var value))
        {
            var target = Activator.CreateInstance(value.BuffType, args:f_Target);
            if (target is BuffBase)
            {
                result.SetData(target as BuffBase);
            }
        }

            return result;
    }
}
public abstract class BuffBase : IExecute
{
    protected uint m_CurSupCount = 0;
    protected uint m_SuperpositionCount = 0;
    protected float m_StartTime = 0;
    public uint SuperpositionCount => m_SuperpositionCount;

    protected uint m_Id = 0;
    public uint ID => m_Id;
    protected string m_Name = "";
    public string Name => m_Name;
    protected ushort m_Level = 0;
    public ushort Level => m_Level;
    protected uint m_HarmBase = 0;
    public uint HarmBase => m_HarmBase;

    protected float m_IntervalTime = 0.0f;
    protected float m_UnitTime = 0;

    protected Entity m_Target = null;
    public Entity Target => m_Target;


    // 当前间隔时间
    public float CurIntervalTime => m_IntervalTime / Mathf.Clamp(m_SuperpositionCount * m_Level * 0.1f, 1, 3);
    // 当前伤害数值
    public uint CurHarmValue => m_HarmBase * m_Level * (m_SuperpositionCount - m_CurSupCount);
    // 当前 层 CommandQueue 持续时间进度
    public float CurSupRatio = 0.0f;
    // 当前 CommandQueue 持续时间总进度
    public float CurRatio = 0.0f;
    public abstract UniTask ExecuteResultAsync(uint f_Value, float f_Ratio);
    public virtual async UniTask StartExecute()
    {
        m_StartTime = GTools.CurTime;


        while (m_CurSupCount < m_SuperpositionCount && GTools.GetEntityActively(m_Target))
        {
            await GTools.WaitSecondAsync(CurIntervalTime);

            var timed = GTools.CurTime - m_StartTime;
            m_CurSupCount = (uint)Mathf.FloorToInt(timed / m_UnitTime);
            CurSupRatio = timed % m_UnitTime / m_UnitTime;
            CurRatio = timed / (m_UnitTime * m_SuperpositionCount);
            await ExecuteResultAsync(CurHarmValue, CurSupRatio);

        }

        await StopExecute();
    }

    public virtual async UniTask StopExecute()
    {
        m_SuperpositionCount = 0;
        m_CurSupCount = 0;
        m_Level = 0;
        m_StartTime = 0;
        m_IntervalTime = 0;
        CurSupRatio = 0;
        CurRatio = 0;
    }

    public BuffBase(Entity f_Target)
    {
        m_Target = f_Target;
    }
    public async UniTask ReleaseAsync()
    {
        m_Target = null;
    }
    public async UniTask AddAsync(ushort f_Level)
    {
        if (f_Level > m_Level)
        {
            m_Level = f_Level;

            m_HarmBase = 2;
            m_UnitTime = 3.0f;
            m_IntervalTime = 1.0f;
            m_CurSupCount = 0;
        }


        if (m_SuperpositionCount++ == 0)
        {
            await StartExecute();
        }
    }

    public async UniTask RemoveAsync()
    {
        m_SuperpositionCount = (uint)Mathf.Max(m_SuperpositionCount - 1, 0);
    }

}
// 加血 CommandQueue
public class Buff_AddBlood : BuffBase
{
    public Buff_AddBlood(Entity f_Target) : base(f_Target)
    {
        m_Target = f_Target;
        m_Id = 1;
        m_Name = "Add Blood";
    }

    public override async UniTask ExecuteResultAsync(uint f_Value, float f_Ratio)
    {
        await m_Target.RediuceBlood(f_Value, EDamageType.AddBlood);
    }
}
// 中毒 CommandQueue
public class Buff_Poison : BuffBase
{
    public Buff_Poison(Entity f_Target) : base(f_Target)
    {
        m_Target = f_Target;
        m_Id = 2;
        m_Name = "Poison";
    }

    public override async UniTask ExecuteResultAsync(uint f_Value, float f_Ratio)
    {
        await m_Target.RediuceBlood(f_Value, EDamageType.Magic);
    }

}
