using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Emitter_GuidedMissileBaseCommon : Emitter_GuidedMissileBase
{

    public override bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio)
    {
        return true;
    }





    public override async UniTask LaunchStartAsync(WeaponElementBase f_Element, Entity f_Entity)
    {
        
    }

    public override async UniTask LaunchStopAsync(WeaponElementBase f_Element, Entity f_Entity)
    {
        
    }

    public override async UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio)
    {

    }

    public override async UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {
        
    }
    public override async UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {
        
    }

    public override async UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, Entity f_Target, float f_Ratio)
    {
        
    }


}
