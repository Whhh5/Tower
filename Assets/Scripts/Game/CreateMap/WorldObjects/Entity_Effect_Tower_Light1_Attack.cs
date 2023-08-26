using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Effect_Tower_Light1_AttackData : EntityEffectBaseData
{
    public Entity_Effect_Tower_Light1_AttackData(Vector3 f_StartPos, WorldObjectBaseData f_Initiator, int f_Damage, bool f_IsAddMagic = false) : base(f_StartPos, f_Initiator)
    {
        m_IsAddMagic = f_IsAddMagic;
        m_DamageValue = f_Damage;
    }

    public override AssetKey AssetPrefabID => AssetKey.Entity_Effect_Tower_Light1_Attack;

    private bool m_IsAddMagic;
    private int m_DamageValue = 0;
    public override void OnUnLoad()
    {
        base.OnUnLoad();
    }
    public override void ExecuteEffect(WorldObjectBaseData m_TargetEnemy)
    {
        base.ExecuteEffect(m_TargetEnemy);

        GTools.MathfMgr.EntityDamage(Initiator, m_TargetEnemy, EDamageType.Physical, m_DamageValue, m_IsAddMagic);
    }
}
public class Entity_Effect_Tower_Light1_Attack : EntityEffectBase
{

}
