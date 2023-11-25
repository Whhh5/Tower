using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Effect_HeroToWarSeatData : UnityObjectData
{
    public Entity_Effect_HeroToWarSeatData() : base(0)
    {

    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Effect_HeroToWarSeat;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public override async void AfterLoad()
    {
        base.AfterLoad();

        await UniTask.Delay(3000);
        ILoadPrefabAsync.UnLoad(this);
    }
}
public class Entity_Effect_HeroToWarSeat : ObjectPoolBase
{

}
