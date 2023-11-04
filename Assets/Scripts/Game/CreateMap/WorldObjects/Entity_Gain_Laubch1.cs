using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Gain_Laubch1Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.Launch1;
    public override EBuffView GainView => EBuffView.Launch;

    protected override float DurationTime => 20;

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Gain_Laubch1;

    public override void Initialization(WorldObjectBaseData f_Initiator, WorldObjectBaseData f_Recipient)
    {
        base.Initialization(f_Initiator, f_Recipient);
        m_Probability = 30;
    }

    public override async void ExecuteContext(int f_CurProbability)
    {
        var wepon = new Emitter_GuidedMissileBaseCommonData(0, m_Recipient);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(wepon));
        await wepon.StartExecute();
        await wepon.StopExecute();
        ILoadPrefabAsync.UnLoad(wepon);
    }

    public override Vector3 GetPosiion()
    {
        return m_Recipient.WorldPosition;
    }
}
public class Entity_Gain_Laubch1 : EntityGainBase
{

}
