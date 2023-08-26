using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;


public class WorldWindowManager : MonoSingleton<WorldWindowManager>
{
    public Dictionary<int, WorldUIEntityHintData> m_BloodData = new();

    [SerializeField]
    private RectTransform m_Root = null;

    public void UpdateBloodHint(WorldObjectBaseData f_Target)
    {
        var key = f_Target.LoadKey;

        if (!m_BloodData.TryGetValue(key, out var value))
        {
            value = new(key, f_Target);
            value.SetParent(m_Root);
            m_BloodData.Add(key, value);
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(value));
        }
        value.UpdateInfo();
    }
    public void RemoveBloodHint(WorldObjectBaseData f_Target)
    {
        var key = f_Target.LoadKey;

        if (m_BloodData.ContainsKey(key))
        {
            m_BloodData.Remove(key);
        }
    }

    public void UpdateMagicHint(WorldObjectBaseData f_Target)
    {
        var key = f_Target.LoadKey;

        if (!m_BloodData.TryGetValue(key, out var value))
        {
            value = new(key, f_Target);
            value.SetParent(m_Root);
            m_BloodData.Add(key, value);
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(value));
        }
        value.UpdateInfo();
    }
}
