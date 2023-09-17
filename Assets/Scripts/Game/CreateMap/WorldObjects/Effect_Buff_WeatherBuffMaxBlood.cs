using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Buff_WeatherBuffMaxBloodData : Effect_BuffBaseData
{
    public override AssetKey AssetPrefabID => AssetKey.Effect_Buff_WeatherBuffMaxBlood;
    protected override float IntervalTime => base.IntervalTime + 100000;
    protected override float UnitTime => base.UnitTime + 1000000;

    private float m_AddMaxBlood = 3.0f;
    public override void StartExecute()
    {
        m_Target.ChangeMaxBlood(m_AddMaxBlood);
        base.StartExecute();
    }
    public override void StopExecute()
    {
        m_Target.ChangeMaxBlood(-m_AddMaxBlood);
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
public class Effect_Buff_WeatherBuffMaxBlood : Effect_BuffBase
{

}
