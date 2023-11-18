using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



//[CustomEditor(typeof(SOHeroFormation))]
public class SOHeroFormationEditor : Editor
{
    public SOHeroFormation TargetData = null;
    private void OnEnable()
    {
        TargetData = target as SOHeroFormation;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Config Window", GUILayout.Height(50)))
        {
            var window = EditorWindow.GetWindow<SOHeroFormationWindow>();
            window.InitData(TargetData);
        }
        base.OnInspectorGUI();
    }


}


public class SOHeroFormationWindow : EditorWindow
{
    public SOHeroFormation TargetData = null;

    // 阵型类型
    public EFormationType FormationType;
    // 阵型中心点
    public int CentreIndex;
    // 矩阵大小
    public Vector2Int ArraySize;
    // 矩阵阵型
    public int[,] ArrayIndex;
    public Vector2Int CentreRowCol;
    public void InitData(SOHeroFormation f_Target)
    {
        TargetData = f_Target;
        FormationType = f_Target.FormationType;
        CentreIndex = f_Target.CentreIndex;
        ArraySize = f_Target.ArraySize;
        CentreRowCol = new Vector2Int(CentreIndex / ArraySize.y, CentreIndex % ArraySize.y);

        var arrSize = f_Target.ArraySize;
        var oldArr = f_Target.ArrayIndex;
        var newArr = new int[arrSize.x, arrSize.y];
        ArrayIndex = newArr;
        if (oldArr == null)
        {
            return;
        }
        for (int i = 0; i < arrSize.x; i++)
        {
            for (int j = 0; j < arrSize.y; j++)
            {
                if (i >= oldArr.Rank || j >= oldArr.GetLength(0))
                {
                    continue;
                }
                newArr[i, j] = oldArr[i, j];
            }
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Save To Asset", GUILayout.Width(300), GUILayout.Height(50)))
        {
            TargetData.FormationType = FormationType;
            TargetData.CentreIndex = CentreIndex;
            TargetData.ArraySize = ArraySize;
            TargetData.ArrayIndex = ArrayIndex;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        CentreIndex = EditorGUILayout.IntField("", CentreIndex);
        for (int i = 0; i < ArraySize.x; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                for (int j = 0; j < ArraySize.y; j++)
                {
                    var value = ArrayIndex[i, j];
                    var type = (EHeroVocationalType)EditorGUILayout.EnumPopup((EHeroVocationalType)value, GUILayout.Width(50), GUILayout.Height(30));
                    ArrayIndex[i, j] = (int)type;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < ArraySize.x; i++)
        {
            EditorGUILayout.BeginHorizontal();
            { 
                if ((CentreRowCol.x % 2) == (i % 2))
                {
                    GUILayout.Label("", GUILayout.Width(25));
                }
                for (int j = 0; j < ArraySize.y; j++)
                {
                    var value = (EHeroVocationalType)ArrayIndex[i, j];
                    Texture txt = null;
                    if (GTools.TableMgr.TryGetHeroVocationalInfo(value, out var vocInfo))
                    {
                        if (GTools.TableMgr.TryGetAssetPath(vocInfo.IconID, out var path))
                        {
                            var fullPath = $"Assets/Resources/{path}.png";
                            var sprite = AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture));
                            txt = sprite as Texture;
                        }
                    }
                    if (GUILayout.Button(txt, GUILayout.Width(50), GUILayout.Height(50)))
                    {

                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}