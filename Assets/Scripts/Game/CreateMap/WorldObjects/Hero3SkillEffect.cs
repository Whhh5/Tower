using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Hero3SkillEffectData : EntityEffectBaseData
{
    public void Initialization(Vector3 f_StartPos, int f_HarmValue, WorldObjectBaseData f_Initiator)
    {
        base.Initialization(f_StartPos, f_Initiator, DirectorWrapMode.Loop);
        m_HarmValue = f_HarmValue;

    }
    public override EAssetKey AssetPrefabID => EAssetKey.Hero3SkillEffect;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    private int m_HarmValue = 0;

    public override void ExecuteEffect(WorldObjectBaseData f_Target)
    {
        base.ExecuteEffect(f_Target);
        GTools.MathfMgr.EntityDamage(Initiator, f_Target, EDamageType.AddBlood, m_HarmValue);
    }
    public override void OnUnLoad()
    {
        base.OnUnLoad();
    }
}

public class Hero3SkillEffect : EntityEffectBase
{

}