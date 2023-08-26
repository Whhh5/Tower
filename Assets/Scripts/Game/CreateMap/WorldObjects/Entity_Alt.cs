using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


// 山体对象
public sealed class WorldObjectBaseObjectDataAlt : DependChunkData
{
    public float Height { get; private set; }

    public WorldObjectBaseObjectDataAlt(int f_Index, int f_TargetChunk, float f_Height) : base(f_Index, f_TargetChunk)
    {
        Height = f_Height;
    }

    private Dictionary<int, AssetKey> m_DicAsset = new()
    {
        { 0, AssetKey.Alp1 },
        { 1, AssetKey.Alp2 },
        { 2, AssetKey.Alp3 },
    };

    public override EWorldObjectType ObjectType => EWorldObjectType.Wall;

    public override ELayer LayerMask => ELayer.Terrain;

    public override ELayer AttackLayerMask => ELayer.Default;

    public override AssetKey AssetPrefabID
    {
        get {
            var index = Random.Range(0, m_DicAsset.Count);
            return m_DicAsset[index];
        }
    }
}
public class Entity_Alt : DependChunk
{
}
