using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;


public class WeaponMgr : Singleton<WeaponMgr>
{
    public async UniTask<WeaponBase> GetWeaponAsync(EAssetName f_WeaponName)
    {
        var weapon =
            await GTools.AssetsMgr.LoadPrefabPoolAsync<WeaponBase>(f_WeaponName, null, Vector3.zero, true, null);
        return weapon;
    }


    public async UniTask<WeaponBase> GetSetWeaponAsync(EAssetName f_WeaponName, Person f_TargetEntity)
    {
        var weapon = await GetWeaponAsync(f_WeaponName);
        await weapon.InitializationAsync(f_TargetEntity);
        return weapon;
    }

    public async UniTask DestroyWeaponAsync(WeaponBase f_Weapon)
    {
        await GTools.AssetsMgr.UnLoadPrefabPoolAsync<WeaponBase>(f_Weapon);
    }
}

public enum ESwordStatus : ushort
{
    None,

    // ׼��
    Prepare,

    // ȡ��
    Take,

    // ����
    Launch,

    // ����
    Score,

    // ����
    Dropping,

    // �ջ�
    Collect,

    EnumCount,
}

#region ��������

public abstract class WeaponElementBaseData: EntityData
{
    protected WeaponElementBaseData(int f_index) : base(f_index)
    {
    }
}
public abstract class WeaponElementBase : Entity, IEntityDestry
{
    [SerializeField] protected WeaponBase m_TargetEmttier = null;
    public WeaponBase TargetEmttier => m_TargetEmttier;
    [SerializeField] protected uint m_Harm = 0;
    [SerializeField] protected ESwordStatus m_CurStatus = ESwordStatus.None;
    public ESwordStatus CurStatus => m_CurStatus;
    [SerializeField] protected float m_Radius = 0;
    [SerializeField] protected EDamageType m_DamegeType = EDamageType.None;
    [SerializeField] protected Entity m_Target = null;
    [SerializeField] protected Vector3 m_ToTargetDistance = Vector3.zero;
    [SerializeField] protected Dictionary<int, Entity> m_Targets = new();
    [SerializeField] protected Vector3 m_Size = Vector3.zero;
    public Vector3 BosSize => GetComponent<BoxCollider>().size;

    /// <summary>
    /// �Ƿ��赲
    /// </summary>
    [SerializeField] protected bool m_IsBeResist = false;

    [SerializeField] protected ELayer m_LayerMask = ELayer.Default;
    public ELayer LayerMask => m_LayerMask;

    public async UniTask OnConstructedAsync(ELayer f_LayerMask)
    {
        m_LayerMask = f_LayerMask;
    }

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        m_LayerMask = ELayer.Default;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_CurStatus == ESwordStatus.Score && GetIsTarget())
        {
            //await SetForward((m_Target.Forward + m_ToTargetDirection).normalized);
            SetWorldPos(m_Target.Position + m_ToTargetDistance);
        }
    }

    public override async UniTask StartExecute()
    {
        m_Target = null;
        m_IsBeResist = false;
    }

    public override async UniTask StopExecute()
    {
    }

    public async UniTask<List<Entity>> GetTargets(ELayer m_AttackLayer)
    {
        var targets = GTools.MathfMgr.GetTargets_Sphere(CentralPoint.position, m_Radius, m_AttackLayer);
        return targets;
    }

    public async UniTask<Entity> GetNearTarget(ELayer m_AttackLayer)
    {
        var target = GTools.MathfMgr.NearerTarget(CentralPoint.position, m_Radius, m_AttackLayer);
        return target;
    }

    public async UniTask SetTargetAsync(Entity f_Target)
    {
        if (f_Target == null) return;
        m_Target = f_Target;
        m_ToTargetDistance = Position - f_Target.Position;
    }

    public bool GetIsTarget()
    {
        return !GTools.RefIsNull(m_Target) && m_Target.IsActively;
    }

    public async UniTask AttackTargetAsync(Entity f_Target)
    {
        if (!m_Targets.ContainsKey(f_Target.UpdateLevelID))
        {
            await f_Target.RediuceBlood(m_Harm, m_DamegeType);
            m_Targets.Add(f_Target.UpdateLevelID, f_Target);
        }
    }

    public void Setstatus(ESwordStatus f_status)
    {
        m_CurStatus = f_status;
    }

    public void ClearTargets()
    {
        m_Targets.Clear();
    }

    public void SetTargetPerson(WeaponBase f_TargetEmttier)
    {
        m_TargetEmttier = f_TargetEmttier;
    }

    public async UniTask CurEntityDestroyAsync()
    {
    }

    public bool GetResistStatus()
    {
        return m_IsBeResist;
    }

    /// <summary>
    /// ��ǰ�������赲
    /// </summary>
    /// <returns></returns>
    public async UniTask BeResistAsync()
    {
        m_IsBeResist = true;
    }
}

public abstract class WeaponBaseData<T> : VirtualEntityData
    where T : WeaponBase
{
    protected WeaponBaseData(int f_Index) : base(f_Index)
    {
    }
}

public abstract class WeaponBase : VirtualEntity, IEntityDestry
{
    [SerializeField] protected float m_UnitTime = 0;
    [SerializeField] protected ushort m_Count = 0;
    [SerializeField] protected float m_Radius = 0;
    [SerializeField] protected float m_LaunchSpeed = 0;
    [SerializeField] protected float m_CollectSpeed = 0;
    [SerializeField] protected float m_Harm = 0;
    [SerializeField] protected EAssetName m_WeaponElement = EAssetName.None;
    [SerializeField] protected Dictionary<uint, Entity> m_Results = new();
    [SerializeField] protected List<Entity> m_Targets = new();
    [SerializeField] protected Person m_TargetPerson = null;
    [SerializeField] protected ELayer m_LayerMask = ELayer.Default;
    public ELayer LayerMask => m_LayerMask;
    [SerializeField] protected ELayer m_AttackLayer = ELayer.Default;

    public virtual async UniTask InitializationAsync(Person f_TargetEntity)
    {
        m_TargetPerson = f_TargetEntity;
        m_LayerMask = f_TargetEntity.LayerMask;
        m_AttackLayer = f_TargetEntity.AttackLayerMask;
        SetForward(Vector3.up);
        UpdateCurrentWorldPos();
    }

    // ����
    public abstract UniTask LaunchAsync(Entity f_Target);

    // �ջ�
    public abstract UniTask CollectAsync(Vector3 f_Point);

    public async UniTask SetWeaponElementAsync(EAssetName f_WeaponElement)
    {
        m_WeaponElement = f_WeaponElement;
    }

    public async UniTask<WeaponElementBase> CreateWeaponElementAsync(Vector3? f_StartPoint = null,
        params object[] f_Params)
    {
        var target =
            await GTools.AssetsMgr.LoadPrefabPoolAsync<WeaponElementBase>(m_WeaponElement, null, f_StartPoint, true,
                f_Params);
        await target.OnConstructedAsync(m_LayerMask);
        target.SetWorldPos(Position);
        return target;
    }

    public virtual async UniTask DestroyWeaponElementAsync(WeaponElementBase f_Target)
    {
        await GTools.AssetsMgr.UnLoadPrefabPoolAsync(f_Target);
    }

    public async UniTask CurEntityDestroyAsync()
    {
    }

    public override void OnUpdate()
    {
        UpdateCurrentWorldPos();
    }

    protected async void UpdateCurrentWorldPos()
    {
        if (GTools.RefIsNull(m_TargetPerson)) return;

        SetWorldPos(m_TargetPerson.WeaponPoint.position);
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        m_TargetPerson = null;
        m_LayerMask = ELayer.Default;
        m_AttackLayer = ELayer.Default;
    }
}

#endregion


#region ͻ��Ч��

public abstract class Emitter_GuidedMissileBase : WeaponBase
{
    [SerializeField] // 
    protected Vector2 m_MoveTimeOffset = Vector2.one;

    [SerializeField] protected Vector2 m_OffsetValue = Vector2.zero;
    [SerializeField] protected Vector2 m_FormBezierLength = Vector2.one;
    [SerializeField] protected Vector2 m_ToBezierLength = Vector2.one;
    [SerializeField] protected Vector2Int m_CountRandom = Vector2Int.one;
    [SerializeField] protected ushort m_MaxAttactCount = 1;


    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        var targets = GTools.MathfMgr.GetTargets_Sphere(Position, m_Radius, m_AttackLayer);
        if (targets.Count > 0)
        {
            List<(Vector3 tForm, Vector3 tTo)> pointList = new();
            foreach (var item in targets)
            {
                pointList.Add((Position, item.CentralPoint.position));
            }

            GTools.DrawGizom.AddLine(UpdateLevelID, pointList);
        }
        else
        {
            GTools.DrawGizom.ClearLine(UpdateLevelID);
        }
    }


    public override async UniTask StartExecute()
    {
        var targets = GTools.MathfMgr.GetTargets_Sphere(Position, m_Radius, m_AttackLayer);
        if (targets.Count <= 0)
        {
            return;
        }

        m_Targets = targets;
        int[] fireCount = new int[targets.Count];
        var taskCount = 0;
        var targetsCount = Mathf.Min(m_MaxAttactCount, targets.Count);
        for (int i = 0; i < targetsCount; i++)
        {
            var value = GTools.MathfMgr.GetRandomValue(m_CountRandom.x, m_CountRandom.y) + m_Count;
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
                tasks[taskIndex++] = UniTask.Create(async () => { await LaunchAsync(item); });
            }
        }


        await UniTask.WhenAll(tasks);
    }

    public override async UniTask StopExecute()
    {
    }


    // Ŀ�����
    public override async UniTask LaunchAsync(Entity f_Target)
    {
        var targetPoint = f_Target.CentralPoint.position;
        var curMaxTime = Vector3.Distance(targetPoint, Position) / m_Radius;
        // �ƶ�ʱ��
        var moveTime = (GTools.MathfMgr.GetRandomValue(m_MoveTimeOffset.x, m_MoveTimeOffset.y) + curMaxTime) /
                       m_LaunchSpeed;
        // ��ʼ�� bezier ���Ʊ�����
        var thisLength = Random.Range(m_FormBezierLength.x, m_FormBezierLength.y);
        // Ŀ��� bezier ���Ʊ�����
        var targetLength = Random.Range(m_ToBezierLength.x, m_ToBezierLength.y);
        // ��ǰ���ӵ���ʼ����
        var forward = Forward;
        // ��ǰ�ӵ���ʼλ��
        var startPoint = transform.position;
        var direction = new Vector3(GTools.MathfMgr.GetRandomValue(m_OffsetValue.x, m_OffsetValue.y),
            GTools.MathfMgr.GetRandomValue(m_OffsetValue.x, m_OffsetValue.y),
            GTools.MathfMgr.GetRandomValue(1, 1 + m_OffsetValue.y));
        // ��ǰ bezier ��ʼ����Ʊ�ƫ����
        var dirOffset = transform.TransformDirection(direction.normalized);
        // ��ǰ bezier ��ʼ����Ʊ�����
        var formLength = dirOffset * thisLength;
        // ��ǰ bezier Ŀ�����Ʊ�����
        var toLength = dirOffset * targetLength;

        // ��ȡһ���ӵ�
        var buttle = await CreateWeaponElementAsync(Position);
        buttle.SetWorldPos(startPoint);
        buttle.SetForward(forward);

        await buttle.StartExecute();
        await LaunchStartAsync(buttle, f_Target);
        // �����ӵ��˶�
        await GTools.DoTweenAsync(moveTime, async (value) =>
        {
            var worldPos = GTools.MathfMgr.GetBezierValue(startPoint, targetPoint, formLength, toLength, value);
            buttle.SetForward(worldPos - buttle.CentralPoint.position);
            buttle.SetWorldPos(worldPos);
            await LaunchUpdateAsync(buttle, f_Target, value);
        }, (value) => GetStopCondition(buttle, f_Target, value));
        // �ӵ�����
        await buttle.StopExecute();
        await LaunchStopAsync(buttle, f_Target);
    }


    public override async UniTask CollectAsync(Vector3 f_Point)
    {
    }

    public abstract UniTask LaunchStartAsync(WeaponElementBase f_Element, Entity f_Entity);
    public abstract UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio);
    public abstract UniTask LaunchStopAsync(WeaponElementBase f_Element, Entity f_Entity);
    public abstract UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target);

    public abstract UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, Entity f_Target,
        float f_Ratio);

    public abstract UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target);
    public abstract bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio);
}

#endregion


#region �ɽ����

// ������
public abstract class Emitter_SwordBase : WeaponBase
{
    protected enum EStatus
    {
        Enable,
        Close,
    }

    [SerializeField, Range(0, 1)] protected float m_ExcuteTimeOffset = 0;
    [SerializeField] protected bool m_IsAotuMove = true;
    [SerializeField] protected EStatus m_LaunchStatus = EStatus.Enable;
    [SerializeField] protected EStatus m_CollectStatus = EStatus.Enable;
    [SerializeField] protected float MaxExcuteTime => m_UnitTime / m_CollectSpeed;
    [SerializeField] protected Dictionary<ESwordStatus, ListStack<WeaponElementBase>> m_AllSwordElements = new();

    protected Dictionary<ushort, Vector3> m_SwordElementPoint = new();

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        // ��ʼ������
        for (ushort i = 0; i < (ushort)ESwordStatus.EnumCount; i++)
        {
            m_AllSwordElements.Add((ESwordStatus)i, new($"Emitter_SwordBase   {(ESwordStatus)i}", m_Count + 1));
        }
    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        //m_Target = f_Params[0] as Person;
        // ��ʼ��Ŀ���
        await GTools.ParallelTaskAsync(m_Count, async (index) =>
        {
            m_SwordElementPoint.Add(index, new Vector3
            (
                GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f),
                GTools.MathfMgr.GetRandomValue(0, 0.5f),
                GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f)
            ));
        });
    }

    public override async UniTask InitializationAsync(Person f_TargetEntity)
    {
        await base.InitializationAsync(f_TargetEntity);
        await InitElementCount(m_Count);
    }

    public async UniTask<ushort> GetCurrentElementCountAsync()
    {
        ushort count = 0;
        foreach (var item in m_AllSwordElements)
        {
            foreach (var element in item.Value.GetEnumerator())
            {
                count++;
            }
        }

        return count;
    }

    public async UniTask InitElementCount(ushort f_Count)
    {
        var curCount = await GetCurrentElementCountAsync();
        var count = (ushort)Mathf.Min(f_Count, m_Count - curCount);
        // ��ʼ��Ԫ��
        await GTools.ParallelTaskAsync(count, async (index) =>
        {
            var element = await CreateWeaponElementAsync(Position);
            element.SetTargetPerson(this);
            SetWeaponStatus(element, ESwordStatus.Prepare);
        });
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        await GTools.ParallelTaskAsync(m_AllSwordElements, async (key, value) =>
        {
            while (value.TryPop(out var element))
            {
                await GTools.AssetsMgr.UnLoadPrefabPoolAsync(element);
            }
        });
        m_AllSwordElements.Clear();
        m_SwordElementPoint.Clear();
    }

    public override async void OnUpdate()
    {
        base.OnUpdate();

        // ����׼��״̬�Ľ�δ׷��״̬
        if (m_IsAotuMove)
        {
            foreach (var item in m_AllSwordElements[ESwordStatus.Prepare].GetEnumerator())
            {
                item.Value.SetWorldPos(Vector3.Lerp(item.Value.Position,
                    m_SwordElementPoint[(ushort)item.Key] + Position, GTools.UpdateDeltaTime * 10));
                item.Value.SetForward(Vector3.Lerp(item.Value.Forward, Forward, GTools.UpdateDeltaTime * 10));
            }
        }


        // ���Դ���
        var targets = GTools.MathfMgr.NearerTarget(Position, m_Radius, m_AttackLayer);
        if (GTools.RefIsNull(targets))
        {
            GTools.DrawGizom.ClearLine(UpdateLevelID);
        }
        else
        {
            GTools.DrawGizom.AddLine(UpdateLevelID, new() { (Position, targets.CentralPoint.position) });
        }
    }

    public override async UniTask StartExecute()
    {
        var target = GTools.MathfMgr.NearerTarget(Position, m_Radius, m_AttackLayer);
        if (GTools.RefIsNull(target))
        {
            return;
        }

        m_Targets = new() { target };
        await LaunchAsync(target);
    }

    public override async UniTask StopExecute()
    {
        await CollectAsync(Position);
    }

    public virtual async UniTask<ResultData<WeaponElementBase>> GetWeaponElementAsync()
    {
        ResultData<WeaponElementBase> resultData = new();
        if (m_AllSwordElements[ESwordStatus.Prepare].TryPop(out var target))
        {
            resultData.SetData(target);
            SetWeaponStatus(target, ESwordStatus.Take);
        }

        return resultData;
    }

    public void SetWeaponStatus(WeaponElementBase f_Target, ESwordStatus f_ToStatus)
    {
        RemoveWeaponElement(f_Target);
        f_Target.Setstatus(f_ToStatus);
        m_AllSwordElements[f_ToStatus].Push(f_Target);
    }

    public override async UniTask LaunchAsync(Entity f_Target)
    {
        if (m_LaunchStatus == EStatus.Close) return;
        var targetPoint = (f_Target.CentralPoint.position - Position).normalized * m_Radius + Position;
        // ��ȡһ��Ԫ��
        var resultData = await GetWeaponElementAsync();
        if (resultData.Result == EResult.Succeed)
        {
            var buttle = resultData.Value;
            SetWeaponStatus(buttle, ESwordStatus.Launch);

            var startPoint = Position;
            var timeOffset = GTools.MathfMgr.GetRandomValue(-m_ExcuteTimeOffset, m_ExcuteTimeOffset) + 1;

            await buttle.StartExecute();
            await LaunchStartAsync(buttle, f_Target);
            await GTools.DoTweenAsync(MaxExcuteTime * timeOffset, async (value) =>
            {
                await LaunchUpdateAsync(buttle, f_Target, value);
                var posValue = await GetWorldPosition(startPoint, targetPoint, buttle.Position, value);
                buttle.SetForward(posValue - buttle.Position);
                buttle.SetWorldPos(posValue);
            }, (value) =>
            {
                // û�нӴ���ʵ�����ִ��
                var isExcute = GetStopCondition(buttle, f_Target, value);
                return isExcute;
            });
            SetWeaponStatus(buttle, buttle.GetIsTarget() ? ESwordStatus.Score : ESwordStatus.Dropping);

            await buttle.StopExecute();
            await LaunchStopAsync(buttle, f_Target);
            await m_TargetPerson.ExecuteGainAsync(EGainType.Launch);
        }
    }

    public override async UniTask CollectAsync(Vector3 f_Point)
    {
        if (m_CollectStatus == EStatus.Close) return;
        await CollectAwakeAsync(f_Point);

        var list = ListStack<WeaponElementBase>.ListStackAdd(m_AllSwordElements[ESwordStatus.Dropping],
            m_AllSwordElements[ESwordStatus.Score]);

        if (list.Count > 0)
        {
            await GTools.ParallelTaskAsync(list, async (buttle) =>
            {
                SetWeaponStatus(buttle, ESwordStatus.Collect);
                var startPoint = buttle.Position;
                await buttle.StartExecute();
                await CollectStartAsync(buttle, f_Point);
                await GTools.DoTweenAsync(MaxExcuteTime / m_CollectSpeed, async (ratio) =>
                {
                    await CollectUpdateAsync(buttle, f_Point, ratio);
                    var posValue = await GetWorldPosition(startPoint, f_Point, buttle.Position, ratio);
                    buttle.SetForward(posValue - buttle.Position);
                    buttle.SetWorldPos(posValue);
                }, (ratio) => true);
                await CollectStopAsync(buttle, f_Point);
                SetWeaponStatus(buttle, ESwordStatus.Prepare);
                await buttle.StopExecute();
                buttle.ClearTargets();
            });
            await m_TargetPerson.ExecuteGainAsync(EGainType.Collect);
        }

        await CollectSleepAsync(f_Point);
    }

    public void RemoveWeaponElement(WeaponElementBase f_Target)
    {
        m_AllSwordElements[f_Target.CurStatus].Remove(f_Target);
    }

    public override async UniTask DestroyWeaponElementAsync(WeaponElementBase f_Target)
    {
        await base.DestroyWeaponElementAsync(f_Target);
        RemoveWeaponElement(f_Target);
    }

    public abstract UniTask LaunchStartAsync(WeaponElementBase f_Element, Entity f_Entity);
    public abstract UniTask LaunchUpdateAsync(WeaponElementBase f_Element, Entity f_Entity, float f_Ratio);
    public abstract UniTask LaunchStopAsync(WeaponElementBase f_Element, Entity f_Entity);
    public abstract UniTask CollectAwakeAsync(Vector3 f_Target);
    public abstract UniTask CollectStartAsync(WeaponElementBase f_Element, Vector3 f_Target);
    public abstract UniTask CollectUpdateAsync(WeaponElementBase f_Element, Vector3 f_TargetPoint, float f_Ratio);
    public abstract UniTask CollectStopAsync(WeaponElementBase f_Element, Vector3 f_Target);
    public abstract UniTask CollectSleepAsync(Vector3 f_Target);

    public abstract UniTask<Vector3> GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint,
        float f_Ratio);

    public abstract bool GetStopCondition(WeaponElementBase f_Buttle, Entity f_Target, float f_Ratio);
}

#endregion