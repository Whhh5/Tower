using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;


public class WeaponMgr : Singleton<WeaponMgr>
{




    public void DestroyWeaponAsync(WeaponBaseData f_Weapon)
    {
        
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