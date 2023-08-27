using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Emitter_SwordBaseData: WeaponBaseData
{
    public enum EStatus
    {
        Enable,
        Close,
    }
    protected Emitter_SwordBaseData(int f_Index, WorldObjectBaseData f_TargetEntity) : base(f_Index, f_TargetEntity)
    {
    }


    [SerializeField, Range(0, 1)] protected float m_ExcuteTimeOffset = 0;
    [SerializeField] protected bool m_IsAotuMove = true;
    [SerializeField] protected EStatus m_LaunchStatus = EStatus.Enable;
    [SerializeField] protected EStatus m_CollectStatus = EStatus.Enable;
    [SerializeField] protected float MaxExcuteTime => UnitTime / CollectSpeed;
    [SerializeField] protected Dictionary<ESwordStatus, ListStack<WeaponElementBaseData>> m_AllSwordElements = new();

    protected Dictionary<ushort, Vector3> m_SwordElementPoint = new();

    public override void AfterLoad()
    {
        base.AfterLoad();
        for (ushort i = 0; i < (ushort)ESwordStatus.EnumCount; i++)
        {
            m_AllSwordElements.Add((ESwordStatus)i, new($"Emitter_SwordBase   {(ESwordStatus)i}", Count + 1));
        }

        for (ushort i = 0; i < Count; i++)
        {
            m_SwordElementPoint.Add(i, new Vector3
                (
                    GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f),
                    GTools.MathfMgr.GetRandomValue(0, 0.5f),
                    GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f)
                ));
        }

        InitElementCount((ushort)Count);
    }

    public int GetCurrentElementCountAsync()
    {
        int count = 0;
        foreach (var item in m_AllSwordElements)
        {
            foreach (var element in item.Value.GetEnumerator())
            {
                count++;
            }
        }

        return count;
    }

    public void InitElementCount(ushort f_Count)
    {
        var curCount = GetCurrentElementCountAsync();
        var count = (ushort)Mathf.Min(f_Count, Count - curCount);

        for (ushort i = 0; i < count; i++)
        {
            var element = CreateWeaponElementAsync(WorldPosition);
            element.SetTargetPerson(this);
            SetWeaponStatus(element, ESwordStatus.Prepare);
        }
    }

    public override async void OnUnLoad()
    {
        base.OnUnLoad();
        await GTools.ParallelTaskAsync(m_AllSwordElements, async (key, value) =>
        {
            while (value.TryPop(out var element))
            {
                ILoadPrefabAsync.UnLoad(element);
            }
        });
        m_AllSwordElements.Clear();
        m_SwordElementPoint.Clear();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_IsAotuMove)
        {
            foreach (var item in m_AllSwordElements[ESwordStatus.Prepare].GetEnumerator())
            {
                item.Value.SetPosition(Vector3.Lerp(item.Value.WorldPosition,
                    m_SwordElementPoint[(ushort)item.Key] + WorldPosition, GTools.UpdateDeltaTime * 10));
                item.Value.SetForward(Vector3.Lerp(item.Value.Forward, Forward, GTools.UpdateDeltaTime * 10));
            }
        }


        var targets = GTools.MathfMgr.NearerTarget(WorldPosition, Radius, AttackLayer);
        if (GTools.RefIsNull(targets))
        {
            GTools.DrawGizom.ClearLine(UpdateLevelID);
        }
        else
        {
            GTools.DrawGizom.AddLine(UpdateLevelID, new() { (WorldPosition, targets.CentralPoint) });
        }

        SetPosition(Initiator.WeaponPoint);
    }

    public override async UniTask StartExecute()
    {
        var target = GTools.MathfMgr.NearerTarget(WorldPosition, Radius, AttackLayer);
        if (GTools.RefIsNull(target))
        {
            return;
        }

        Targets = new() { target };
        await LaunchAsync(target);
    }

    public override async UniTask StopExecute()
    {
        await CollectAsync(WorldPosition);
    }

    public virtual ResultData<WeaponElementBaseData> GetWeaponElementAsync()
    {
        ResultData<WeaponElementBaseData> resultData = new();
        if (m_AllSwordElements[ESwordStatus.Prepare].TryPop(out var target))
        {
            resultData.SetData(target);
            SetWeaponStatus(target, ESwordStatus.Take);
        }

        return resultData;
    }

    public void SetWeaponStatus(WeaponElementBaseData f_Target, ESwordStatus f_ToStatus)
    {
        RemoveWeaponElement(f_Target);
        f_Target.Setstatus(f_ToStatus);
        m_AllSwordElements[f_ToStatus].Push(f_Target);
    }

    public override async UniTask LaunchAsync(WorldObjectBaseData f_Target)
    {
        if (m_LaunchStatus == EStatus.Close) return;

        var radius = Radius + GTools.MathfMgr.GetRandomValue(-1.0f, 1.0f);

        var targetPoint = (f_Target.CentralPoint - WorldPosition).normalized * radius + WorldPosition;
        var rangeX = GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f);
        var rangeZ = GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f);
        targetPoint += new Vector3(rangeX, 0, rangeZ);


        var resultData = GetWeaponElementAsync();
        if (resultData.Result == EResult.Succeed)
        {
            var buttle = resultData.Value;
            SetWeaponStatus(buttle, ESwordStatus.Launch);

            var startPoint = WorldPosition;
            var timeOffset = GTools.MathfMgr.GetRandomValue(-m_ExcuteTimeOffset, m_ExcuteTimeOffset) + 1;

            buttle.StartExecute();
            LaunchStartAsync(buttle, f_Target);
            await GTools.DoTweenAsync(MaxExcuteTime * timeOffset, async (value) =>
            {
                LaunchUpdateAsync(buttle, f_Target, value);
                var posValue = GetWorldPosition(startPoint, targetPoint, buttle.WorldPosition, value);
                buttle.SetForward(posValue - buttle.WorldPosition);
                buttle.SetPosition(posValue);
            }, (value) =>
            {
                var isExcute = GetStopCondition(buttle, f_Target, value);
                return isExcute;
            });
            SetWeaponStatus(buttle, buttle.GetIsTarget() ? ESwordStatus.Score : ESwordStatus.Dropping);

            buttle.StopExecute();
            LaunchStopAsync(buttle, f_Target);
            Initiator.ExecuteGainAsync(EGainView.Launch);
        }
    }

    public override async UniTask CollectAsync(Vector3 f_Point)
    {
        if (m_CollectStatus == EStatus.Close) return;
        CollectAwakeAsync(f_Point);

        var list = ListStack<WeaponElementBaseData>.ListStackAdd(m_AllSwordElements[ESwordStatus.Dropping],
            m_AllSwordElements[ESwordStatus.Score]);

        if (list.Count > 0)
        {
            foreach (var buttle in list)
            {
                SetWeaponStatus(buttle, ESwordStatus.Collect);

                var startPoint = buttle.WorldPosition;
                buttle.StartExecute();
                CollectStartAsync(buttle, f_Point);
                await GTools.DoTweenAsync(MaxExcuteTime / CollectSpeed, async (ratio) =>
                {
                    CollectUpdateAsync(buttle, f_Point, ratio);
                    var posValue = GetWorldPosition(startPoint, f_Point, buttle.WorldPosition, ratio);
                    buttle.SetForward(posValue - buttle.WorldPosition);
                    buttle.SetPosition(posValue);
                }, (ratio) => true);
                CollectStopAsync(buttle, f_Point);
                SetWeaponStatus(buttle, ESwordStatus.Prepare);
                buttle.StopExecute();
                buttle.ClearTargets();
            }

            Initiator.ExecuteGainAsync(EGainView.Collect);
        }

        CollectSleepAsync(f_Point);
    }

    public void RemoveWeaponElement(WeaponElementBaseData f_Target)
    {
        m_AllSwordElements[f_Target.CurWeaponStatus].Remove(f_Target);
    }

    public override void DestroyWeaponElementAsync(WeaponElementBaseData f_Target)
    {
        base.DestroyWeaponElementAsync(f_Target);
        RemoveWeaponElement(f_Target);
    }

    public abstract void LaunchStartAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity);
    public abstract void LaunchUpdateAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity, float f_Ratio);
    public abstract void LaunchStopAsync(WeaponElementBaseData f_Element, WorldObjectBaseData f_Entity);
    public abstract void CollectAwakeAsync(Vector3 f_Target);
    public abstract void CollectStartAsync(WeaponElementBaseData f_Element, Vector3 f_Target);
    public abstract void CollectUpdateAsync(WeaponElementBaseData f_Element, Vector3 f_TargetPoint, float f_Ratio);
    public abstract void CollectStopAsync(WeaponElementBaseData f_Element, Vector3 f_Target);
    public abstract void CollectSleepAsync(Vector3 f_Target);

    public abstract Vector3 GetWorldPosition(Vector3 f_StartPoint, Vector3 f_EndPoint, Vector3 f_CurPoint,
        float f_Ratio);

    public abstract bool GetStopCondition(WeaponElementBaseData f_Buttle, WorldObjectBaseData f_Target, float f_Ratio);

}
public abstract class Emitter_SwordBase : WeaponBase
{

}

