using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter_SwordHeightData: Emitter_SwordBaseData
{
    public Emitter_SwordHeightData(int f_Index, WorldObjectBaseData f_Target, float f_Speed):base(f_Index, f_Target)
    {
        m_speed = f_Speed;
    }
    private enum EWeaponStatus
    {
        Attack,
        Defense,
    }

    private float m_speed = 1;
    private bool m_IsPenetrate = true;
    private bool m_DefenseIsATK = false;
    private float m_DefenceRadius = 2.0f;
    private float m_OneElementRadius = 1.0f;
    private EWeaponStatus m_CurWeaponStatus = EWeaponStatus.Attack;

    public override AssetKey AssetPrefabID => AssetKey.Emitter_SwordHeight;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public override void LaunchStartAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {
    }

    public override void LaunchUpdateAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity, float f_Ratio)
    {
        var target =  f_Element.GetNearTarget(AttackLayer);
        if (!GTools.RefIsNull(target) && target.CurStatus != EPersonStatusType.Die)
        {
            f_Element.AttackTargetAsync(target);
            f_Element.SetTargetAsync(m_IsPenetrate ? null : target);
        }
    }

    public override void LaunchStopAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity)
    {
        f_Element.ClearTargets();
        DestroyWeaponElementAsync(f_Element);

        var curCount = GetCurrentElementCountAsync();


        if (m_CurWeaponStatus == EWeaponStatus.Defense && curCount <= 0)
        {
            CollectAwakeAsync(Vector3.zero);
        }
    }

    public override void CollectAwakeAsync(Vector3 f_Target)
    {
        switch (m_CurWeaponStatus)
        {
            default:
                break;
            case EWeaponStatus.Attack:
                {
                    m_IsAotuMove = false;
                    // ��ʼ������
                    InitElementCount((ushort)Count);
                    m_CurWeaponStatus = EWeaponStatus.Defense;
                    m_LaunchStatus = m_DefenseIsATK ? m_LaunchStatus : EStatus.Close;
                }
                break;
            case EWeaponStatus.Defense:
                {
                    m_CurWeaponStatus = EWeaponStatus.Attack;
                    m_IsAotuMove = true;
                    m_LaunchStatus = EStatus.Enable;
                }
                break;
        }
    }

    public override void CollectStartAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {
    }

    public override void CollectUpdateAsync(WeaponElementBaseData f_Element, Vector3 f_TargetPoint, float f_Ratio)
    {
    }

    public override void CollectStopAsync(WeaponElementBaseData f_Element, Vector3 f_Target)
    {
    }

    public override void CollectSleepAsync(Vector3 f_Target)
    {
    }

    public override Vector3 GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint, float f_Ratio)
    {
        var posValue = Vector3.Lerp(f_StartPoint, f_EndPoint, f_Ratio);
        return posValue;
    }

    public override bool GetStopCondition(WeaponElementBaseData f_Buttle, WorldObjectBaseData f_Target, float f_Ratio)
    {
        return f_Buttle.GetResistStatus() ? false : m_IsPenetrate ? true : !f_Buttle.GetIsTarget();
    }

    public override WeaponElementBaseData GetWeaponElementData()
    {
        var result = new EmitterElement_SwordLowData(0, Initiator);
        return result;
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

    public override async void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurWeaponStatus == EWeaponStatus.Defense)
        {
            var allWeapon = m_AllSwordElements[ESwordStatus.Prepare];
            var radius = ((float)allWeapon.Count / Count + 1) * m_DefenceRadius;
            await GTools.ParallelTaskAsync((ushort)allWeapon.Count, async (index) =>
            {
                var element = allWeapon[index];
                var curWorldPos = element.WorldPosition;

                var spherePoint = GTools.MathfMgr.GetSpherePoint(360.0f / allWeapon.Count * index + 360 * GTools.CurTime) * radius;
                var worldPos = new Vector3(spherePoint.x, 0, spherePoint.y) + WorldPosition;

                // ����λ��
                {
                    var curPos = element.WorldPosition;
                    var targetPos = Vector3.Lerp(curPos, worldPos, GTools.UpdateDeltaTime * m_speed);
                    element.SetPosition(targetPos);
                }


                // ���ó���
                {
                    // ǰ����
                    var curForward = element.Forward;
                    var targetForward = Vector3.Lerp(curForward, Vector3.up, GTools.UpdateDeltaTime * 10);
                    allWeapon[index].SetForward(targetForward);

                    // �Ϸ��� + ����
                    var centralPoint = WorldPosition;
                    Vector2 direction = new Vector2(element.WorldPosition.x - centralPoint.x, element.WorldPosition.z - centralPoint.z);
                    var dir = Vector2.Dot(direction.normalized, Vector2.right);
                    var angle = Mathf.Acos(dir) * Mathf.Rad2Deg + 90.0f;

                    var forward = Vector3.Cross(Vector2.right, direction);

                    var curRotation = allWeapon[index].LocalRotation;
                    curRotation.x = 90;
                    curRotation.z = angle * forward.z / Mathf.Abs(forward.z);
                    allWeapon[index].SetLocalRotation(curRotation);
                }


                // �赲���е���
                {
                    // �����ɽ��赲С��Χ
                    //List<WeaponElementBase> collArr2 = GTools.MathfMgr.GetOverlapBox<WeaponElementBase>(worldPos, element.BosSize * 0.5f, element.Rotation, ELayer.FlyingProp, (target) =>
                    //{
                    //    var condition1 = target.CurStatus == ESwordStatus.Launch;
                    //    var condition2 = (target.LayerMask & m_AttackLayer) != 0;
                    //    return condition1 
                    //            && condition2;
                    //});
                    List<WeaponElementBaseData> collArr2 = GTools.MathfMgr.GetTargets_Sphere<WeaponElementBaseData>(worldPos, m_OneElementRadius, ELayer.FlyingProp, (target) =>
                    {
                        var condition1 = target.CurWeaponStatus == ESwordStatus.Launch;
                        var condition2 = (target.LayerMask & AttackLayer) != 0;
                        return condition1
                                && condition2;
                    });
                    if (collArr2.Count > 0)
                    {
                        await GTools.ParallelTaskAsync(collArr2, async (item) =>
                        {
                            await item.BeResistAsync();
                        });
                    }

                    // �赲һ��Բ��
                }
            });


        
    }

}
}
public class Emitter_SwordHeight : Emitter_SwordBase
{
    public override async UniTask StartExecute()
    {
    }

    public override async UniTask StopExecute()
    {
    }
}
