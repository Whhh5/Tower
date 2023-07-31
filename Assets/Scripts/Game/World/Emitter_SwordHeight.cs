using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter_SwordHeight : Emitter_SwordBase
{
    private enum EWeaponStatus
    {
        Attack,
        Defense,
    }
    [SerializeField] // �Ƿ񴥷���͸
    private bool m_IsPenetrate = false;
    [SerializeField] // ����״̬�Ƿ�������
    private bool m_DefenseIsATK = false;
    [SerializeField, Range(0.2f, 5.0f)] // ����״̬�뾶
    private float m_DefenceRadius = 1.0f;
    [SerializeField, Range(0.2f, 5.0f)] // ���ѽ��񵲰뾶
    private float m_OneElementRadius = 1.0f;
    [SerializeField, Range(0, 1)] // 
    private float m_TestFloat = 1.0f;
    [SerializeField]
    private EWeaponStatus m_CurWeaponStatus = EWeaponStatus.Attack;


    public override async UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {

    }

    public override async UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {

    }

    public override async UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, float f_Ratio)
    {

    }

    public override bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio)
    {
        return f_Buttle.GetResistStatus() ? false : m_IsPenetrate ? true : !f_Buttle.GetIsTarget();
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
        await DestroyWeaponElementAsync(f_Element);

        var curCount = await GetCurrentElementCountAsync();


        if (m_CurWeaponStatus == EWeaponStatus.Defense && curCount <= 0)
        {
            await CollectAwakeAsync(Vector3.zero);
        }
    }

    public override async UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio)
    {
        var target = await f_Element.GetNearTarget(m_AttackLayer);
        if (!GTools.RefIsNull(target) && target.IsActively)
        {
            await f_Element.AttackTargetAsync(target);
            await f_Element.SetTargetAsync(m_IsPenetrate ? null : target);
        }
    }

    public override async UniTask<ResultData<WeaponElementBase>> GetWeaponElementAsync()
    {
        var result = await base.GetWeaponElementAsync();
        if (result.Result == EResult.Defeated)
        {
            var element = await CreateWeaponElementAsync(Position);
            result.SetData(element);
        }
        return result;
    }

    public override async UniTask CollectAwakeAsync(Vector3 f_Target)
    {
        switch (m_CurWeaponStatus)
        {
            default:
                break;
            case EWeaponStatus.Attack:
                {
                    m_IsAotuMove = false;
                    // ��ʼ������
                    await InitElementCount(m_Count);
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

    public override async UniTask CollectSleepAsync(Vector3 f_Target)
    {

    }


    public override async void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurWeaponStatus == EWeaponStatus.Defense)
        {
            var allWeapon = m_AllSwordElements[ESwordStatus.Prepare];
            var radius = ((float)allWeapon.Count / m_Count + 1) * m_DefenceRadius;
            await GTools.ParallelTaskAsync((ushort)allWeapon.Count, async (index) =>
            {
                var element = allWeapon[index];
                var curWorldPos = element.Position;

                var spherePoint = GTools.MathfMgr.GetSpherePoint(360.0f / allWeapon.Count * index + 360 * GTools.CurTime) * radius;
                var worldPos = new Vector3(spherePoint.x, 0, spherePoint.y) + m_TargetPerson.CentralPoint.position;

                // ����λ��
                {
                    var curPos = element.Position;
                    var targetPos = Vector3.Lerp(curPos, worldPos, GTools.UpdateDeltaTime * 10);
                    element.SetWorldPos(targetPos);
                }


                // ���ó���
                {
                    // ǰ����
                    var curForward = element.Forward;
                    var targetForward = Vector3.Lerp(curForward, Vector3.up, GTools.UpdateDeltaTime * 10);
                    allWeapon[index].SetForward(targetForward);

                    // �Ϸ��� + ����
                    var centralPoint = m_TargetPerson.CentralPoint.position;
                    Vector2 direction = new Vector2(element.Position.x - centralPoint.x, element.Position.z - centralPoint.z);
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
                    List<WeaponElementBase> collArr2 = GTools.MathfMgr.GetTargets_Sphere<WeaponElementBase>(worldPos, m_OneElementRadius, ELayer.FlyingProp, (target) =>
                    {
                        var condition1 = target.CurStatus == ESwordStatus.Launch;
                        var condition2 = (target.LayerMask & m_AttackLayer) != 0;
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



    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }

}
