using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Buff_WeatherBuffDefenceData : Effect_BuffBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Effect_Buff_WeatherBuffDefence;
    protected override float IntervalTime => base.IntervalTime + 100000;
    protected override float UnitTime => base.UnitTime + 1000000;

    private int m_AddDefence = 3;
    public override void StartExecute()
    {
        m_Target.ChangeDefence(m_AddDefence);
        base.StartExecute();
    }
    public override void StopExecute()
    {
        m_Target.ChangeDefence(-m_AddDefence);
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
public class Effect_Buff_WeatherBuffDefence : Effect_BuffBase
{

}
