using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public EMapLevelType MapLevel;
    public bool IsPass;
    public List<int> RewardList = new();
}
[CreateAssetMenu(fileName = "NewGameData", menuName = "ScriptableObject/GameData", order = 1)]
public class GameData : SerializedScriptableObject
{
    [SerializeField]
    private Dictionary<EMapLevelType, LevelData> MapLevelData = new();

    [Button]
    public void InitGameData_Level(bool f_IsForce)
    {
        if (f_IsForce)
        {
            MapLevelData.Clear();
        }
        for (int i = 0; i < (int)EMapLevelType.EnumCount; i++)
        {
            var item = (EMapLevelType)i;
            if (MapLevelData.ContainsKey(item))
            {
                continue;
            }

            MapLevelData.Add(item, new());
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
    }
    public void SaveGameLevelData_LevelPass(EMapLevelType f_MapLevel, bool f_ToStaus = true)
    {
        if (MapLevelData.TryGetValue(f_MapLevel, out var levelData))
        {
            levelData.IsPass = f_ToStaus;
        }
    }
    public bool TryGetLevelData(EMapLevelType f_MapLevel, out LevelData f_LevelData)
    {
        return MapLevelData.TryGetValue(f_MapLevel, out f_LevelData);
    }
}
