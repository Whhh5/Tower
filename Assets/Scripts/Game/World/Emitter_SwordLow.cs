using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter_SwordLow : Emitter_SwordBase
{
    // ½áÊøÌõ¼þ
    public override bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio)
    {
        return f_Buttle.GetResistStatus() ? false : !f_Buttle.GetIsTarget();
    }

    public override async UniTask<Vector3> GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint, float f_Ratio)
    {
        var posValue = Vector3.Lerp(f_StartPoint, f_EndPoint, f_Ratio);
        return posValue;
    }

    public override async UniTask LaunchStartAsync(WeaponElementBase f_Element, Entity f_Entity)
    {

    }

    public override async UniTask LaunchStopAsync(WeaponElementBase f_Element, Entity f_Entity)
    {
        f_Element.ClearTargets();
    }

    public override async UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio)
    {
        var target = await f_Element.GetNearTarget(m_AttackLayer);
        if (!GTools.RefIsNull(target) && target.IsActively)
        {
            await f_Element.AttackTargetAsync(target);
            await f_Element.SetTargetAsync(target);
        }
    }
    public override async UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {

    }

    public override async UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {

    }

    public override async UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, float f_Ratio)
    {
        var target = await f_Element.GetNearTarget(m_AttackLayer);
        if (!GTools.RefIsNull(target) && target.IsActively)
        {
            await f_Element.AttackTargetAsync(target);
        }
    }

    public override async UniTask CollectAwakeAsync(Vector3 f_Target)
    {

    }

    public override async UniTask CollectSleepAsync(Vector3 f_Target)
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

}
