using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter_SwordLowData: Emitter_SwordBaseData
{
    public override AssetKey AssetPrefabID => AssetKey.Emitter_SwordLow;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public override WeaponElementBaseData GetWeaponElementData()
    {
        var value = new EmitterElement_GuidedMissileData(0, Initiator);
        return value;
    }

    public Emitter_SwordLowData(int f_Index, WorldObjectBaseData f_TargetEntity) : base(f_Index, f_TargetEntity)
    {

    }

    // �Ƿ�͸Ŀ��
    private bool m_IsPenetrate = false;
    private bool m_IsTargetStop = false;

    public void SetPenrtrate(bool f_IsPenrtrate)
    {
        m_IsPenetrate = f_IsPenrtrate;
    }
    public void SetTargetStop(bool f_IsTargetStop)
    {
        m_IsTargetStop = f_IsTargetStop;
    }
    // ��������
    public override bool GetStopCondition(WeaponElementBaseData f_Buttle, WorldObjectBaseData f_Target, float f_Ratio)
    {
        return f_Buttle.GetResistStatus() ? false : (m_IsPenetrate ? true :!f_Buttle.GetIsTarget());
    }

    public override Vector3 GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint, float f_Ratio)
    {
        var posValue = Vector3.Lerp(f_StartPoint, f_EndPoint, f_Ratio);
        return posValue;
    }

    public override void LaunchStartAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {

    }

    public override void LaunchStopAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {
        f_Element.ClearTargets();
        if (m_IsTargetStop)
        {

        }
        else
        {
            DestroyWeaponElementAsync(f_Element);
        }
    }

    public override void LaunchUpdateAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity, float f_Ratio)
    {
        var target = f_Element.GetNearTarget(AttackLayer);
        if (!GTools.RefIsNull(target) && target.CurStatus != EPersonStatusType.Die)
        {
            f_Element.AttackTargetAsync(target);
            f_Element.SetTargetAsync(m_IsPenetrate ? null : target);
        }
    }
    public override void CollectStartAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {

    }

    public override void CollectStopAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {

    }

    public override void CollectUpdateAsync(WeaponElementBaseData f_Element, Vector3 f_TargetPoint, float f_Ratio)
    {
        var target = f_Element.GetNearTarget(AttackLayer);
        if (!GTools.RefIsNull(target) && target.CurStatus != EPersonStatusType.Die)
        {
            f_Element.AttackTargetAsync(target);
        }
    }

    public override void CollectAwakeAsync(Vector3 f_Target)
    {

    }

    public override void CollectSleepAsync(Vector3 f_Target)
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override ResultData<WeaponElementBaseData> GetWeaponElementAsync()
    {
        var value = base.GetWeaponElementAsync();
        if (value.Result == EResult.Defeated)
        {
            var bubble = CreateWeaponElementAsync();
            value.SetData(bubble);
        }
        return value;
    }

}
public class Emitter_SwordLow : Emitter_SwordBase
{
    public override async UniTask StartExecute()
    {
        
    }

    public override async UniTask StopExecute()
    {
        
    }
}
