using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(QuadTreeData))]
public class QuadTreeDataEditor : Editor
{
    private QuadTreeData m_Target = null;
    private void OnEnable()
    {
        m_Target ??= target as QuadTreeData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //EditorGUILayout.BeginVertical();
        //{
        //    foreach (var item in m_Target.NodeDic) 
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        {
        //            EditorGUILayout.LabelField($"[{item.Key}] = {item.Value}", GUILayout.Width(50));
        //            EditorGUILayout.TextField($"{item.Value}");
        //        }
        //        EditorGUILayout.EndHorizontal();
        //    }
        //}
        //EditorGUILayout.BeginVertical();
    }

}
 