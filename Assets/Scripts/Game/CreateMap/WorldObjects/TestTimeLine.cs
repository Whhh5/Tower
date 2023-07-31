using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestTimeLineData : EntityEffectBaseData
{
    public TestTimeLineData(int f_Index, WorldObjectBaseData f_Initiator, DirectorWrapMode f_WrapMode = DirectorWrapMode.None) : base(f_Index, f_WrapMode)
    {
        Original = f_Initiator;
    }
    public override AssetKey AssetPrefabID => AssetKey.TestTimeLine;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public WorldObjectBaseData Original = null;

    public void EntityDamage(EntityData f_Target, int f_Value)
    {
        GTools.MathfMgr.EntityDamage(Original, f_Target, f_Value);
    }
}
public class TestTimeLine : EntityEffectBase
{

}
