using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;
using System.Reflection;


public interface IBuffUtil
{
    public static T CreateBuffData<T>(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
        where T : Effect_BuffBaseData, new()
    {
        var result = new T();
        result.Initialization(f_Initiator, f_Target);
        return result;
    }
    public static void InflictionBuff(EBuffType f_Buff, WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Target)
    {
        f_Target.AddBuffAsync(f_Buff, f_Initiator);
    }
}
public class BuffInsData
{
    public bool Active = true;
    public WorldObjectBaseData Target = null;
    public Dictionary<EBuffView, EntityGainBaseData> BuffData = null;
    public void AddBuff(EGainType f_BuffType, WorldObjectBaseData f_Initiator)
    {
        if (!GTools.TableMgr.TryGetGainInfo(f_BuffType, out var buffInfo))
        {
            return;
        }
        var buffData = buffInfo.CreateGain(f_Initiator, Target);
    }
}
public class BuffMgr : Singleton<BuffMgr>
{
    private Dictionary<WorldObjectBaseData, BuffInsData> m_BuffDatas = new();

    private void AddObjBuffList(WorldObjectBaseData f_ObjTarget )
    {

    }
    private void RemoveObjBuffList()
    {

    }
    public bool AddBuff(WorldObjectBaseData f_Intiator, WorldObjectBaseData f_Target, EGainType f_BuffType)
    {
        if (!GTools.TableMgr.TryGetGainData(f_BuffType, out var buffInfo))
        {
            return false;
        }
        if (!m_BuffDatas.TryGetValue(f_Target, out var buffInsData))
        {
            buffInsData = new()
            {
                Active = true,
                BuffData = new(),
                Target = f_Target,
            };
            m_BuffDatas.Add(f_Target, buffInsData);
        }
        buffInsData.AddBuff(f_BuffType, f_Intiator);
        return true;
    }
    public void RemoveBuff()
    {

    }
    public void OnClick(EBuffView f_ClickType)
    {

    }
}