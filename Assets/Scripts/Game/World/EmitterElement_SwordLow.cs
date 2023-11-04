using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterElement_SwordLowData : WeaponElementBaseData
{
    public EmitterElement_SwordLowData(int f_index, WorldObjectBaseData f_Initiator) : base(f_index, f_Initiator)
    {
        Initiator = f_Initiator;
    }
    public override EAssetKey AssetPrefabID => EAssetKey.EmitterElement_SwordLow;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
}
public class EmitterElement_SwordLow : WeaponElementBase
{

}
