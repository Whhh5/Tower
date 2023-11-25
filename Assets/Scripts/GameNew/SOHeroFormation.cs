using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SOHeroFormation", menuName = "ScriptableObject/SOHeroFormation", order = 1)]
public class SOHeroFormation : SerializedScriptableObject
{
    // 数字映射
    private static Dictionary<int, EHeroVocationalType> MathMap = new()
    {
        { 1, EHeroVocationalType.Warrior },
        { 2, EHeroVocationalType.Enchanter },
        { 3, EHeroVocationalType.Supplementary },
        { 4, EHeroVocationalType.MainTank },
    };
    public EHeroVocationalType GetCentreHeroVocationalType()
    {
        var row = CentreIndex / ArraySize.y;
        var col = CentreIndex % ArraySize.y;
        var value = ArrayIndex[row, col];
        return GetHeroVocationalType(value);
    }
    public EHeroVocationalType GetHeroVocationalType(int f_Key)
    {
        return MathMap[f_Key];
    }
    public Vector2Int GetCentreRowCol()
    {
        var row = CentreIndex / ArraySize.y;
        var col = CentreIndex % ArraySize.y;
        return new Vector2Int(row, col);
    }
    //
    public string Name = "";
    // 描述
    [TextArea]
    public string Describes = "";
    // 阵型类型
    public EFormationType FormationType;
    // 阵型中心点
    public int CentreIndex;
    // 矩阵大小
    public Vector2Int ArraySize;
    // 矩阵阵型
    public int[,] ArrayIndex;
    public void CopyTo(SOHeroFormation f_Target)
    {
        f_Target.FormationType = FormationType;
        f_Target.CentreIndex = CentreIndex;
        f_Target.ArraySize = ArraySize;
        f_Target.ArrayIndex = new int[ArrayIndex.Rank, ArrayIndex.GetLength(0)];
        for (int i = 0; i < ArrayIndex.Rank; i++)
        {
            for (int j = 0; j < ArrayIndex.GetLength(0); j++)
            {
                f_Target.ArrayIndex[i, j] = ArrayIndex[i, j];
            }
        }
    }
    [Button]
    public void OpenWindow()
    {
#if UNITY_EDITOR
        var window = UnityEditor.EditorWindow.GetWindow<SOHeroFormationWindow2>();
        window.InitData(this);
#endif
    }
}
#if UNITY_EDITOR
public class SOHeroFormationWindow2 : UnityEditor.EditorWindow
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
                var col = oldArr.GetLength(1);
                var row = oldArr.GetLength(0);

                if (i >= row || j >= col)
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
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        CentreIndex = UnityEditor.EditorGUILayout.IntField("", CentreIndex);
        for (int i = 0; i < ArraySize.x; i++)
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                for (int j = 0; j < ArraySize.y; j++)
                {
                    var value = ArrayIndex[i, j];
                    var type = (EHeroVocationalType)UnityEditor.EditorGUILayout.EnumPopup((EHeroVocationalType)value, GUILayout.Width(50), GUILayout.Height(30));
                    ArrayIndex[i, j] = (int)type;
                }
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < ArraySize.x; i++)
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                if ((CentreRowCol.x % 2) == (i % 2))
                {
                    GUILayout.Label("", GUILayout.Width(25));
                }
                for (int j = 0; j < ArraySize.y; j++)
                {
                    var curIndex = i * ArraySize.y + j;
                    var value = (EHeroVocationalType)ArrayIndex[i, j];
                    Texture txt = null;
                    if (GTools.TableMgr.TryGetHeroVocationalInfo(value, out var vocInfo))
                    {
                        if (GTools.TableMgr.TryGetAssetPath(vocInfo.IconID, out var path))
                        {
                            var fullPath = $"Assets/Resources/{path}.png";
                            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture));
                            txt = sprite as Texture;
                        }
                    }
                    if (GUILayout.Button(txt, GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        CentreIndex = curIndex;
                    }
                }
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
        }
    }
}
#endif