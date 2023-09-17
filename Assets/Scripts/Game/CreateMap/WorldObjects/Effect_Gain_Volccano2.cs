using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Gain_Volccano2Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.Volccano2;

    public override EBuffView GainView => EBuffView.Interval;

    public override AssetKey AssetPrefabID => AssetKey.Effect_Gain_Volccano2;

    protected override float DurationTime => 0;

    protected override float IntervalTime => 0.5f;

    public override void ExecuteContext(int f_CurProbability)
    {
        var damageValue = Mathf.CeilToInt(m_Initiator.CurHarm / 2);
        GTools.MathfMgr.EntityDamage(m_Initiator, m_Recipient, EDamageType.Magic, damageValue);
    }

    public override Vector3 GetPosiion()
    {
        return m_Recipient.WorldPosition;
    }
}
public class Effect_Gain_Volccano2 : EntityGainBase
{

}
