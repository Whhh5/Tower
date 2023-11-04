using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Buff_AddBloodData : Effect_BuffBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Effect_Buff_AddBlood;

    public override void Initialization(WorldObjectBaseData f_Original, WorldObjectBaseData f_Target)
    {
        base.Initialization(f_Original, f_Target);
        m_Id = 1;
        m_Name = "Add Blood";
    }

    public override void ExecuteResultAsync(int f_Value, float f_Ratio)
    {
        GTools.MathfMgr.EntityDamage(m_Initiator, m_Target, EDamageType.AddBlood, -f_Value);
    }

    public override Vector3 GetTargetPoint()
    {
        return m_Target.WorldPosition;
    }
}
public class Effect_Buff_AddBlood : Effect_BuffBase
{

}