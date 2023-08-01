using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Emitter_GuidedMissileBaseData: WeaponBaseData
{
    public Emitter_GuidedMissileBaseData(int f_Index, WorldObjectBaseData f_TargetEntity) : base(f_Index, f_TargetEntity)
    {

    }

    [SerializeField] // 
    protected Vector2 m_MoveTimeOffset = Vector2.one;

    [SerializeField] protected Vector2 m_OffsetValue = Vector2.zero;
    [SerializeField] protected Vector2 m_FormBezierLength = Vector2.one;
    [SerializeField] protected Vector2 m_ToBezierLength = Vector2.one;
    [SerializeField] protected Vector2Int m_CountRandom = Vector2Int.one;
    [SerializeField] protected ushort m_MaxAttactCount = 1;

    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();
        var targets = GTools.MathfMgr.GetTargets_Sphere(WorldPosition, Radius, AttackLayer);
        if (targets.Count > 0)
        {
            List<(Vector3 tForm, Vector3 tTo)> pointList = new();
            foreach (var item in targets)
            {
                pointList.Add((WorldPosition, item.CentralPoint));
            }

            GTools.DrawGizom.AddLine(UpdateLevelID, pointList);
        }
        else
        {
            GTools.DrawGizom.ClearLine(UpdateLevelID);
        }
    }


    public override async void StartExecute()
    {
        var targets = GTools.MathfMgr.GetTargets_Sphere(WorldPosition, Radius, AttackLayer);
        if (targets.Count <= 0)
        {
            return;
        }

        Targets = targets;
        int[] fireCount = new int[targets.Count];
        var taskCount = 0;
        var targetsCount = Mathf.Min(m_MaxAttactCount, targets.Count);
        for (int i = 0; i < targetsCount; i++)
        {
            var value = GTools.MathfMgr.GetRandomValue(m_CountRandom.x, m_CountRandom.y) + Count;
            fireCount[i] = value;
            taskCount += value;
        }

        UniTask[] tasks = new UniTask[taskCount];
        var taskIndex = 0;
        for (int i = 0; i < targetsCount; i++)
        {
            var item = targets[i];
            var fireNum = fireCount[i];

            var targetPoint = item.BeHitPoint;

            for (int j = 0; j < fireNum; j++)
            {
                tasks[taskIndex++] = UniTask.Create(async () => { LaunchAsync(item); });
            }
        }


        await UniTask.WhenAll(tasks);
    }

    public override async void StopExecute()
    {
    }


    // Ŀ�����
    public override async void LaunchAsync(EntityData f_Target)
    {
        var targetPoint = f_Target.CentralPoint;
        var curMaxTime = Vector3.Distance(targetPoint, WorldPosition) / Radius;
        // �ƶ�ʱ��
        var moveTime = (GTools.MathfMgr.GetRandomValue(m_MoveTimeOffset.x, m_MoveTimeOffset.y) + curMaxTime) /
                       LaunchSpeed;
        // ��ʼ�� bezier ���Ʊ�����
        var thisLength = Random.Range(m_FormBezierLength.x, m_FormBezierLength.y);
        // Ŀ��� bezier ���Ʊ�����
        var targetLength = Random.Range(m_ToBezierLength.x, m_ToBezierLength.y);
        // ��ǰ���ӵ���ʼ����
        var forward = Forward;
        // ��ǰ�ӵ���ʼλ��
        var startPoint = WorldPosition;
        var direction = new Vector3(GTools.MathfMgr.GetRandomValue(m_OffsetValue.x, m_OffsetValue.y),
            GTools.MathfMgr.GetRandomValue(m_OffsetValue.x, m_OffsetValue.y),
            GTools.MathfMgr.GetRandomValue(1, 1 + m_OffsetValue.y));
        // ��ǰ bezier ��ʼ����Ʊ�ƫ����
        var dirOffset = Tran.TransformDirection(direction.normalized);
        // ��ǰ bezier ��ʼ����Ʊ�����
        var formLength = dirOffset * thisLength;
        // ��ǰ bezier Ŀ�����Ʊ�����
        var toLength = dirOffset * targetLength;

        // ��ȡһ���ӵ�
        var buttle = CreateWeaponElementAsync(WorldPosition);
        buttle.SetPosition(startPoint);
        buttle.SetForward(forward);

        buttle.StartExecute();
        LaunchStartAsync(buttle, f_Target);
        // �����ӵ��˶�
        await GTools.DoTweenAsync(moveTime, async (value) =>
        {
            var worldPos = GTools.MathfMgr.GetBezierValue(startPoint, targetPoint, formLength, toLength, value);
            buttle.SetForward(worldPos - buttle.CentralPoint);
            buttle.SetPosition(worldPos);
            LaunchUpdateAsync(buttle, f_Target, value);
        }, (value) => GetStopCondition(buttle, f_Target, value));
        // �ӵ�����
        buttle.StopExecute();
        LaunchStopAsync(buttle, f_Target);
    }


    public override void CollectAsync(Vector3 f_Point)
    {
    }

    public abstract void LaunchStartAsync(WeaponElementBaseData f_Element, EntityData f_Entity);
    public abstract void LaunchUpdateAsync(WeaponElementBaseData f_Element, EntityData f_Entity, float f_Ratio);
    public abstract void LaunchStopAsync(WeaponElementBaseData f_Element, EntityData f_Entity);
    public abstract void CollectStartAsync(WeaponElementBaseData f_Element, Vector3 f_Target);

    public abstract void CollectUpdateAsync(WeaponElementBaseData f_Element, Vector3 f_TargetPoint, EntityData f_Target,
        float f_Ratio);

    public abstract void CollectStopAsync(WeaponElementBaseData f_Element, Vector3 f_Target);
    public abstract bool GetStopCondition(WeaponElementBaseData f_Buttle, EntityData f_Target, float f_Ratio);
}
public abstract class Emitter_GuidedMissileBase : WeaponBase
{
}
