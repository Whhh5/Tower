using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 路面对象
public sealed class WorldBaseObjectDataRoad : DependChunkData
{
    public WorldBaseObjectDataRoad(int f_Index, int f_TargetChunk) : base(f_Index, f_TargetChunk)
    {
    }

    public override EWorldObjectType ObjectType => EWorldObjectType.Road;

    public override ELayer LayerMask => ELayer.Terrain;

    public override ELayer AttackLayerMask => ELayer.Default;

    public override AssetKey AssetPrefabID => AssetKey.Road1;
}
public abstract class Entity_Road : DependChunk
{

}
