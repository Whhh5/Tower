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
                    BuffType = typeof(Buff_AddBloodData),
                    Desc = "持续回血 + 持续回血"
                }
            },
            {
                ++index,
                new()
                {
                    ID = index,
                    Name = "中毒",
                    BuffType = typeof(Buff_PoisonData),
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
    public ResultData<BuffData> GetBuff(ushort f_BuffID, EntityData f_Target)
    {
        ResultData<BuffData> result = new();
        if (m_BuffMap.TryGetValue(f_BuffID, out var value))
        {
            var target = Activator.CreateInstance(value.BuffType, args:f_Target);
            if (target is BuffData)
            {
                result.SetData(target as BuffData);
            }
        }

            return result;
    }
}
public abstract class BuffData : IExecute
{
    protected int m_CurSupCount = 0;
    protected int m_SuperpositionCount = 0;
    protected float m_StartTime = 0;
    public int SuperpositionCount => m_SuperpositionCount;

    protected int m_Id = 0;
    public int ID => m_Id;
    protected string m_Name = "";
    public string Name => m_Name;
    protected ushort m_Level = 0;
    public ushort Level => m_Level;
    protected int m_HarmBase = 0;
    public int HarmBase => m_HarmBase;

    protected float m_IntervalTime = 0.0f;
    protected float m_UnitTime = 0;

    protected EntityData m_Original = null;
    protected EntityData m_Target = null;
    public EntityData Target => m_Target;


    // 当前间隔时间
    public float CurIntervalTime => m_IntervalTime / Mathf.Clamp(m_SuperpositionCount * m_Level * 0.1f, 1, 3);
    // 当前伤害数值
    public int CurHarmValue => m_HarmBase * m_Level * (m_SuperpositionCount - m_CurSupCount);
    // 当前 层 CommandQueue 持续时间进度
    public float CurSupRatio = 0.0f;
    // 当前 CommandQueue 持续时间总进度
    public float CurRatio = 0.0f;
    public abstract UniTask ExecuteResultAsync(int f_Value, float f_Ratio);
    public virtual async UniTask StartExecute()
    {
        m_StartTime = GTools.CurTime;


        while (m_CurSupCount < m_SuperpositionCount && GTools.GetEntityActively(m_Target))
        {
            await GTools.WaitSecondAsync(CurIntervalTime);

            var timed = GTools.CurTime - m_StartTime;
            m_CurSupCount = Mathf.FloorToInt(timed / m_UnitTime);
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

    public BuffData(EntityData f_Original, EntityData f_Target)
    {
        m_Target = f_Target;
        m_Original = f_Original;
    }
    public void ReleaseAsync()
    {
        m_Target = null;
        m_Original = null;
    }
    public async void AddAsync(ushort f_Level)
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

    public void RemoveAsync()
    {
        m_SuperpositionCount = Mathf.Max(m_SuperpositionCount - 1, 0);
    }

}
// 加血 CommandQueue
public class Buff_AddBloodData : BuffData
{
    public Buff_AddBloodData(EntityData f_Original, EntityData f_Target) : base(f_Original, f_Target)
    {
        m_Id = 1;
        m_Name = "Add Blood";
    }

    public override async UniTask ExecuteResultAsync(int f_Value, float f_Ratio)
    {
        GTools.MathfMgr.EntityDamage(m_Original, m_Target, EDamageType.AddBlood, -f_Value);
    }
}
// 中毒 CommandQueue
public class Buff_PoisonData : BuffData
{
    public Buff_PoisonData(EntityData f_Original, EntityData f_Target) : base(f_Original, f_Target)
    {
        m_Id = 2;
        m_Name = "Poison";
    }

    public override async UniTask ExecuteResultAsync(int f_Value, float f_Ratio)
    {
        GTools.MathfMgr.EntityDamage(m_Original, m_Target, EDamageType.Magic, -f_Value);
    }

}
