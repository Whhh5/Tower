using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using B1;

public class EventSystemParamData
{
    public string Description = "未添加注释";
}
public class EventSystemMgr : Singleton<EventSystemMgr>
{
    private Dictionary<EEventSystemType, Dictionary<IEventSystem, int>> m_DicEvent = new();
    public override void Awake()
    {
        base.Awake();
    }
    public void SendEvent(EEventSystemType f_EEvent, EventSystemParamData f_Param = null)
    {
        if (m_DicEvent.TryGetValue(f_EEvent, out var value))
        {
            foreach (var item in value)
            {
                var tempItem = item;
                if (tempItem.Key != null)
                {
                    Log($"触发事件  event name = {f_EEvent}  description = {f_Param?.Description}");
                    tempItem.Key.ReceptionEvent(f_EEvent, f_Param);
                }
            }
        }
        else
        {
            LogWarning($"当前事件未注册 event name = {f_EEvent}");
        }
    }
    public void Subscribe<T>(EEventSystemType f_EEvent, T f_EventReception)
        where T : IEventSystem
    {
        if (!m_DicEvent.ContainsKey(f_EEvent))
        {
            m_DicEvent.Add(f_EEvent, new());
        }
        if (m_DicEvent[f_EEvent].ContainsKey(f_EventReception))
        {
            LogError($"重复添加事件 event name = {f_EEvent}");
            return;
        }

        m_DicEvent[f_EEvent].Add(f_EventReception, 0);
        LogWarning($"订阅事件   event name = {f_EEvent}");
    }
    public void Unsubscribe<T>(EEventSystemType f_EEvent, T f_EventReception)
        where T : IEventSystem
    {
        if (m_DicEvent.TryGetValue(f_EEvent, out var value) && value.ContainsKey(f_EventReception))
        {
            if (value.Count > 1)
            {
                value.Remove(f_EventReception);
            }
            else
            {
                m_DicEvent.Remove(f_EEvent);
            }
            LogWarning($"取消订阅事件 event name = {f_EEvent}");
        }
        else
        {
            LogError($"取消订阅事件, 不存在该事件 event name = {f_EEvent}");
        }
    }
    public void UnsubscribeAllByEvent(EEventSystemType f_EEvent)
    {
        if (m_DicEvent.TryGetValue(f_EEvent, out var list))
        {
            #region Log
            string str = $"当前取消全部订阅 event name = {f_EEvent}    count = {list.Count}";
            str += "\n{";
            foreach (var item in list)
            {
                str += $"\n\t event desc = {item.Value} ";
            }
            str += "\n}";
            LogWarning(str);
            #endregion

            m_DicEvent.Remove(f_EEvent);
        }
        else
        {
            LogError($"当前事件未注册 event name = {f_EEvent}");
        }
    }
    public void UnsubscribeAll()
    {
        foreach (var item in m_DicEvent)
        {
            #region Log
            string str = $"当前取消全部订阅 event name = {item.Key}    count = {item.Value.Count}";
            str += "\n{";
            foreach (var value in item.Value)
            {
                str += $"\n\t event desc = {value.Value} ";
            }
            str += "\n}";
            LogWarning(str);
            #endregion
        }
        m_DicEvent = new();
    }
    public void LogEvent()
    {
        string str = $" Console Event Subscribe   Event Count = {m_DicEvent.Count}";
        uint index = 0;
        foreach (var dicEvent in m_DicEvent)
        {
            str += $"\n[ {index++} ] [ {dicEvent.Key} {dicEvent.Value.Count} ] = " +
                $"\n{{";
            uint eventIndex = 0;
            foreach (var item in dicEvent.Value)
            {
                str += $"\n\t[ {eventIndex++} ] = " +
                    $"\n\t{{" +
                    $"\n\t\tevent layer \t= {item.Value}" +
                    $"\n\t\tevent action \t= {item.Key}" +
                    $"\n\t}}";
            }
            str += $"\n}}";
        }
        LogWarning(str);
    }
}

