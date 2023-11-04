using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


// 山体对象
public sealed class WorldObjectBaseObjectDataAlt : DependChunkData
{
    public float Height { get; private set; }

    public WorldObjectBaseObjectDataAlt() : base()
    {
        
    }

    private Dictionary<int, EAssetKey> m_DicAsset = new()
    {
        { 0, EAssetKey.Alp1 },
        { 1, EAssetKey.Alp2 },
        { 2, EAssetKey.Alp3 },
    };

    public override EWorldObjectType ObjectType => EWorldObjectType.Wall;

    public override ELayer LayerMask => ELayer.Terrain;

    public override ELayer AttackLayerMask => ELayer.Default;

    public override EAssetKey AssetPrefabID
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
