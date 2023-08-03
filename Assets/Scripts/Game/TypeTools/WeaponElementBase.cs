using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class WeaponElementBaseData : EntityData
{
    protected WeaponElementBaseData(int f_index, WorldObjectBaseData f_Initiator) : base(f_index)
    {
        Initiator = f_Initiator;
    }
    public WeaponElementBase ElementTarget => GetCom<WeaponElementBase>();


    public WorldObjectBaseData Initiator = null;
    public WeaponBaseData TargetEmttier = null;
    public int Harm = 23;
    public ESwordStatus CurWeaponStatus = ESwordStatus.None;
    public float Radius = 1;
    public EDamageType DamegeType = EDamageType.None;
    public EntityData Target = null;
    public Vector3 ToTargetDistance = Vector3.zero;
    public Dictionary<int, EntityData> Targets = new();
    public Vector3 Size = Vector3.one;

    public Vector3 BosSize => ElementTarget != null ? BosSize : Vector3.one;

    /// <summary>
    /// 是否被阻挡
    /// </summary>
    public bool IsBeResist = false;

    public ELayer m_LayerMask = ELayer.Default;
    public ELayer LayerMask => m_LayerMask;

    public void OnConstructed(ELayer f_LayerMask)
    {
        m_LayerMask = f_LayerMask;
    }


    public override void AfterLoad()
    {
        base.AfterLoad();
        m_LayerMask = ELayer.Default;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (CurWeaponStatus == ESwordStatus.Score && GetIsTarget())
        {
            //await SetForward((m_Target.Forward + m_ToTargetDirection).normalized);
            SetPosition(Target.WorldPosition + ToTargetDistance);
        }
    }

    public virtual void StartExecute()
    {
        Target = null;
        IsBeResist = false;
    }

    public virtual void StopExecute()
    {
    }

    public List<EntityData> GetTargets(ELayer m_AttackLayer)
    {
        var targets = GTools.MathfMgr.GetTargets_Sphere(CentralPoint, Radius, m_AttackLayer);
        return targets;
    }

    public EntityData GetNearTarget(ELayer m_AttackLayer)
    {
        var target = GTools.MathfMgr.NearerTarget(CentralPoint, Radius, m_AttackLayer);
        return target;
    }

    public void SetTargetAsync(EntityData f_Target)
    {
        if (f_Target == null) return;
        Target = f_Target;
        ToTargetDistance = WorldPosition - f_Target.WorldPosition;
    }

    public bool GetIsTarget()
    {
        return !GTools.RefIsNull(Target) && Target.CurStatus != EPersonStatusType.Die;
    }

    public void AttackTargetAsync(EntityData f_Target)
    {
        if (!Targets.ContainsKey(f_Target.UpdateLevelID))
        {
            GTools.MathfMgr.EntityDamage(Initiator, f_Target, DamegeType, -Harm);

            Targets.Add(f_Target.UpdateLevelID, f_Target);
        }
    }

    public void Setstatus(ESwordStatus f_status)
    {
        CurWeaponStatus = f_status;
    }

    public void ClearTargets()
    {
        Targets.Clear();
    }

    public void SetTargetPerson(WeaponBaseData f_TargetEmttier)
    {
        TargetEmttier = f_TargetEmttier;
    }

    public async UniTask CurEntityDestroyAsync()
    {
    }

    public bool GetResistStatus()
    {
        return IsBeResist;
    }

    /// <summary>
    /// ��ǰ�������赲
    /// </summary>
    /// <returns></returns>
    public async UniTask BeResistAsync()
    {
        IsBeResist = true;
    }
}
public abstract class WeaponElementBase : Entity
{
    public Vector3 BosSize => GetComponent<BoxCollider>().size;
    public WeaponElementBaseData ElementData => GetData<WeaponElementBaseData>();
}

