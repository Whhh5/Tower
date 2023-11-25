using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_GameBackgroundData : UnityObjectData
{
    public Entity_GameBackgroundData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_GameBackground;
    public override EWorldObjectType ObjectType => EWorldObjectType.None;
}
public class Entity_GameBackground : ObjectPoolBase
{

}
