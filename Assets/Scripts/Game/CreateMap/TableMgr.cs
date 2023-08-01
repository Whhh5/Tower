using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using System;

[Flags]
public enum EWorldObjectType : int
{
    None = 0,

    // 道路
    Road = 1 << 1,

    // 建筑物 -- 可破坏的静态物体
    Construction = 1 << 2,

    // 墙体 -- 不可破坏的静态物体
    Wall = 1 << 3,
    // 实体人
    Preson = 1 << 4,

    // 特效
    Effect = 1 << 5,

    EnumCount = 6,
}
public enum AssetKey
{
    Alp1,
    Alp2,
    Alp3,
    Road1,
    Chunk1,
    SpawnPointMonster1,
    SpawnPointPlayer1,
    Person_Enemy,
    Entity_Monster_Default1,
    Entity_Player_Default1,
    Entity_Monster_Default2,
    Entity_Player_Default2,
    TestTimeLine,
    WorldUIEntityHint,
    Entity_Player_Hero1,
    EmitterElement_GuidedMissile,
    EmitterElement_SwordLow,

    Emitter_SwordLow,
    Emitter_SwordHeight,
    Emitter_GuidedMissileBaseCommon,
}
public class TableMgr: Singleton<TableMgr>
{
    private readonly Dictionary<EWorldObjectType, Color> DicMapColor = new()
    {
        { EWorldObjectType.None, Color.black },
        { EWorldObjectType.Road, Color.yellow },
        { EWorldObjectType.Construction, Color.gray },
        { EWorldObjectType.Wall, Color.cyan },
        { EWorldObjectType.Preson, Color.green },
    };

    public bool TryGetColorByObjectType(EWorldObjectType f_ObjectType, out Color f_Result)
    {
        return DicMapColor.TryGetValue(f_ObjectType, out f_Result);
    }



    private static readonly Dictionary<AssetKey, string> m_DicIDToPath = new()
    {
        { AssetKey.Alp1, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Alp2, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Alp3, "Prefabs/WorldObject/Entity_Alt1" },
        { AssetKey.Road1, "Prefabs/WorldObject/Entity_Road1" },
        { AssetKey.Chunk1, "Prefabs/WorldObject/Entity_Chunk1" },
        { AssetKey.SpawnPointMonster1, "Prefabs/WorldObject/Entity_SpawnPointMonster1" },
        { AssetKey.SpawnPointPlayer1, "Prefabs/WorldObject/Entity_SpawnPointPlayer1" },
        { AssetKey.Entity_Monster_Default1, "Prefabs/WorldObject/Entity_Monster_Default1" },
        { AssetKey.Entity_Monster_Default2, "Prefabs/WorldObject/Entity_Monster_Default2" },
        { AssetKey.Entity_Player_Default1, "Prefabs/WorldObject/Entity_Player_Default1" },
        { AssetKey.Entity_Player_Default2, "Prefabs/WorldObject/Entity_Player_Default2" },
        { AssetKey.Entity_Player_Hero1, "Prefabs/WorldObject/Entity_Player_Hero1" },
        
        { AssetKey.EmitterElement_GuidedMissile, "Prefabs/WorldObject/EmitterElement_GuidedMissile" },
        { AssetKey.EmitterElement_SwordLow, "Prefabs/WorldObject/EmitterElement_SwordLow" },

        { AssetKey.Emitter_SwordLow, "Prefabs/WorldObject/Emitter_SwordLow" },
        { AssetKey.Emitter_SwordHeight, "Prefabs/WorldObject/Emitter_SwordHeight" },
        { AssetKey.Emitter_GuidedMissileBaseCommon, "Prefabs/WorldObject/Emitter_GuidedMissileBaseCommon" },



        { AssetKey.TestTimeLine, "Prefabs/TimeLine/TestTimeLine" },
        { AssetKey.WorldUIEntityHint, "Prefabs/WorldUI/WorldUIEntityHint" },


    };
    public bool GetAssetPath(AssetKey f_Key, out string f_Result)
    {
        return m_DicIDToPath.TryGetValue(f_Key, out f_Result);
    }
}
