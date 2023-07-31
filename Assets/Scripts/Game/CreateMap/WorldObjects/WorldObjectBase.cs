using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WorldObjectBase : Person
{
}


public abstract class WorldObjectBaseData : PersonData
{
    protected WorldObjectBaseData(int f_index, int f_ChunkIndex) : base(f_index)
    {
        CurrentIndex = f_ChunkIndex;
        CurrentMapKey = WorldMapManager.Ins.CurrentMapKey;
    }

    public int CurrentMapKey { get; private set; }
    public int CurrentIndex { get; protected set; }
    public void SetCurrentChunkIndex(int f_ToIndex)
    {
        CurrentIndex = f_ToIndex;
    }
    public override void AfterLoad()
    {
        base.AfterLoad();

        if (WorldMapManager.Ins.TryGetChunkData(CurrentIndex, out var targetChunk))
        {
            SetPosition(targetChunk.PointUp);
        }

        if (GTools.TableMgr.TryGetColorByObjectType(ObjectType, out var color))
        {
            SetColor(color);
        }
    }
    public override void OnUnLoad()
    {
        WorldMapManager.Ins.RemoveChunkElement(this);
        base.OnUnLoad();
    }
    public override int ChangeBlood(int f_Increment)
    {
        var value = base.ChangeBlood(f_Increment);

        if (value <= 0)
        {
            SetPersonStatus(EPersonStatusType.Die, EAnimatorStatus.Stop);
            WorldMapManager.Ins.RemoveChunkElement(this);
        }

        return value;
    }
}

public abstract class Entity_SpawnPointData : WorldObjectBaseData
{

    public Entity_SpawnPointData(int f_Index, int f_ChunkIndex, EDirection f_Direction) : base(f_Index, f_ChunkIndex)
    {
        DoorDirection = f_Direction;
    }

    public override EWorldObjectType ObjectType => EWorldObjectType.Construction;

    public override AssetKey AssetPrefabID => AssetKey.SpawnPointMonster1;

    public EDirection DoorDirection { get; private set; }
    // 怪物列表
    public Dictionary<int, WorldObjectBase> EntityList = new();
    // 
}
public abstract class Entity_SpawnPoint : WorldObjectBase
{

}