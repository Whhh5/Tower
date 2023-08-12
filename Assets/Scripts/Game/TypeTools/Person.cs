using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PersonData : EntityData
{
    public abstract ELayer LayerMask { get; }
    public abstract ELayer AttackLayerMask { get; }
    public virtual int HarmBase => 12;
    public WeaponBaseData CurWeaponData { get; protected set; }

    protected PersonData(int f_index) : base(f_index)
    {
    }
    protected void PlayAnimation(bool f_IsForce = false)
    {

    }
    public void PlayerAnimation(EPersonStatusType f_StatusType)
    {
        var animaName = $"{AssetPrefabID}_{f_StatusType}";
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override async void AfterLoad()
    {
        base.AfterLoad();
        await GTools.ParallelTaskAsync((ushort)EGainType.EnumCount, async (index) =>
        {
            m_CurGainList.Add((EGainType)index, new());
        });
    }
    public override void OnUnLoad()
    {
        base.OnUnLoad();

        m_CurGainList.Clear();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public void StartExecute()
    {

    }
    public void StopExecute()
    {

    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 武器篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public WeaponBaseData CurWeapon = null;
    public void SetWeapon(WeaponBaseData f_Weapon)
    {
        if (!GTools.RefIsNull(CurWeapon))
        {
            ILoadPrefabAsync.UnLoad(CurWeapon);
        }
        CurWeapon = f_Weapon;
    }
    public void OnClickDownKeyCodeD_Launch()
    {
        if (GTools.RefIsNull(CurWeapon)) return;
        CurWeapon.StartExecute();

    }
    public void OnClickDownKeyCodeF_Collect()
    {
        if (GTools.RefIsNull(CurWeapon)) return;
        CurWeapon.StopExecute();
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 增益篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // Gain
    protected Dictionary<EGainType, Dictionary<EGainView, GainBaseData>> m_CurGainList = new();
    // 触发增益
    public async void ExecuteGainAsync(EGainType f_GainType)
    {
        await GTools.ParallelTaskAsync(m_CurGainList[f_GainType], async (key, value) =>
        {
            await value.StartExecute();
        });
    }

    public void AddGainAsync(EGainView f_GainView)
    {
        var gainType = GTools.GainMgr.GetGainType(f_GainView);
        if (!m_CurGainList[gainType].ContainsKey(f_GainView))
        {
            var ins = GTools.GainMgr.GetGain(f_GainView, this as WorldObjectBaseData);
            m_CurGainList[gainType].Add(f_GainView, ins);
        }
    }
    public void RemoveGainAsync(EGainView f_GainView)
    {
        var gainType = GTools.GainMgr.GetGainType(f_GainView);
        if (m_CurGainList[gainType].ContainsKey(f_GainView))
        {
            m_CurGainList[gainType].Remove(f_GainView);
        }
    }
    public void SetGainLevelAsync(ushort f_ID, ushort f_Level)
    {

    }
    public void AddGainTierAsync(ushort f_ID)
    {

    }
    public void RediusGainTierAsync(ushort f_ID)
    {

    }
}


public abstract class Person : Entity
{
    public Animator Animator => GetComponent<Animator>();



}