using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Effect_Tower_Light1_AttackData : EntityEffectBaseData
{
    public void Initialization(Vector3 f_StartPos, WorldObjectBaseData f_Initiator, int f_Damage, bool f_IsAddMagic = false)
    {
        base.Initialization(f_StartPos, f_Initiator, DirectorWrapMode.Loop);
        m_IsAddMagic = f_IsAddMagic;
        m_DamageValue = f_Damage;

    }

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Effect_Tower_Light1_Attack;

    private bool m_IsAddMagic;
    private int m_DamageValue = 0;
    public override void UnLoad()
    {
        base.UnLoad();
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
