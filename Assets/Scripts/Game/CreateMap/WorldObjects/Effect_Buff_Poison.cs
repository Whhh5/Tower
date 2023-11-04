using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Buff_PoisonData : Effect_BuffBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Effect_Buff_Poison;

    public override void Initialization(WorldObjectBaseData f_Original, WorldObjectBaseData f_Target)
    {
        base.Initialization(f_Original, f_Target);
        m_Id = 2;
        m_Name = "Poison";
    }

    public override void ExecuteResultAsync(int f_Value, float f_Ratio)
    {
        GTools.MathfMgr.EntityDamage(m_Initiator, m_Target, EDamageType.Magic, -f_Value);
    }
    public override Vector3 GetTargetPoint()
    {
        return m_Target.CentralPoint;
    }
}
public class Effect_Buff_Poison : Effect_BuffBase
{

}
