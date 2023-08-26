using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestTimeLineData : EntityEffectBaseData
{
    public TestTimeLineData(WorldObjectBaseData f_Initiator, Vector3 f_StartPosition, WorldObjectBaseData f_TargetEnemy, int f_DamageValue, bool f_IsChangeMagic, DirectorWrapMode f_WrapMode = DirectorWrapMode.None) : base(f_StartPosition, f_Initiator, f_WrapMode)
    {

        DamageValue = f_DamageValue;
        TargetEnemy = f_TargetEnemy;
        m_IsChangeMagic = f_IsChangeMagic;
    }
    public override AssetKey AssetPrefabID => AssetKey.TestTimeLine;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public int DamageValue = 1;
    public WorldObjectBaseData TargetEnemy = null;
    public bool m_IsChangeMagic;

    public override void OnUnLoad()
    {
        GTools.MathfMgr.EntityDamage(Initiator, TargetEnemy, EDamageType.True, DamageValue, m_IsChangeMagic);

        base.OnUnLoad();
    }
}
public class TestTimeLine : EntityEffectBase
{

}
