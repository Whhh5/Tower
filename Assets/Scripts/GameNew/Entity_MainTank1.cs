using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_MainTank1Data : Entity_Hero_MainTankBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_MainTank1;

    public override EHeroCardType HeroType => EHeroCardType.Hero4;
}
public class Entity_MainTank1 : Entity_Hero_MainTankBase
{

}

