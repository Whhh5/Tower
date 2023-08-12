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
public class BuffMgr : Singleton<BuffMgr>
{

}