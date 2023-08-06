using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestTimeLineData : EntityEffectBaseData
{
    public TestTimeLineData(int f_Index, WorldObjectBaseData f_Initiator, Vector3 f_StartPosition, WorldObjectBaseData f_TargetEnemy, int f_DamageValue, DirectorWrapMode f_WrapMode = DirectorWrapMode.None) : base(f_Index, f_StartPosition, f_WrapMode)
    {
        Original = f_Initiator;

        DamageValue = f_DamageValue;
        TargetEnemy = f_TargetEnemy;
    }
    public override AssetKey AssetPrefabID => AssetKey.TestTimeLine;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public WorldObjectBaseData Original = null;

    public int DamageValue = 1;
    public WorldObjectBaseData TargetEnemy = null;

    public override void OnUnLoad()
    {
        GTools.MathfMgr.EntityDamage(Original, TargetEnemy, EDamageType.True, DamageValue);

        base.OnUnLoad();
    }
}
public class TestTimeLine : EntityEffectBase
{

}
