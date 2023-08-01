using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterElement_GuidedMissileData : WeaponElementBaseData
{
    public EmitterElement_GuidedMissileData(int f_Index, WorldObjectBaseData f_Initiator) : base(f_Index, f_Initiator)
    {

    }
    public override AssetKey AssetPrefabID => AssetKey.EmitterElement_GuidedMissile;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    [SerializeField]
    private TrailRender_Common m_Trail = null;
    [SerializeField]
    Dictionary<ushort, ushort> m_BuffList = new()
    {
        { 1, 2 },
        { 2, 1 },
    };


    public override void StopExecute()
    {
        base.StopExecute();

        var targets = GTools.MathfMgr.GetTargets_Sphere(CentralPoint, 10, ELayer.Enemy);

        foreach (var entity in targets)
        {
            foreach (var item in m_BuffList)
            {
                entity.AddBuffAsync(item.Key, item.Value);
            }
        }

        ILoadPrefabAsync.UnLoad(this);
    }
}
public class EmitterElement_GuidedMissile : WeaponElementBase
{


}
