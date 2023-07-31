using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class GizomMgr : MonoSingleton<GizomMgr>
{
    private Dictionary<int, List<(Vector3 tFrom, Vector3 tTo)>> m_DicLine = new();




    public void AddLine(int f_Key, List<(Vector3 tFrom, Vector3 tTo)> f_DrawList)
    {
        if (!m_DicLine.ContainsKey(f_Key))
        {
            m_DicLine.Add(f_Key, new());
        }

        m_DicLine[f_Key] = f_DrawList;
    }
    public void ClearLine(int f_Key)
    {
        if (m_DicLine.TryGetValue(f_Key, out var value))
        {
            value.Clear();
        }
    }




    private void OnDrawGizmos()
    {
        foreach (var item in m_DicLine)
        {
            foreach (var line in item.Value)
            {
                Gizmos.DrawLine(line.tFrom, line.tTo);
            }
        }
    }



    private void OnDrawGizmosSelected()
    {

    }
}
