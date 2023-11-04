using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Buff_WeatherBuffAttackData : Effect_BuffBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Effect_Buff_WeatherBuffAttack;
    protected override float IntervalTime => base.IntervalTime + 100000;
    protected override float UnitTime => base.UnitTime + 1000000;

    private float m_AddHarm = 3.0f;
    public override void StartExecute()
    {
        m_Target.ChangeHarm(m_AddHarm);
        base.StartExecute();
    }
    public override void StopExecute()
    {
        m_Target.ChangeHarm(-m_AddHarm);
        base.StopExecute();
    }
    public override void ExecuteResultAsync(int f_Value, float f_Ratio)
    {

    }

    public override Vector3 GetTargetPoint()
    {
        return m_Target.WorldPosition;
    }
}
public class Effect_Buff_WeatherBuffAttack : Effect_BuffBase
{

}
