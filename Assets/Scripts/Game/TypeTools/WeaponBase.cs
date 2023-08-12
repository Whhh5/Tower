using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class WeaponBaseData : VirtualEntityData
{
    protected WeaponBaseData(int f_Index, WorldObjectBaseData f_TargetEntity) : base(f_Index)
    {
        Initiator = f_TargetEntity;
        LayerMask = f_TargetEntity.LayerMask;
        AttackLayer = f_TargetEntity.AttackLayerMask;
        SetForward(Vector3.up);
    }

    public PersonData Initiator = null;
    public float UnitTime = 1;
    public int Count = 10;
    public float Radius = 30;
    public float LaunchSpeed = 1;
    public float CollectSpeed = 3;
    public float Harm = 6;
    public EAssetName WeaponElement = EAssetName.None;
    public Dictionary<uint, WorldObjectBaseData> Results = new();
    public List<WorldObjectBaseData> Targets = new();
    public ELayer LayerMask = ELayer.Default;
    public ELayer AttackLayer = ELayer.Default;
    public abstract UniTask StartExecute();
    public abstract UniTask StopExecute();

    public abstract WeaponElementBaseData GetWeaponElementData();

    public abstract UniTask LaunchAsync(WorldObjectBaseData f_Target);

    public abstract UniTask CollectAsync(Vector3 f_Point);

    public void SetWeaponElementAsync(EAssetName f_WeaponElement)
    {
        WeaponElement = f_WeaponElement;
    }

    public WeaponElementBaseData CreateWeaponElementAsync(Vector3? f_StartPoint = null, params object[] f_Params)
    {
        var target = GetWeaponElementData();
        target.OnConstructed(LayerMask);
        target.SetPosition(WorldPosition);
        ILoadPrefabAsync.LoadAsync(target);
        return target;
    }
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        SetPosition(Initiator.WeaponPoint);
    }
    public virtual void DestroyWeaponElementAsync(WeaponElementBaseData f_Target)
    {
        ILoadPrefabAsync.UnLoad(f_Target);
    }
}

public abstract class WeaponBase : VirtualEntity
{
}
