using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Effect_AddBloodData : UnityObjectData
{
    public Entity_Effect_AddBloodData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Effect_AddBlood;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;



}
public class Entity_Effect_AddBlood : ObjectPoolBase
{

}
