using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Gain_Volccano3Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.Volccano3;

    public override EGainView GainView => EGainView.Interval;

    public override AssetKey AssetPrefabID => AssetKey.Effect_Gain_Volccano3;

    protected override float DurationTime => 0;

    protected override float IntervalTime => 0.2f;

    public override void ExecuteContext(int f_CurProbability)
    {
        var damageValue = Mathf.CeilToInt(m_Initiator.HarmBase / 1);
        GTools.MathfMgr.EntityDamage(m_Initiator, m_Recipient, EDamageType.Magic, damageValue);
    }

    public override Vector3 GetPosiion()
    {
        return m_Recipient.WorldPosition;
    }
}
public class Effect_Gain_Volccano3 : EntityGainBase
{

}
