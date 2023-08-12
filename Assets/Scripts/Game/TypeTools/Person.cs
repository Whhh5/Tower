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


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ÎäÆ÷Æª
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

}


public abstract class Person : Entity
{
    public Animator Animator => GetComponent<Animator>();



}