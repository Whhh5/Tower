using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Effect_Attack_Default1Data : EntityEffectBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Effect_Attack_Default1;
    public override void Initialization(Vector3 f_StartPos, WorldObjectBaseData f_Initiator, DirectorWrapMode f_WrapMode = DirectorWrapMode.Hold)
    {
        base.Initialization(f_StartPos, f_Initiator, f_WrapMode);

    }
    public override void OnUpdate()
    {
        if (CurPlaySchedule >= 1)
        {
            ILoadPrefabAsync.UnLoad(this);
        }
        base.OnUpdate();
    }
}
public class Entity_Effect_Attack_Default1 : EntityEffectBase
{
    
}
