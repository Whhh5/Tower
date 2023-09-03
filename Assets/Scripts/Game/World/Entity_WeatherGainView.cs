using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_WeatherGainViewData : UnityObjectData
{
    public Entity_WeatherGainViewData() : base(0)
    {
    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_WeatherGainView;

    public override EWorldObjectType ObjectType => EWorldObjectType.Wall;
}
public class Entity_WeatherGainView : ObjectPoolBase
{
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

    }
    private void OnMouseEnter()
    {

    }
    private void OnMouseExit()
    {

    }
}
