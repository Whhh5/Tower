using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity_Gain_AddAttackSpeed1Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.AttackSpeed1;

    public override EBuffView GainView => EBuffView.perpetual;

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Gain_AddAttackSpeed1;

    private float m_AddAttackSpeed = 0.2f;

    protected override float DurationTime => 5.0f;

    public override void StartExecute()
    {
        m_Recipient.ChangeReleaseSpeed(m_AddAttackSpeed);


        base.StartExecute();
    }
    public override void StopExecute()
    {
        m_Recipient.ChangeReleaseSpeed(-m_AddAttackSpeed);


        base.StopExecute();
    }
    public override void ExecuteContext(int f_CurProbability)
    {
        
    }

    public override Vector3 GetPosiion()
    {
        return m_Recipient.AttackSpeedPoint;
    }
}
public class Entity_Gain_AddAttackSpeed1 : EntityGainBase
{
    
}
