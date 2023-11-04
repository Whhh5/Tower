using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using DG.Tweening;
using UnityEngine.EventSystems;


public class Emitter_GuidedMissileBaseCommonData: Emitter_GuidedMissileBaseData
{
    public Emitter_GuidedMissileBaseCommonData(int f_Index, WorldObjectBaseData f_TargetEntity) : base(f_Index, f_TargetEntity)
    {
        SetPosition(f_TargetEntity.WeaponPoint);
    }

    public override EAssetKey AssetPrefabID => EAssetKey.Emitter_GuidedMissileBaseCommon;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public override void CollectStartAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {
    }

    public override void CollectStopAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {
    }

    public override void CollectUpdateAsync(WeaponElementBaseData f_Element, Vector3 f_TargetPoint, WorldObjectBaseData f_Target, float f_Ratio)
    {
    }

    public override bool GetStopCondition(WeaponElementBaseData f_Buttle, WorldObjectBaseData f_Target, float f_Ratio)
    {
        return true;
    }

    public override WeaponElementBaseData GetWeaponElementData()
    {
        var value = new EmitterElement_GuidedMissileData(0, Initiator as WorldObjectBaseData);
        return value;
    }

    public override void LaunchStartAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {
    }

    public override void LaunchStopAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {
        DestroyWeaponElementAsync(f_Element);
    }

    public override void LaunchUpdateAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity, float f_Ratio)
    {
    }
}
public class Emitter_GuidedMissileBaseCommon : Emitter_GuidedMissileBase
{
    public override async UniTask StartExecute()
    {
    }

    public override async UniTask StopExecute()
    {
    }
}
