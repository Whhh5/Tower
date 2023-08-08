using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;


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
    public override int ChangeBlood(ChangeBloodData f_Increment)
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

public class WorldObjectBase : Person, IUpdateBase
{
    public override EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    public override void OnUpdate()
    {
        base.OnUpdate();


        if (Input.GetMouseButtonUp(0))
        {

        }
    }

    private void OnMouseDown()
    {
        
    }
}
