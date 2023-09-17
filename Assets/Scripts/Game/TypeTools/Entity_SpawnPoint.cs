using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_SpawnPointData : WorldObjectBaseData
{

    public Entity_SpawnPointData(int f_Index, int f_ChunkIndex, EDirection f_Direction) : base(f_Index, f_ChunkIndex)
    {
        DoorDirection = f_Direction;
    }
    public override EEntityType EntityType => EEntityType.Item;
    public override EWorldObjectType ObjectType => EWorldObjectType.Construction;

    public override AssetKey AssetPrefabID => AssetKey.SpawnPointMonster1;

    public EDirection DoorDirection { get; private set; }
    // π÷ŒÔ¡–±Ì
    public Dictionary<int, WorldObjectBase> EntityList = new();
    // 
}
public abstract class Entity_SpawnPoint : WorldObjectBase
{

}