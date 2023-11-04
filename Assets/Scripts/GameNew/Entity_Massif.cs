using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_MassifData : DependChunkData
{
    public Entity_MassifData() : base()
    {
    }

    public override ELayer LayerMask => ELayer.Terrain;

    public override ELayer AttackLayerMask => ELayer.Default;

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Marrizer;

    public override EWorldObjectType ObjectType => EWorldObjectType.Wall;

    public override void InitData(int f_ChunkIndex)
    {
        base.InitData(f_ChunkIndex);
    }

}
public class Entity_Massif : DependChunk
{

}
