using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// �ϰ�������
/// </summary>
[Serializable]
public enum EBarrierType
{
    /// <summary>
    /// ɽ��
    /// </summary>
    Massif,
}
[Serializable]
public class BarrierData
{
    public Vector2Int Index;
}
[CreateAssetMenu(fileName = "NewMapData", menuName = "ScriptableObject/MapData", order = 0)]
public class MapConfig : SerializedScriptableObject
{
    public Vector2Int MapWH;
    public float MapChunkLength;
    public Vector2 MapChunkInterval;
    public Dictionary<EBarrierType, List<BarrierData>> BarrierData = new();


    public int WarSeatCount;
    public int WarSeatRowCount;
    public int HeroPoolCount;
    public float WarSeatLength;
    public Vector2 WarSeatInterval;

    [Space(10), Header("ˢ���б���")]
    public int LevelUpdateExpenditure;
    [Header("��Ϸ��ʼ���")]
    public int LevelInitGlod;
}
