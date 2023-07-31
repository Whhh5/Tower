using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter_Sword3 : Emitter_SwordBase
{

    // �����߶�
    float height = 3.0f;
    // �漴�߶�
    Vector2 heightRandom = Vector2.one * 2.0f;
    // ��Ŀ��ľ���
    Vector2 toTargetDisRandom = Vector2.one * 2;
    // �ٶ�
    Vector2 speedRandom = Vector2.one * 5.0f;
    // ѭ������
    Vector2 loopCountRamdom = Vector2.one * 3;
    // �뾶
    Vector2 radiusRamdom = Vector2.one * 2;

    public async UniTask Test1(Entity f_Target)
    {

        // ÿ��ѭ����ˢ��
        float distance = GTools.MathfMgr.GetRandomValue(toTargetDisRandom.x, toTargetDisRandom.y);
        float speed = GTools.MathfMgr.GetRandomValue(speedRandom.x, speedRandom.y);
        float loopCount = GTools.MathfMgr.GetRandomValue(loopCountRamdom.x, loopCountRamdom.y);


        // ����Ŀ���
        var target = f_Target;

        var list = m_AllSwordElements[ESwordStatus.Prepare];

        await GTools.ParallelTaskAsync((ushort)list.Count, async (index) =>
        {
            var targetElement = list[index];


            // ��ʼλ��
            var angle = 90;
            var radius = 2;
            var spherePoint = GTools.MathfMgr.GetSpherePoint(angle, radius);
            var worldPos = target.Position + new Vector3(spherePoint.x, height, spherePoint.y);


            List<(Vector3 tTarget, Vector3 tFormDir, Vector3 tToDir, float tTime)> pointLisy = new()
            {

            };
            for (int i = 0; i < 3; i++)
            {
                var worldPosData = GetWorldPosData(worldPos, target);
                pointLisy.Add((worldPosData.tWorldPos1, (worldPosData.tWorldPos1 - worldPos).normalized, (worldPos - worldPosData.tWorldPos1).normalized, 1));
                pointLisy.Add((worldPosData.tWorldPos2, (worldPosData.tWorldPos2 - target.Position).normalized, (target.Position - worldPosData.tWorldPos2).normalized, 1));
                worldPos = worldPosData.tWorldPos2;
            }


            for (int i = 0; i < pointLisy.Count; i++)
            {
                var itemData = pointLisy[i];
                await Skill3(targetElement, itemData.tTarget, itemData.tFormDir, itemData.tToDir, itemData.tTime);
            }
        });



    }
    public (Vector3 tWorldPos1, Vector3 tWorldPos2) GetWorldPosData(Vector3 f_StartPos, Entity f_Target)
    {
        // ǰ������
        var toTargetDir = f_Target.Position - f_StartPos;
        var forword = toTargetDir.normalized;
        // ��ǰѭ��Ŀ��λ��
        var rangeDis = GTools.MathfMgr.GetRandomValue(toTargetDisRandom.x, toTargetDisRandom.y);
        var curLoopTargetPoint = rangeDis * forword + toTargetDir;


        // ��ǰѭ��������
        var rangeHeight = GTools.MathfMgr.GetRandomValue(heightRandom.x, heightRandom.y);
        var rangeAngle = GTools.MathfMgr.GetRandomValue(0, 360.0f);
        var rangeRadius = GTools.MathfMgr.GetRandomValue(radiusRamdom.x, radiusRamdom.y);
        var spherePoint = GTools.MathfMgr.GetSpherePoint(rangeAngle, rangeRadius);
        var endPoint = f_Target.Position + new Vector3(spherePoint.x, rangeHeight, spherePoint.y);

        return (curLoopTargetPoint, endPoint);
    }



    public async UniTask Skill3(WeaponElementBase f_Element, Vector3 f_Target, Vector3 f_FormeDircetion, Vector3 f_ToDircetion, float f_Time)
    {
        var startPoint = f_Element.Position;
        await GTools.DoTweenAsync(f_Time, async (value) =>
        {
            var bezierValue = GTools.MathfMgr.GetBezierValue(startPoint, f_Target, f_FormeDircetion, f_ToDircetion, value);
            f_Element.SetForward(bezierValue - f_Element.Position);
            f_Element.SetWorldPos(bezierValue);


        }, (value) => true);
    }


    public override async UniTask LaunchStartAsync(WeaponElementBase f_Element, Entity f_Entity)
    {
        
    }

    public override async UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio)
    {
        
    }

    public override async UniTask LaunchStopAsync(WeaponElementBase f_Element, Entity f_Entity)
    {
        
    }

    public override async UniTask CollectAwakeAsync(Vector3 f_Target)
    {
        
    }

    public override async UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {
        
    }

    public override async UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, float f_Ratio)
    {
        
    }

    public override async UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target)
    {
        
    }

    public override async UniTask CollectSleepAsync(Vector3 f_Target)
    {
        
    }

    public override async UniTask<Vector3> GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint, float f_Ratio)
    {
        return Vector3.zero;
    }

    public override bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio)
    {
        return true;
    }


}
