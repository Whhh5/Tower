using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity_Gain_Collect1Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.Collect1;
    public override EBuffView GainView => EBuffView.Collect;

    protected override float DurationTime => 30;

    public override AssetKey AssetPrefabID => AssetKey.Entity_Gain_Collect1;

    public override void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        base.Initialization(f_Initiator, f_Recipient);
        m_Probability = 100;
    }

    public override void ExecuteContext(int f_CurProbability)
    {

    }

    public override Vector3 GetPosiion()
    {
        return m_Recipient.WorldPosition;
    }
}
public class Entity_Gain_Collect1 : EntityGainBase
{

}
